using ShippingParser.Entities;

namespace ShippingParser.Core;

public class FilePublisher : IObservable<Box>
{
    private readonly List<IObserver<Box>> _observers = new();

    public IDisposable Subscribe(IObserver<Box> observer)
    {
        if (!_observers.Contains(observer))
            _observers.Add(observer);

        return new Unsubscriber(_observers, observer);
    }

    public void Publish(Box box)
    {
        foreach (var observer in _observers)
        {
            observer.OnNext(box);
        }
    }
}