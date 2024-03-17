using System;
using System.Collections.Generic;

public enum EventType
{
    Bpm,
    TimerElapse,
    SampleRate,
    Repeat,
    overwrite
}

public static class EventManager
{
    private static Dictionary<EventType, Delegate> eventDictionary = new Dictionary<EventType, Delegate>();

    public static void AddListener<T>(EventType _type, Action<T> _action)
    {
        if (!eventDictionary.ContainsKey(_type))
        {
            eventDictionary.Add(_type, null);
        }
        eventDictionary[_type] = (Action<T>)eventDictionary[_type] + _action;
    }

    public static void RemoveListener<T>(EventType _type, Action<T> _action)
    {
        if (eventDictionary.ContainsKey(_type) && eventDictionary[_type] != null)
        {
            eventDictionary[_type] = (Action<T>)eventDictionary[_type] - _action;
        }
    }

    public static void RemoveAllListeners()
    {
        foreach (var kvp in eventDictionary)
        {
            eventDictionary[kvp.Key] = null;
        }

        Parameterless.RemoveAllListeners();
    }

    public static void InvokeEvent<T>(EventType _type, T _parameter)
    {
        if (eventDictionary.ContainsKey(_type) && eventDictionary[_type] != null)
        {
            ((Action<T>)eventDictionary[_type])?.Invoke(_parameter);
        }
    }

    public class Parameterless
    {
        private static readonly Dictionary<EventType, Action> eventDictionary = new Dictionary<EventType, Action>();

        public static void AddListener(EventType _type, Action _action)
        {
            if (!eventDictionary.ContainsKey(_type))
            {
                eventDictionary.Add(_type, null);
            }
            eventDictionary[_type] += _action;
        }

        public static void RemoveListener(EventType _type, Action _action)
        {
            if (eventDictionary.ContainsKey(_type) && eventDictionary[_type] != null) { }
            eventDictionary[_type] -= _action;
        }

        public static void RemoveAllListeners()
        {
            foreach (var kvp in eventDictionary)
            {
                eventDictionary[kvp.Key] = null;
            }
        }

        public static void InvokeEvent(EventType _type)
        {
            eventDictionary[_type]?.Invoke();
        }
    }
}