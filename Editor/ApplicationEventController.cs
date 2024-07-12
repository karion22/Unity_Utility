using UnityEditor;
using UnityEngine;

namespace KRN.Utility
{
    public class ApplicationEventController : MonoBehaviour
    {
        private static ApplicationEventController s_Instance = null;
        public static bool s_IsPause;
        public static float s_PausedTime = 0f;
        public static bool s_IsQuit;

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

        private void Awake()
        {
#if UNITY_EDITOR
            EditorApplication.playModeStateChanged += EditorApplication_playModeStateChanged;
            EditorApplication.pauseStateChanged += EditorApplication_pauseStateChanged;
#endif
        }

#if UNITY_EDITOR
        private static void EditorApplication_playModeStateChanged(PlayModeStateChange inPlayModeState)
        {
            if(inPlayModeState == PlayModeStateChange.ExitingPlayMode)
            {
                s_IsQuit = true;

                EditorApplication.playModeStateChanged -= EditorApplication_playModeStateChanged;
                EditorApplication.pauseStateChanged -= EditorApplication_pauseStateChanged;
            }
        }

        private static void EditorApplication_pauseStateChanged(PauseState inPauseState)
        {
            s_IsPause = (inPauseState == PauseState.Paused);
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
    }
}
