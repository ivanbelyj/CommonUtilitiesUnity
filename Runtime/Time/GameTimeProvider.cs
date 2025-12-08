using Mirror;
using UnityEditor;
using UnityEngine;

public class GameTimeProvider : MonoBehaviour, IGameTimeProvider
{
    private static GameTimeProvider instance;

    public static GameTimeProvider Instance
    {
        get
        {
            if (instance == null)
            {
                Debug.LogError(
                    $"{nameof(GameTimeProvider)} is required in the scene to be used via singleton property. " +
                    "Please, ensure it is created on the scene.");
                return null;
            }
            else
            {
                return instance;
            }   
        }
    }

    public static double GameTime => Instance.GetGameTime();

    /// <summary>
    /// The time in seconds since the unified game timeline started.
    /// </summary>
    public double GetGameTime()
    {
        return NetworkTime.time;
    }

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            instance = this;
        }
    }

    private void OnDestroy()
    {
        if (instance == this)
        {
            instance = null;
        }
    }
}
