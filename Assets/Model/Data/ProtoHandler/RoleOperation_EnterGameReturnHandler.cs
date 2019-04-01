//===================================================
//创建时间：2018-11-18 02:18:18
//备    注：
//===================================================
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 服务器返回进入游戏消息（工具只生成一次）
/// </summary>
public sealed class RoleOperation_EnterGameReturnHandler
{
    public static void OnRoleOperation_EnterGameReturn(byte[] buffer)
    {
        RoleOperation_EnterGameReturnProto proto = RoleOperation_EnterGameReturnProto.GetProto(buffer);
#if DEBUG_LOG_PROTO
        Debug.Log("<color=#00eaff>接收消息:</color><color=#00ff9c>" + proto.ProtoEnName + " " + proto.ProtoCode + "</color>");
        Debug.Log("<color=#c5e1dc>==>>" + JsonUtility.ToJson(proto) + "</color>");
#endif
    }
}