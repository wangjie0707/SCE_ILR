//===================================================
//创建时间：2018-11-18 02:18:18
//备    注：
//===================================================
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 服务器返回配置列表（工具只生成一次）
/// </summary>
public sealed class System_GameServerConfigReturnHandler
{
    public static void OnSystem_GameServerConfigReturn(byte[] buffer)
    {
        System_GameServerConfigReturnProto proto = System_GameServerConfigReturnProto.GetProto(buffer);
#if DEBUG_LOG_PROTO
        Debug.Log("<color=#00eaff>接收消息:</color><color=#00ff9c>" + proto.ProtoEnName + " " + proto.ProtoCode + "</color>");
        Debug.Log("<color=#c5e1dc>==>>" + JsonUtility.ToJson(proto) + "</color>");
#endif
    }
}