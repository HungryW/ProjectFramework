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
    public class CHotFixLogicEntityAgentBase
    {
        public CHotFixLogicEntity EntityLogic { private set; get; }
        public void __Init(CHotFixLogicEntity a_refUILogic, object a_oUserData)
        {
            EntityLogic = a_refUILogic;
            _OnInit(a_oUserData);
        }
        protected virtual void _OnInit(object userData)
        {

        }
        public virtual void OnRecycle()
        {

        }

        public virtual void OnShow(object userData)
        {

        }

        public virtual void OnHide(bool isShutdown, object userData)
        {

        }

        public virtual void OnUpdate(float elapseSeconds, float realElapseSeconds)
        {

        }
        public void __OnVisibleChange(bool a_bVisible)
        {
            _OnVisibleChange(a_bVisible);  
        }
        protected virtual void _OnVisibleChange(bool a_bVisible)
        {

        }
    }
}