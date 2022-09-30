using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityGameFramework.Runtime;
using GameFrameworkPackage;

namespace GameFrameworkPackage
{
    public class CUIToolFunBtnMgr : MonoBehaviour
    {
        [SerializeField]
        private CUIToolFunBtnBase PrefabFun;
        [SerializeField]
        private Transform ContentFun;

        private Dictionary<int, CUIToolFunBtnBase> m_mapAllFun;

        private void Awake()
        {
            m_mapAllFun = new Dictionary<int, CUIToolFunBtnBase>();
        }

        public void AddFun(CUIParamToolFunBtnBase a_param)
        {
            if (m_mapAllFun.ContainsKey(a_param.GetId()))
            {
                Log.Error("CUIToolFunBtnMgr AddFun Fail Id Repeat Id ={0]", a_param.GetId());
                return;
            }
            GameObject goFun = GameObject.Instantiate(PrefabFun.gameObject, ContentFun, false);
            m_mapAllFun.Add(a_param.GetId(), goFun.GetComponent<CUIToolFunBtnBase>());
            m_mapAllFun[a_param.GetId()].Init(a_param);
        }

        public void UpdateAllShow()
        {
            foreach (var item in m_mapAllFun)
            {
                item.Value.UpdateShow();
            }
        }

        public void UpdateShowById(int a_nId)
        {
            if (!m_mapAllFun.ContainsKey(a_nId))
            {
                return;
            }
            m_mapAllFun[a_nId].UpdateShow();
        }

        public CUIToolFunBtnBase GetFunBtnById(int a_nId)
        {
            if (!m_mapAllFun.ContainsKey(a_nId))
            {
                return null;
            }
            return m_mapAllFun[a_nId];
        }

        public int GetShowNum()
        {
            int nNum = 0;
            foreach (Transform child in ContentFun)
            {
                if (child.GetComponent<CUIToolFunBtnBase>().GetParam().IsShow())
                {
                    nNum++;
                }
            }
            return nNum;
        }
    }

}
