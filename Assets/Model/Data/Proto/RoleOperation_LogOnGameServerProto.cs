//===================================================
//创建时间：2018-11-18 02:18:18
//备    注：
//===================================================
using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine.UI;
using Myth;

/// <summary>
/// 客户端发送登录区服消息
/// </summary>
public struct RoleOperation_LogOnGameServerProto : IProto
{
    public ushort ProtoCode { get { return 10001; } }
    public string ProtoEnName { get { return "RoleOperation_LogOnGameServer"; } }

    public int AccountId; //账户ID

    public byte[] ToArray()
    {
        using (MMO_MemoryStream ms = new MMO_MemoryStream())
        {
            ms.WriteUShort(ProtoCode);
            ms.WriteInt(AccountId);
            return ms.ToArray();
        }
    }

    public static RoleOperation_LogOnGameServerProto GetProto(byte[] buffer)
    {
        RoleOperation_LogOnGameServerProto proto = new RoleOperation_LogOnGameServerProto();
        using (MMO_MemoryStream ms = new MMO_MemoryStream(buffer))
        {
            proto.AccountId = ms.ReadInt();
        }
        return proto;
    }
}