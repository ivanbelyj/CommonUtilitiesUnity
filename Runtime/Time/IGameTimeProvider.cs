using UnityEngine;

/// <summary>
/// Provides time in the game world / history timeline
/// </summary>
public interface IGameTimeProvider
{
    double GetGameTime();
}
