namespace ShippingParser;

public class AsnReading
{
    private string _incompleteLine = string.Empty;

    public async Task SaveDataToDatabaseAsync(string filePath)
    {
        await using var context = new MyDbContext();

        await context.Database.EnsureCreatedAsync();

        using var streamReader = new StreamReader(filePath);
        Box? currentBox = null;
        char[] buffer = new char[1096]; // Chunk size (buffer size)4096

        // Chunk 1: Reading lines in chunks
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
                ProcessBuffer(context, buffer, bytesRead, ref currentBox);
            }
        }

        // // Chunk 2: Save any remaining incomplete line
        // if (!string.IsNullOrEmpty(_incompleteLine))
        // {
        //     ProcessLine(context, _incompleteLine, ref currentBox);
        // }

        // Chunk 3: Saving the last box and displaying a message
        SavePreviousBoxToDatabase(context, currentBox);
        Console.WriteLine("Shipping info fully saved to database");
    }

    private void ProcessBuffer(MyDbContext context, char[] buffer, int bytesRead, ref Box currentBox)
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
            ProcessLine(context, line, ref currentBox);

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

    private void ProcessLine(MyDbContext context, string line, ref Box currentBox)
    {
        // Existing logic for processing lines
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

    private string[] ClearingAndSplittingLine(string line)
    {
        return line.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
    }

    private void SavePreviousBoxToDatabase(MyDbContext context, Box? currentBox)
    {
        if (currentBox != null)
        {
            context.Boxes.Add(currentBox);
        }

        context.SaveChanges();
    }
}