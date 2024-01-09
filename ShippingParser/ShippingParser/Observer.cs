namespace ShippingParser;

public abstract class Obser // unused??
{
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
        Console.WriteLine("New box added.");
    }
}