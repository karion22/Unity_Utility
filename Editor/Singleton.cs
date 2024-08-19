using UnityEngine;

namespace KRN.Utility
{
    public abstract class Singleton<T> : MonoBehaviour where T : Singleton<T>
    {
        private static T s_Instance = null;

        public static bool IsInstantiate { get { return s_Instance != null; } }

        public static T Instance
        {
            get
            {
                if (s_Instance == null)
                {
                    if(ApplicationEventController.s_IsQuit)
                    {
#if UNITY_EDITOR
                        DebugLog.Assert(Utility.BuildString("{0} - Create after application quit", typeof(T).ToString()));
#endif
                        return default(T);
                    }

                    s_Instance = FindAnyObjectByType<T>() as T;
                    if (s_Instance == null)
                        s_Instance = new GameObject(typeof(T).ToString(), typeof(T)).GetOrAddComponent<T>();
                    else
                        CreateInstance(s_Instance);
                }

                if (s_Instance == null)
                    DebugLog.Assert(Utility.BuildString("{0} - Problem to create instatnce", typeof(T).ToString()));

                return s_Instance;
            }
        }

        private static void CreateInstance(Object inTarget)
        {
            s_Instance = inTarget as T;
            if (s_Instance != null)
                s_Instance.Start();
        }

        public virtual void Awake()
        {
            CreateInstance(this);
        }

        public virtual void Start()
        {
#if UNITY_EDITOR
            if(Application.isPlaying)
                DontDestroyOnLoad(s_Instance);
#else
            DontDestroyOnLoad(s_Instance);
#endif
        }

        public void OnDestroy()
        {
            s_Instance = null;
        }

        public void OnApplicationQuit()
        {
            s_Instance = null;
        }
    }
}