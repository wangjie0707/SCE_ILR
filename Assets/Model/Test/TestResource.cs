using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Myth;

public class TestResource : MonoBehaviour
{

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyUp(KeyCode.A))
        {
            GameEntry.Resource.UpdatePrefixUri = Application.dataPath + "/../assetbundle/Windows";
            Debug.Log(GameEntry.Resource.UpdatePrefixUri);
        }
        if (Input.GetKeyUp(KeyCode.B))
        {
            GameEntry.Resource.UpdateVersionList(71328, 1174852721, 23984, 499451105, new UpdateVersionListCallbacks((downloadPath, dodownloadurl) =>
            {
                Debug.Log(downloadPath);
                Debug.Log(dodownloadurl);
            },
            (downloadUri, errorMessage) =>
            {
                Debug.Log(downloadUri);
                Debug.Log(errorMessage);
            }));
        }
        if (Input.GetKeyUp(KeyCode.C))
        {
            GameEntry.Resource.CheckResources(OnCheckResourcesCompleteCallback);
        }
        if (Input.GetKeyUp(KeyCode.D))
        {
            GameEntry.Resource.UpdateResources(OnUpdateResourcesCompleteCallback);
        }
        if (Input.GetKeyUp(KeyCode.E))
        {
            GameEntry.Resource.LoadManifest(() =>
            {
                Debug.Log("加载依赖配置完毕");
            });
        }
        if (Input.GetKeyUp(KeyCode.F))
        {
            GameEntry.Resource.LoadAsset("102000.Prefab",  typeof(GameObject), 0,
                new LoadAssetCallbacks((assetName, asset, duration, userData) =>
              {
                  Debug.Log(assetName);
                  Instantiate(asset);
                  Debug.Log(duration);
                  Debug.Log(userData);
              }), 1);

            GameEntry.Resource.LoadAsset("103000",  typeof(GameObject), 0,
               new LoadAssetCallbacks((assetName, asset, duration, userData) =>
               {
                   Debug.Log(assetName);
                   Instantiate(asset);
                   Debug.Log(duration);
                   Debug.Log(userData);
               },(string assetName, string errorMessage, object userData)=> 
               {
                   Debug.Log(assetName);
                   Debug.Log(errorMessage);
               }), 1);

            GameEntry.Resource.LoadAsset("Assets/DownLoad/Prefab/CreateRole/104000.prefab",  typeof(GameObject), 0,
               new LoadAssetCallbacks((assetName, asset, duration, userData) =>
               {
                   Debug.Log(assetName);
                   Instantiate(asset);
                   Debug.Log(duration);
                   Debug.Log(userData);
               }), 1);

            GameEntry.Resource.LoadAsset("102000",  typeof(GameObject), 0,
               new LoadAssetCallbacks((assetName, asset, duration, userData) =>
               {
                   Debug.Log(assetName);
                   Instantiate(asset);
                   Debug.Log(duration);
                   Debug.Log(userData);
               }), 1);
        }

        if (Input.GetKeyUp(KeyCode.Z))
        {
            //Shader.WarmupAllShaders();
            GameEntry.Resource.PreLoadShader(new PreLoadShaderCallBack(() =>
            {
                Debug.Log("预加载Shdaer完毕");
            },
            (string errormessage) =>
            {
                Debug.Log(errormessage);
            }
            ));

        }

        if (Input.GetKeyUp(KeyCode.S))
        {
            //Shader.WarmupAllShaders();
            GameEntry.Scene.LoadScene("Assets/DownLoad/Scene/SelectRole.unity");
            RenderSettings.skybox = material;
        }

        if (Input.GetKeyUp(KeyCode.W))
        {
            GameEntry.Scene.UnloadScene("Assets/DownLoad/Scene/SelectRole.unity");

        }
    }
    public Material material;

    private void OnUpdateResourcesCompleteCallback()
    {
        Debug.Log("更新完成");
    }

    private void OnCheckResourcesCompleteCallback(bool needUpdateResources, int removedCount, int updateCount, long updateTotalLength, long updateTotalZipLength)
    {
        Debug.Log(needUpdateResources);
        Debug.Log(removedCount);
        Debug.Log(updateCount);
        Debug.Log(updateTotalLength);
        Debug.Log(updateTotalZipLength);

    }
}
