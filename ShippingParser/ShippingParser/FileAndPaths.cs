namespace ShippingParser;

public class FileAndPaths
{
    private AsnReading asnReadier = new AsnReading();
    
    public async void SaveShippingInfoFromFileToDbAsync(object sender, FileSystemEventArgs e)
    {
        if (ShouldProcessFile(e))
        {
            var fileInfo = new FileInfo(e.FullPath);

            if (fileInfo.Length <= 0)
            {
                Console.WriteLine($"Text file '{e.Name}' created, but it is empty.");
                return;
            }

            await asnReadier.SaveDataToDatabaseAsync(e.FullPath);
        }
    }

    public string DetermineDesktopPath()
    {
        string? desktopPath = null;

        switch (Environment.OSVersion.Platform)
        {
            case PlatformID.Win32NT:
                desktopPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
                break;
            case PlatformID.Unix:
                desktopPath = Path.Combine(Environment.GetEnvironmentVariable("HOME"), "Desktop");
                break;
            default:
                Console.WriteLine("Unsupported operating system.");
                Environment.Exit(1);
                break;
        }

        return desktopPath;
    }
    

    private async Task SaveDataToDatabaseAsync(string filePath)
    {
        await using var context = new MyDbContext();

        // context.Database.EnsureCreated();

        using var streamReader = new StreamReader(filePath);
        Box? currentBox = null;
        //this will possibility to add multi threading
        // char[] buffer = new char[4096];

        while (!streamReader.EndOfStream)
        {
            string? line = await streamReader.ReadLineAsync();
            ProcessLine(context, line, ref currentBox);
        }

        SavePreviousBoxToDatabase(context, currentBox);
        Console.WriteLine("Shipping info fully saved to database");
    }

    private void SavePreviousBoxToDatabase(MyDbContext context, Box? currentBox)
    {
        if (currentBox != null)
        {
            context.Boxes.Add(currentBox);
        }

        context.SaveChanges();
    }

    private void ProcessLine(MyDbContext context, string line, ref Box currentBox)
    {
        if (line.StartsWith("HDR"))
        {
            SavePreviousBoxToDatabase(context, currentBox);

            var tokens = ClearingAndSplittingLine(line);
            currentBox = new Box()
            {
                Identifier = tokens[1],
                SupplierIdentifier = tokens[2],
                Contents = new List<Box.Content>()
            };
        }
        else if (line.StartsWith("LINE"))
        {
            var tokens = ClearingAndSplittingLine(line);
            var boxContent = new Box.Content
            {
                PoNumber = tokens[1],
                Isbn = tokens[2],
                Quantity = int.Parse(tokens[3])
            };
            currentBox.Contents.Add(boxContent);
        }
    }

    private bool ShouldProcessFile(FileSystemEventArgs e)
    {
        return e.ChangeType == WatcherChangeTypes.Created &&
               Path.GetFileName(e.FullPath).Equals("data.txt", StringComparison.OrdinalIgnoreCase);
    }

    private string[] ClearingAndSplittingLine(string line)
    {
        return line.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
    }
}