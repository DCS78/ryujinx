using CommunityToolkit.Mvvm.ComponentModel;
using Ryujinx.Ava.UI.ViewModels;
using Ryujinx.Common.Configuration.Hid;

namespace Ryujinx.Ava.UI.Models.Input
{
    public partial class HotkeyConfig : BaseModel
    {
        [ObservableProperty] private Key _toggleVSyncMode;

        [ObservableProperty] private Key _screenshot;

        [ObservableProperty] private Key _showUI;

        [ObservableProperty] private Key _pause;

        [ObservableProperty] private Key _toggleMute;

        [ObservableProperty] private Key _resScaleUp;

        [ObservableProperty] private Key _resScaleDown;

        [ObservableProperty] private Key _volumeUp;

        [ObservableProperty] private Key _volumeDown;

        [ObservableProperty] private Key _customVSyncIntervalIncrement;

        [ObservableProperty] private Key _customVSyncIntervalDecrement;

        [ObservableProperty] private Key _turboMode;

        [ObservableProperty] private bool _turboModeWhileHeld;

        [ObservableProperty] private Key _cycleInputDevicePlayer1;

        [ObservableProperty] private Key _cycleInputDevicePlayer2;

        [ObservableProperty] private Key _cycleInputDevicePlayer3;

        [ObservableProperty] private Key _cycleInputDevicePlayer4;

        [ObservableProperty] private Key _cycleInputDevicePlayer5;

        [ObservableProperty] private Key _cycleInputDevicePlayer6;

        [ObservableProperty] private Key _cycleInputDevicePlayer7;

        [ObservableProperty] private Key _cycleInputDevicePlayer8;

        [ObservableProperty] private Key _cycleInputDeviceHandheld;

        public HotkeyConfig(KeyboardHotkeys config)
        {
            if (config == null)
                return;

            ToggleVSyncMode = config.ToggleVSyncMode;
            Screenshot = config.Screenshot;
            ShowUI = config.ShowUI;
            Pause = config.Pause;
            ToggleMute = config.ToggleMute;
            ResScaleUp = config.ResScaleUp;
            ResScaleDown = config.ResScaleDown;
            VolumeUp = config.VolumeUp;
            VolumeDown = config.VolumeDown;
            CustomVSyncIntervalIncrement = config.CustomVSyncIntervalIncrement;
            CustomVSyncIntervalDecrement = config.CustomVSyncIntervalDecrement;
            TurboMode = config.TurboMode;
            TurboModeWhileHeld = config.TurboModeWhileHeld;
            CycleInputDevicePlayer1 = config.CycleInputDevicePlayer1;
            CycleInputDevicePlayer2 = config.CycleInputDevicePlayer2;
            CycleInputDevicePlayer3 = config.CycleInputDevicePlayer3;
            CycleInputDevicePlayer4 = config.CycleInputDevicePlayer4;
            CycleInputDevicePlayer5 = config.CycleInputDevicePlayer5;
            CycleInputDevicePlayer6 = config.CycleInputDevicePlayer6;
            CycleInputDevicePlayer7 = config.CycleInputDevicePlayer7;
            CycleInputDevicePlayer8 = config.CycleInputDevicePlayer8;
            CycleInputDeviceHandheld = config.CycleInputDeviceHandheld;
        }

        public KeyboardHotkeys GetConfig() =>
            new()
            {
                ToggleVSyncMode = ToggleVSyncMode,
                Screenshot = Screenshot,
                ShowUI = ShowUI,
                Pause = Pause,
                ToggleMute = ToggleMute,
                ResScaleUp = ResScaleUp,
                ResScaleDown = ResScaleDown,
                VolumeUp = VolumeUp,
                VolumeDown = VolumeDown,
                CustomVSyncIntervalIncrement = CustomVSyncIntervalIncrement,
                CustomVSyncIntervalDecrement = CustomVSyncIntervalDecrement,
                TurboMode = TurboMode,
                TurboModeWhileHeld = TurboModeWhileHeld,
                CycleInputDevicePlayer1 = CycleInputDevicePlayer1,
                CycleInputDevicePlayer2 = CycleInputDevicePlayer2,
                CycleInputDevicePlayer3 = CycleInputDevicePlayer3,
                CycleInputDevicePlayer4 = CycleInputDevicePlayer4,
                CycleInputDevicePlayer5 = CycleInputDevicePlayer5,
                CycleInputDevicePlayer6 = CycleInputDevicePlayer6,
                CycleInputDevicePlayer7 = CycleInputDevicePlayer7,
                CycleInputDevicePlayer8 = CycleInputDevicePlayer8,
                CycleInputDeviceHandheld = CycleInputDeviceHandheld
            };
    }
}
