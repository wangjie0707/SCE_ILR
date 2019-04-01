using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Myth;

public class TestUI : MonoBehaviour
{

    // Use this for initialization
    void Start()
    {

    }
    int task1 = 0;
    int task2 = 0;
    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyUp(KeyCode.A))
        {
            GameEntry.Resource.UpdatePrefixUri = Application.dataPath + "/../assetbundle/Windows";
            Debug.Log(GameEntry.Resource.UpdatePrefixUri);

            GameEntry.Resource.UpdateVersionList(72971, 1072024751, 24683, -854591110, new UpdateVersionListCallbacks((downloadPath, dodownloadurl) =>
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
            task1= GameEntry.UI.OpenUIForm("Assets/DownLoad/UI/UIPrefab/SelectRole/CreateRoleForm.prefab", "UIHeadBarGroup",false);
           
        }
        if (Input.GetKeyUp(KeyCode.Z))
        {
            task2 = GameEntry.UI.OpenUIForm("NoticeForm",  "UIHeadBarGroup", true);

        }
       
        if (Input.GetKeyUp(KeyCode.C))
        {
            GameEntry.UI.CloseUIForm(task1);
        }
        if (Input.GetKeyUp(KeyCode.D))
        {
            GameEntry.UI.CloseUIForm(task2);
        }
        if (Input.GetKeyUp(KeyCode.E))
        {
            GameEntry.UI.CloseAllLoadedUIForms();
        }
        if (Input.GetKeyUp(KeyCode.F))
        {
            GameEntry.UI.RefocusUIForm(GameEntry.UI.GetUIForm(task1));
        }
        if (Input.GetKeyUp(KeyCode.P))
        {
            
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
