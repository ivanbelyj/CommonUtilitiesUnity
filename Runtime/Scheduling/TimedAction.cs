using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Manages timed execution of actions with configurable intervals.
/// This class provides a flexible way to schedule and manage recurring actions.
/// </summary>
public class TimedAction
{
    /// <summary>
    /// Internal timer data structure.
    /// </summary>
    [Serializable]
    private class TimedActionElement
    {
        /// <summary>
        /// Seconds
        /// </summary>
        public float interval;

        /// <summary>
        /// Seconds
        /// </summary>
        public float nextExecutionTime;

        public Action action;
    }

    private List<TimedActionElement> timedActions = new();

    /// <summary>
    /// Adds a new action to be executed at specified intervals.
    /// </summary>
    /// <param name="action">The action to execute</param>
    /// <param name="interval">Time in seconds between executions</param>
    public void AddAction(Action action, float interval)
    {
        var timer = new TimedActionElement
        {
            interval = interval,
            nextExecutionTime = Time.time + interval,
            action = action
        };
        timedActions.Add(timer);
    }

    /// <summary>
    /// Updates all timers and executes actions that are due.
    /// Must be called regularly for the timing system to work.
    /// </summary>
    public void Update()
    {
        var currentTime = Time.time;

        for (var i = timedActions.Count - 1; i >= 0; i--)
        {
            var timedAction = timedActions[i];

            if (currentTime >= timedAction.nextExecutionTime)
            {
                timedAction.action?.Invoke();
                timedAction.nextExecutionTime = currentTime + timedAction.interval;
            }
        }
    }

    /// <summary>
    /// Stops all scheduled actions.
    /// </summary>
    public void StopAllActions()
    {
        timedActions.Clear();
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
