using System;
using System.Timers;

public class Timeline : ISaveable
{
    private int currentTimePos;
    private int timelineMaxLength = 29;
    private bool repeatTimeline = true;
    private bool isPaused = false;
    private readonly Timer timer;
    private readonly GameManager gameManager;
    public static event Action<int> OnTimeLineElapsed;
    
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

    public void ToggleRepeatTimeline()
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
        OnTimeLineElapsed?.Invoke(currentTimePos);
    }

    public void RemoveListener()
    {
        if (timer == null) return;
        timer.Elapsed -= TimerElapsed;
    }
}
