using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameFrameworkPackage
{
    public enum EBlurType
    {
        RealTime = 0,
        ScreenShot = 1,
    }

    public class CBlurData
    {
        public float m_fBlurSize;
        public int m_nBlurIteration;
        public int m_nBlurDownSample;
        public float m_fBlurSpread;
    }

    [RequireComponent(typeof(Camera))]
    public partial class CScreenBlurEffect : MonoBehaviour
    {
        private const int mc_nBlurHPass = 0;
        private const int mc_nBlurVPass = 1;

        public Material m_matBlur; //挂高斯模糊shader的材质球
        [Range(0, 127)]
        public float m_fBlurSize = 1.0f;  // 模糊额外散步大小
        [Range(1, 10)]
        public int m_nBlurIteration = 4; // 模糊采样迭代次数
        public float m_fBlurSpread = 1;  // 模糊散值
        public int m_nBlurDownSample = 4; // 模糊初始降采样比率

        private bool m_bIsSupport = true;
        private bool m_bRealTimeEffect = false; //实时模糊
        private bool m_bScreenShotEffect = false; //截图模糊
        private int m_nCurIterateNum = 1;
        private RenderTexture m_rtFinal;
        private RenderTexture m_rtTemp;
        private Action<RenderTexture> m_fnBlurCallback;

        void Awake()
        {
            m_bIsSupport = SystemInfo.supportsImageEffects
                           && m_matBlur != null
                           && m_matBlur.shader.isSupported;
        }

        private void OnRenderImage(RenderTexture source, RenderTexture destination)
        {
            if (m_bIsSupport && (m_bRealTimeEffect || m_bScreenShotEffect))
            {
                // 首先对输出的结果做一次降采样，也就是降低分辨率，减小RT图的大小
                int nWidth = source.width / m_nBlurDownSample;
                int nHeight = source.height / m_nBlurDownSample;

                m_rtFinal = RenderTexture.GetTemporary(nWidth, nHeight, 0);
                Graphics.Blit(source, m_rtFinal);

                for (m_nCurIterateNum = 1; m_nCurIterateNum < m_nBlurIteration; m_nCurIterateNum++)
                {
                    // 设置模糊扩散uv偏移
                    m_matBlur.SetFloat("_BlurSize", (1.0f + m_nCurIterateNum * m_fBlurSpread) * m_fBlurSize);
                    m_rtTemp = RenderTexture.GetTemporary(nWidth, nHeight, 0);
                    Graphics.Blit(m_rtFinal, m_rtTemp, m_matBlur, mc_nBlurHPass);
                    Graphics.Blit(m_rtTemp, m_rtFinal, m_matBlur, mc_nBlurVPass);
                    RenderTexture.ReleaseTemporary(m_rtTemp);
                }

                if (m_bScreenShotEffect)
                {
                    _OnScreenShotBlurRenderEnd(m_rtFinal);
                    RenderTexture.ReleaseTemporary(m_rtFinal); //回调里自己去复制贴图，谁调用谁申请和释放复制的贴图
                    Graphics.Blit(source, destination); // 不修改最终输出画面
                }
                else if (m_bRealTimeEffect)
                {
                    Graphics.Blit(m_rtFinal, destination);
                    RenderTexture.ReleaseTemporary(m_rtFinal);
                }
                else
                {
                    RenderTexture.ReleaseTemporary(m_rtFinal);
                    Graphics.Blit(source, destination);
                }
            }
            else
            {
                Graphics.Blit(source, destination);
            }
        }

        private void _OnScreenShotBlurRenderEnd(RenderTexture a_rt)
        {
            if (m_bScreenShotEffect)
            {
                m_bScreenShotEffect = false;
                this.enabled = false;
            }

            if (m_fnBlurCallback != null)
            {
                Action<RenderTexture> tempFn = m_fnBlurCallback;
                m_fnBlurCallback = null;
                tempFn.Invoke(a_rt);
            }
        }

        public void EnableBlurRender(EBlurType a_eType, CBlurData a_blurData = null, Action<RenderTexture> a_fnCallback = null)
        {
            if (a_blurData != null)
            {
                m_nBlurDownSample = a_blurData.m_nBlurDownSample;
                m_nBlurIteration = a_blurData.m_nBlurIteration;
                m_fBlurSize = a_blurData.m_fBlurSize;
                m_fBlurSpread = a_blurData.m_fBlurSpread;
            }

            if (a_eType == EBlurType.ScreenShot)
            {
                m_bScreenShotEffect = true;
            }
            else if (a_eType == EBlurType.RealTime)
            {
                m_bRealTimeEffect = true;
            }

            m_fnBlurCallback = a_fnCallback;
            this.enabled = true;
        }

        public void DisableBlurRender()
        {
            m_bRealTimeEffect = false;
            m_bScreenShotEffect = false;
            this.enabled = false;
        }
    }
}
