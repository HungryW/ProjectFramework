using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityGameFramework.Runtime;
using GameFramework;

namespace GameFrameworkPackage
{
    /// <summary>
    /// 这个组件直接引用了初始化需要使用的资源，这部分资源是不需要加载的，因为这个组件再初始场景中
    /// </summary>
    public class CPreloadDataComponent : GameFrameworkComponent
    {
        [SerializeField]
        private TextAsset m_TxtBuildInfo;
        [SerializeField]
        private CUIUpdateRes PrefabUIUpdateRes;
        [SerializeField]
        private CUILoading PrefabUILoading;
        [SerializeField]
        private TextAsset TxtPreload;
        [SerializeField]
        private GameObject PrefabUIBg;

        public void InitData()
        {
            _InitBuildInfo();
            _InitPreloadTxt();
        }

        private CBuildInfo m_BuildInfo = null;
        public CBuildInfo GetBuildInfo()
        {
            return m_BuildInfo;
        }

        private CUIUpdateRes m_UpdateResUI;
        public void CreateUpdateResUI()
        {
            if (m_UpdateResUI == null)
            {
                m_UpdateResUI = Instantiate(PrefabUIUpdateRes, transform);
            }
        }

        public void SetUpdateResProgress(float a_fProgess, string a_szProgressDesc)
        {
            if (m_UpdateResUI != null)
            {
                m_UpdateResUI.UpdateProgressShow(a_fProgess, a_szProgressDesc);
            }
        }

        public void DelUpdateResUI()
        {
            if (null != m_UpdateResUI)
            {
                Destroy(m_UpdateResUI.gameObject);
                m_UpdateResUI = null;
            }
        }

        private CUILoading m_loadingUI;
        public void CreateLoadingUI()
        {
            if (m_loadingUI == null)
            {
                m_loadingUI = Instantiate(PrefabUILoading, transform);
            }
        }

        public void UpdateLoadingUI(float a_fProgess, string a_szProgressDescKey)
        {
            if (m_loadingUI != null)
            {
                m_loadingUI.UpdateProgressShow(a_fProgess, a_szProgressDescKey);
            }
        }

        public void DelLoadingUI()
        {
            if (null != m_loadingUI)
            {
                Destroy(m_loadingUI.gameObject);
                m_loadingUI = null;
            }
        }

        private CUILoadingBg m_UIBg = null;
        public void CreateUIBg()
        {
            DelUIBg();
            m_UIBg = Instantiate(PrefabUIBg, transform).GetComponent<CUILoadingBg>();
        }

        public void SetBgVisible(bool a_IsShow)
        {
            if (m_UIBg == null)
            {
                return;
            }
            m_UIBg.gameObject.SetActive(a_IsShow);
        }

        public CUILoadingBg GetUIBg()
        {
            return m_UIBg;
        }

        public void DelUIBg()
        {
            if (m_UIBg != null)
            {
                Destroy(m_UIBg);
                m_UIBg = null;
            }
        }

        private void _InitBuildInfo()
        {
            if (m_TxtBuildInfo == null || string.IsNullOrEmpty(m_TxtBuildInfo.text))
            {
                return;
            }

            m_BuildInfo = Utility.Json.ToObject<CBuildInfo>(m_TxtBuildInfo.text);
        }

        private void _InitPreloadTxt()
        {
            if (null == TxtPreload || string.IsNullOrEmpty(TxtPreload.text))
            {
                Log.Info("Default dictionary can not be found or empty.");
                return;
            }

            if (!CGameEntryMgr.Localization.ParseData(TxtPreload.text))
            {
                Log.Warning("Parse default dictionary failure.");
                return;
            }
        }
    }
}