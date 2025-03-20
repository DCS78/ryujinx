using LibHac;
using Ryujinx.Audio.Integration;
using Ryujinx.Cpu;
using Ryujinx.Horizon.Sdk.Account;
using Ryujinx.Horizon.Sdk.Fs;

namespace Ryujinx.Horizon
{
    public readonly struct HorizonOptions
    {
#if DEBUG
        public bool IgnoreMissingServices { get; }
#endif
        public bool ThrowOnInvalidCommandIds { get; }

        public HorizonClient BcatClient { get; }
        public IFsClient FsClient { get; }
        public IEmulatorAccountManager AccountManager { get; }
        public IHardwareDeviceDriver AudioDeviceDriver { get; }
        public ITickSource TickSource { get; }

        public HorizonOptions(
#if DEBUG
            bool ignoreMissingServices,
#endif
            HorizonClient bcatClient,
            IFsClient fsClient,
            IEmulatorAccountManager accountManager,
            IHardwareDeviceDriver audioDeviceDriver,
            ITickSource tickSource)
        {
#if DEBUG
            IgnoreMissingServices = ignoreMissingServices;
#endif
            ThrowOnInvalidCommandIds = true;
            BcatClient = bcatClient;
            FsClient = fsClient;
            AccountManager = accountManager;
            AudioDeviceDriver = audioDeviceDriver;
            TickSource = tickSource;
        }
    }
}
