using Avalonia.Data.Converters;
using Ryujinx.Ava.Common.Locale;
using System;
using System.Globalization;

namespace Ryujinx.Ava.UI.Helpers
{
    internal class PlayerHotkeyLabelConverter : IValueConverter
    {
        public static readonly PlayerHotkeyLabelConverter Instance = new();

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is string playerName && !string.IsNullOrEmpty(playerName))
            {
                string baseText = LocaleManager.Instance[LocaleKeys.SettingsTabHotkeysCycleInputDevicePlayerX];
                return string.Format(baseText, playerName);
            }

            return string.Empty;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
} 
