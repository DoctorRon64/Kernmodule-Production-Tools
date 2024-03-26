using System;
using System.Collections.Generic;

public enum EventType
{
    Bpm,
    TimerElapse,
    SampleRate,
    Repeat,
    OverwriteToggle,
    SelectTool,
    ButtonHoverText,
    OnMouseHover
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
        if (eventDictionary.TryGetValue(_type, out Delegate currentEvent))
        {
            if (currentEvent != null)
            {
                eventDictionary[_type] = (Action<T>)currentEvent - _action;
            }
        }
    }

    public static void RemoveAllListeners()
    {
        eventDictionary.Clear();
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
            eventDictionary.Clear();
        }

        public static void InvokeEvent(EventType _type)
        {
            eventDictionary[_type]?.Invoke();
        }
    }
}