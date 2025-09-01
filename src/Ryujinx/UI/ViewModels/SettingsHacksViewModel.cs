using CommunityToolkit.Mvvm.ComponentModel;
using Gommon;
using Ryujinx.Ava.Systems.Configuration;

namespace Ryujinx.Ava.UI.ViewModels
{
    public partial class SettingsHacksViewModel : BaseModel
    {
        private readonly SettingsViewModel _baseViewModel;

        public SettingsHacksViewModel() { }

        public SettingsHacksViewModel(SettingsViewModel settingsVm)
        {
            _baseViewModel = settingsVm;
        }

        [ObservableProperty] private bool _xc2MenuSoftlockFix = ConfigurationState.Instance.Hacks.Xc2MenuSoftlockFix;
        [ObservableProperty] private bool _nifmDisableIsAnyInternetRequestAccepted = ConfigurationState.Instance.Hacks.DisableNifmIsAnyInternetRequestAccepted;
        [ObservableProperty] private bool _tmntSrCutsceneCrashFix = ConfigurationState.Instance.Hacks.TmntSrCutsceneCrashFix;
        public static string Xc2MenuFixTooltip { get; } = Lambda.String(sb =>
        {
            sb.AppendLine(
                    "This hack applies a 2ms delay (via 'Thread.Sleep(2)') every time the game tries to read data from the emulated Switch filesystem.")
                .AppendLine();

            sb.AppendLine("From the issue on GitHub:").AppendLine();
            sb.Append(
                "When clicking very fast from game main menu to 2nd submenu, " +
                "there is a low chance that the game will softlock, " +
                "the submenu won't show up, while background music is still there.");
        });

        public static string NifmDisableIsAnyInternetRequestAcceptedTooltip { get; } = Lambda.String(sb =>
        {
            sb.AppendLine(
                    "This hack simply sets 'IsAnyInternetRequestAccepted' to 'false' when initializing the Nifm IGeneralService.")
                .AppendLine();

            sb.Append("Lets DOOM 2016 go in game.");
        });

        public static string TmntSrCutsceneCrashFixTooltip { get; } = Lambda.String(sb =>
        {
            sb.AppendLine(
                    "This hack adds a 50ms delay to NvGpuAsMagic NvIoctl calls. This prevents the game from crashing when the cutscene starts.")
                .AppendLine();

            sb.Append(
                "This simply just gives the game some time to properly interact with guest memory");
        });
    }
}
