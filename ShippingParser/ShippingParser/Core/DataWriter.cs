using ShippingParser.Db;
using ShippingParser.Entities;

namespace ShippingParser.Core;

public class DataWriter : IObserver<Box>
{
    private int _lastBoxId;
    private readonly AsnDbContext _context;
    
    public DataWriter(AsnDbContext context)
    {
        _context = context;
    }

    public void OnCompleted()
    {
        Console.WriteLine("Writing of 1 line completed.");
    }

    public void OnError(Exception error)
    {
        Console.WriteLine($"Error occurred: {error.Message}");
    }

    public void OnNext(Box value)
    {
        if (value.SupplierIdentifier is not null && value.Identifier is not null)
        {
            SaveBoxToDatabase(_context, value);
        }
        else
        {
            SaveContentToDatabase(_context, value);
        }
    }

    private void SaveBoxToDatabase(AsnDbContext context, Box currentBox)
    {
        context.Boxes.Add(currentBox);
        context.SaveChanges();
        _lastBoxId = currentBox.Id;
    }

    private void SaveContentToDatabase(AsnDbContext context, Box currentBox)
    {
        context.Boxes.Find(_lastBoxId).Contents.Add(currentBox.Contents.FirstOrDefault());
        context.SaveChanges();
    }
}