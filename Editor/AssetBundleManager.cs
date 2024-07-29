using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace KRN.Utility
{
    public class StreamAssetBundleMgr : Singleton<StreamAssetBundleMgr>
    {
        private AssetBundle m_AssetBundle = null;

        public void Initialize(string inBundleName)
        {
            m_AssetBundle = AssetBundle.LoadFromFile(Application.streamingAssetsPath + "/" + inBundleName);
        }

        private void OnDisable()
        {
            if (m_AssetBundle != null)
                m_AssetBundle.Unload(true);
        }

        public T Load<T>(string inAssetName) where T : UnityEngine.Object
        {
            if (m_AssetBundle == null)
            {
                DebugLog.Assert("AssetBundle is not loaded");
                return null;
            }
            else
            {
                return m_AssetBundle.LoadAsset<T>(inAssetName);
            }
        }

        public IEnumerator LoadAsync<T>(string inAssetName, Action<T> onFinished) where T : UnityEngine.Object
        {
            if (m_AssetBundle == null)
            {
                DebugLog.Assert("AssetBundle is not loaded");
                yield break;
            }
            else
            {
                var request = m_AssetBundle.LoadAssetAsync<T>(inAssetName);
                yield return request;

                if (request.isDone == false)
                {
                    DebugLog.Warning("AssetBundle is not done");
                    yield break;
                }
                else
                {
                    onFinished?.Invoke(request.asset as T);
                }
            }

            yield break;
        }
    }

    public class AssetBundleManager
    {
        public static T LoadResource<T>(string inResourceName) where T : UnityEngine.Object
        {
            if (string.IsNullOrEmpty(inResourceName))
                throw new System.ArgumentNullException("LoadResource - inResourceName is null or empty");

            if (inResourceName.IndexOf('.') != -1)
            {
                DebugLog.Warning(Utility.BuildString("This is incorrect extenstion. Please not include extension. {0}", inResourceName));
            }
            else
            {
                T res = Resources.Load<T>(inResourceName);
                if (res != null)
                    return res;
            }

            return null;
        }

        public static T LoadStramingAsset<T>(string inAssetName) where T : UnityEngine.Object
        {
            if (string.IsNullOrEmpty(inAssetName))
                throw new System.ArgumentNullException("LoadStreamAsset - inAssetName is null or empty");

            return StreamAssetBundleMgr.Instance.Load<T>(inAssetName);
        }

        public static void LoadStreamingAssetAsync<T>(string inAssetName, [NotNull] Action<T> onFinished) where T : UnityEngine.Object
        {
            if (string.IsNullOrEmpty(inAssetName))
                throw new System.ArgumentNullException("LoadStreamAsset - inAssetName is null or empty");

            StreamAssetBundleMgr.Instance.LoadAsync<T>(inAssetName, onFinished);
        }

        public static void LoadAsset<T>(string inAssetName, [NotNull] Action<T> onFinished) where T : UnityEngine.Object
        {
            if(string.IsNullOrEmpty(inAssetName))
                throw new System.ArgumentNullException("LoadAsset - inAssetName is null or empty");

            if (inAssetName.IndexOf('.') != -1)
            {
                DebugLog.Warning(Utility.BuildString("This is incorrect extension. Please not include extension. {0}", inAssetName));
            }
            else
            {
                Addressables.LoadAssetAsync<T>(inAssetName).Completed += (handle) => { 
                    if(handle.Status == UnityEngine.ResourceManagement.AsyncOperations.AsyncOperationStatus.Succeeded)
                        onFinished?.Invoke(handle.Result);
                };
            }
        }

        public static void Instantiate(string inAssetName, Action<GameObject> onFinished)
        {
            if (string.IsNullOrEmpty(inAssetName))
                throw new System.ArgumentNullException("LoadAsset - inAssetName is null or empty");

            if (inAssetName.IndexOf('.') != -1)
            {
                DebugLog.Warning(Utility.BuildString("This is incorrect extension. Please not include extension. {0}", inAssetName));
            }
            else
            {
                Addressables.InstantiateAsync(inAssetName).Completed += (handle) => {
                    if (handle.Status == UnityEngine.ResourceManagement.AsyncOperations.AsyncOperationStatus.Succeeded)
                        onFinished?.Invoke(handle.Result);
                };
            }
        }
    }
}
