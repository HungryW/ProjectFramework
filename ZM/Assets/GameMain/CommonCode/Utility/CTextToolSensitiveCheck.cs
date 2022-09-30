using GameFrameworkPackage;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UI
{
    [DisallowMultipleComponent]
    public class CTextToolSensitiveCheck : MonoBehaviour
    {
        [SerializeField]
        private TextAsset TxtSensitive;

        protected static CTextToolSensitiveCheck s_instance;
        public static CTextToolSensitiveCheck Instance
        {
            get
            {
                if (s_instance == null)
                {
                    GameObject PrefabTool = CResourceMgr.Instance.GetRes(CAssestPathUtility.GetResToolPrefab("PrefabSensitiveCheckTool"));
                    return Instantiate(PrefabTool).GetComponent<CTextToolSensitiveCheck>();
                }
                else
                {
                    return s_instance;
                }
            }
        }

        protected virtual void Awake()
        {
            if (s_instance != null)
            {
                Destroy(gameObject);
                return;
            }
            s_instance = this;
            DontDestroyOnLoad(gameObject);
        }

        private HashSet<string> m_setSenWords;
        private int m_nLongestNum = 0;
        private int m_nShortestNum = 10;

        public void InitData()
        {
#if !DEBUG
            if (m_setSenWords != null)
            {
                return;
            }

            m_setSenWords = new HashSet<string>();
            string[] arrSensitiveString = TxtSensitive.text.Split('、');

            foreach (string szWord in arrSensitiveString)
            {
                if (string.IsNullOrEmpty(szWord))
                {
                    continue;
                }
                m_setSenWords.Add(szWord);
                m_nLongestNum = Mathf.Max(m_nLongestNum, szWord.Length);
                m_nShortestNum = Mathf.Min(m_nShortestNum, szWord.Length);
            }
#endif
        }

        public bool CheckHaveSensivieWord(string a_szWord)
        {
            for (int i = 0; i < a_szWord.Length; i++)
            {
                for (int j = m_nShortestNum; j < m_nLongestNum; j++)
                {
                    if (i + j > a_szWord.Length)
                    {
                        break;
                    }
                    if (m_setSenWords.Contains(a_szWord.Substring(i, j)))
                    {
                        return true;
                    }
                }
            }

            return false;
        }
    }
}

