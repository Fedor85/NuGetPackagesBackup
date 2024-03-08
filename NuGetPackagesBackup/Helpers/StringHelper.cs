using System.Text.Json;
using System.Text.Json.Nodes;

namespace NuGetPackagesBackup.Helpers
{
    public static class StringHelper
    {

        public static List<string> GetTextByLineAndCut(string text, string cutMarker)
        {
            List<string> nugetSource = new List<string>();
            List<string> strings = text.Split('\n').Select(item => item.Trim()).ToList();
            return strings.SkipWhile(item => item != cutMarker).Skip(1).TakeWhile(item => item != cutMarker).ToList();
        }

        public static Dictionary<string, List<string>> GetNugetVersions(string jasonText, string areaMarker)
        {
            Dictionary<string, List<string>> nugetVersions = new Dictionary<string, List<string>>();
            List<string> strings = GetTextByLineAndCut(jasonText, areaMarker);
            string result = string.Join("\n", strings);
            JsonNode data = JsonSerializer.Deserialize<JsonNode>(result);

            JsonNode projects = data["projects"];
            if (projects is JsonArray && projects.AsArray().Count == 0)
                return nugetVersions;

            JsonNode frameworks = projects[0]["frameworks"];

            if (frameworks is JsonArray && frameworks.AsArray().Count == 0)
                return nugetVersions;

            JsonNode topLevelPackages = frameworks[0]["topLevelPackages"];
            if (topLevelPackages is JsonArray && topLevelPackages.AsArray().Count != 0)
                FillNugetVersions(nugetVersions, topLevelPackages.AsArray());

            JsonNode transitivePackages = frameworks[0]["transitivePackages"];
            if (transitivePackages is JsonArray && transitivePackages.AsArray().Count != 0)
                FillNugetVersions(nugetVersions, transitivePackages.AsArray());

            return nugetVersions;
        }

        private static void FillNugetVersions(Dictionary<string, List<string>> nugetVersions, JsonArray jsonArray)
        {
            foreach (JsonNode jsonNode in jsonArray)
            {
                string nugetName = jsonNode["id"].ToJsonString().Trim('"');
                string nugetVersion = jsonNode["resolvedVersion"].ToJsonString().Trim('"');
                if (!nugetVersions.ContainsKey(nugetName))
                {
                    nugetVersions.Add(nugetName, [nugetVersion]);
                    continue;
                }

                List<string> nuget = nugetVersions[nugetName];
                if (!nuget.Contains(nugetVersion))
                    nuget.Add(nugetVersion);
            }
        }
    }
}