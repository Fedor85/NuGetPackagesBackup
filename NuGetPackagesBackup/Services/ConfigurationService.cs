using System.Configuration;

namespace NuGetPackagesBackup.Services
{
    public static class ConfigurationService
    {
        private static string solutionDir = "SolutionDir";

        private static string solutionName = "SolutionName";

        private static string nuGetPackagesBackupDir = "NuGetPackagesBackupDir";

        private static string nuGetDistributionDir = "NuGetDistributionDir";

        private static Dictionary<string, string> items = new();

        public static void Initialize()
        {
            AddItemByConfig(solutionDir, Constants.SolutionDir);
            AddItemByConfig(solutionName, Constants.SolutionName);
            AddItemByConfig(nuGetPackagesBackupDir, Constants.NuGetPackagesBackupDir);
            AddItemByConfig(nuGetDistributionDir, Constants.NuGetDistributionDir);
        }

        public static string GetSolutionDir()
        {
            return GetValue(solutionDir);
        }

        public static string GetSolutionName()
        {
            return GetValue(solutionName);
        }

        public static string GeNuGetPackagesBackupDir()
        {
            return GetValue(nuGetPackagesBackupDir);
        }

        public static string GeNuGetDistributionDir()
        {
            return GetValue(nuGetDistributionDir);
        }

        private static string GetValue(string key)
        {
            return items[key];
        }

        private static void AddItemByConfig(string key, string? defaultValue)
        {
            string value = ConfigurationManager.AppSettings.Get(key);
            if (!string.IsNullOrEmpty(value))
                items.Add(key, value);
            else if (defaultValue != null)
                items.Add(key, defaultValue);
        }
    }
}