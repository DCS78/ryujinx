using Ryujinx.Ava.Systems.AppLibrary;
using Ryujinx.Common;
using Starscript;

namespace Ryujinx.Ava.Systems.Starscript
{
    public static class RyujinxStarscript
    {
        public static readonly StarscriptHypervisor Hypervisor = StarscriptHypervisor.Create().WithStandardLibrary(true);

        static RyujinxStarscript()
        {
            Hypervisor.Set("ryujinx.releaseChannel",
                ReleaseInformation.IsCanaryBuild
                    ? "Canary"
                    : ReleaseInformation.IsReleaseBuild
                        ? "Stable"
                        : "Custom");
            Hypervisor.Set("ryujinx.version", Program.Version);
            Hypervisor.Set("appLibrary", RyujinxApp.MainWindow.ApplicationLibrary);
            Hypervisor.Set("currentApplication", () => 
                RyujinxApp.MainWindow.ApplicationLibrary.FindApplication(
                    RyujinxApp.MainWindow.ViewModel.AppHost?.ApplicationId ?? 0,
                    out ApplicationData appData) 
                    ? StarscriptHelper.Wrap(appData)
                    : Value.Null);
        }
    }
}
