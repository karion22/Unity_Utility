using UnityEditor;
using UnityEngine;
using UnityEngine.Events;

namespace KRN.Utility
{
    public class ApplicationEventController : MonoBehaviour
    {
        private static ApplicationEventController s_Instance = null;
        public static bool s_IsPause;
        public static float s_PausedTime = 0f;
        public static bool s_IsQuit;
        private static Vector2 s_WindowSize = Vector2.zero;

        public static UnityAction onApplicationPlayed = null;
        public static UnityAction<bool> onApplicationPaused = null;
        public static UnityAction onApplicationQuited = null;
        public static UnityAction<Vector2> onApplicationSizeChanged = null;

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void ApplicationEventController_Init()
        {
            if (!Application.isPlaying) return;
            s_IsQuit = false;

            if(s_Instance == null)
            {
                GameObject go = new GameObject("ApplicationEventController");
                go.hideFlags = HideFlags.HideAndDontSave;
                s_Instance = go.AddComponent<ApplicationEventController>();
                GameObject.DontDestroyOnLoad(go);
            }
        }

        public static ApplicationEventController Instance
        {
            get
            {
                if (s_Instance == null)
                {
                    if (s_IsQuit)
                    {
#if UNITY_EDITOR
                        DebugLog.Assert(Utility.BuildString("{0} - Create after application quit", typeof(ApplicationEventController).ToString()));
#endif
                        return null;
                    }

                    s_Instance = FindAnyObjectByType<ApplicationEventController>();
                    if (s_Instance == null)
                        s_Instance = new GameObject("ApplicationEventController", typeof(ApplicationEventController)).GetOrAddComponent<ApplicationEventController>();
                    else
                    {
                        if (s_Instance != null)
                        {
#if UNITY_EDITOR
                            if (Application.isPlaying)
                                DontDestroyOnLoad(s_Instance);
#else
            DontDestroyOnLoad(s_Instance);
#endif
                        }
                    }
                }

                if (s_Instance != null)
                    DebugLog.Assert(Utility.BuildString("{0} - Problem to create instatnce", typeof(ApplicationEventController).ToString()));

                return s_Instance;
            }
        }

        private void Awake()
        {
#if UNITY_EDITOR
            EditorApplication.playModeStateChanged += EditorApplication_playModeStateChanged;
            EditorApplication.pauseStateChanged += EditorApplication_pauseStateChanged;
#endif
            s_WindowSize = new Vector2(Screen.width, Screen.height);
        }

#if UNITY_EDITOR
        private static void EditorApplication_playModeStateChanged(PlayModeStateChange inPlayModeState)
        {
            switch(inPlayModeState)
            {
                case PlayModeStateChange.EnteredPlayMode:
                    onApplicationPlayed?.Invoke();
                    break;

                case PlayModeStateChange.ExitingPlayMode:
                    {
                        s_IsQuit = true;

                        EditorApplication.playModeStateChanged -= EditorApplication_playModeStateChanged;
                        EditorApplication.pauseStateChanged -= EditorApplication_pauseStateChanged;
                        onApplicationQuited?.Invoke();
                    }
                    break;

            }
        }

        private static void EditorApplication_pauseStateChanged(PauseState inPauseState)
        {
            DebugLog.Print(Utility.BuildString("Application pause state changed : {0}", inPauseState.ToString()));

            s_IsPause = (inPauseState == PauseState.Paused);
            onApplicationPaused?.Invoke(s_IsPause);
        }
#endif

        private void OnApplicationQuit()
        {
            DebugLog.Print("OnApplication Quit called");

            if (!s_IsQuit)
                s_IsQuit = true;
        }

        private void OnApplicationFocus(bool inFocus)
        {
            s_IsPause = !inFocus;

            if (s_IsPause)
                s_PausedTime = Time.realtimeSinceStartup;
        }

        private void OnApplicationPause(bool isPause)
        {
            if (isPause)
                s_PausedTime = Time.realtimeSinceStartup;

            s_IsPause = isPause;
        }

        private void Update()
        {
            if(onApplicationSizeChanged != null)
            {
                if(s_WindowSize.x != Screen.width ||  s_WindowSize.y != Screen.height)
                {
                    s_WindowSize = new Vector2(Screen.width, Screen.height);
                    onApplicationSizeChanged?.Invoke(s_WindowSize);
                    DebugLog.Print(Utility.BuildString("Application size changed. {0}", s_WindowSize));
                }
            }
        }
    }
}
