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
    public class CHotFixUILogicAgentBase
    {
        public CHotFixLogicUI UILogic { private set; get; }
        public void Init(CHotFixLogicUI a_refUILogic, object a_oUserData)
        {
            UILogic = a_refUILogic;
            _OnInit(a_oUserData);
        }
        protected virtual void _OnInit(object userData)
        {

        }
        /// <summary>
        /// 界面回收。
        /// </summary>
        public virtual void OnRecycle()
        {

        }

        /// <summary>
        /// 界面打开。
        /// </summary>
        /// <param name="userData">用户自定义数据。</param>
        public virtual void OnOpen(object userData)
        {

        }
        /// <summary>
        /// 界面关闭。
        /// </summary>
        /// <param name="isShutdown">是否是关闭界面管理器时触发。</param>
        /// <param name="userData">用户自定义数据。</param>
        public virtual void OnClose(bool isShutdown, object userData)
        {

        }

        /// <summary>
        /// 界面暂停。
        /// </summary>
        public virtual void OnPause()
        {
        }

        /// <summary>
        /// 界面暂停恢复。
        /// </summary>
        public virtual void OnResume()
        {
        }

        /// <summary>
        /// 界面遮挡。
        /// </summary>
        public virtual void OnCover()
        {
        }

        /// <summary>
        /// 界面遮挡恢复。
        /// </summary>
        public virtual void OnReveal()
        {
        }

        /// <summary>
        /// 界面激活。
        /// </summary>
        /// <param name="userData">用户自定义数据。</param>
        public virtual void OnRefocus(object userData)
        {
        }

        /// <summary>
        /// 界面轮询。
        /// </summary>
        /// <param name="elapseSeconds">逻辑流逝时间，以秒为单位。</param>
        /// <param name="realElapseSeconds">真实流逝时间，以秒为单位。</param>
        public virtual void OnUpdate(float elapseSeconds, float realElapseSeconds)
        {
        }

        /// <summary>
        /// 界面深度改变。
        /// </summary>
        /// <param name="uiGroupDepth">界面组深度。</param>
        /// <param name="depthInUIGroup">界面在界面组中的深度。</param>
        public virtual void OnDepthChanged(int uiGroupDepth, int depthInUIGroup)
        {
        }
    }
}