using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace GameFrameworkPackage
{
    public class CUIUpdateRes : MonoBehaviour
    {
        [SerializeField]
        private Image ImgProgress;
        [SerializeField]
        private Text LbProgress;
        private CanvasScaler m_CanvasScaler;
        private void Start()
        {
            m_CanvasScaler = gameObject.GetOrAddComponent<CanvasScaler>();
            m_CanvasScaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            m_CanvasScaler.screenMatchMode = CanvasScaler.ScreenMatchMode.MatchWidthOrHeight;
            m_CanvasScaler.matchWidthOrHeight = (1.0f * Screen.width / Screen.height) > CConstDevResolution.W_H_Radio ? 1 : 0;
        }

        public void UpdateProgressShow(float a_fProgess, string a_szDesc)
        {
            ImgProgress.fillAmount = a_fProgess;
            LbProgress.text = a_szDesc;
        }

    }
}