using System;
using System.Collections.Generic;
public abstract class Event
{
    private readonly List<Action> _callbacks = new List<Action>();

    public void Subscribe(Action callback)
    {
        _callbacks.Add(callback);
    }

    public void Publish()
    {
        foreach (Action callback in _callbacks)
        {
            callback();
        }
    }
}
