using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;


namespace GameFrameworkPackage
{
    public class CUIParamToolFunBtnHaveIcon : CUIParamToolFunBtnBase
    {
        private string m_szIcon;
        public CUIParamToolFunBtnHaveIcon(int a_nId) : base(a_nId)
        {
            m_szIcon = "";
        }

        public CUIParamToolFunBtnHaveIcon SetIcon(string a_szIcon)
        {
            m_szIcon = a_szIcon;
            return this;
        }

        public string GetIcon()
        {
            return m_szIcon;
        }
    }

    public class CUIToolFunBtnHaveIcon : CUIToolFunBtnBase
    {
        [SerializeField]
        private Image ImgIcon;


        public override void UpdateShow()
        {
            base.UpdateShow();
            ImgIcon.sprite = CUISpriteMgr.GetSprite("FunBtnIcon",(GetParam() as CUIParamToolFunBtnHaveIcon).GetIcon());
        }
    }
}

