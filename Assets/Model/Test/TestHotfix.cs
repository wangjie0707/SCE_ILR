using ILRuntime.CLR.Method;
using ILRuntime.CLR.TypeSystem;
using ILRuntime.CLR.Utils;
using ILRuntime.Runtime.Intepreter;
using ILRuntime.Runtime.Stack;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using Myth;

public class TestHotfix : MonoBehaviour
{



    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyUp(KeyCode.A))
        {
            GameEntry.ILRuntime.InitILRuntime();


        }

        if (Input.GetKeyUp(KeyCode.X))
        {
            GameEntry.Resource.UpdatePrefixUri = Application.dataPath + "/../assetbundle/Windows";
            Debug.Log(GameEntry.Resource.UpdatePrefixUri);

            GameEntry.Resource.InitResources(OnInitResourcesCompleteCallback);
        }

        if (Input.GetKeyUp(KeyCode.B))
        {

        }
        if (Input.GetKeyUp(KeyCode.B))
        {
            //GameEntry.UI.OpenUIForm("Assets/DownLoad/UI/UIPrefab/SelectRole/CreateRoleForm.prefab", "UIHeadBarGroup", false);

        }
        if (Input.GetKeyUp(KeyCode.C))
        {

        }
    }


    private void OnInitResourcesCompleteCallback()
    {
        GameEntry.Resource.LoadManifest(() =>
        {
            Debug.Log("加载依赖配置完毕");
        });
    }
}
