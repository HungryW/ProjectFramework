using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using GameFramework;
using UnityEngine.Events;

namespace GameFrameworkPackage
{
    public class CUIToolTextNumChangeShow : MonoBehaviour
    {
        [SerializeField]
        private float fAddTotalTime = 1.0f;
        [SerializeField]
        private float fSubTimeScale = 1.0f;

        private Text LbNum = null;
        private Color m_colorInit;
        private UnityAction m_fnOnChangeEnd = null;

        private void _InitData()
        {
            if (LbNum == null)
            {
                LbNum = GetComponent<Text>();
                m_colorInit = LbNum.color;
            }
            StopAllCoroutines();
        }

        public void SetChangeEndCallback(UnityAction a_fn)
        {
            m_fnOnChangeEnd = a_fn;
        }

        public void OnNumChangeShow(int a_nCurNum, int a_nChangeNum, string a_szNumFormat = "{0}")
        {
            if (a_nChangeNum > 0)
            {
                OnNumAddShow(a_nCurNum, a_nChangeNum, a_szNumFormat);
            }
            else
            {
                OnNumSubShow(a_nCurNum, a_nChangeNum, a_szNumFormat);
            }
        }

        public void OnNumAddShow(float a_fTotalTime, int a_nCurNum, int a_nChangeNum, string a_szNumFormat = "{0}")
        {
            fAddTotalTime = a_fTotalTime;
            OnNumAddShow(a_nCurNum, a_nChangeNum, a_szNumFormat);
        }

        public void OnNumAddShow(int a_nCurNum, int a_nChangeNum, string a_szNumFormat = "{0}")
        {
            if (!gameObject.activeInHierarchy)
            {
                return;
            }
            _InitData();
            StartCoroutine(_NumAddShow(a_nCurNum, a_nChangeNum, a_szNumFormat));
        }

        private IEnumerator _NumAddShow(int a_nCurNum, int a_nChangeNum, string a_szNumFormat)
        {
            int nNum = Mathf.Max(0, a_nCurNum - a_nChangeNum);
            float fTickStep = 0.03f;
            float fTickAddNum = a_nChangeNum / fAddTotalTime * fTickStep;
            WaitForSeconds wait = new WaitForSeconds(fTickStep);
            while (nNum < a_nCurNum)
            {
                yield return wait;
                LbNum.text = Utility.Text.Format(a_szNumFormat, nNum);
                nNum = Mathf.Min(Mathf.CeilToInt(nNum + fTickAddNum), a_nCurNum);
            }
            LbNum.text = Utility.Text.Format(a_szNumFormat, a_nCurNum);
            _InvokeOnChnageEnd();
        }

        public void OnNumSubShow(int a_nCurNum, int a_nChangeNum, string a_szNumFormat = "{0}")
        {
            if (a_nChangeNum == 0)
            {
                return;
            }
            _InitData();
            Sequence seq = DOTween.Sequence();
            seq.timeScale = fSubTimeScale;
            seq.Append(transform.DOScale(1.3f, 0.2f))
                .Insert(0, LbNum.DOColor(Color.red, 0.3f))
                .AppendCallback(() => { LbNum.text = Utility.Text.Format(a_szNumFormat, a_nCurNum); _InvokeOnChnageEnd(); })
                .Append(transform.DOScale(0.8f, 0.08f))
                .Append(transform.DOScale(1f, 0.02f))
                .Insert(0.3f, LbNum.DOColor(m_colorInit, 0.1f));
        }

        public void OnNumSubShow(float a_fTotalTime, int a_nCurNum, int a_nChangeNum, string a_szNumFormat = "{0}")
        {
            fAddTotalTime = a_fTotalTime;
            OnNumSubShow(a_nCurNum, a_nChangeNum, a_szNumFormat);
        }

        private void _InvokeOnChnageEnd()
        {
            if (m_fnOnChangeEnd != null)
            {
                m_fnOnChangeEnd.Invoke();
            }
        }

    }
}


