using GameFramework.Event;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityGameFramework.Runtime;
using UnityEngine.SceneManagement;

namespace GameFrameworkPackage
{
    public class CListenEventData
    {
        public int m_nEventId = -1;
        public EventHandler<GameEventArgs> m_handlerEvent = null;

        public CListenEventData(int a_nId, EventHandler<GameEventArgs> a_handler)
        {
            m_nEventId = a_nId;
            m_handlerEvent = a_handler;
        }
    }

    public partial class CLogicUI
    {
        protected bool m_bNeedBlurBg = true;
        private RawImage m_ImgBlurBg = null;
        private RenderTexture m_rtBlurBg = null;

        private void _OnOpenCreateBlurBg(object a_oUserData)
        {
            _CleanBlurBg();
            InternalSetVisible(false);
            CScreenBlurEffectMgr.Instance.EnableBlurScreenshot(true, null, (rt) => { _OnCreateBlurBgEnd(rt, a_oUserData); });
        }

        private void _OnCloseDelBlurBg()
        {
            _CleanBlurBg();
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
                Destroy(m_ImgBlurBg.gameObject);
                m_ImgBlurBg = null;
            }
        }

        private void _OnCreateBlurBgEnd(RenderTexture a_texture, object a_oUserData)
        {
            m_ImgBlurBg = _CreateBlurBgImg();
            m_rtBlurBg = RenderTexture.GetTemporary(a_texture.width, a_texture.height, 0);
            Graphics.Blit(a_texture, m_rtBlurBg);
            m_ImgBlurBg.texture = m_rtBlurBg;
            InternalSetVisible(true);
            OnUpdateShow(a_oUserData);
        }

        private RawImage _CreateBlurBgImg()
        {
            GameObject goBg = new GameObject("BgBlur");
            goBg.transform.SetParent(this.transform, false);
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

    public abstract partial class CLogicUI : UIFormLogic
    {
        public const int MC_nDepthFactor = 100;
        private const float MC_fFadeTime = 0.3f;

        protected Canvas m_Canvas = null;
        protected bool m_bDontClose = false;
        private CanvasGroup m_CanvasGroup = null;
        private CanvasScaler m_CanvasScaler = null;
        private int m_nBeforeDepth;
        private int m_nConfigId;
        private bool m_bNeedOpenAnim = true;
        private bool m_bNeedResetTxtLanguageShow = true;

        private List<CListenEventData> m_listEeventData = new List<CListenEventData>();

        public int OriginalDepth
        {
            get;
            private set;
        }

        public int Depth
        {
            get
            {
                return m_Canvas.sortingOrder;
            }
        }

        public int GetConfigId()
        {
            return m_nConfigId;
        }


        public void Close()
        {
            Close(true);
        }

        public void Close(bool a_bIgnoreFade)
        {
            StopAllCoroutines();
            if (a_bIgnoreFade)
            {
                CGameEntryMgr.UI.CloseUIForm(this);
            }
            else
            {
                StartCoroutine(_CloseCo(MC_fFadeTime));
            }
        }

        public void BeforeRefocus()
        {
            m_nBeforeDepth = Depth;
        }

        public void ReLoseFocus()
        {
            Canvas[] canvases = GetComponentsInChildren<Canvas>(true);
            for (int i = 0; i < canvases.Length; i++)
            {
                canvases[i].sortingOrder = m_nBeforeDepth;
            }
            m_nBeforeDepth = Depth;
        }

        public void SubscribeEvent(int a_nEventId, EventHandler<GameEventArgs> a_handler)
        {
            m_listEeventData.Add(new CListenEventData(a_nEventId, a_handler));
        }

        public static void SetMainFont(Font a_Font)
        {
            if (a_Font == null)
            {
                Log.Error("Main font is invalid.");
                return;
            }


            GameObject go = new GameObject();
            go.AddComponent<Text>().font = a_Font;
            Destroy(go);
        }

        protected override void OnInit(object userData)
        {
            base.OnInit(userData);
            CUIPackageUserData param = userData as CUIPackageUserData;
            m_bNeedBlurBg = param.NeedBlurBg;
            m_nConfigId = param.nConfigId;
            CGameEntryMgr.UI.SetUIInstanceLocked(this, true);
            m_listEeventData.Clear();
            m_Canvas = gameObject.GetOrAddComponent<Canvas>();
            m_Canvas.worldCamera = CCamera.GetUICamera();
            m_Canvas.overrideSorting = true;
            OriginalDepth = m_Canvas.sortingOrder;

            m_CanvasGroup = gameObject.GetOrAddComponent<CanvasGroup>();

            m_CanvasScaler = gameObject.GetOrAddComponent<CanvasScaler>();
            m_CanvasScaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            m_CanvasScaler.screenMatchMode = CanvasScaler.ScreenMatchMode.MatchWidthOrHeight;
            m_CanvasScaler.matchWidthOrHeight = (1.0f * Screen.width / Screen.height) > CConstDevResolution.W_H_Radio ? 1 : 0;


            RectTransform transform = GetComponent<RectTransform>();
            transform.anchorMin = Vector2.zero;
            transform.anchorMax = Vector2.one;
            transform.anchoredPosition = Vector2.zero;
            transform.sizeDelta = Vector2.zero;

            gameObject.GetOrAddComponent<GraphicRaycaster>();
            _InitLayer();
        }

        private void _InitLayer()
        {
            ParticleSystem[] parS = GetComponentsInChildren<ParticleSystem>(true);
            for (int i = 0; i < parS.Length; i++)
            {
                parS[i].gameObject.layer = GetUILayerIdx();
            }
        }

        protected override void OnOpen(object userData)
        {
            base.OnOpen(userData);
            _SetLogicNessTxtContent();
            CUIPackageUserData param = userData as CUIPackageUserData;
            if (m_bNeedBlurBg)
            {
                _OnOpenCreateBlurBg(param.UserData);
            }
            else
            {
                OnUpdateShow(param.UserData);
            }
        }

        private void _SetLogicNessTxtContent()
        {
            if (!m_bNeedResetTxtLanguageShow)
            {
                return;
            }
            Text[] arrTxt = transform.GetComponentsInChildren<Text>(true);
            foreach (var txt in arrTxt)
            {
                if (txt.name.Contains("txt_"))
                {
                    string szKey = txt.name.Remove(0, 4);
                    if (string.IsNullOrEmpty(szKey))
                    {
                        continue;
                    }
                    if (!CGameEntryMgr.Localization.HasRawString(szKey))
                    {
                        szKey = System.Text.RegularExpressions.Regex.Replace(szKey, @"\s", "");
                    }
                    txt.text = CGameEntryMgr.Localization.GetStringEX(szKey);
                }
            }
            m_bNeedResetTxtLanguageShow = false;
        }

        protected virtual void OnUpdateShow(object userData)
        {

        }

        protected override void OnClose(bool isShutdown, object userData)
        {
            base.OnClose(isShutdown, userData);
            _OnCloseDelBlurBg();
            Log.Debug(string.Format("WHX2 Base Close  UI= {0}", GetType()));
        }

        protected override void OnPause()
        {
            base.OnPause();
        }

        protected override void OnResume()
        {
            base.OnResume();

            //m_CanvasGroup.alpha = 0f;
            //StopAllCoroutines();
            //StartCoroutine(m_CanvasGroup.FadeToAlpha(1f, MC_fFadeTime));
        }

        protected override void OnCover()
        {
            base.OnCover();
        }

        protected override void OnReveal()
        {
            base.OnReveal();
        }

        protected override void OnRefocus(object userData)
        {
            base.OnRefocus(userData);
        }

        protected override void OnUpdate(float a_fElapseSeconds, float a_fRealElapeSeconds)
        {
            base.OnUpdate(a_fElapseSeconds, a_fRealElapeSeconds);
        }


        protected override void OnDepthChanged(int a_nUIGroupDepth, int a_nDepthInUIGroup)
        {
            int nOldDepth = Depth;
            base.OnDepthChanged(a_nUIGroupDepth, a_nDepthInUIGroup);
            int deltaDepth = CUIGroupHelper.MC_nDepthFactor * a_nUIGroupDepth + MC_nDepthFactor * a_nDepthInUIGroup + OriginalDepth - nOldDepth;
            Canvas[] canvases = GetComponentsInChildren<Canvas>(true);
            for (int i = 0; i < canvases.Length; i++)
            {
                canvases[i].sortingOrder += deltaDepth;
            }
            ParticleSystem[] parS = GetComponentsInChildren<ParticleSystem>(true);
            for (int i = 0; i < parS.Length; i++)
            {
                parS[i].GetComponent<Renderer>().sortingOrder += deltaDepth;
            }
            SpriteRenderer[] sprite = GetComponentsInChildren<SpriteRenderer>(true);
            for (int i = 0; i < sprite.Length; i++)
            {
                sprite[i].sortingLayerName = "UI";
                sprite[i].sortingOrder += deltaDepth;
            }
        }

        protected override void InternalSetVisible(bool visible)
        {
            base.InternalSetVisible(visible);
            _OnVisibleChangeHandleEventList(visible);
        }

        protected CanvasScaler GetCanvasScaler()
        {
            return m_CanvasScaler;
        }

        protected Canvas GetCanvas()
        {
            return m_Canvas;
        }

        private void _OnVisibleChangeHandleEventList(bool a_bVisible)
        {
            if (a_bVisible)
            {
                _SubscribeAllEvent();
            }
            else
            {
                _UnsubscribeAllEvent();
            }
        }

        private void _SubscribeAllEvent()
        {
            for (int i = 0; i < m_listEeventData.Count; i++)
            {
                CGameEntryMgr.Event.Subscribe(m_listEeventData[i].m_nEventId, m_listEeventData[i].m_handlerEvent);
            }
        }

        private void _UnsubscribeAllEvent()
        {
            for (int i = 0; i < m_listEeventData.Count; i++)
            {
                CGameEntryMgr.Event.Unsubscribe(m_listEeventData[i].m_nEventId, m_listEeventData[i].m_handlerEvent);
            }
        }

        private IEnumerator _CloseCo(float a_fTime)
        {
            yield return m_CanvasGroup.FadeToAlpha(0f, a_fTime);
            CGameEntryMgr.UI.CloseUIForm(this);
        }


        public static int ms_nUILayerIdx = -1;
        public static int GetUILayerIdx()
        {
            if (ms_nUILayerIdx == -1)
            {
                ms_nUILayerIdx = UnityEngine.LayerMask.NameToLayer("UI");
            }
            return ms_nUILayerIdx;
        }
    }
}