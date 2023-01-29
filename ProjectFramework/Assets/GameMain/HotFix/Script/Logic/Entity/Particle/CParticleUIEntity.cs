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
    public class CParticleUIEntityData : CParticleEntityData, IReference
    {
        public Transform m_refParent { private set; get; }
        public Action<CParticleUIEntity> m_fnOnShow { private set; get; }
        public CParticleUIEntityData() : base(-1, -1)
        {
            _CleanData();
        }

        protected override void _CleanData()
        {
            base._CleanData();
            m_refParent = null;
            m_fnOnShow = null;
        }
        public void Clear()
        {
            _CleanData();
        }
        public static CParticleUIEntityData Create(int a_nId, int a_nConfigId, Transform a_refParent, Action<CParticleUIEntity> a_fnOnShow)
        {
            CParticleUIEntityData data = ReferencePool.Acquire<CParticleUIEntityData>();
            data.m_nId = a_nId;
            data.m_nParConfigId = a_nConfigId;
            data.m_refParent = a_refParent;
            data.m_fnOnShow = a_fnOnShow;
            return data;
        }

        public static void Del(CParticleUIEntityData a_refData)
        {
            if (null != a_refData)
            {
                ReferencePool.Release(a_refData);
            }
        }
    }
    public class CParticleUIEntity : CEntityBase
    {
        private CParticle m_particle;
        private CParticleUIEntityData m_parData;

        protected override void _InitComponents()
        {
            base._InitComponents();
        }

        protected override void _OnInit(object userData)
        {
            base._OnInit(userData);
            m_particle = new CParticle(transform, _OnParStop);
        }

        private void _OnParStop()
        {
            CHotFixEntry.Entity.HideEntity(this);
        }

        public override void OnShow(object userData)
        {
            base.OnShow(userData);
            gameObject.SetActive(true);
            m_parData = (CParticleUIEntityData)userData;
            gameObject.SetLayerRecursively(UnityEngine.LayerMask.NameToLayer("UI"));
            transform.SetParent(m_parData.m_refParent);
            transform.localScale = 100 * Vector3.one;
            transform.localPosition = Vector3.zero;
            m_particle.SetDepth(_CalcParDepth(m_parData.m_refParent));
            Play();
            if (m_parData.m_fnOnShow != null)
            {
                m_parData.m_fnOnShow.Invoke(this);
            }
        }

        public override void OnHide(bool isShutdown, object userData)
        {
            base.OnHide(isShutdown, userData);
            gameObject.SetActive(false);
            CParticleUIEntityData.Del(m_parData);
            m_parData = null;
        }

        public void Play()
        {
            m_particle.Play();
        }

        public void Stop()
        {
            m_particle.Stop();
        }

        private int _CalcParDepth(Transform a_refRoot)
        {
            int nLoop = 0;
            while (a_refRoot != null)
            {
                Canvas parentCanvas = a_refRoot.GetComponent<Canvas>();
                if (parentCanvas != null)
                {
                    return parentCanvas.sortingOrder;
                }
                a_refRoot = a_refRoot.parent;
                nLoop++;
                if (nLoop == 100)
                {
                    break;
                }
            }
            return 0;
        }

        public static void PlayPar(EParticleID a_eParId, Transform a_refParent, Action<CParticleUIEntity> a_fnOnShow)
        {
            CParticleUIEntityData data = CParticleUIEntityData.Create(CEntityData.GenerateId(), (int)a_eParId, a_refParent, a_fnOnShow);
            CHotFixEntry.Entity.ShowParticle(data);
        }
    }
}

