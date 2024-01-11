using ShippingParser.Entities;

namespace ShippingParser.Core;

public class AsnProcessor
{
    private readonly FilePublisher _filePublisher;

    public AsnProcessor(FilePublisher filePublisher)
    {
        _filePublisher = filePublisher;
    }

    public void ParseLine(string line)
    {
        if (line.StartsWith("HDR"))
        {
            var tokens = ClearingAndSplittingLine(line);
            var currentBox = new Box()
            {
                Identifier = tokens[1],
                SupplierIdentifier = tokens[2],
                Contents = new List<Box.Content>()
            };

            PublishBox(currentBox);
        }
        else if (line.StartsWith("LINE"))
        {
            var tokens = ClearingAndSplittingLine(line);

            var content = new Box.Content
            {
                PoNumber = tokens[1],
                Isbn = tokens[2],
                Quantity = int.Parse(tokens[3])
            };

            var contentBox = new Box()
            {
                Identifier = null,
                SupplierIdentifier = null,
                Contents = new List<Box.Content> { content }
            };

            PublishBox(contentBox);
        }
    }

    private void PublishBox(Box box)
    {
        _filePublisher.Publish(box);
    }

    private string[] ClearingAndSplittingLine(string line)
    {
        return line.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
    }
}