using System;
using System.Timers;
using UnityEngine;

public class Timeline : ISaveable
{
    private int currentTimePos;
    private int timelineMaxLength = 10;
    private bool repeatTimeline;
    private readonly Timer timer;
    private readonly SaveFile saveFile;
    private bool isPaused = false;

    public Timeline(SaveFile _saveFile)
    {
        saveFile = _saveFile ?? throw new ArgumentNullException(nameof(_saveFile));
        currentTimePos = 0;
        timer = new Timer(1000); //<---- 1000ms = 1s
        timer.Elapsed += TimerElapsed;
    }

    public void Load()
    {
        currentTimePos = saveFile.currentTimePos;
        timelineMaxLength = saveFile.timelineLength;
        repeatTimeline = saveFile.repeatTimeline;
    }

    public void Save()
    {
        saveFile.currentTimePos = currentTimePos;
        saveFile.timelineLength = timelineMaxLength;
        saveFile.repeatTimeline = repeatTimeline;
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
