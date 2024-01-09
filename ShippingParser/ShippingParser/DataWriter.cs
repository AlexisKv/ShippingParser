namespace ShippingParser;

public class DataWriter
{
    private void SaveBoxToDatabase(MyDbContext context, Box? currentBox)
    {
        if (currentBox != null)
        {
            context.Boxes.Add(currentBox);
        }

        context.SaveChanges();
    }
}