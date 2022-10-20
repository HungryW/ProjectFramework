using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;
using UnityEngine.Events;
using GameFramework;

namespace GameFrameworkPackage
{
    public struct CProgressGrowthSegmentData
    {
        public CProgressGrowthSegmentData(int a_nStartNum, int a_nEndNum, int a_nFullNum)
        {
            m_nStartNum = a_nStartNum;
            m_nEndNum = a_nEndNum;
            m_nFullNum = a_nFullNum;
        }

        public int m_nStartNum
        {
            private set;
            get;
        }

        public int m_nEndNum
        {
            private set;
            get;
        }

        public int m_nFullNum
        {
            private set;
            get;
        }

        public float GetStartProgress()
        {
            if (m_nFullNum == 0)
            {
                return 0;
            }
            return 1.0f * m_nStartNum / m_nFullNum;
        }

        public float GetEndProgress()
        {
            if (m_nFullNum == 0)
            {
                return 0;
            }
            return 1.0f * m_nEndNum / m_nFullNum;
        }

        public float GetAddProgress()
        {
            return GetEndProgress() - GetStartProgress();
        }
    }

    public class CUIToolProgressGrowthAnim : MonoBehaviour
    {
        [SerializeField]
        private GameObject GoProgress;
        [SerializeField]
        private Image ImgProgress;
        [SerializeField]
        private Text LbProgress;
        [SerializeField]
        private CUIToolTextNumChangeShow ProgressNumChange;

        private UnityAction m_fnOnProgressFull;
        private UnityAction m_fnOnPlayEnd;

        public void SetPlayEndCallback(UnityAction a_fn)
        {
            m_fnOnPlayEnd = a_fn;
        }

        public void SetProgressFullCallback(UnityAction a_fn)
        {
            m_fnOnProgressFull = a_fn;
        }

        public void PlayGrowth(List<CProgressGrowthSegmentData> a_listSegmentData, float a_fTime)
        {
            float fTotalProgress = 0;
            foreach (var item in a_listSegmentData)
            {
                fTotalProgress += item.GetAddProgress();
            }

            float fCurUseTime = 0;
            Sequence seq = DOTween.Sequence();
            for (int i = 0; i < a_listSegmentData.Count; i++)
            {
                float fStartProgress = a_listSegmentData[i].GetStartProgress();
                float fEndProgress = a_listSegmentData[i].GetEndProgress();
                float fUseTime = (fEndProgress - fStartProgress) / fTotalProgress * a_fTime;
                int nIdx = i;
                seq.Append(ImgProgress.DOFillAmount(fEndProgress, fUseTime));
                seq.InsertCallback(fCurUseTime, () =>
                {
                    if (ProgressNumChange != null)
                    {
                        ProgressNumChange.OnNumAddShow(fUseTime, a_listSegmentData[nIdx].m_nEndNum
                                                      , a_listSegmentData[nIdx].m_nEndNum - a_listSegmentData[nIdx].m_nStartNum
                                                      , Utility.Text.Format("{0}/{1}", "{0}", a_listSegmentData[nIdx].m_nFullNum));
                    }
                });
                fCurUseTime += fUseTime;

                if (i < a_listSegmentData.Count - 1)
                {
                    seq.AppendCallback(() => { 
                        ImgProgress.fillAmount = 0;
                        if (m_fnOnProgressFull != null)
                        {
                            m_fnOnProgressFull.Invoke();
                        }
                    });
                }
            }

            seq.AppendCallback(() =>
            {
                if (m_fnOnPlayEnd != null)
                {
                    m_fnOnPlayEnd.Invoke();
                }
            });
        }

        public void PlayGrowth(int a_nStartLv, float a_fStartProgress, int a_nEndLv, float a_fEndProgress, float a_fTime)
        {
            Sequence seq = DOTween.Sequence();
            int nSegmentNum = a_nEndLv - a_nStartLv + 1;
            float fTotalProgress = (a_nEndLv + a_fEndProgress) - (a_nStartLv + a_fStartProgress);
            for (int i = 1; i <= nSegmentNum; i++)
            {
                float fStartProgress = i == 1 ? a_fStartProgress : 0;
                float fEndProgress = i == nSegmentNum ? a_fEndProgress : 1;
                float fUseTime = (fEndProgress - fStartProgress) / fTotalProgress * a_fTime;
                seq.Append(ImgProgress.DOFillAmount(fEndProgress, fUseTime));
                if (i < nSegmentNum)
                {
                    seq.AppendCallback(() => { ImgProgress.fillAmount = 0; });
                }
            }
            seq.AppendCallback(() =>
            {
                if (m_fnOnPlayEnd != null)
                {
                    m_fnOnPlayEnd.Invoke();
                }
            });
        }
    }
}

