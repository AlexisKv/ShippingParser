namespace ShippingParser;

public class AsnReading
{
    public AsnReading(FileQueue fileQueue)
    {
        _fileQueue = fileQueue;
    }

    private FileQueue _fileQueue;
    private string _incompleteLine = string.Empty;

    public void StartReading(object sender, FileSystemEventArgs e)
    {
        Task.Run(() => StartReadingAsync(sender, e));
    }

    private async Task StartReadingAsync(object sender, FileSystemEventArgs e)
    {
        using var streamReader = new StreamReader(e.FullPath);
        Box? currentBox = null;
        char[] buffer = new char[4096];

        bool moreContentToRead = true;

        while (moreContentToRead)
        {
            int bytesRead = await streamReader.ReadAsync(buffer, 0, buffer.Length);

            if (bytesRead == 0)
            {
                moreContentToRead = false;
            }
            else
            {
                ProcessBuffer(buffer, bytesRead, ref currentBox);
            }
        }

        if (currentBox != null) AddBox(currentBox);

        Console.WriteLine("Shipping info fully saved to database");
    }

    private void ProcessBuffer(char[] buffer, int bytesRead, ref Box currentBox)
    {
        int position = 0;

        // If there is an incomplete line from the previous buffer, prepend it
        string bufferContent = _incompleteLine + new string(buffer, 0, bytesRead);

        while (position < bufferContent.Length)
        {
            // Locate the end of the line within the buffer
            int lineEnd = bufferContent.IndexOf('\n', position);

            // If lineEnd is -1, it means the end of the line is not found in the current buffer
            if (lineEnd == -1)
            {
                // Save the incomplete line and break
                _incompleteLine = bufferContent.Substring(position);
                break;
            }

            // Extract the line from the buffer
            string line = bufferContent.Substring(position, lineEnd - position);

            // Process the line
            ProcessLine(line, ref currentBox);

            // Move the position to the next line
            position = lineEnd + 1;
        }

        // Save any remaining incomplete line if the stream has ended
        if (position < bufferContent.Length)
        {
            _incompleteLine = bufferContent.Substring(position);
        }
        else
        {
            _incompleteLine = string.Empty;
        }
    }

    private void ProcessLine(string line, ref Box? currentBox)
    {
        if (line.StartsWith("HDR"))
        {
            if (currentBox != null)
            {
                AddBox(currentBox);
            }

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

    private string[] ClearingAndSplittingLine(string line)
    {
        return line.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
    }

    private void AddBox(Box box)
    {
        _fileQueue.AddBox(box);
    }
}