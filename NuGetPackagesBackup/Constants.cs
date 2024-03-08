namespace NuGetPackagesBackup
{
    public static class Constants
    {
        public const string SolutionDir = @"../../src/";

        public const string SolutionName = "UntappdViewer";

        public const string NuGetPackagesBackupDir = @"../../Additions/packages";

        public const string NuGetDistributionDir = @"../nuget";

        public const string NugetListSourceCommand = "dotnet nuget list source --format short";

        public const string NugetGlobalListSourceCommand = "nuget locals all -list";

        public const string NugetUsedBySolutionCommand = "dotnet list package --include-transitive";

        public const string NugetUsedBySolutionJsonCommand = NugetUsedBySolutionCommand + " --format json";

        public const string NuGetPackagesBackupAreaMarker = "NuGetPackagesBackupAreaMarker";

        public const string LogFilePath = "Log.txt";
    }
}