using System;
using System.Collections.Generic;

public class ActionQueueManager
{
    private static ActionQueueManager instance;
    public static ActionQueueManager Instance
    {
        get
        {
            if (instance == null)
            {
                instance = new ActionQueueManager();
            }
            return instance;
        }
    }

    private readonly Queue<Action> actionQueue = new Queue<Action>();

    private ActionQueueManager() { }

    public void EnqueueAction(Action _action)
    {
        lock (actionQueue)
        {
            actionQueue.Enqueue(_action);
        }
    }

    public void ExecuteActions()
    {
        lock (actionQueue)
        {
            while (actionQueue.Count > 0)
            {
                Action action = actionQueue.Dequeue();
                action?.Invoke();
            }
        }
    }

    public void ClearQueue()
    {
        lock (actionQueue)
        {
            actionQueue.Clear();
        }
    }
}