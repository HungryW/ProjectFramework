using GameFrameworkPackage;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

namespace GameFrameworkPackage
{
    public class CUITagOptions : CLogicUI
    {
        [SerializeField]
        private GameObject SclOption;

        [SerializeField]
        private GameObject PrefabOption;

        [SerializeField]
        private List<CUITagOptionsChildBase> ListOptionsCtrl;


        protected int m_nSeletedOptionIdx;

        protected override void OnInit(object userData)
        {
            base.OnInit(userData);

            for (int i = 0; i < ListOptionsCtrl.Count; i++)
            {
                ListOptionsCtrl[i].OnInit(this);
                ListOptionsCtrl[i].SetIdx(i);
            }
        }

        protected override void OnUpdateShow(object userData)
        {
            base.OnUpdateShow(userData);
            _InitData(userData);
            _UpdateShow();
        }

        private void _InitData(object userData)
        {
            m_nSeletedOptionIdx = userData == null ? 0 : (int)(userData as int?);
        }

        private void _UpdateShow()
        {
            _UpdateInfoOptionScl();
            _UpdateInfoContentShow();
        }

        private void _UpdateInfoOptionScl()
        {
            SclOption.transform.DestoryAllChildren();
            for (int i = 0; i < ListOptionsCtrl.Count; i++)
            {
                int nIdx = i;
                GameObject go = GameObject.Instantiate(PrefabOption, SclOption.transform, false);
                go.transform.Find("ImgBgUnSelect/LbOptionName").GetComponent<Text>().text = ListOptionsCtrl[i].GetShowName();
                go.transform.Find("ImgBgSelected/LbOptionName").GetComponent<Text>().text = ListOptionsCtrl[i].GetShowName();
                go.transform.Find("ImgBgUnSelect").gameObject.SetActive(i != m_nSeletedOptionIdx);
                go.transform.Find("ImgBgSelected").gameObject.SetActive(i == m_nSeletedOptionIdx);
                go.transform.Find("ImgCorner").gameObject.SetActive(ListOptionsCtrl[i].HaveCorner());
                go.transform.Find("BtnSelect").GetComponent<Button>().onClick.AddListener(() => { _OnOptionClick(nIdx); });
                go.transform.name = i.ToString();
                go.SetActive(ListOptionsCtrl[i].CanShow());
            }
        }

        private void _UpdateInfoContentShow()
        {
            for (int i = 0; i < ListOptionsCtrl.Count; i++)
            {
                if (i == m_nSeletedOptionIdx)
                {
                    ListOptionsCtrl[i].gameObject.SetActive(true);
                    ListOptionsCtrl[i].OnOpen();
                }
                else
                {
                    ListOptionsCtrl[i].gameObject.SetActive(false);
                }
            }
        }

        private void _UpdateOptionShow(int a_nId)
        {
            Transform tranOption = _GetOptionGoByIdx(a_nId);
            tranOption.Find("ImgBgUnSelect").gameObject.SetActive(a_nId != m_nSeletedOptionIdx);
            tranOption.Find("ImgBgSelected").gameObject.SetActive(a_nId == m_nSeletedOptionIdx);
        }


        private Transform _GetOptionGoByIdx(int a_nId)
        {
            foreach (Transform tran in SclOption.transform)
            {
                if (tran.name == a_nId.ToString())
                {
                    return tran;
                }
            }
            return null;
        }


        private void _OnOptionClick(int a_nIdx)
        {
            if (a_nIdx == m_nSeletedOptionIdx)
            {
                return;
            }
            if (ListOptionsCtrl[m_nSeletedOptionIdx].NeedHideCheck())
            {
                //CUICommonConfirm.TipConfirm(ListOptionsCtrl[m_nSeletedOptionIdx].GetCheckContent()
                //                            , () => { ListOptionsCtrl[m_nSeletedOptionIdx].OnCheckConfirm(); _ChangeOption(a_nIdx); }
                //                            , () => { _ChangeOption(a_nIdx); });
            }
            else
            {
                _ChangeOption(a_nIdx);
            }
        }

        private void _ChangeOption(int a_nIdx)
        {
            int nTempIdx = m_nSeletedOptionIdx;
            m_nSeletedOptionIdx = a_nIdx;
            _UpdateOptionShow(nTempIdx);
            _UpdateOptionShow(m_nSeletedOptionIdx);
            ListOptionsCtrl[nTempIdx].gameObject.SetActive(false);
            ListOptionsCtrl[nTempIdx].OnClose();
            ListOptionsCtrl[m_nSeletedOptionIdx].gameObject.SetActive(true);
            ListOptionsCtrl[m_nSeletedOptionIdx].OnOpen();
        }

        protected void _OnTryClose(UnityAction a_fnClose)
        {
            if (ListOptionsCtrl[m_nSeletedOptionIdx].NeedHideCheck())
            {
                //CUICommonConfirm.TipConfirm(ListOptionsCtrl[m_nSeletedOptionIdx].GetCheckContent()
                //                            , () => { a_fnClose(); ListOptionsCtrl[m_nSeletedOptionIdx].OnCheckConfirm(); }
                //                            , a_fnClose);
            }
            else
            {
                a_fnClose();
            }
        }

        public void UpdateCornerShow(int a_nId)
        {
            Transform tranOption = _GetOptionGoByIdx(a_nId);
            if (null == tranOption)
            {
                return;
            }
            tranOption.Find("ImgCorner").gameObject.SetActive(ListOptionsCtrl[a_nId].HaveCorner());
        }

    }

}