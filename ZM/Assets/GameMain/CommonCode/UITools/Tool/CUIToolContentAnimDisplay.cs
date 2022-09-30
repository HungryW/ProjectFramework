using DG.Tweening;
using GameFrameworkPackage;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace GameFrameworkPackage
{
    public class CUIToolContentAnimDisplay : MonoBehaviour
    {
        [SerializeField]
        private Transform Content;

        private float m_fInterval = 0.2f;
        public void Init()
        {
            for (int i = 0; i < Content.childCount; i++)
            {
                Content.GetChild(i).gameObject.SetActive(true);
                Content.GetChild(i).ActiveAllChildren(false);
            }
        }

        public void SetInterval(float a_fInterval)
        {
            m_fInterval = a_fInterval;
        }

        public float GetInterval()
        {
            return m_fInterval * Content.childCount;
        }

        public void StartOpenPlay(UnityAction a_fnCallback = null)
        {
            Sequence seq = DOTween.Sequence();
            float fDelay = m_fInterval/1.8f;
            for (int i = 0; i < Content.childCount; i++)
            {
                int index = i;
                seq.InsertCallback(fDelay*i,() => { Content.GetChild(index).ActiveAllChildren(true); });
                seq.Join(Content.GetChild(i).DOPunchScale(new Vector3(0.2f, 0.2f, 0.2f), m_fInterval));
            }
            seq.AppendCallback(() => { a_fnCallback.Invoke(); });
            seq.Play();
        }

        public void StartEndPlay(UnityAction a_fnCallback = null)
        {
            Sequence seq = DOTween.Sequence();
            for (int i = 0; i < Content.childCount; i++)
            {
                int index = i;
                seq.AppendCallback(() => { Content.GetChild(index).ActiveAllChildren(true); });
                seq.Insert(0.03f, Content.GetChild(i).DOScale(new Vector3(0, 0, 0), m_fInterval));
            }
            seq.AppendInterval(m_fInterval);
            seq.AppendCallback(() => { a_fnCallback.Invoke(); });
            seq.Play();
        }
    }
}

