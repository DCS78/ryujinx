using Avalonia;
using Ryujinx.Common.SystemInterop;
using System;

namespace Ryujinx.Ava.Utilities
{
    public static class WindowingSystemTypeExtensions
    {
        public static WindowingSystemType GetWindowingSystemType(this AppBuilder builder)
        {
            // null means uninitialized, "" means the name isn't set.
            if (builder?.WindowingSubsystemName is null)
            {
                throw new InvalidOperationException("The windowing subsystem must be configured before calling this method.");
            }

            if (builder.WindowingSubsystemName == "")
            {
                // TODO: They forget to set the WindowingSubsystemName for X11, we assume it's X11 because every other Linux backend sets it.
                // https://github.com/AvaloniaUI/Avalonia/blob/22c4c630ce5910343006fd58d611b286ed87c740/src/Avalonia.X11/X11Platform.cs#L414
                if (OperatingSystem.IsLinux())
                {
                    return WindowingSystemType.X11;
                }

                // TODO: Same for Cocoa.
                // https://github.com/AvaloniaUI/Avalonia/blob/22c4c630ce5910343006fd58d611b286ed87c740/src/Avalonia.Native/AvaloniaNativePlatformExtensions.cs#L26
                if (OperatingSystem.IsMacOS())
                {
                    return WindowingSystemType.Cocoa;
                }
            }

            return builder.WindowingSubsystemName switch
            {
                "Win32" => WindowingSystemType.Win32,
                "X11" => WindowingSystemType.X11,
                "Cocoa" => WindowingSystemType.Cocoa,
                _ => WindowingSystemType.Unknown,
            };
        }
    }
}
