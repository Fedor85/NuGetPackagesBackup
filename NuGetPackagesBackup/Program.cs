// See https://aka.ms/new-console-template for more information
using NuGetPackagesBackup;
using NuGetPackagesBackup.Helpers;
using NuGetPackagesBackup.Services;
using System.Reflection;

internal class Program
{
    static void Main(string[] args)
    {
        CommunicationServices communicationServices = new CommunicationServices();
        ConfigurationService.Initialize();

        communicationServices.PrintMessage($"{Assembly.GetExecutingAssembly().GetName()} Run...");

        string solutionPath = Path.GetFullPath(PathBulder.GetSolutionPath());
        communicationServices.PrintMessage(solutionPath);
        if (!File.Exists(solutionPath))
        {
            End(communicationServices, "Solution file not found.");
            return;
        }

        List<string> nugetSource = new List<string>();

        CMDProcess nugetGlobalSource = new CMDProcess(Constants.NuGetPackagesBackupAreaMarker);
        nugetGlobalSource.RunCD(ConfigurationService.GeNuGetDistributionDir());
        nugetGlobalSource.Run(Constants.NugetGlobalListSourceCommand);
        string nugetListGlobalSourceResponse = nugetGlobalSource.Close();
        communicationServices.PrintMessage(nugetListGlobalSourceResponse);
        string nugetGlobalPath = PathBulder.GetGlobalNugetSource(nugetListGlobalSourceResponse, Constants.NuGetPackagesBackupAreaMarker);
        if (!String.IsNullOrEmpty(nugetGlobalPath))
            nugetSource.Add(nugetGlobalPath);

        CMDProcess nugetSourceCMD = new CMDProcess(Constants.NuGetPackagesBackupAreaMarker);
        nugetSourceCMD.Run(Constants.NugetListSourceCommand);
        string nugetListSourceResponse = nugetSourceCMD.Close();
        communicationServices.PrintMessage(nugetListSourceResponse);

        nugetSource.AddRange(PathBulder.GetNugetSource(nugetListSourceResponse, Constants.NuGetPackagesBackupAreaMarker));
        if (nugetSource.Count == 0)
        {
            End(communicationServices, "Directory NuGet source not recognized.");
            return;
        }

        communicationServices.PrintMessage("Found nuget source directories:");
        foreach (string path in nugetSource)
            communicationServices.PrintMessage(path);

        CMDProcess nugetUsedBySolution = new CMDProcess();
        nugetUsedBySolution.RunCD(ConfigurationService.GetSolutionDir());
        nugetUsedBySolution.Run(Constants.NugetUsedBySolutionCommand);
        communicationServices.PrintMessage(nugetUsedBySolution.Close());

        CMDProcess nugetUsedBySolutionJson = new CMDProcess(Constants.NuGetPackagesBackupAreaMarker);
        nugetUsedBySolutionJson.RunCD(ConfigurationService.GetSolutionDir());
        nugetUsedBySolutionJson.Run(Constants.NugetUsedBySolutionJsonCommand);
        Dictionary<string, List<string>> nugetVersions = StringHelper.GetNugetVersions(nugetUsedBySolutionJson.Close(), Constants.NuGetPackagesBackupAreaMarker);

        string nugetPackagesBackupDir = Path.GetFullPath(ConfigurationService.GeNuGetPackagesBackupDir());
        communicationServices.PrintMessage($"NuGet packages backup dir: {nugetPackagesBackupDir}");
        foreach (KeyValuePair<string, List<string>> nugetVersion in nugetVersions)
        {
            foreach (string version in nugetVersion.Value)
            {
                if (NugetBackupHelper.NugetCopy(nugetVersion.Key, version, nugetSource, nugetPackagesBackupDir))
                    communicationServices.PrintMessage($"Copy: {nugetVersion.Key} [{version}]");
            }
        }

        List<string> notUsedNuGet = NugetBackupHelper.CheckNotUsedNuGet(nugetPackagesBackupDir, nugetVersions);
        if (notUsedNuGet.Count > 0)
        {
            communicationServices.PrintMessage("Remove unused Nuget packages:");
            foreach (string nuget in notUsedNuGet)
                communicationServices.PrintMessage(nuget);
        }

        End(communicationServices);
    }

    private static void End(CommunicationServices communicationServices, string message = null)
    {
        if (!String.IsNullOrEmpty(message))
            communicationServices.PrintMessage(message);

        communicationServices.PrintMessage("End.");
        File.WriteAllText(Constants.LogFilePath, communicationServices.GetLog());
        Console.ReadLine();
    }
}