using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace KRN.Utility
{
    public class AssetBundleManager
    {
        public static T LoadAsset<T>(string inAssetName) where T : UnityEngine.Object
        {
            if (string.IsNullOrEmpty(inAssetName))
                throw new System.ArgumentNullException("inAssetName is null or empty");

            if (inAssetName.IndexOf('.') != -1)
            {
                DebugLog.Warning(Utility.BuildString("This is incorrect extenstion. {0}", inAssetName));
            }
            else
            {
                T res = Resources.Load<T>(inAssetName);
                if (res != null)
                    return res;
            }

            return null;
        }
    }
}
