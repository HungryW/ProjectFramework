using System;
using UnityEngine;
namespace GameFrameworkPackage
{
    public class CScreenBlurEffectMgr : ISingleton<CScreenBlurEffectMgr>
    {
        // 获取模糊脚本
        public CScreenBlurEffect m_sceneEffect;
        public CScreenBlurEffect m_uiEffect;

        // 提供模糊截屏
        public void EnableBlurScreenshot(bool use_ui_camera, CBlurData data = null, Action<RenderTexture> callback = null)
        {
            if (use_ui_camera)
            {
                _GetUIEffect().EnableBlurRender(EBlurType.ScreenShot, data, callback);
            }
            else
            {
                _GetSceneEffect().EnableBlurRender(EBlurType.ScreenShot, data, callback);
            }
        }

        // 提供摄像机模糊
        public void EnableBlurRealTimeEffect(bool use_ui_camera, CBlurData data = null)
        {
            if (use_ui_camera)
            {
                _GetUIEffect().EnableBlurRender(EBlurType.RealTime, data);
            }
            else
            {
                _GetSceneEffect().EnableBlurRender(EBlurType.RealTime, data);
            }
        }

        public void DisabledBlurCameraEffect(bool use_ui_camera)
        {
            if (use_ui_camera)
            {
                _GetUIEffect().DisableBlurRender();
            }
            else
            {
                _GetSceneEffect().DisableBlurRender();
            }
        }

        private CScreenBlurEffect _GetUIEffect()
        {
            if (m_uiEffect == null)
            {
                m_uiEffect = CCamera.GetUICamera().GetComponent<CScreenBlurEffect>();
            }
            return m_uiEffect;
        }

        private CScreenBlurEffect _GetSceneEffect()
        {
            if (m_sceneEffect == null)
            {
                m_sceneEffect = CCamera.GetUICamera().GetComponent<CScreenBlurEffect>();
            }
            return m_sceneEffect;
        }

    }
}
