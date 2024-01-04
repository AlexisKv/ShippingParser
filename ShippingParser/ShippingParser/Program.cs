using ShippingParser;

namespace ShippingParser
{
    public static class Program
    {
        private static string _desktopPath;
        
        static void Main()
        {
            var filesAndPaths =  new FileAndPaths();
            
            _desktopPath = filesAndPaths.DetermineDesktopPath();
            
            string newFolderName = "ShippingParsing";
            string newFolderPath = Path.Combine(_desktopPath, newFolderName);
            
            if (!Directory.Exists(newFolderPath))
            {
                Directory.CreateDirectory(newFolderPath);
                Console.WriteLine($"Created folder: {newFolderPath}");
            }

            // Set up a FileSystemWatcher to monitor the folder
            FileSystemWatcher watcher = new FileSystemWatcher(newFolderPath);
            watcher.EnableRaisingEvents = true;
            watcher.Created += filesAndPaths.SaveShippingInfoFromFileToDb;

            Console.WriteLine($"Monitoring folder: {newFolderPath}");

            Console.WriteLine("Press Enter to exit.");
            Console.ReadLine();

            // Clean up the watcher
            watcher.Dispose();
        }
    }
}