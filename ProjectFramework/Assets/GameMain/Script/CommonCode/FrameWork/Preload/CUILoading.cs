using GameFrameworkPackage;
using System.Collections;
using System.Collections.Generic;
using GameFramework;
using UnityEngine;
using UnityEngine.UI;
/// <summary>
/// 初始化过程中需要 loading 界面的，使用者实例化该prefab，使用者负责释放实例
/// </summary>
namespace GameFrameworkPackage
{
    public class CUILoading : MonoBehaviour
    {
        [SerializeField]
        private Text LbProgress;
        [SerializeField]
        private Image ImgFill;
        [SerializeField]
        private Slider SliderProgerss;
        private CanvasScaler m_CanvasScaler;
        private void Start()
        {
            m_CanvasScaler = gameObject.GetOrAddComponent<CanvasScaler>();
            m_CanvasScaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            m_CanvasScaler.screenMatchMode = CanvasScaler.ScreenMatchMode.MatchWidthOrHeight;
            m_CanvasScaler.matchWidthOrHeight = (1.0f * Screen.width / Screen.height) > CConstDevResolution.W_H_Radio ? 1 : 0;
        }

        public void UpdateProgressShow(float a_fProgess, string a_szProgressDescKey)
        {
            int nProgress = Mathf.Clamp((Mathf.CeilToInt(a_fProgess * 100)), 0, 100);
            //LbProgress.text = CGameEntryMgr.Localization.GetStringEX(a_szProgressDescKey, nProgress);
            LbProgress.text = Utility.Text.Format("{0}%", nProgress);
            ImgFill.fillAmount = Mathf.Clamp(a_fProgess, 0, 1);
            SliderProgerss.value = Mathf.Clamp(a_fProgess, 0, 1);
        }

    }
}



