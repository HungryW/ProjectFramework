using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityGameFramework.Runtime;

namespace UI
{
    public class CTextCtrl
    {
        private Text m_LbContent;

        private CTextData m_textData;
        private int m_nCurBlockIndex = 0;
        private float m_fBlockDelayTime = 0f;
        private List<int> m_listNewLineIndex;
        private UnityAction m_fnOnRunEnd = null;

        public void StartShow(Text a_lbContent, string a_szContent)
        {
            m_LbContent = a_lbContent;
            m_LbContent.text = "";
            m_textData = new CTextData(a_szContent);
            m_nCurBlockIndex = 0;
            m_fBlockDelayTime = 0;
            bIsRuning = true;
        }

        public void SkipTypewriter()
        {
            m_nCurBlockIndex = m_textData.GetBlockNum() - 1;
            m_fBlockDelayTime = m_textData.GetBlockByIndex(m_nCurBlockIndex).GetDelayTime();
        }

        public void SetRunEndCallbak(UnityAction a_fn)
        {
            m_fnOnRunEnd = a_fn;
        }

        private void _OnTextShowOver()
        {
            bIsRuning = false;
            m_textData = null;
            m_LbContent = null;
            if (null != m_fnOnRunEnd)
            {
                m_fnOnRunEnd.Invoke();
            }
        }


        public bool bIsRuning
        {
            get;
            private set;
        }

        public void Update()
        {
            if (bIsRuning)
            {
                _MakeTypewriterText();
            }
        }


        private void _MakeTypewriterText()
        {
            if (null == m_LbContent)
            {
                return;
            }
            CTextBlock textBlock = m_textData.GetBlockByIndex(m_nCurBlockIndex);
            if (null == textBlock)
            {
                return;
            }
            m_fBlockDelayTime += Time.deltaTime;
            if (m_fBlockDelayTime < textBlock.GetDelayTime())
            {
                return;
            }
            string szContent = m_textData.SubBlock(0, m_nCurBlockIndex + 1);
            string szEmptyString = new string(' ', m_textData.GetBlockNum());
            if (m_nCurBlockIndex < m_textData.GetBlockNum()) szContent = szContent + szEmptyString.Substring(m_nCurBlockIndex);
            m_LbContent.text = szContent;
            textBlock.OnShow();
            m_nCurBlockIndex++;
            m_fBlockDelayTime = 0f;
            if (m_nCurBlockIndex == m_textData.GetBlockNum())
            {
                _OnTextShowOver();
            }
        }
    }
}


