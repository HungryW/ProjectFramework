using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;


namespace GameFrameworkPackage
{
    public class CUIParamToolFunBtnHaveNum : CUIParamToolFunBtnHaveIcon
    {
        public delegate int fnGetNum();

        private fnGetNum m_fnGetNumFn;
        public CUIParamToolFunBtnHaveNum(int a_nId) : base(a_nId)
        {
            m_fnGetNumFn = null;
        }

        public CUIParamToolFunBtnHaveNum SetGetNumFun(fnGetNum a_fnGetNum)
        {
            m_fnGetNumFn = a_fnGetNum;
            return this;
        }

        public int GetNum()
        {
            if (m_fnGetNumFn == null)
            {
                return 0;
            }
            return m_fnGetNumFn.Invoke();
        }

        public bool NeedShowNum()
        {
            return m_fnGetNumFn != null;
        }
    }

    public class CUIToolFunBtnHaveNum : CUIToolFunBtnHaveIcon
    {
        [SerializeField]
        private GameObject GoNum;
        [SerializeField]
        private Text LbNum;

        public override void UpdateShow()
        {
            base.UpdateShow();
            CUIParamToolFunBtnHaveNum parm = GetParam() as CUIParamToolFunBtnHaveNum;
            if (null == parm)
            {
                return;
            }
            GoNum.SetActive(parm.NeedShowNum());
            LbNum.text = parm.GetNum().ToString();
        }
    }
}

