using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Myth;

public class TestSound : MonoBehaviour
{

    // Use this for initialization
    void Start()
    {

    }
    int sound1;
    int sound2;
    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyUp(KeyCode.X))
        {
            GameEntry.Resource.UpdatePrefixUri = Application.dataPath + "/../assetbundle/Windows";
            Debug.Log(GameEntry.Resource.UpdatePrefixUri);

            GameEntry.Resource.InitResources(OnInitResourcesCompleteCallback);
        }

        if (Input.GetKeyUp(KeyCode.A))
        {
            GameEntry.Resource.UpdatePrefixUri = Application.dataPath + "/../assetbundle/Windows";
            Debug.Log(GameEntry.Resource.UpdatePrefixUri);

            GameEntry.Resource.UpdateVersionList(152190, -2095824658, 34739, -1588960418, new UpdateVersionListCallbacks((downloadPath, dodownloadurl) =>
            {
                Debug.Log(downloadPath);
                Debug.Log(dodownloadurl);
                GameEntry.Resource.CheckResources(OnCheckResourcesCompleteCallback);
            },
           (downloadUri, errorMessage) =>
           {
               Debug.Log(downloadUri);
               Debug.Log(errorMessage);
           }));
        }


        if (Input.GetKeyUp(KeyCode.B))
        {
            sound1 =
            GameEntry.Sound.PlaySound("Assets/DownLoad/Sound/BgMusic/bgm_byd.mp3", "Music", 0, new PlaySoundParams()
            {
                Priority = 0,
                Loop = true,
                VolumeInSoundGroup = 1,
            });
            sound2 =
            GameEntry.Sound.PlaySound("bgm_createrole", "Music", 0, new PlaySoundParams()
            {
                Priority = 1,
                Loop = true,
            });
        }
        if (Input.GetKeyUp(KeyCode.C))
        {
            GameEntry.Sound.PauseSound(sound1);
            //sound2 =
            //GameEntry.Sound.PlaySound("bgm_createrole", "Sound/BgMusic/bgm_createrole", "Music", 0, new PlaySoundParams()
            //{
            //    Priority = 1,
            //    Loop = true,
            //});
            //GameEntry.Sound.PlaySound("bgm_digong", "Sound/BgMusic/bgm_digong", "Music", 0, new PlaySoundParams()
            //{
            //    Priority = 2,
            //    Loop = true,
            //});
        }

        if (Input.GetKeyUp(KeyCode.D))
        {
            GameEntry.Sound.ResumeSound(sound1, 5f);
        }

        if (Input.GetKeyUp(KeyCode.E))
        {
            GameEntry.Entity.ShowEntity<ABCEntity>(1, "104000", "ABC", "258");
        }

        if (Input.GetKeyUp(KeyCode.F))
        {
            ABCEntity aBCEntity = (ABCEntity)GameEntry.Entity.GetEntity(1);
            GameEntry.Sound.PlaySound("bgm_digong",  "Sound", 0, new PlaySoundParams()
            {
                Priority = 2,
                Loop = true,
            }, aBCEntity);
        }
    }

    private void OnInitResourcesCompleteCallback()
    {
        GameEntry.Resource.LoadManifest(() =>
        {
            Debug.Log("加载依赖配置完毕");
        });
    }

    private void OnCheckResourcesCompleteCallback(bool needUpdateResources, int removedCount, int updateCount, long updateTotalLength, long updateTotalZipLength)
    {
        Debug.Log(needUpdateResources);
        if (needUpdateResources)
        {
            GameEntry.Resource.UpdateResources(OnUpdateResourcesCompleteCallback);
        }
        else
        {
            GameEntry.Resource.LoadManifest(() =>
            {
                Debug.Log("加载依赖配置完毕");
            });
        }
        Debug.Log(removedCount);
        Debug.Log(updateCount);
        Debug.Log(updateTotalLength);
        Debug.Log(updateTotalZipLength);

    }

    private void OnUpdateResourcesCompleteCallback()
    {
        Debug.Log("更新完成");
        GameEntry.Resource.LoadManifest(() =>
        {
            Debug.Log("加载依赖配置完毕");
        });
    }
}
