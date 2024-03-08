using System.Text;

namespace NuGetPackagesBackup.Services
{
    public class CommunicationServices
    {
        private StringBuilder logger;

        public CommunicationServices()
        {
            logger = new StringBuilder();
        }

        public void PrintMessage(string message)
        {
            Console.WriteLine(message);
            logger.AppendLine(message);
        }

        public string GetLog()
        {
            return logger.ToString();
        }
    }
}