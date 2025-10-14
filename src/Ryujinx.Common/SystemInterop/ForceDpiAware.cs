using Ryujinx.Common.Logging;
using System;
using System.Globalization;
using System.Runtime.InteropServices;

namespace Ryujinx.Common.SystemInterop
{
    public static partial class ForceDpiAware
    {
        [LibraryImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static partial bool SetProcessDPIAware();

        private const double StandardDpiScale = 96.0;
        private const double MaxScaleFactor = 3.0;

        private static X11Helper.XSettingsListener xSettingsHelper = null;

        /// <summary>
        /// Marks the application as DPI-Aware when running on the Windows operating system.
        /// </summary>
        public static void Windows()
        {
            // Make process DPI aware for proper window sizing on high-res screens.
            if (OperatingSystem.IsWindowsVersionAtLeast(6))
            {
                SetProcessDPIAware();
            }
        }

        public static double GetActualScaleFactor(WindowingSystemType windowingSystem)
        {
            double userDpiScale = 96.0;

            try
            {
                if (OperatingSystem.IsWindows())
                {
                    userDpiScale = GdiPlusHelper.GetDpiX(nint.Zero);
                }
                else if (OperatingSystem.IsLinux())
                {
                    if (windowingSystem == WindowingSystemType.X11)
                    {
                        var avaScaleFactor = Environment.GetEnvironmentVariable("AVALONIA_GLOBAL_SCALE_FACTOR");
                        if (avaScaleFactor is string avaScaleStr &&
                            double.TryParse(avaScaleStr, NumberStyles.Any, CultureInfo.InvariantCulture, out double avaScale) &&
                            avaScale > 0)
                        {
                            // userDpiScale = avaScale * 96.0; // TODO: avalonia uses logical size?
                            return userDpiScale;
                        }

                        if (xSettingsHelper == null)
                        {
                            var display = X11Helper.XDisplay.Open(null);
                            xSettingsHelper = new X11Helper.XSettingsListener(display);
                        }

                        xSettingsHelper.CurrentSettings.TryGetValue("Gdk/UnscaledDPI", out var gdkUnscaledDPI);
                        xSettingsHelper.CurrentSettings.TryGetValue("Gdk/WindowScalingFactor", out var gdkWindowScalingFactor);
                        xSettingsHelper.CurrentSettings.TryGetValue("Xft/DPI", out var xftDpiSetting);

                        double scaleFactor = 1.0;

                        if (gdkUnscaledDPI?.Type == X11Helper.XSettingType.Integer && gdkWindowScalingFactor?.Type == X11Helper.XSettingType.Integer)
                        {
                            var unscaledDPI = (int)gdkUnscaledDPI.Value / (96d * 1024);
                            var windowScalingFactor = (double)(int)gdkWindowScalingFactor.Value;

                            scaleFactor = unscaledDPI * windowScalingFactor;
                        }
                        else
                        {
                            var display = xSettingsHelper.Display;
                            string dpiString = Marshal.PtrToStringAnsi(display.GetDefault("Xft", "dpi"));
                            if (dpiString == null || !double.TryParse(dpiString, NumberStyles.Any, CultureInfo.InvariantCulture, out userDpiScale))
                            {
                                userDpiScale = display.GetWidth(0) * 25.4 / display.GetWidthMM(0);
                            }
                        }

                        scaleFactor = Math.Max(scaleFactor, 1.0);
                        // userDpiScale = 96.0 * scaleFactor; // TODO: avalonia uses logical size?

                        Environment.SetEnvironmentVariable("AVALONIA_GLOBAL_SCALE_FACTOR", scaleFactor.ToString(CultureInfo.InvariantCulture));
                    }
                }
            }
            catch (Exception e)
            {
                Logger.Warning?.Print(LogClass.Application, $"Couldn't determine monitor DPI: {e.Message}");
            }

            return userDpiScale;
        }

        public static double GetWindowScaleFactor(WindowingSystemType windowingSystem)
        {
            double userDpiScale = GetActualScaleFactor(windowingSystem);

            return Math.Min(userDpiScale / StandardDpiScale, MaxScaleFactor);
        }
    }
}
