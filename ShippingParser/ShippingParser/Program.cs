using Microsoft.Extensions.DependencyInjection;
using ShippingParser.Core;
using ShippingParser.Db;

namespace ShippingParser;

public static class Program
{
    static void Main()
    {
        var (filePublisher, dataWriter, asnReading) = InitializeAll();

        filePublisher.Subscribe(dataWriter);

        string folderPath = GetFolderPath();

        var watcher = SetUpWatcher(folderPath, asnReading);

        Console.WriteLine("Press Enter to exit.");
        Console.ReadLine();

        watcher.Dispose();
    }

    private static (FilePublisher,  DataWriter, AsnReader) InitializeAll()
    {
        var filePublisher = new FilePublisher();
        var context = new AsnDbContext();
        var asnProcessor = new AsnProcessor(filePublisher);
        var dataWriter = new DataWriter(context);
        var asnReading = new AsnReader(asnProcessor);

        return (filePublisher, dataWriter, asnReading);
    }

    private static string GetFolderPath()
    {
        string desktopPath = FileManager.DetermineDesktopPath();
        string newFolderName = "ShippingParsing";
        string newFolderPath = Path.Combine(desktopPath, newFolderName);

        if (!Directory.Exists(newFolderPath))
        {
            Directory.CreateDirectory(newFolderPath);
            Console.WriteLine($"Created folder: {newFolderPath}");
        }

        return newFolderPath;
    }

    private static FileSystemWatcher SetUpWatcher(string newFolderPath, AsnReader asnReading)
    {
        var watcher = new FileSystemWatcher(newFolderPath);
        watcher.EnableRaisingEvents = true;
        watcher.Filter = "data.txt";

        watcher.Created += asnReading.Start;
        
        Console.WriteLine($"Monitoring folder: {newFolderPath}");

        return watcher;
    }
}