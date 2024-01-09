using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using ShippingParser;

public class FileQueue : IObservable<Box>
{
    private readonly ConcurrentQueue<Box> _boxes;
    private readonly List<IObserver<Box>> _observers;

    public FileQueue()
    {
        _boxes = new ConcurrentQueue<Box>();
        _observers = new List<IObserver<Box>>();
    }

    public IDisposable Subscribe(IObserver<Box> observer)
    {
        if (!_observers.Contains(observer))
            _observers.Add(observer);
        return new Unsubscriber(_observers, observer);
    }

    public void AddBox(Box box)
    {
        _boxes.Enqueue(box);
        foreach (var observer in _observers)
        {
            observer.OnNext(box);
        }
    }

    private class Unsubscriber : IDisposable
    {
        private readonly List<IObserver<Box>> _observers;
        private readonly IObserver<Box> _observer;

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
}