using System.Collections.Concurrent;

namespace ShippingParser;

public class DataWriter : IObserver<Box>
{
    public DataWriter(FileQueue fileQueue, MyDbContext context)
    {
        _context = context;
    }

    private MyDbContext _context;

    public void OnCompleted()
    {
        Console.WriteLine("Transmission completed.");
    }

    public void OnError(Exception error)
    {
        Console.WriteLine($"Error occurred: {error.Message}");
    }

    public void OnNext(Box value)
    {
        SaveBoxToDatabase(_context, value);
    }
    
    private void SaveBoxToDatabase(MyDbContext context, Box? currentBox)
    {
        if (currentBox != null)
        {
            context.Boxes.Add(currentBox);
        }

        context.SaveChanges();
    }
}