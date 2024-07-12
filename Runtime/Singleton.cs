using UnityEngine;

public class Singleton<T> : MonoBehaviour where T : Singleton<T>
{
    private static T _instance = null;

    public static bool IsInstantiate { get { return _instance != null; } }

    public static T Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindAnyObjectByType<T>() as T;
                if (_instance == null)
                    _instance = new GameObject(typeof(T).ToString(), typeof(T)).GetComponent<T>();
                else
                    CreateInstance(_instance);
            }

            return _instance;
        }
    }

    private static void CreateInstance(Object inTarget)
    {
        _instance = inTarget as T;
        if (_instance != null)
        {
            _instance.Start();
        }
    }

    public virtual void Awake()
    {
        CreateInstance(this);
    }

    public virtual void Start()
    {
        DontDestroyOnLoad(_instance);
    }

    public void OnDestroy()
    {
        _instance = null;
    }

    public void OnApplicationQuit()
    {
        OnDestroy();
    }
}
