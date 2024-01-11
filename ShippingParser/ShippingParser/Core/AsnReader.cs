using ShippingParser.Entities;

namespace ShippingParser.Core;

public class AsnReader
{
    public AsnReader(AsnProcessor asnProcessor)
    {
        _asnProcessor = asnProcessor;
    }

    private string _incompleteLine = string.Empty;
    private AsnProcessor _asnProcessor;

    public void Start(object sender, FileSystemEventArgs e)
    {
        Task.Run(() => StartReadingAsync(e.FullPath));
    }

    private async Task StartReadingAsync(string fullPath)
    {
        using var streamReader = new StreamReader(fullPath);
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
                ProcessBuffer(buffer, bytesRead);
            }
        }
        
        Console.WriteLine("Shipping info fully saved to database");
    }

    private void ProcessBuffer(char[] buffer, int bytesRead)
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
            _asnProcessor.ParseLine(line);

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

    //Parser class


    private string[] ClearingAndSplittingLine(string line)
    {
        return line.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
    }
}