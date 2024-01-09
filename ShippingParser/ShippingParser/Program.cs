using ShippingParser;

namespace ShippingParser
{
    public static class Program
    {
        
        static void Main()
        {
            var filesAndPaths =  new FileAndPaths();
            
            string desktopPath = filesAndPaths.DetermineDesktopPath();
            string newFolderName = "ShippingParsing";
            string newFolderPath = Path.Combine(desktopPath, newFolderName);

            var fileQueue = new FileQueue();
            
            if (!Directory.Exists(newFolderPath))
            {
                Directory.CreateDirectory(newFolderPath);
                Console.WriteLine($"Created folder: {newFolderPath}");
            }

            // Set up a FileSystemWatcher to monitor the folder
            var watcher = new FileSystemWatcher(newFolderPath);
            watcher.EnableRaisingEvents = true;
            watcher.Filter = "data.txt";///????
            watcher.Created += filesAndPaths.SaveShippingInfoFromFileToDbAsync;// function is async, should be there await???
            
            Console.WriteLine($"Monitoring folder: {newFolderPath}");

            Console.WriteLine("Press Enter to exit.");
            Console.ReadLine();

            // Clean up the watcher
            watcher.Dispose();
        }
    }
}