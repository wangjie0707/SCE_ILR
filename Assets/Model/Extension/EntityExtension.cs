using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Myth
{
    public static class EntityExtension 
    {
        /// <summary>
        /// 显示热更新层实体
        /// </summary>
        public static void ShowHotfixEntity(this EntityComponent entityComponent,int entityId,string entityAssetName,string entityGroupName, int priority, HotfixEntityData data)
        {
            //entityComponent.ShowEntity<HotfixEntity>(entityId,entityAssetName, entityGroupName);
        }
    }
}
