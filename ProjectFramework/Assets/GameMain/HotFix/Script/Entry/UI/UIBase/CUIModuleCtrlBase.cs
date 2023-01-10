using UnityEngine;
using GameFrameworkPackage;
using UnityEngine.UI;
using System;
using UnityGameFramework.Runtime;

namespace HotFixEntry
{
    public class CUIModuleCtrlBase : IGameObj
    {
        private Transform m_transform;
        public Transform transform => m_transform;
        public GameObject gameObject => transform.gameObject;

        private void _Init(Transform a_tranAttachedTo)
        {
            m_transform = a_tranAttachedTo;
            _InitComponents();
            _OnInit();
        }
        protected virtual void _InitComponents()
        {

        }

        protected virtual void _OnInit()
        {

        }
        public static T CreateModuleCtrl<T>(IGameObj a_oParent, string a_szChildPath) where T : CUIModuleCtrlBase
        {
            return (T)CreateModuleCtrl(a_oParent, a_szChildPath, typeof(T));
        }

        public static CUIModuleCtrlBase CreateModuleCtrl(IGameObj a_oParent, string a_szChildPath, Type a_tLogicType)
        {
            if (a_tLogicType == null)
            {
                Log.Error("CreateModuleCtrl Fail a_tLogicType == null, a_szChildPath={0}", a_szChildPath);
                return null;
            }
            if (a_oParent == null)
            {
                Log.Error("CreateModuleCtrl Fail a_oParent == null, a_szChildPath={0}", a_szChildPath);
                return null;
            }
            Transform tranAttached = string.IsNullOrEmpty(a_szChildPath) ? a_oParent.transform : a_oParent.transform.Find(a_szChildPath);
            if (tranAttached == null)
            {
                Log.Error("CreateModuleCtrl Fail tranAttached == null, a_szChildPath={0}", a_szChildPath);
                return null;
            }
            return _Create(tranAttached, a_tLogicType);
        }

        public static CUIModuleCtrlBase AttachMoudleCtrl(Transform a_tranAttached, Type a_tLogicType)
        {
            return _Create(a_tranAttached, a_tLogicType);
        }

        private static CUIModuleCtrlBase _Create(Transform a_tranAttached, Type a_tLogicType)
        {
            CUIModuleCtrlBase itemCtrl = (CUIModuleCtrlBase)Activator.CreateInstance(a_tLogicType);
            itemCtrl._Init(a_tranAttached);
            return itemCtrl;
        }
    }

}