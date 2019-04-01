using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

namespace Myth
{
    public class ResourceHelper : MonoBehaviour
    {
        private  Shader[] m_Allshader;
        /// <summary>
        /// 解析依赖项资源
        /// </summary>
        /// <param name="buffer">需要解析的数据</param>
        /// <param name="loadManifestCallBack">依赖解析回调</param>
        public void Parse(byte[] buffer, LoadManifestCallBack loadManifestCallBack)
        {
            StartCoroutine(ParseAssetManifest(buffer, loadManifestCallBack));
        }

        private IEnumerator ParseAssetManifest(byte[] buffer, LoadManifestCallBack loadManifestCallBack)
        {
            AssetBundleCreateRequest BytesAssetBundleCreateRequest = AssetBundle.LoadFromMemoryAsync(buffer);
            yield return BytesAssetBundleCreateRequest;

            AssetBundle assetBundle = BytesAssetBundleCreateRequest.assetBundle;

            AssetBundleManifest assetBundleManifest = assetBundle.LoadAsset<AssetBundleManifest>("AssetBundleManifest");
            if (loadManifestCallBack != null)
            {
                loadManifestCallBack(assetBundleManifest);
            }
        }

        /// <summary>
        /// 预加载Shader
        /// </summary>
        /// <param name="bytes">数据流</param>
        /// <param name="preLoadShaderCallBack">预加载Shader回调</param>
        public void PreLoadShader(byte[] bytes, PreLoadShaderCallBack preLoadShaderCallBack)
        {
            StartCoroutine(LoadShaderCoroutine(bytes, preLoadShaderCallBack));
        }

        private IEnumerator LoadShaderCoroutine(byte[] bytes, PreLoadShaderCallBack preLoadShaderCallBack)
        {
            AssetBundleCreateRequest BytesAssetBundleCreateRequest = AssetBundle.LoadFromMemoryAsync(bytes);
            yield return BytesAssetBundleCreateRequest;

            AssetBundle assetBundle = BytesAssetBundleCreateRequest.assetBundle;

            try
            {
                m_Allshader = assetBundle.LoadAllAssets<Shader>();
                Debug.Log(m_Allshader.Length);
            }
            catch (Exception errorMessage)
            {
                if (preLoadShaderCallBack.PreLoadShaderFailure != null)
                {
                    preLoadShaderCallBack.PreLoadShaderFailure(errorMessage.ToString());
                }
                yield break;
            }
            Shader.WarmupAllShaders();
            assetBundle.Unload(false);
            if (preLoadShaderCallBack.PreLoadShaderSuccess != null)
            {
                preLoadShaderCallBack.PreLoadShaderSuccess();
            }
        }



        /// <summary>
        /// 卸载场景
        /// </summary>
        /// <param name="sceneAssetName">场景资源名称</param>
        /// <param name="unloadSceneCallbacks">卸载场景回调函数集</param>
        /// <param name="userData">用户自定义数据</param>
        public void UnloadScene(string sceneAssetName, UnloadSceneCallbacks unloadSceneCallbacks, object userData)
        {
#if UNITY_5_5_OR_NEWER
            if (gameObject.activeInHierarchy)
            {
                StartCoroutine(UnloadSceneCo(sceneAssetName, unloadSceneCallbacks, userData));
            }
            else
            {
                UnityEngine.SceneManagement.SceneManager.UnloadSceneAsync(GetSceneName(sceneAssetName));
            }
#else
            if (UnityEngine.SceneManagement.SceneManager.UnloadScene(sceneAssetName))
            {
                if (unloadSceneCallbacks.UnloadSceneSuccessCallback != null)
                {
                    unloadSceneCallbacks.UnloadSceneSuccessCallback(sceneAssetName, userData);
                }
            }
            else
            {
                if (unloadSceneCallbacks.UnloadSceneFailureCallback != null)
                {
                    unloadSceneCallbacks.UnloadSceneFailureCallback(sceneAssetName, userData);
                }
            }
#endif
        }

#if UNITY_5_5_OR_NEWER
        private IEnumerator UnloadSceneCo(string sceneAssetName, UnloadSceneCallbacks unloadSceneCallbacks, object userData)
        {
            AsyncOperation asyncOperation = UnityEngine.SceneManagement.SceneManager.UnloadSceneAsync(GetSceneName(sceneAssetName));
            if (asyncOperation == null)
            {
                yield break;
            }

            yield return asyncOperation;

            if (asyncOperation.allowSceneActivation)
            {
                if (unloadSceneCallbacks.UnloadSceneSuccessCallback != null)
                {
                    unloadSceneCallbacks.UnloadSceneSuccessCallback(sceneAssetName, userData);
                }
            }
            else
            {
                if (unloadSceneCallbacks.UnloadSceneFailureCallback != null)
                {
                    unloadSceneCallbacks.UnloadSceneFailureCallback(sceneAssetName, userData);
                }
            }
        }
#endif

        /// <summary>
        /// 直接从指定文件路径读取数据流
        /// </summary>
        /// <param name="fileUri">文件路径</param>
        /// <param name="loadBytesCallback">读取数据流回调函数</param>
        public void LoadBytes(string fileUri, LoadBytesCallback loadBytesCallback)
        {
            StartCoroutine(LoadBytesCo(fileUri, loadBytesCallback));
        }

        private IEnumerator LoadBytesCo(string fileUri, LoadBytesCallback loadBytesCallback)
        {
            byte[] bytes = null;
            string errorMessage = null;

#if UNITY_5_4_OR_NEWER
            UnityWebRequest unityWebRequest = UnityWebRequest.Get(fileUri);
#if UNITY_2017_2_OR_NEWER
            yield return unityWebRequest.SendWebRequest();
#else
            yield return unityWebRequest.Send();
#endif

            bool isError = false;
#if UNITY_2017_1_OR_NEWER
            isError = unityWebRequest.isNetworkError;
#else
            isError = unityWebRequest.isError;
#endif
            bytes = unityWebRequest.downloadHandler.data;
            errorMessage = isError ? unityWebRequest.error : null;
            unityWebRequest.Dispose();
#else
            WWW www = new WWW(fileUri);
            yield return www;

            bytes = www.bytes;
            errorMessage = www.error;
            www.Dispose();
#endif

            if (loadBytesCallback != null)
            {
                loadBytesCallback(fileUri, bytes, errorMessage);
            }
        }


        /// <summary>
        /// 获取场景名称。
        /// </summary>
        /// <param name="sceneAssetName">场景资源名称。</param>
        /// <returns>场景名称。</returns>
        public static string GetSceneName(string sceneAssetName)
        {
            if (string.IsNullOrEmpty(sceneAssetName))
            {
                Log.Error("Scene asset name is invalid.");
                return null;
            }

            int sceneNamePosition = sceneAssetName.LastIndexOf('/');
            if (sceneNamePosition + 1 >= sceneAssetName.Length)
            {
                Log.Error("Scene asset name '{0}' is invalid.", sceneAssetName);
                return null;
            }

            string sceneName = sceneAssetName.Substring(sceneNamePosition + 1);
            sceneNamePosition = sceneName.LastIndexOf(".unity");
            if (sceneNamePosition > 0)
            {
                sceneName = sceneName.Substring(0, sceneNamePosition);
            }

            return sceneName;
        }
    }
}
