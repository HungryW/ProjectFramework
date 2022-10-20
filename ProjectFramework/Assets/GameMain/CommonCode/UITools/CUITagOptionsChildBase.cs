using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameFrameworkPackage
{
    public class CUITagOptionsChildBase : MonoBehaviour
    {
        protected CUITagOptions m_refParnetUI;
        protected int m_nIdx;
        public virtual void OnInit(CUITagOptions a_LogicUI)
        {
            m_refParnetUI = a_LogicUI;
        }

        public void SetIdx(int a_nIdx)
        {
            m_nIdx = a_nIdx;
        }

        public virtual bool CanShow()
        {
            return true;
        }

        public virtual void OnClose()
        {

        }

        public virtual void OnOpen()
        {

        }


        public virtual string GetShowName()
        {
            return "";
        }

        public virtual bool NeedHideCheck()
        {
            return false;
        }

        public virtual string GetCheckContent()
        {
            return "";
        }

        public virtual void OnCheckConfirm()
        {

        }

        public virtual bool HaveCorner()
        {
            return false;
        }
    }
}

