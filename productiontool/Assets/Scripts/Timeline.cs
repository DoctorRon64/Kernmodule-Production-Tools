using System;
using System.Timers;
using UnityEngine;

public class Timeline : ISaveable
{
    private int currentTimePos;
    private int timelineMaxLength = 10;
    private bool repeatTimeline;
    private readonly Timer timer;
    private bool isPaused = false;
    private readonly GameManager gameManager;
    
    public Timeline(GameManager _gameManager)
    {
        gameManager = _gameManager;
        currentTimePos = 0;
        timer = new Timer(1000); //<---- 1000ms = 1s
        timer.Elapsed += TimerElapsed;
    }

    public void Load()
    {
        currentTimePos = gameManager.SaveFile.currentTimePos;
        timelineMaxLength = gameManager.SaveFile.timelineLength;
        repeatTimeline = gameManager.SaveFile.repeatTimeline;
    }

    public void Save()
    {
        gameManager.SaveFile.currentTimePos = currentTimePos;
        gameManager.SaveFile.timelineLength = timelineMaxLength;
        gameManager.SaveFile.repeatTimeline = repeatTimeline;
    }

    public void StartTimeline()
    {
        if (!timer.Enabled)
        {
            if (!isPaused)
                currentTimePos = 0; // Reset only if not paused
            timer.Start();
            isPaused = false;
        }
    }

    public void PauseTimeline()
    {
        if (timer.Enabled)
        {
            timer.Stop();
            isPaused = true;
        }
    }

    public void StopTimeline()
    {
        timer.Stop();
        isPaused = false;
    }

    public void ToggleRepeatTimeline()
    {
        repeatTimeline = !repeatTimeline;
    }

    private void TimerElapsed(object _sender, ElapsedEventArgs _event)
    {
        currentTimePos++;
        Debug.Log(currentTimePos);

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
            //mischien event raisen
        }
    }

    public void RemoveListener()
    {
        if (timer == null) return;
        timer.Elapsed -= TimerElapsed;
    }
}
