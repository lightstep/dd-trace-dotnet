using System;
using System.Linq;

namespace PrepareRelease
{
    public class Program
    {
        public const string Versions = "versions";
        public const string Integrations = "integrations";
        public const string Msi = "msi";

        public static void Main(string[] args)
        {
            if (JobShouldRun(Integrations, args))
            {
                Console.WriteLine("--------------- Integrations Job Started ---------------");
                GenerateIntegrationDefinitions.Run();
                Console.WriteLine("--------------- Integrations Job Complete ---------------");
            }

            if (JobShouldRun(Versions, args))
            {
                Console.WriteLine("--------------- Versions Job Started ---------------");
                SetAllVersions.Run();
                Console.WriteLine("--------------- Versions Job Complete ---------------");
            }

            if (JobShouldRun(Msi, args))
            {
                Console.WriteLine("--------------- MSI Job Started ---------------");
                SyncMsiContent.Run();
                Console.WriteLine("--------------- MSI Job Complete ---------------");
            }
        }

        private static bool JobShouldRun(string jobName, string[] args)
        {
            return args.Length == 0 || args.Any(a => string.Equals(a, jobName, StringComparison.OrdinalIgnoreCase));
        }
    }
}
