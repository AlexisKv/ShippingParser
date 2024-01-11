namespace ShippingParser.Core;

public class AsnReader
{
    private string _incompleteLine = string.Empty;
    private readonly AsnProcessor _asnProcessor;
    
    public AsnReader(AsnProcessor asnProcessor)
    {
        _asnProcessor = asnProcessor;
    }
    
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

            if (bytesRead is 0)
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
        string bufferContent = _incompleteLine + new string(buffer, 0, bytesRead);

        while (position < bufferContent.Length)
        {
            int lineEnd = bufferContent.IndexOf('\n', position);

            if (lineEnd is -1)
            {
                _incompleteLine = bufferContent.Substring(position);
                break;
            }

            string line = bufferContent.Substring(position, lineEnd - position);
            _asnProcessor.ParseLine(line);
            
            position = lineEnd + 1;
        }

        _incompleteLine = position < bufferContent.Length ? bufferContent.Substring(position) : string.Empty;
    }
}