using System.Diagnostics;
using System.Text;
using UnityEditor;

namespace KRN.Utility
{
    public static class DebugLog
    {
        public enum eLogType { Default, Warning, Error }
        public enum eLogColor { Default, White, Black, Red, Green, Blue, Yellow, Magenta, Cyan }

        private static StringBuilder s_Sb = new StringBuilder(256);

        private const string LOG_DEFINE_SYMBOL = "ENABLE_LOG";

        public static string GetColor(eLogColor inColor)
        {
            // C# 8.0 Expression
            return (inColor switch
            {
                eLogColor.Default => "#a0a0a0ff",
                eLogColor.White => "white",
                eLogColor.Black => "black",
                eLogColor.Red => "red",
                eLogColor.Green => "green",
                eLogColor.Blue => "blue",
                eLogColor.Yellow => "yellow",
                eLogColor.Magenta => "magenta",
                eLogColor.Cyan => "cyan",
                _ => "grey",
            });
        }

        [Conditional(LOG_DEFINE_SYMBOL)]
        public static void Print(eLogColor inColor, object inMessage) { Print_Impl(inColor, eLogType.Default, inMessage); }

        [Conditional(LOG_DEFINE_SYMBOL)]
        public static void Print(object inMessage) { Print_Impl(eLogColor.Default, eLogType.Default, inMessage); }

        [Conditional(LOG_DEFINE_SYMBOL)]
        public static void PrintWarning(object inMessage) { Print_Impl(eLogColor.Yellow, eLogType.Warning, inMessage); }
        [Conditional (LOG_DEFINE_SYMBOL)]
        public static void Warning(object inMessage) { Print_Impl(eLogColor.Yellow, eLogType.Warning, inMessage); }

        [Conditional(LOG_DEFINE_SYMBOL)]
        public static void PrintError(object inMessage) { Print_Impl(eLogColor.Red, eLogType.Error, inMessage); }

        [Conditional (LOG_DEFINE_SYMBOL)]
        public static void Assert(object inMessage) { Print_Impl(eLogColor.Red, eLogType.Error, inMessage); }

#if UNITY_EDITOR
        [Conditional(LOG_DEFINE_SYMBOL)]
        public static void AssetWithDialogue(object inMessage, string inDialogue)
        {
            Assert(inMessage);
            EditorUtility.DisplayDialog("Assert", inDialogue, "Confirm");
        }
#endif

        [Conditional(LOG_DEFINE_SYMBOL)]
        private static void Print_Impl(eLogColor inColor, eLogType inType, object inMessage)
        {
            s_Sb.Length = 0;

            // 색상 정보 열기
            s_Sb.Append("<color=");
            s_Sb.Append(GetColor(inColor));
            s_Sb.Append('>');

            // 메시지를 발생시킨 화면 정보
            s_Sb.Append('[');
            s_Sb.Append(UnityEngine.SceneManagement.SceneManager.GetActiveScene().name);
            s_Sb.Append(']');

            // 타입 별 추가 문구 
            if (inType == eLogType.Warning)
                s_Sb.Append("[Warning] ");
            else if (inType == eLogType.Error)
                s_Sb.Append("[Error] ");

            // 색상 정보 닫기
            s_Sb.Append("</color>");

            // 가공된 메시지를 유니티 디버그에 전달
            if(inType == eLogType.Default)
                UnityEngine.Debug.Log(s_Sb.ToString());
            else if(inType == eLogType.Warning)
                UnityEngine.Debug.LogWarning(s_Sb.ToString());
            else
                UnityEngine.Debug.LogError(s_Sb.ToString());
        }
    }
}
