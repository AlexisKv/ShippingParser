namespace ShippingParser;

public class FileAndPaths
{
    public string DetermineDesktopPath()
    {
        if (Environment.OSVersion.Platform == PlatformID.Win32NT)
        {
           return Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
        }
        else if (Environment.OSVersion.Platform == PlatformID.Unix)
        {
            return Path.Combine(Environment.GetEnvironmentVariable("HOME"), "Desktop");
        }
        else
        {
            Console.WriteLine("Unsupported operating system.");
            Environment.Exit(1);
        }

        return null;
    }

    public void SaveShippingInfoFromFileToDb(object sender, FileSystemEventArgs e)
    {
        if (e.ChangeType == WatcherChangeTypes.Created &&
            Path.GetFileName(e.FullPath).Equals("data.txt", StringComparison.OrdinalIgnoreCase))
        {
            string content = File.ReadAllText(e.FullPath);
            if (string.IsNullOrWhiteSpace(content))
            {
                Console.WriteLine($"Text file '{e.Name}' created, but it is empty.");
                return;
            }
            else
            {
                using (var context = new MyDbContext())
                {
                    context.Database.EnsureCreated();

                    var lines = File.ReadAllLines(e.FullPath);
                    Box currentBox = null;

                    foreach (var line in lines)
                    {
                        if (line.StartsWith("HDR"))
                        {
                            if (currentBox != null) context.MyEntities.Add(currentBox);

                            // New box is being described
                            var tokens = ClearingAndSplittingLine(line);
                            currentBox = new Box()
                            {
                                Identifier = tokens[1], //Should I create new Id or I can use same?
                                SupplierIdentifier = tokens[2], //when it's adding box id?
                                Contents = new List<Box.Content>()
                            };
                        }
                        else if (line.StartsWith("LINE"))
                        {
                            // Product item in the box
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

                    if (currentBox != null) context.MyEntities.Add(currentBox);

                    context.MyEntities.Add(currentBox);
                    context.SaveChanges();
                    Console.WriteLine("Saved to database");
                }
            }
        }
    }

    private string[] ClearingAndSplittingLine(string line)
    {
        return line.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
    }
}