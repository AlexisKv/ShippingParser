using Microsoft.Extensions.DependencyInjection;

namespace ShippingParser;

public static class Program
{
    static void Main()
    {
        var fileQueue = new FileQueue();
       
        
        var context = new MyDbContext();
        context.Database.EnsureCreated();
        
        var dataWriter = new DataWriter(fileQueue, context);
        var filesAndPaths = new FileAndPaths();

        fileQueue.Subscribe(dataWriter);

        string desktopPath = filesAndPaths.DetermineDesktopPath();
        string newFolderName = "ShippingParsing";
        string newFolderPath = Path.Combine(desktopPath, newFolderName);
        
        var asnReading = new AsnReading(fileQueue);


        if (!Directory.Exists(newFolderPath))
        {
            Directory.CreateDirectory(newFolderPath);
            Console.WriteLine($"Created folder: {newFolderPath}");
        }

        // Set up a FileSystemWatcher to monitor the folder
        var watcher = new FileSystemWatcher(newFolderPath);
        watcher.EnableRaisingEvents = true;
        watcher.Filter = "data.txt";

        watcher.Created += asnReading.StartReading;
        
        Console.WriteLine($"Monitoring folder: {newFolderPath}");

        Console.WriteLine("Press Enter to exit.");
        Console.ReadLine();

        // Clean up the watcher
        watcher.Dispose();
    }

    // public static void ConfigureServices(IServiceCollection services) Should I add DI?
    // {
    //     services.AddSingleton<FileQueue>()
    //         .AddSingleton<DataWriter>()
    //         .AddSingleton<AsnReading>()
    //         .AddSingleton<FileAndPaths>();
    // }
}