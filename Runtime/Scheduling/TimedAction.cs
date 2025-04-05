using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Manages timed execution of actions with configurable intervals.
/// This class provides a flexible way to schedule and manage recurring actions.
/// </summary>
public class TimedAction
{
    /// <summary>
    /// Delegate type for actions to be executed.
    /// </summary>
    public delegate void ActionDelegate();

    private List<Timer> timers = new();
    private float lastUpdateTime;

    /// <summary>
    /// Internal timer data structure.
    /// </summary>
    [System.Serializable]
    private class Timer
    {
        public float interval;
        public float nextExecution;
        public ActionDelegate action;
    }

    /// <summary>
    /// Adds a new action to be executed at specified intervals.
    /// </summary>
    /// <param name="action">The action to execute</param>
    /// <param name="interval">Time in seconds between executions</param>
    public void AddAction(ActionDelegate action, float interval)
    {
        Timer timer = new Timer
        {
            interval = interval,
            nextExecution = Time.time + interval,
            action = action
        };
        timers.Add(timer);
    }

    /// <summary>
    /// Updates all timers and executes actions that are due.
    /// Must be called regularly for the timing system to work.
    /// </summary>
    public void Update()
    {
        float currentTime = Time.time;
        
        for (int i = timers.Count - 1; i >= 0; i--)
        {
            Timer timer = timers[i];
            
            if (currentTime >= timer.nextExecution)
            {
                timer.action?.Invoke();
                timer.nextExecution = currentTime + timer.interval;
            }
        }
        
        lastUpdateTime = currentTime;
    }

    /// <summary>
    /// Stops execution of a specific action.
    /// </summary>
    /// <param name="action">The action to stop</param>
    public void StopAction(ActionDelegate action)
    {
        timers.RemoveAll(timer => timer.action == action);
    }

    /// <summary>
    /// Stops all scheduled actions.
    /// </summary>
    public void StopAllActions()
    {
        timers.Clear();
    }

    /// <summary>
    /// Cleans up all resources.
    /// Call this when the TimedAction instance is no longer needed.
    /// </summary>
    public void Cleanup()
    {
        StopAllActions();
    }
}
