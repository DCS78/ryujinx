using Ryujinx.Common.Memory;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Runtime.Versioning;
using System.Text;

namespace Ryujinx.Common.SystemInterop
{
    [SupportedOSPlatform("linux")]
    public static partial class X11Helper
    {
        private const string X11LibraryName = "libX11.so.6";

        [LibraryImport(X11LibraryName)]
        private static partial nint XOpenDisplay([MarshalAs(UnmanagedType.LPStr)] string display);

        [LibraryImport(X11LibraryName)]
        private static partial nint XGetDefault(nint display, [MarshalAs(UnmanagedType.LPStr)] string program, [MarshalAs(UnmanagedType.LPStr)] string option);

        [LibraryImport(X11LibraryName)]
        private static partial int XDisplayWidth(nint display, int screenNumber);

        [LibraryImport(X11LibraryName)]
        private static partial int XDisplayWidthMM(nint display, int screenNumber);

        [LibraryImport(X11LibraryName)]
        private static partial int XCloseDisplay(nint display);

        [LibraryImport(X11LibraryName)]
        private static partial nint XInternAtom(nint display, [MarshalAs(UnmanagedType.LPStr)] string atom_name, [MarshalAs(UnmanagedType.U4)] bool only_if_exists);

        [LibraryImport(X11LibraryName)]
        private static partial nint XGetSelectionOwner(nint display, nint selection);

        [LibraryImport(X11LibraryName)]
        private static partial int XGetWindowProperty(nint display, nint window, nint atom, nint long_offset,
            nint long_length, [MarshalAs(UnmanagedType.U4)] bool delete, nint req_type, out nint actual_type, out int actual_format,
            out nint nitems, out nint bytes_after, out nint prop);

        [LibraryImport(X11LibraryName)]
        private static partial int XFree(nint data);

        [LibraryImport(X11LibraryName)]
        private static partial int XSelectInput(nint display, nint window, nint event_mask);

        [LibraryImport(X11LibraryName)]
        private static partial int XSync(nint display, [MarshalAs(UnmanagedType.U4)] bool discard);

        [LibraryImport(X11LibraryName)]
        private static partial int XPending(nint display);

        [LibraryImport(X11LibraryName)]
        private static partial int XNextEvent(nint display, out XEvent event_return);

        [LibraryImport(X11LibraryName)]
        private static partial nint XSetErrorHandler(nint handler);

        [LibraryImport(X11LibraryName)]
        private static partial int XDefaultScreen(nint display);

        // X11 constants
        private const int PropertyChangeMask = (1 << 22);
        private const int StructureNotifyMask = (1 << 17);
        private const int PropertyNotify = 28;
        private const int DestroyNotify = 17;
        private const int Success = 0;

        [StructLayout(LayoutKind.Sequential)]
        private struct XEvent
        {
            public int type;
            public int pad0;
            Array23<long> pads;
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct XPropertyEvent
        {
            public int type;
            public nuint serial;
            public int send_event;
            public nint display;
            public nint window;
            public nint atom;
            public nint time;
            public int state;
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct XDestroyWindowEvent
        {
            public int type;
            public nuint serial;
            public int send_event;
            public nint display;
            public nint eventWindow;
            public nint window;
        }

        public enum XSettingType : byte
        {
            Integer = 0,
            String = 1,
            Color = 2
        }

        public enum XSettingsByteOrder : byte
        {
            LittleEndian = 0,
            BigEndian = 1,
        }

        public class XSettingValue
        {
            public XSettingType Type { get; set; }
            public object Value { get; set; }
            public uint Serial { get; set; }

            public override string ToString()
            {
                return Type switch
                {
                    XSettingType.Integer => $"XInteger({Value})",
                    XSettingType.String => $"XString({Value})",
                    XSettingType.Color => $"XColor{Value}",
                    _ => "Unknown",
                };
            }
        }

        public class XSettingsMap
        {
            private readonly Dictionary<string, XSettingValue> _settings = new();
            public uint Serial { get; set; }

            public bool TryGetValue(string name, out XSettingValue value)
            {
                return _settings.TryGetValue(name, out value);
            }

            public void Set(string name, XSettingValue value)
            {
                _settings[name] = value;
            }

            public void Clear()
            {
                _settings.Clear();
            }

            public IEnumerable<KeyValuePair<string, XSettingValue>> GetAll()
            {
                return _settings;
            }
        }

        public class XSettingsListener : IDisposable
        {
            private readonly XDisplay _display;
            private readonly int _screen;
            private nint _settingsWindow;
            private nint _settingsAtom;
            private nint _selectionAtom;
            private readonly XSettingsMap _currentSettings = new();
            private readonly List<Action<XSettingsMap>> _listeners = new();

            public XDisplay Display => _display;

            public XSettingsMap CurrentSettings => _currentSettings;

            public XSettingsListener(XDisplay display, int screen = -1)
            {
                _display = display;
                _screen = screen < 0 ? XDefaultScreen(_display.Handle) : screen;

                _selectionAtom = XInternAtom(_display.Handle, $"_XSETTINGS_S{_screen}", false);
                _settingsAtom = XInternAtom(_display.Handle, "_XSETTINGS_SETTINGS", false);

                if (_selectionAtom == nint.Zero || _settingsAtom == nint.Zero)
                {
                    throw new Exception("Failed to intern required atoms");
                }

                InitializeSettings();
            }

            public void RegisterListener(Action<XSettingsMap> listener)
            {
                _listeners.Add(listener);
                listener?.Invoke(_currentSettings);
            }

            public void UnregisterListener(Action<XSettingsMap> listener)
            {
                _listeners.Remove(listener);
            }

            public void Poll()
            {
                while (XPending(_display.Handle) > 0)
                {
                    XNextEvent(_display.Handle, out XEvent xevent);

                    if (xevent.type == PropertyNotify)
                    {
                        unsafe
                        {
                            var propEvent = Marshal.PtrToStructure<XPropertyEvent>(new IntPtr(&xevent));
                            if (propEvent.window == _settingsWindow && propEvent.atom == _settingsAtom)
                            {
                                HandleSettingsChange();
                            }
                        }
                    }
                    else if (xevent.type == DestroyNotify)
                    {
                        unsafe
                        {
                            var destroyEvent = Marshal.PtrToStructure<XDestroyWindowEvent>(new IntPtr(&xevent));
                            if (destroyEvent.window == _settingsWindow)
                            {
                                HandleSettingsManagerDestroyed();
                            }
                        }
                    }
                }
            }

            private void InitializeSettings()
            {
                _settingsWindow = XGetSelectionOwner(_display.Handle, _selectionAtom);

                if (_settingsWindow == nint.Zero)
                {
                    _currentSettings.Clear();
                    return;
                }

                XSelectInput(_display.Handle, _settingsWindow, PropertyChangeMask | StructureNotifyMask);
                XSync(_display.Handle, false);

                ReadSettings();
            }

            private void ReadSettings()
            {
                if (_settingsWindow == nint.Zero)
                {
                    _currentSettings.Clear();
                    return;
                }

                bool error = false;
                nint oldHandler = XSetErrorHandler(Marshal.GetFunctionPointerForDelegate<ErrorHandler>((display, errorEvent) =>
                {
                    error = true;
                    return 0;
                }));

                try
                {
                    int result = XGetWindowProperty(
                        _display.Handle,
                        _settingsWindow,
                        _settingsAtom,
                        0,
                        0x7FFFFFFF,
                        false,
                        _settingsAtom,
                        out nint actualType,
                        out int actualFormat,
                        out nint nitems,
                        out nint bytesAfter,
                        out nint prop);

                    XSync(_display.Handle, false);

                    if (error || result != Success || actualType != _settingsAtom || actualFormat != 8 || prop == nint.Zero)
                    {
                        if (prop != nint.Zero)
                        {
                            XFree(prop);
                        }

                        _currentSettings.Clear();
                        _settingsWindow = nint.Zero;
                        return;
                    }

                    try
                    {
                        ParseSettings(prop, (int)nitems);
                    }
                    finally
                    {
                        XFree(prop);
                    }
                }
                finally
                {
                    XSetErrorHandler(oldHandler);
                }
            }

            private void ParseSettings(nint data, int length)
            {
                if (length < 12)
                    return;

                byte[] bytes = new byte[length];
                Marshal.Copy(data, bytes, 0, length);

                int offset = 0;

                XSettingsByteOrder byteOrder = (XSettingsByteOrder)bytes[offset++];
                offset += 3;
                uint serial = ReadUInt32(bytes, ref offset, byteOrder);
                uint numSettings = ReadUInt32(bytes, ref offset, byteOrder);

                _currentSettings.Serial = serial;
                var newSettings = new Dictionary<string, XSettingValue>();

                for (uint i = 0; i < numSettings && offset < length; i++)
                {
                    try
                    {
                        var setting = ReadSetting(bytes, ref offset, byteOrder);
                        if (setting.HasValue)
                        {
                            newSettings[setting.Value.name] = setting.Value.value;
                        }
                    }
                    catch
                    {
                        break;
                    }
                }

                _currentSettings.Clear();
                foreach (var kvp in newSettings)
                {
                    _currentSettings.Set(kvp.Key, kvp.Value);
                }
            }

            private (string name, XSettingValue value)? ReadSetting(byte[] bytes, ref int offset, XSettingsByteOrder byteOrder)
            {
                if (offset >= bytes.Length)
                    return null;

                XSettingType type = (XSettingType)bytes[offset++];
                offset++;

                ushort nameLen = ReadUInt16(bytes, ref offset, byteOrder);
                if (offset + nameLen > bytes.Length)
                    return null;

                string name = Encoding.UTF8.GetString(bytes, offset, nameLen);
                offset += nameLen;
                offset += Pad(nameLen);

                uint lastChangeSerial = ReadUInt32(bytes, ref offset, byteOrder);

                object value = null;

                switch (type)
                {
                    case XSettingType.Integer:
                        if (offset + 4 > bytes.Length)
                            return null;
                        value = ReadInt32(bytes, ref offset, byteOrder);
                        break;

                    case XSettingType.String:
                        if (offset + 4 > bytes.Length)
                            return null;
                        uint strLen = ReadUInt32(bytes, ref offset, byteOrder);
                        if (offset + strLen > bytes.Length)
                            return null;
                        value = Encoding.UTF8.GetString(bytes, offset, (int)strLen);
                        offset += (int)strLen;
                        offset += Pad((int)strLen);
                        break;

                    case XSettingType.Color:
                        if (offset + 8 > bytes.Length)
                            return null;
                        ushort red = ReadUInt16(bytes, ref offset, byteOrder);
                        ushort green = ReadUInt16(bytes, ref offset, byteOrder);
                        ushort blue = ReadUInt16(bytes, ref offset, byteOrder);
                        ushort alpha = ReadUInt16(bytes, ref offset, byteOrder);
                        value = (red, green, blue, alpha);
                        break;

                    default:
                        return null;
                }

                return (name, new XSettingValue
                {
                    Type = type,
                    Value = value,
                    Serial = lastChangeSerial
                });
            }

            private static int Pad(int n)
            {
                return ((n + 3) & ~3) - n;
            }

            // all of the read methods assume the system is little-endian - rest of the emulator won't likely even work on big-endian systems

            private static uint ReadUInt32(byte[] bytes, ref int offset, XSettingsByteOrder byteOrder)
            {
                uint value = BitConverter.ToUInt32(bytes, offset);
                offset += 4;
                if (byteOrder != XSettingsByteOrder.LittleEndian)
                {
                    return ReverseBytes(value);
                }
                return value;
            }

            private static int ReadInt32(byte[] bytes, ref int offset, XSettingsByteOrder byteOrder)
            {
                return (int)ReadUInt32(bytes, ref offset, byteOrder);
            }

            private static ushort ReadUInt16(byte[] bytes, ref int offset, XSettingsByteOrder byteOrder)
            {
                ushort value = BitConverter.ToUInt16(bytes, offset);
                offset += 2;
                if (byteOrder != XSettingsByteOrder.LittleEndian)
                {
                    return ReverseBytes(value);
                }
                return value;
            }

            private static uint ReverseBytes(uint value)
            {
                return ((value & 0x000000FFU) << 24) |
                       ((value & 0x0000FF00U) << 8) |
                       ((value & 0x00FF0000U) >> 8) |
                       ((value & 0xFF000000U) >> 24);
            }

            private static ushort ReverseBytes(ushort value)
            {
                return (ushort)(((value & 0x00FF) << 8) | ((value & 0xFF00) >> 8));
            }

            private void HandleSettingsChange()
            {
                ReadSettings();
                NotifyListeners();
            }

            private void HandleSettingsManagerDestroyed()
            {
                _currentSettings.Clear();
                _settingsWindow = nint.Zero;

                InitializeSettings();
                NotifyListeners();
            }

            private void NotifyListeners()
            {
                foreach (var listener in _listeners)
                {
                    try
                    {
                        listener?.Invoke(_currentSettings);
                    }
                    catch
                    {
                    }
                }
            }

            public void Dispose()
            {
                _display.Dispose();
            }

            [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
            private delegate int ErrorHandler(nint display, nint errorEvent);
        }

        public struct XDisplay : IDisposable
        {
            public nint Handle;

            public static XDisplay Open(string display = null)
            {
                nint handle = XOpenDisplay(display);
                if (handle == nint.Zero)
                {
                    throw new Exception("Couldn't open X11 display.");
                }

                return new XDisplay { Handle = handle };
            }

            public nint GetDefault(string program, string option)
            {
                return XGetDefault(Handle, program, option);
            }

            public int GetWidth(int screenNumber)
            {
                return XDisplayWidth(Handle, screenNumber);
            }

            public int GetWidthMM(int screenNumber)
            {
                return XDisplayWidthMM(Handle, screenNumber);
            }

            public void Dispose()
            {
                if (Handle != nint.Zero)
                {
                    _ = XCloseDisplay(Handle);
                    Handle = nint.Zero;
                }
            }
        }
    }
}
