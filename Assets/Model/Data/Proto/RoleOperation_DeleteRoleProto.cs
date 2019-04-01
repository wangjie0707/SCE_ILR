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
/// 客户端发送删除角色消息
/// </summary>
public struct RoleOperation_DeleteRoleProto : IProto
{
    public ushort ProtoCode { get { return 10005; } }
    public string ProtoEnName { get { return "RoleOperation_DeleteRole"; } }

    public int RoleId; //角色ID

    public byte[] ToArray()
    {
        using (MMO_MemoryStream ms = new MMO_MemoryStream())
        {
            ms.WriteUShort(ProtoCode);
            ms.WriteInt(RoleId);
            return ms.ToArray();
        }
    }

    public static RoleOperation_DeleteRoleProto GetProto(byte[] buffer)
    {
        RoleOperation_DeleteRoleProto proto = new RoleOperation_DeleteRoleProto();
        using (MMO_MemoryStream ms = new MMO_MemoryStream(buffer))
        {
            proto.RoleId = ms.ReadInt();
        }
        return proto;
    }
}