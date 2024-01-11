namespace ShippingParser.Entities;

public class Unsubscriber : IDisposable
{
    private readonly List<IObserver<Box>> _observers;
    private readonly IObserver<Box>? _observer;

    public Unsubscriber(List<IObserver<Box>> observers, IObserver<Box> observer)
    {
        _observers = observers;
        _observer = observer;
    }

    public void Dispose()
    {
        if (_observer != null && _observers.Contains(_observer))
            _observers.Remove(_observer);
    }
}