﻿using UnityEngine;
using System.Collections;
using UnityEngine.UI;

/// <summary>
/// 自动加载图片
/// </summary>
public class AutoLoadTexture : MonoBehaviour
{
    /// <summary>
    /// 图片名称
    /// </summary>
    public string ImgName;

    /// <summary>
    /// 图片路径
    /// </summary>
    public string ImgPath;

    /// <summary>
    /// 是否设置图片原本大小
    /// </summary>
    public bool IsSetNativeSize;

    void Start()
    {
        
    }

    public void SetImg()
    {
        Image img = GetComponent<Image>();

        if (img != null && !string.IsNullOrEmpty(ImgPath))
        {
            //AssetBundleMgr.Instance.LoadOrDownload<Object>(ImgPath, ImgName, (UnityEngine.Object asset, object userData) =>
            //{
            //    Sprite obj = null;
            //    if (asset is Sprite)
            //    {
            //        obj = asset as Sprite;
            //    }
            //    else if (asset is Texture2D)
            //    {
            //        Texture2D texture = asset as Texture2D;
            //        obj = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(.5f, .5f));
            //    }

            //    img.overrideSprite = obj;
            //    if (IsSetNativeSize)
            //    {
            //        img.SetNativeSize();
            //    }
            //}, type: 1);
        }
    }
}