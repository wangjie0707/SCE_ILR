
//===================================================
//备    注：此代码为工具生成 请勿手工修改
//===================================================
using System.Collections;
using System.Collections.Generic;
using System;
using Myth;

/// <summary>
/// Material数据管理
/// </summary>
public partial class MaterialDBModel : DataTableDBModelBase<MaterialDBModel, MaterialEntity>
{
    /// <summary>
    /// 文件名称
    /// </summary>
    public override string DataTableName { get { return "Material"; } }

    /// <summary>
    /// 加载列表
    /// </summary>
    protected override void LoadList(MMO_MemoryStream ms)
    {
        int rows = ms.ReadInt();
        ms.ReadInt();

        for (int i = 0; i < rows; i++)
        {
            MaterialEntity entity = new MaterialEntity();
            entity.Id = ms.ReadInt();
            entity.Name = ms.ReadUTF8String();
            entity.Quality = ms.ReadInt();
            entity.Description = ms.ReadUTF8String();
            entity.Type = ms.ReadInt();
            entity.FixedType = ms.ReadInt();
            entity.FixedAddValue = ms.ReadInt();
            entity.maxAmount = ms.ReadInt();
            entity.packSort = ms.ReadInt();
            entity.CompositionProps = ms.ReadUTF8String();
            entity.CompositionMaterialID = ms.ReadInt();
            entity.CompositionGold = ms.ReadUTF8String();
            entity.SellMoney = ms.ReadInt();

            m_List.Add(entity);
            m_Dic[entity.Id] = entity;
        }
    }
}