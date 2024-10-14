using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static KRN.Utility.Resolution;

namespace KRN.Utility
{
    public class Resolution : MonoBehaviour
    {
        public class ResolutionChangedEventArgs : EventArgs
        {
            public int NewWidth;
            public int NewHeight;
            public int PrevWidth;
            public int PrevHeight;
        }

        public event EventHandler<ResolutionChangedEventArgs> ResolutionChanged;
        public bool OnStartUp = true;
        public int TimeSleepValue = SleepTimeout.NeverSleep;
        private bool m_bChangedResolution = false;

        public enum eResolutionMatch { Width, Height }

        private ResolutionChangedEventArgs m_Args = new ResolutionChangedEventArgs();
        private eResolutionMatch m_MatchType = eResolutionMatch.Width;

        private void Awake()
        {
#if UNITY_IOS || UNITY_ANDROID
            SetFrameRate(30);
#else
            SetFrameRate(60);
#endif
            Screen.sleepTimeout = TimeSleepValue;
        }

        private void Start()
        {
            SetResolution(Screen.width, Screen.height);
            ResolutionChanged = null;
            ResolutionChanged += new EventHandler<ResolutionChangedEventArgs>(OnResolutionChanged);
        }

        private void OnDestroy()
        {
            if(ResolutionChanged != null)
                ResolutionChanged -= new EventHandler<ResolutionChangedEventArgs>(OnResolutionChanged);
        }

        public void SetFrameRate(int inFrameRate)
        {
#if !UNITY_IOS && !UNITY_ANDROID
            QualitySettings.vSyncCount = 1;
#endif
            Application.targetFrameRate = inFrameRate;
        }

        private void OnResolutionChanged(object inSender, ResolutionChangedEventArgs inArgs)
        {
            DebugLog.Print(Utility.BuildString("Resolution Changed : {0} / {1} -> {2} / {3}", inArgs.PrevWidth, inArgs.PrevHeight, inArgs.NewWidth, inArgs.NewHeight));

            m_Args.NewWidth = inArgs.NewWidth;
            m_Args.NewHeight = inArgs.NewHeight;
            m_Args.PrevWidth = inArgs.PrevWidth;
            m_Args.PrevHeight = inArgs.PrevHeight;
        }

        public void SetResolution(int inWidth, int inHeight)
        {

        }
    }
}
