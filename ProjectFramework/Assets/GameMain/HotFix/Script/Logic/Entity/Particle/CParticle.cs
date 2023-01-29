using DG.Tweening;
using GameFrameworkPackage;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace HotFixLogic.Entity
{
    public class CParticle
    {
        private Transform transform;
        private GameObject gameObject;
        private ParticleSystem[] m_arrPars;
        private SpriteRenderer[] m_arrSprite;
        private float m_fLifeTime;
        private bool m_bIsAutoRelease;
        private bool m_bPlaying;
        private Action m_fnOnStop;
        private List<int> m_listCacheInitDepth;

        public CParticle(Transform a_transformRoot, Action a_fnOnStop)
        {
            transform = a_transformRoot;
            gameObject = transform.gameObject;
            m_arrPars = transform.GetComponentsInChildren<ParticleSystem>(true);
            m_arrSprite = transform.GetComponentsInChildren<SpriteRenderer>(true);
            m_fLifeTime = _CalcLifeTime();
            m_bIsAutoRelease = _CheckAutoRelease();
            m_bPlaying = false;
            m_fnOnStop = a_fnOnStop;
            m_listCacheInitDepth = _GenerateCacheInitDepth();
        }



        private float _CalcLifeTime()
        {
            float fTime = 0;
            for (int i = 0; i < m_arrPars.Length; i++)
            {
                float fDuration = m_arrPars[i].GetDuration();
                if (fDuration > fTime)
                {
                    fTime = fDuration;
                }
            }
            return fTime;
        }

        private bool _CheckAutoRelease()
        {
            for (int i = 0; i < m_arrPars.Length; i++)
            {
                if (m_arrPars[i].main.loop)
                {
                    return false;
                }
            }
            return true;
        }

        private List<int> _GenerateCacheInitDepth()
        {
            List<int> listDepth = new List<int>();
            for (int i = 0; i < m_arrPars.Length; i++)
            {
                listDepth.Add(m_arrPars[i].GetComponent<Renderer>().sortingOrder);
            }
            for (int i = 0; i < m_arrSprite.Length; i++)
            {
                listDepth.Add(m_arrSprite[i].sortingOrder);
            }
            return listDepth;
        }

        private int _GetInitDepth(int a_nIdx)
        {
            if (a_nIdx >= m_listCacheInitDepth.Count)
            {
                return 0;
            }
            return m_listCacheInitDepth[a_nIdx];
        }
        /// <summary>
        /// 特效的层级从1开始
        /// </summary>
        /// <param name="a_nDepth"></param>
        public void SetDepth(int a_nDepth)
        {
            int nCacheIdx = 0;
            for (int i = 0; i < m_arrPars.Length; i++)
            {
                m_arrPars[i].GetComponent<Renderer>().sortingOrder = a_nDepth + _GetInitDepth(nCacheIdx);
                nCacheIdx++;
            }
            for (int i = 0; i < m_arrSprite.Length; i++)
            {
                m_arrSprite[i].sortingOrder += a_nDepth + _GetInitDepth(nCacheIdx);
                nCacheIdx++;
            }
        }

        public void Play()
        {
            m_bPlaying = true;
            for (int i = 0; i < m_arrPars.Length; i++)
            {
                m_arrPars[i].Play();
            }
            _TryAutoRelease();
        }

        public void Stop()
        {
            if (!m_bPlaying)
            {
                return;
            }
            m_bPlaying = false;
            for (int i = 0; i < m_arrPars.Length; i++)
            {
                m_arrPars[i].Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
            }
            m_fnOnStop.SafeInvoke();
        }

        public float GetLifeTime()
        {
            return m_fLifeTime;
        }


        private void _TryAutoRelease()
        {
            if (m_bIsAutoRelease)
            {
                Sequence seq = DOTween.Sequence();
                seq.AppendInterval(m_fLifeTime)
                    .AppendCallback(Stop);
            }
        }
    }
}

