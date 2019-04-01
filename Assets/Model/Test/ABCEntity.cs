using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Myth;

public class ABCEntity : Entity
{

    protected override void OnInit(object userData)
    {
        base.OnInit(userData);
        Debug.Log(userData);
        Debug.Log("初始化");
    }

    protected internal override void OnShow(object userData)
    {
        base.OnShow(userData);
        Debug.Log(userData);
        Debug.Log("显示");
    }

    protected internal override void OnHide(object userData)
    {
        base.OnHide(userData);
        Debug.Log(userData);
        Debug.Log("隐藏");
    }

    protected internal override void OnAttached(EntityBase childEntity, Transform parentTransform, object userData)
    {
        base.OnAttached(childEntity, parentTransform, userData);
        Debug.Log("父附加");
    }

    protected internal override void OnAttachTo(EntityBase parentEntity, Transform parentTransform, object userData)
    {
        base.OnAttachTo(parentEntity, parentTransform, userData);
        Debug.Log("子附加");
    }

    protected internal override void OnDetached(EntityBase childEntity, object userData)
    {
        base.OnDetached(childEntity, userData);
        Debug.Log("父解除");
    }

    protected internal override void OnDetachFrom(EntityBase parentEntity, object userData)
    {
        base.OnDetachFrom(parentEntity, userData);
        Debug.Log("子解除");
    }

    protected internal override void OnUpdate(float deltaTime, float unscaledDeltaTime)
    {
        base.OnUpdate(deltaTime, unscaledDeltaTime);
    }
}
