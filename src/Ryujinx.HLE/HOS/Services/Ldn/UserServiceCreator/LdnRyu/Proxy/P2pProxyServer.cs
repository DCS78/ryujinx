using Humanizer;
using Mono.Nat;
using NetCoreServer;
using Ryujinx.Common.Logging;
using Ryujinx.HLE.HOS.Services.Ldn.UserServiceCreator.LdnRyu.Types;
using Ryujinx.HLE.HOS.Services.Ldn.UserServiceCreator.Types;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;

namespace Ryujinx.HLE.HOS.Services.Ldn.UserServiceCreator.LdnRyu.Proxy
{
 class P2pProxyServer : TcpServer, IDisposable
 {
 public const ushort PrivatePortBase =39990;
 public const int PrivatePortRange =10;

 private const ushort PublicPortBase =39990;
 private const int PublicPortRange =10;

 // Increased lease length to reduce frequent renewals (seconds).
 private const ushort PortLeaseLength =3600; //1 hour
 private const ushort PortLeaseRenew =300; // renew every5 minutes

 private const ushort AuthWaitSeconds =1;

 // Discovery timeout (ms)
 private const int DiscoveryTimeoutMs =5000;

 private readonly ReaderWriterLockSlim _lock = new(LockRecursionPolicy.SupportsRecursion);

 public ushort PrivatePort { get; }

 private ushort _publicPort;

 private bool _disposed;
 private readonly CancellationTokenSource _disposedCancellation = new();

 private INatDevice _natDevice;
 private Mapping _portMapping;

 private Task _renewalTask;

 private readonly List<P2pProxySession> _players = new();

 private readonly List<ExternalProxyToken> _waitingTokens = new();
 private readonly AutoResetEvent _tokenEvent = new(false);

 private uint _broadcastAddress;

 private readonly LdnMasterProxyClient _master;
 private readonly RyuLdnProtocol _masterProtocol;
 private readonly RyuLdnProtocol _protocol;

 public P2pProxyServer(LdnMasterProxyClient master, ushort port, RyuLdnProtocol masterProtocol) : base(IPAddress.Any, port)
 {
 if (ProxyHelpers.SupportsNoDelay())
 {
 OptionNoDelay = true;
 }

 PrivatePort = port;

 _master = master;
 _masterProtocol = masterProtocol;

 _masterProtocol.ExternalProxyState += HandleStateChange;
 _masterProtocol.ExternalProxyToken += HandleToken;

 _protocol = new RyuLdnProtocol();
 }

 private void HandleToken(LdnHeader header, ExternalProxyToken token)
 {
 _lock.EnterWriteLock();

 _waitingTokens.Add(token);

 _lock.ExitWriteLock();

 _tokenEvent.Set();
 }

 private void HandleStateChange(LdnHeader header, ExternalProxyConnectionState state)
 {
 if (!state.Connected)
 {
 _lock.EnterWriteLock();

 _waitingTokens.RemoveAll(token => token.VirtualIp == state.IpAddress);

 _players.RemoveAll(player =>
 {
 if (player.VirtualIpAddress == state.IpAddress)
 {
 player.DisconnectAndStop();

 return true;
 }

 return false;
 });

 _lock.ExitWriteLock();
 }
 }

 public void Configure(ProxyConfig config)
 {
 _broadcastAddress = config.ProxyIp | (~config.ProxySubnetMask);
 }

 public async Task<ushort> NatPunch()
 {
 var tcs = new TaskCompletionSource<INatDevice>(TaskCreationOptions.RunContinuationsAsynchronously);

 EventHandler<DeviceEventArgs> handler = (s, e) =>
 {
 if (e?.Device != null)
 {
 tcs.TrySetResult(e.Device);
 }
 };

 NatUtility.DeviceFound += handler;

 try
 {
 NatUtility.StartDiscovery();

 var delayTask = Task.Delay(DiscoveryTimeoutMs);
 var completed = await Task.WhenAny(tcs.Task, delayTask).ConfigureAwait(false);

 if (completed != tcs.Task)
 {
 return 0;
 }

 INatDevice device = await tcs.Task.ConfigureAwait(false);

 _publicPort = PublicPortBase;

 for (int i =0; i < PublicPortRange; i++)
 {
 try
 {
 // Mono.Nat.Mapping constructor includes a description parameter.
 _portMapping = new Mapping(Protocol.Tcp, (int)PrivatePort, _publicPort, PortLeaseLength, "Ryujinx Local Multiplayer");

 // CreatePortMap is synchronous in Mono.Nat, run on thread pool to avoid blocking.
 await Task.Run(() => device.CreatePortMap(_portMapping)).ConfigureAwait(false);

 break;
 }
 catch (Exception)
 {
 // If mapping fails for this port, try next.
 _publicPort++;
 }

 if (i == PublicPortRange -1)
 {
 _publicPort =0;
 }
 }

 if (_publicPort !=0)
 {
 // Start a background renewal loop to refresh the lease periodically.
 _natDevice = device;
 _renewalTask = Task.Run(() => RenewalLoop(_disposedCancellation.Token));
 }

 return _publicPort;
 }
 finally
 {
 NatUtility.DeviceFound -= handler;
 try
 {
 NatUtility.StopDiscovery();
 }
 catch
 {
 // Ignore stop discovery errors.
 }
 }
 }

 // Proxy handlers

 private void RouteMessage(P2pProxySession sender, ref ProxyInfo info, Action<P2pProxySession> action)
 {
 if (info.SourceIpV4 ==0)
 {
 // If they sent from a connection bound on0.0.0.0, make others see it as them.
 info.SourceIpV4 = sender.VirtualIpAddress;
 }
 else if (info.SourceIpV4 != sender.VirtualIpAddress)
 {
 // Can't pretend to be somebody else.
 return;
 }

 uint destIp = info.DestIpV4;

 if (destIp ==0xc0a800ff)
 {
 destIp = _broadcastAddress;
 }

 bool isBroadcast = destIp == _broadcastAddress;

 _lock.EnterReadLock();

 if (isBroadcast)
 {
 _players.ForEach(player =>
 {
 action(player);
 });
 }
 else
 {
 P2pProxySession target = _players.FirstOrDefault(player => player.VirtualIpAddress == destIp);

 if (target != null)
 {
 action(target);
 }
 }

 _lock.ExitReadLock();
 }

 public void HandleProxyDisconnect(P2pProxySession sender, LdnHeader header, ProxyDisconnectMessage message)
 {
 RouteMessage(sender, ref message.Info, (target) =>
 {
 target.SendAsync(sender.Protocol.Encode(PacketId.ProxyDisconnect, message));
 });
 }

 public void HandleProxyData(P2pProxySession sender, LdnHeader header, ProxyDataHeader message, byte[] data)
 {
 RouteMessage(sender, ref message.Info, (target) =>
 {
 target.SendAsync(sender.Protocol.Encode(PacketId.ProxyData, message, data));
 });
 }

 public void HandleProxyConnectReply(P2pProxySession sender, LdnHeader header, ProxyConnectResponse message)
 {
 RouteMessage(sender, ref message.Info, (target) =>
 {
 target.SendAsync(sender.Protocol.Encode(PacketId.ProxyConnectReply, message));
 });
 }

 public void HandleProxyConnect(P2pProxySession sender, LdnHeader header, ProxyConnectRequest message)
 {
 RouteMessage(sender, ref message.Info, (target) =>
 {
 target.SendAsync(sender.Protocol.Encode(PacketId.ProxyConnect, message));
 });
 }

 // End proxy handlers

 private async Task RenewalLoop(CancellationToken token)
 {
 try
 {
 while (!token.IsCancellationRequested)
 {
 try
 {
 await Task.Delay(PortLeaseRenew.Seconds(), token).ConfigureAwait(false);

 if (token.IsCancellationRequested || _natDevice == null || _portMapping == null)
 {
 continue;
 }

 // Recreate the mapping to refresh lease. Run on thread pool because API is synchronous.
 await Task.Run(() => _natDevice.CreatePortMap(_portMapping)).ConfigureAwait(false);
 }
 catch (OperationCanceledException)
 {
 break;
 }
 catch (Exception)
 {
 // Swallow and continue; mapping may fail temporarily.
 }
 }
 }
 catch (Exception)
 {
 // Ensure the loop terminates silently on unexpected errors.
 }
 }

 public bool TryRegisterUser(P2pProxySession session, ExternalProxyConfig config)
 {
 _lock.EnterWriteLock();

 // Attempt to find matching configuration. If we don't find one, wait for a bit and try again.
 // Woken by new tokens coming in from the master server.

 IPAddress address = (session.Socket.RemoteEndPoint as IPEndPoint).Address;
 byte[] addressBytes = ProxyHelpers.AddressTo16Byte(address);

 long time;
 long endTime = Stopwatch.GetTimestamp() + Stopwatch.Frequency * AuthWaitSeconds;

 do
 {
 for (int i =0; i < _waitingTokens.Count; i++)
 {
 ExternalProxyToken waitToken = _waitingTokens[i];

 // Allow any client that has a private IP to connect. (indicated by the server as all0 in the token)

 bool isPrivate = waitToken.PhysicalIp.AsSpan().SequenceEqual(new byte[16]);
 bool ipEqual = isPrivate || waitToken.AddressFamily == address.AddressFamily && waitToken.PhysicalIp.AsSpan().SequenceEqual(addressBytes);

 if (ipEqual && waitToken.Token.AsSpan().SequenceEqual(config.Token.AsSpan()))
 {
 // This is a match.

 _waitingTokens.RemoveAt(i);

 session.SetIpv4(waitToken.VirtualIp);

 ProxyConfig pconfig = new()
 {
 ProxyIp = session.VirtualIpAddress,
 ProxySubnetMask =0xFFFF0000 // TODO: Use from server.
 };

 if (_players.Count ==0)
 {
 Configure(pconfig);
 }

 _players.Add(session);

 session.SendAsync(_protocol.Encode(PacketId.ProxyConfig, pconfig));

 _lock.ExitWriteLock();

 return true;
 }
 }

 // Couldn't find the token.
 // It may not have arrived yet, so wait for one to arrive.

 _lock.ExitWriteLock();

 time = Stopwatch.GetTimestamp();
 int remainingMs = (int)((endTime - time) / (Stopwatch.Frequency /1000));

 if (remainingMs <0)
 {
 remainingMs =0;
 }

 _tokenEvent.WaitOne(remainingMs);

 _lock.EnterWriteLock();

 } while (time < endTime);

 _lock.ExitWriteLock();

 return false;
 }

 public void DisconnectProxyClient(P2pProxySession session)
 {
 _lock.EnterWriteLock();

 bool removed = _players.Remove(session);

 if (removed)
 {
 _master.SendAsync(_masterProtocol.Encode(PacketId.ExternalProxyState, new ExternalProxyConnectionState
 {
 IpAddress = session.VirtualIpAddress,
 Connected = false
 }));
 }

 _lock.ExitWriteLock();
 }

 public new void Dispose()
 {
 base.Dispose();

 _disposed = true;
 _disposedCancellation.Cancel();

 try
 {
 // Delete mapping on thread-pool to avoid blocking.
 var deleteTask = Task.Run(() => _natDevice?.DeletePortMap(new Mapping(Protocol.Tcp, (int)PrivatePort, _publicPort, PortLeaseLength, "Ryujinx Local Multiplayer")));

 // Just absorb any exceptions.
 deleteTask?.ContinueWith((task) => { });

 // Ensure renewal task also ends.
 _renewalTask?.ContinueWith((t) => { });
 }
 catch (Exception)
 {
 // Fail silently.
 }
 }

 protected override TcpSession CreateSession()
 {
 return new P2pProxySession(this);
 }

 protected override void OnError(SocketError error)
 {
 Logger.Info?.PrintMsg(LogClass.ServiceLdn, $"Proxy TCP server caught an error with code {error}");
 }
 }
}
