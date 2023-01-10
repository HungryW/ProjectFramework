//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;
//using GameFrameworkPackage;
//using GameFramework;
//using UnityEngine.UI;
//using DG.Tweening;
//using Defines;

//namespace GameFrameworkPackage
//{
//    public class CUICommonTextTip : CLogicUI
//    {
//        [SerializeField]
//        private GameObject PrefabTip;

//        private int m_nDoingNum = 0;

//        protected override void OnInit(object userData)
//        {
//            base.OnInit(userData);
//        }

//        protected override void OnUpdateShow(object userData)
//        {
//            base.OnUpdateShow(userData);
//            StopAllCoroutines();
//            m_nDoingNum = 0;
//            if(null != userData)
//            {
//                ShowText(userData as string);
//            }
//        }

//        public void ShowText(string a_szContent)
//        {
//            if (string.IsNullOrEmpty(a_szContent))
//            {
//                return;
//            }
//            GameObject go = GameObject.Instantiate(PrefabTip, transform, false);
//            go.transform.Find("LbContent").GetComponent<Text>().text = a_szContent;
//            Sequence seq = DOTween.Sequence();
//            seq.AppendCallback(() => { go.SetActive(true); m_nDoingNum++; })
//                .AppendInterval(0.8f)
//                .Append(go.transform.DOLocalMoveY(220, 1.2f))
//                .Join(go.transform.Find("LbContent").GetComponent<Text>().DOFade(0, 0.4f))
//                .Join(go.GetComponent<Image>().DOFade(0, 0.4f))
//                .AppendCallback(() =>
//                {
//                    GameObject.Destroy(go, 0.03f);
//                    m_nDoingNum--;
//                });
//        }

//        public static void ShowTip(string a_szContent)
//        {
//            if (string.IsNullOrEmpty(a_szContent))
//            {
//                return;
//            }

//            if (!CGameEntryMgr.UI.GetUIFormVisible(EUIFormID.UICommonTextTip))
//            {
//                CGameEntryMgr.UI.OpenUIForm(EUIFormID.UICommonTextTip, a_szContent);
//            }
//            else
//            {
//                CUICommonTextTip ui = (CGameEntryMgr.UI.GetUIForm(EUIFormID.UICommonTextTip) as CUICommonTextTip);
//                if (ui != null)
//                {
//                    ui.ShowText(a_szContent);
//                }
//            }
          
//        }
//    }
//}

