using NuGetPackagesBackup.Services;

namespace NuGetPackagesBackup.Helpers
{
    public static class PathBulder
    {
        public static string GetSolutionPath()
        {
            return Path.Combine(ConfigurationService.GetSolutionDir(), ConfigurationService.GetSolutionName() + ".sln");
        }

        public static string GetGlobalNugetSource(string nugetListGlobalSourceResponse, string areaMarker)
        {
            const string global = "global-packages:";
            List<string> strings = StringHelper.GetTextByLineAndCut(nugetListGlobalSourceResponse, areaMarker);
            string globalPath = strings.Find(item => item.ToLower().StartsWith(global));
            return string.IsNullOrEmpty(globalPath) ? string.Empty : globalPath.Remove(0, global.Length).Trim();
        }

        public static List<string> GetNugetSource(string nugetListSourceResponse, string areaMarker)
        {
            List<string> nugetSource = new List<string>();
            List<string> strings = StringHelper.GetTextByLineAndCut(nugetListSourceResponse, areaMarker);
            IEnumerable<string> nugetPaths = strings.TakeWhile(item => !item.ToLower().StartsWith("warn"));
            foreach (string nugetPath in nugetPaths)
            {
                string path = nugetPath.Remove(0, 1).Trim();
                if (Directory.Exists(path))
                    nugetSource.Add(path);
            }
            return nugetSource;
        }
    }
}
