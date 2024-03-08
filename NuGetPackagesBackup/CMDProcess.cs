using System.Diagnostics;

namespace NuGetPackagesBackup
{
    public class CMDProcess
    {
        private Process cmd;

        private string areaMarker;

        public CMDProcess(string areaMarker = null)
        {
            cmd = new Process();
            this.areaMarker = areaMarker;
            cmd.StartInfo.FileName = "cmd.exe";
            cmd.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
            cmd.StartInfo.RedirectStandardInput = true;
            cmd.StartInfo.RedirectStandardOutput = true;
            cmd.StartInfo.CreateNoWindow = true;
            cmd.StartInfo.UseShellExecute = false;
            cmd.Start();
        }

        public void RunCD(string path)
        {
            cmd.StandardInput.WriteLine($"cd {path}");
            cmd.StandardInput.Flush();
        }

        public void Run(string command)
        {
            string currentCommand = String.IsNullOrEmpty(areaMarker) ? command : $"echo {areaMarker} && {command} && echo {areaMarker}";
            cmd.StandardInput.WriteLine(currentCommand);
            cmd.StandardInput.Flush();
        }

        public string Close()
        {
            cmd.StandardInput.Close();
            return cmd.StandardOutput.ReadToEnd();
        }
    }
}