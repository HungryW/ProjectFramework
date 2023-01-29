using Defines;
using DG.Tweening;
using GameFramework;
using GameFrameworkPackage;
using HotFixEntry;
using System;
using UnityEngine;
using UnityGameFramework.Runtime;

namespace HotFixLogic.Entity
{
    public class CParticleEntityData : CEntityData
    {
        protected int m_nParConfigId;
        protected DRParticle m_refConfig;
        public CParticleEntityData(int a_nId, int a_nConfigId) : base(a_nId)
        {
            m_nParConfigId = a_nConfigId;
            m_refConfig = null;
           
        }

        public DRParticle GetConfig()
        {
            if (null == m_refConfig)
            {
                m_refConfig = CHotFixEntry.DataTable.GetDataRow<DRParticle>(m_nParConfigId);
            }
            if (m_refConfig == null)
            {
                Log.Error("CParticleEntityData Create Fail ConfigId={0}", m_nParConfigId);
            }
            return m_refConfig;
        }

        protected override void _CleanData()
        {
            base._CleanData();
            m_nParConfigId = -1;
            m_refConfig = null;
        }
    }
}

