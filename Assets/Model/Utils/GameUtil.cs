using UnityEngine;
using System.Collections.Generic;
using System;


public class GameUtil
{
    #region GetRandomPos 获取目标点周围的随机点
    /// <summary>
    /// 获取目标点周围的随机点(目标周围360°)
    /// </summary>
    /// <param name="targerPos">目标点</param>
    /// <param name="distance">距离</param>
    /// <returns></returns>
    public static Vector3 GetRandomPos(Vector3 targerPos, float distance)
    {
        return GetRandomPos(Vector3.zero, targerPos, 360, distance);
    }

    /// <summary>
    /// 获取目标点周围的随机点(目标周围180°)
    /// </summary>
    /// <param name="currPos">自身坐标</param>
    /// <param name="targerPos">目标坐标</param>
    /// <param name="distance">距离</param>
    /// <returns></returns>
    public static Vector3 GetRandomPos(Vector3 currPos, Vector3 targerPos, float distance)
    {
        return GetRandomPos(currPos, targerPos, 180, distance);
    }

    /// <summary>
    /// 获取目标点周围的随机点
    /// </summary>
    /// <param name="currPos">自身坐标</param>
    /// <param name="targerPos">目标坐标</param>
    /// <param name="angle">夹角</param>
    /// <param name="distance">距离</param>
    /// <returns></returns>
    public static Vector3 GetRandomPos(Vector3 currPos, Vector3 targerPos, float angle, float distance)
    {
        float semiangle = angle * 0.5f;
        //1.定义一个向量
        Vector3 v = (currPos - targerPos).normalized;
        v.y = 0;

        //2.让向量旋转
        v = Quaternion.Euler(0, UnityEngine.Random.Range(-semiangle, semiangle), 0) * v;

        //3.向量 * 距离(半径) = 坐标点
        Vector3 pos = v * distance * UnityEngine.Random.Range(0.8f, 1f);

        //4.计算出来的 围绕主角的 随机坐标点
        return targerPos + pos;
    }

    #endregion

    #region GetPathLen 计算路径的长度
    /// <summary>
    /// 计算路径的长度
    /// </summary>
    /// <param name="path"></param>
    /// <returns></returns>
    public static float GetPathLen(List<Vector3> path)
    {
        float pathLen = 0f; //路径的总长度 计算出路径

        for (int i = 0; i < path.Count; i++)
        {
            if (i == path.Count - 1) continue;

            float dis = Vector3.Distance(path[i], path[i + 1]);
            pathLen += dis;
        }

        return pathLen;
    }
    #endregion

    #region GetFileName 获取文件名
    /// <summary>
    /// 获取文件名
    /// </summary>
    /// <param name="path"></param>
    /// <returns></returns>
    public static string GetFileName(string path)
    {
        string fileName = path;
        int lastIndex = path.LastIndexOf('/');
        if (lastIndex > -1)
        {
            fileName = fileName.Substring(lastIndex + 1);
        }

        lastIndex = fileName.LastIndexOf('.');
        if (lastIndex > -1)
        {
            fileName = fileName.Substring(0, lastIndex);
        }

        return fileName;
    }
    #endregion

    #region AutoLoadTexture 自动加载图片
    /// <summary>
    /// 自动加载图片
    /// </summary>
    /// <param name="go"></param>
    /// <param name="imgPath"></param>
    /// <param name="imgName"></param>
    public static void AutoLoadTexture(GameObject go, string imgPath, string imgName, bool isSetNativeSize)
    {
        if (go != null)
        {
            AutoLoadTexture component = go.GetOrCreatComponent<AutoLoadTexture>();
            if (component != null)
            {
                component.ImgPath = imgPath;
                component.ImgName = imgName;
                component.IsSetNativeSize = isSetNativeSize;
                component.SetImg();
            }
        }
    }
    #endregion

    #region AutoNumberAnimation 自动数字动画
    /// <summary>
    /// 自动数字动画
    /// </summary>
    /// <param name="go"></param>
    /// <param name="number"></param>
    public static void AutoNumberAnimation(GameObject go, int number)
    {
        if (go != null)
        {
            AutoNumberAnimation component = go.GetOrCreatComponent<AutoNumberAnimation>();
            component.DoNumber(number);
        }
    }
    #endregion

    /// <summary>
    /// 添加子物体
    /// </summary>
    /// <param name="parent"></param>
    /// <param name="prefab"></param>
    /// <returns></returns>
    public static GameObject AddChild(Transform parent, GameObject prefab)
    {
        GameObject go = GameObject.Instantiate(prefab) as GameObject;

        if (go != null && parent != null)
        {
            Transform t = go.transform;
            t.SetParent(parent, false);
            go.layer = parent.gameObject.layer;
        }
        return go;
    }

    /// <summary>
    /// 获取资源的路径
    /// </summary>
    /// <param name="path"></param>
    /// <param name="fileName"></param>
    /// <returns></returns>
    public static String GetAssetPath(string path, string fileName)
    {
#if DISABLE_ASSETBUNDLE && UNITY_EDITOR
        return string.Format("{0}/{1}.assetbundle", path, fileName);
#else
        return string.Format("{0}.assetbundle", path);
#endif

    }
    
}