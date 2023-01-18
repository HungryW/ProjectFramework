using System.Collections;
using System.Collections.Generic;
using Defines;
using GameFramework;
using GameFramework.UI;
using GameFrameworkPackage;
using UnityGameFramework.Runtime;

namespace HotFixEntry
{
    public class CFrameWorkEntity : CFrameworkBase
    {
        private EntityComponent refComponent
        {
            get;
            set;
        }

        public CFrameWorkEntity() : base()
        {
            refComponent = CGameEntryMgr.Entity;
        }

        public override void Clean()
        {
            refComponent = null;
        }

        public void ShowEntity(CEntityData a_EntityData, string a_szEntityGroup, string a_szAssetFullPath, string a_szDllName, string a_szClassFullName)
        {
            refComponent.ShowEntity(a_szEntityGroup, a_szAssetFullPath, a_szDllName, a_szClassFullName, a_EntityData.m_nId, a_EntityData);
        }

        public void HideEntity(CEntityBase a_entity)
        {
            refComponent.HideEntity(a_entity.EntityLogic);
        }
        public CEntityBase GetEntity(int a_nId)
        {
            Entity frameEntity = refComponent.GetEntity(a_nId);
            if (null == frameEntity)
            {
                return null;
            }
            CHotFixLogicEntity frameLogic = frameEntity.Logic as CHotFixLogicEntity;
            if (null == frameLogic)
            {
                return null;
            }
            CEntityBase hotFixEntity = frameLogic.m_AgentHotFix as CEntityBase;
            if (null == hotFixEntity)
            {
                return null;
            }
            return hotFixEntity;
        }
    }
}


