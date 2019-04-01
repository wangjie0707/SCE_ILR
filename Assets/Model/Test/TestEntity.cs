using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Myth;

public class TestEntity : MonoBehaviour
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

            GameEntry.Resource.UpdateVersionList(71328, 1174852721, 23984, 499451105, new UpdateVersionListCallbacks((downloadPath, dodownloadurl) =>
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
            GameEntry.Entity.ShowEntity<ABCEntity>(1, "Assets/DownLoad/Prefab/CreateRole/104000.prefab", "ABC", 258);
            //GameEntry.Entity.ShowEntity<DEFEntity>(2, "104000", "Prefab/CreateRole/104000", "DEF", "46545");1
            //GameEntry.Entity.HideEntity(1);
            //GameEntry.Entity.HideAllLoadingEntities();
        }

        if (Input.GetKeyUp(KeyCode.C))
        {
            GameEntry.Entity.HideEntity(1, 1234);
            GameEntry.Entity.HideEntity(2, 4321);
        }
        if (Input.GetKeyUp(KeyCode.D))
        {
            GameEntry.Entity.AttachEntity(1, 2);
        }
        if (Input.GetKeyUp(KeyCode.E))
        {
            GameEntry.Entity.DetachEntity(1, "接触");
        }
        if (Input.GetKeyUp(KeyCode.F))
        {
            GameEntry.Entity.DetachChildEntities(2);
        }
        if (Input.GetKeyUp(KeyCode.G))
        {
            GameEntry.Entity.HideAllLoadedEntities();
        }

        if (Input.GetKeyUp(KeyCode.H))
        {
            GameEntry.Entity.ShowEntity<ABCEntity>(1, "104000",  "ABC", "258");
        }
        if (Input.GetKeyUp(KeyCode.I))
        {
            GameEntry.Entity.HideEntity(1, 1234);
        }
        if (Input.GetKeyUp(KeyCode.J))
        {
            GameEntry.Entity.ShowEntity<DEFEntity>(2, "104000",  "ABC", "258");
        }
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
