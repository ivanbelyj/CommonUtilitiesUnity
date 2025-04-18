using Mirror;
using UnityEditor;
using UnityEngine;

public class GameTimeProvider : MonoBehaviour, IGameTimeProvider
{
    private static GameTimeProvider instance;

    public static GameTimeProvider Instance => instance;

    public static double GameTime => Instance.GetGameTime();

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
