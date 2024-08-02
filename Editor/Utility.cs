using System;
using System.Globalization;
using System.Text;
using UnityEngine;

namespace KRN.Utility
{
    public static class Utility
    {
        private static StringBuilder m_StrBuilder = new StringBuilder();

        public static string BuildString(string inFormat, params object[] inArgs)
        {
            m_StrBuilder.Length = 0;
            m_StrBuilder.AppendFormat(inFormat, inArgs);
            return m_StrBuilder.ToString();
        }

        public static string CommaFormat(int inValue)
        {
            return string.Format(CultureInfo.InvariantCulture, "{0:N0}", inValue);
        }

        public static string CommaFormat(FormattableString inSource, string inCulture)
        {
            return string.Format(CultureInfo.CreateSpecificCulture(inCulture), inSource.Format, inSource.GetArgument(0));
        }

        public static string GetSpecificCulture(string inNationName, CultureTypes inType = CultureTypes.AllCultures)
        {
            var cultures = CultureInfo.GetCultures(inType);

            foreach (var culture in cultures)
            {
                if (culture.EnglishName.Contains(inNationName) == false) continue;

                try
                {
                    CultureInfo cultureInfo = CultureInfo.CreateSpecificCulture(culture.Name);
                    return cultureInfo.Name;
                }
                catch
                {
                    return string.Empty;
                }
            }
            return string.Empty;
        }

        public static string GetConvertDate(DateTime inDateTime, string inFormat = "yyyy/mm/dd")
        {
            return inDateTime.ToString(inFormat);
        }

        public static string GetCurrentDate(string inFormat = "yyyy/mm/dd")
        {
            return System.DateTime.Now.ToString(inFormat);
        }

        public static Transform FindEx(this Transform inCurrent, string inName)
        {
            if (inCurrent.name == inName)
                return inCurrent;

            for (int i = 0, end = inCurrent.childCount; i < end; i++)
            {
                Transform trans = FindEx(inCurrent.GetChild(i), inName);
                if (trans != null)
                    return trans;
            }

            return null;
        }

        public static T FindEx<T>(this Transform inCurrent, string name)
        {
            var child = FindEx(inCurrent, name);
            if (child == null)
                return default(T);
            else
                return child.GetComponent<T>();
        }

        public static void FindEx(this Transform inCurrent, out Transform inDest, string inName)
        {
            inDest = inCurrent.FindEx(inName);
        }

        public static void FindEx<T>(this Transform inCurrent, out T inDest, string inName)
        {
            inDest = inCurrent.FindEx<T>(inName);
        }

        public static void FindEx(this Transform inCurrent, Transform[] array, string name, int startNum = 1, bool bZeroZero = false)
        {
            int count = inCurrent.childCount;
            string szTemp = "";
            for (int i = 0; i < array.Length; i++)
            {
                if (bZeroZero)
                    szTemp = BuildString("{0}{1:00}", name, i + startNum);
                else
                    szTemp = BuildString("{0}{1}", name, i + startNum);

                array[i] = inCurrent.FindEx(szTemp);
            }
        }

        public static void FindEx<T>(this Transform inCurrent, T[] array, string name, int startNum = 1, bool bZeroZero = false)
        {
            int count = inCurrent.childCount;
            string szTemp = "";
            for (int i = 0; i < array.Length; i++)
            {
                if (bZeroZero)
                    szTemp = BuildString("{0}{1:00}", name, i + startNum);
                else
                    szTemp = BuildString("{0}{1}", name, i + startNum);

                array[i] = inCurrent.FindEx<T>(szTemp);
            }
        }

        public static T GetOrAddComponent<T>(this GameObject inGameObject) where T : Component
        {
            T res = inGameObject.GetComponent<T>();
            if(res == null)
                res = inGameObject.AddComponent<T>();
            return res;
        }
    }
}