using System;
using System.Timers;
using UnityEngine;

public class Timeline : ISaveable, ISaveSettings
{
    private int currentTimePos;
    private int timelineMaxLength = 29;
    private bool repeatTimeline = true;
    private bool isPaused = false;
    private int BPM = 60;
    
    private readonly Timer timer;
    private readonly GameManager gameManager;
    
    public Timeline(GameManager _gameManager)
    {
        gameManager = _gameManager;
        currentTimePos = 0;
        
        int milliSeconds = 60000 / BPM;
        timer = new Timer(milliSeconds);
        timer.Elapsed += TimerElapsed;
        
        EventManager.AddListener<int>(EventType.Bpm, ChangeBpm);
        EventManager.Parameterless.AddListener(EventType.Repeat, ToggleRepeatTimeline);
    }

    private void ChangeBpm(int _newBpm)
    {
        BPM = _newBpm;
        int milliSeconds = 60000 / BPM;
        timer.Interval = milliSeconds;
    }

    public int GetBpm()
    {
        return BPM;
    }

    public void StartTimeline()
    {
        if (timer.Enabled) return;
        if (!isPaused)
            currentTimePos = 0;
        timer.Start();
        isPaused = false;
    }
    
    public void PauseTimeline()
    {
        if (!timer.Enabled) return;
        timer.Stop();
        isPaused = true;
    }

    public void StopTimeline()
    {
        timer.Stop();
        isPaused = false;
    }

    private void ToggleRepeatTimeline()
    {
        repeatTimeline = !repeatTimeline;
    }

    private void TimerElapsed(object _sender, ElapsedEventArgs _event)
    {
        if (currentTimePos >= timelineMaxLength)
        {
            if (repeatTimeline)
            {
                currentTimePos = 0;
                StartTimeline();
            }
            else
            {
                StopTimeline();
            }
        }
        
        currentTimePos++;
        EventManager.InvokeEvent(EventType.TimerElapse, currentTimePos);
    }

    public void RemoveListener()
    {
        if (timer == null) return;
        timer.Elapsed -= TimerElapsed;
    }

    public void Load(SaveFile _save)
    {
        BPM = _save.BPM;
    }

    public void Save(SaveFile _load)
    {
        _load.BPM = BPM;
    }

    public void Load(SettingsFile _save)
    {
        repeatTimeline = _save.RepeatTimeline;
    }

    public void Save(SettingsFile _load)
    {
        _load.RepeatTimeline = repeatTimeline;
    }
}
