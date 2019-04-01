using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Myth;

public class TaskUI : UIForm {

    protected override void OnInit(object userData)
    {
        base.OnInit(userData);
        Debug.Log("初始化");
    }

    protected internal override void OnOpen(object userData)
    {
        base.OnOpen(userData);
        Debug.Log("打开");
    }

    protected internal override void OnClose(object userData)
    {
        base.OnClose(userData);
        Debug.Log("关闭");
    }

    protected internal override void OnPause()
    {
        base.OnPause();
        Debug.Log("暂停");
    }

    protected internal override void OnResume()
    {
        base.OnResume();
        Debug.Log("暂停恢复");
    }

    protected internal override void OnCover()
    {
        base.OnCover();
        Debug.Log("遮挡");
    }

    protected internal override void OnRefocus(object userData)
    {
        base.OnRefocus(userData);
        Debug.Log("界面激活");
    }

    protected internal override void OnReveal()
    {
        base.OnReveal();
        Debug.Log("遮挡恢复");
    }

    protected internal override void OnUpdate(float deltaTime, float unscaledDeltaTime)
    {
        base.OnUpdate( deltaTime,  unscaledDeltaTime);
        //Debug.Log("更新");
    }
}


