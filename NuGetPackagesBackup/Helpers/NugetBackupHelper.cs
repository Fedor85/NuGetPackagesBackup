namespace NuGetPackagesBackup.Helpers
{
    public static class NugetBackupHelper
    {
        public static bool NugetCopy(string nugetName, string nugetVersion, List<string> nugetDirs, string backupDir)
        {
            string nugetBackupDir = Path.Combine(backupDir, nugetName, nugetVersion);
            if (Directory.Exists(nugetBackupDir))
                return false;

            foreach (string nugetDir in nugetDirs)
            {
                string nugetDirFull = Path.Combine(nugetDir, nugetName, nugetVersion);
                if (!Directory.Exists(nugetDirFull))
                    continue;

                CopyDirectory(nugetDirFull, nugetBackupDir);
                return true;

            }
            return false;
        }

        public static List<string> CheckNotUsedNuGet(string nugetPackagesBackupDir, Dictionary<string, List<string>> currentNugetVersions)
        {
            List<string> notUsedNuGet = new List<string>();
            Dictionary<string, List<string>> backupNuget = new Dictionary<string, List<string>>();
            DirectoryInfo souceDirectoryInfo = new DirectoryInfo(nugetPackagesBackupDir);
            foreach (DirectoryInfo directoryName in souceDirectoryInfo.GetDirectories())
            {
                List<string> versions = new List<string>();
                foreach (DirectoryInfo directoryVersion in directoryName.GetDirectories())
                    versions.Add(directoryVersion.Name);

                backupNuget.Add(directoryName.Name, versions);
            }

            foreach (KeyValuePair<string, List<string>> nuget in backupNuget)
            {
                if (!currentNugetVersions.ContainsKey(nuget.Key))
                {
                    notUsedNuGet.Add(nuget.Key);
                    continue;
                }

                foreach (string version in nuget.Value)
                {
                    if (!currentNugetVersions[nuget.Key].Contains(version))
                        notUsedNuGet.Add($"{nuget.Key} [{version}]");
                }
            }
            return notUsedNuGet;
        }

        private static void CopyDirectory(string source, string target)
        {
            Directory.CreateDirectory(target);
            DirectoryInfo souceDirectoryInfo = new DirectoryInfo(source);
            foreach (FileInfo fileInfo in souceDirectoryInfo.GetFiles())
                fileInfo.CopyTo(Path.Combine(target, fileInfo.Name), true);

            foreach (DirectoryInfo subDir in souceDirectoryInfo.GetDirectories())
            {
                string subDirFull = Path.Combine(target, subDir.Name);
                CopyDirectory(subDir.FullName, subDirFull);
            }
        }
    }
}