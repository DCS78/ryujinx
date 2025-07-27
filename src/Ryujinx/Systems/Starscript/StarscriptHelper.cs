using Gommon;
using Ryujinx.Ava.Systems.AppLibrary;
using Starscript;
using System;

namespace Ryujinx.Ava.Systems.Starscript
{
    public static class StarscriptHelper
    {
        public static ValueMap Wrap(ApplicationLibrary appLib)
        {
            ValueMap lMap = new();
            lMap.Set("appCount", () => appLib.Applications.Count);
            lMap.Set("dlcCount", () => appLib.DownloadableContents.Count);
            lMap.Set("updateCount", () => appLib.TitleUpdates.Count);
            lMap.Set("has", ctx =>
            {
                ulong titleId;

                try
                {
                    titleId = ctx.Constrain(Constraint.ExactlyOneArgument).NextString(1).ToULong();
                }
                catch (FormatException)
                {
                    throw ctx.Error(
                        $"Invalid input to {ctx.FormattedName}; input must be a hexadecimal number in a string.");
                }

                return appLib.FindApplication(titleId, out _);
            });
            lMap.Set("get", ctx =>
            {
                ulong titleId;

                try
                {
                    titleId = ctx.Constrain(Constraint.ExactlyOneArgument).NextString(1).ToULong();
                }
                catch (FormatException)
                {
                    throw ctx.Error(
                        $"Invalid input to {ctx.FormattedName}; input must be a hexadecimal number in a string.");
                }

                return appLib.FindApplication(titleId,
                    out ApplicationData applicationData)
                    ? Wrap(applicationData)
                    : null;
            });
            return lMap;
        }

        public static ValueMap Wrap(ApplicationData appData)
        {
            ValueMap aMap = new();
            aMap.Set("name", appData.Name);
            aMap.Set("version", appData.Version);
            aMap.Set("developer", appData.Developer);
            aMap.Set("fileExtension", appData.FileExtension);
            aMap.Set("fileSize", appData.FileSizeString);
            aMap.Set("hasLdnGames", appData.HasLdnGames);
            aMap.Set("timePlayed", appData.TimePlayedString);
            aMap.Set("isFavorite", appData.Favorite);
            return aMap;
        }
    }
}
