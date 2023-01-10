using UnityEngine;
using GameFrameworkPackage;
using UnityEngine.UI;
using System;

namespace HotFixEntry
{
    public class CUIBlurBg
    {
        private Transform m_refParentUI;
        private RawImage m_ImgBlurBg;
        private RenderTexture m_rtBlurBg;

        public CUIBlurBg(Transform a_refParentUI)
        {
            m_refParentUI = a_refParentUI;
            m_ImgBlurBg = null;
            m_rtBlurBg = null;
        }

        public void CreateBlurBg(Action a_fnOnCreateEnd)
        {
            _CleanBlurBg();
            _SetParentVisible(false);
            CScreenBlurEffectMgr.Instance.EnableBlurScreenshot(true, null, (rt) =>
            {
                _SetBlurBg(rt);
                a_fnOnCreateEnd.SafeInvoke();
            });
        }

        public void DelBlurBg()
        {
            _CleanBlurBg();
        }

        private void _SetParentVisible(bool a_bIsShow)
        {
            m_refParentUI.gameObject.SetActive(a_bIsShow);
        }

        private void _CleanBlurBg()
        {
            if (m_rtBlurBg != null)
            {
                RenderTexture.ReleaseTemporary(m_rtBlurBg);
                m_rtBlurBg = null;
            }

            if (m_ImgBlurBg != null)
            {
                UnityEngine.Object.Destroy(m_ImgBlurBg.gameObject);
                m_ImgBlurBg = null;
            }
        }

        private void _SetBlurBg(RenderTexture a_texture)
        {
            m_ImgBlurBg = _CreateBlurBgImg();
            m_rtBlurBg = RenderTexture.GetTemporary(a_texture.width, a_texture.height, 0);
            Graphics.Blit(a_texture, m_rtBlurBg);
            m_ImgBlurBg.texture = m_rtBlurBg;
            _SetParentVisible(true);
        }

        private RawImage _CreateBlurBgImg()
        {
            GameObject goBg = new GameObject("BgBlur");
            goBg.transform.SetParent(m_refParentUI, false);
            goBg.transform.localScale = Vector3.one;
            goBg.transform.SetAsFirstSibling();
            RectTransform tran = goBg.AddComponent<RectTransform>();
            tran.anchorMin = Vector2.zero;
            tran.anchorMax = Vector2.one;
            tran.anchoredPosition = Vector2.zero;
            tran.sizeDelta = Vector2.zero;
            Vector3 local_pos = tran.localPosition;
            tran.localPosition = new Vector3(local_pos.x, local_pos.y, 0);
            return goBg.AddComponent<RawImage>();
        }
    }
}