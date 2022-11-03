using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

public class CConstTextParse
{
    public static string mc_szTagHeadStart = "<user";
    public static string mc_szTagParmStart = "?";
    public static string mc_szTagHeadEnd = ">";
    public static string mc_szTagTail = "</user>";

    public static char mc_szTagEventSeparator = ';';
    public static char mc_szEventInfoSeparator = '=';

    public static float mc_fDafaultTime = 0.1f;
    public static string mc_szDafaultColor = "";

    public static string mc_szTagTypeTime = "t";
    public static string mc_szTagTypeColor = "c";
}

public abstract class CTextBlockEventBase
{
    protected string m_szParam;

    public virtual void Init(string a_szParam)
    {
        m_szParam = a_szParam;

    }

    public virtual void Fire()
    {

    }

    public virtual bool CheckVaild()
    {
        return true;
    }
}

public class CTextBlockEventTime : CTextBlockEventBase
{
    public float DelayTime
    {
        get;
        private set;
    }

    public override void Init(string a_szParam)
    {
        base.Init(a_szParam);
        DelayTime = float.Parse(a_szParam);
    }

    public override void Fire()
    {
        base.Fire();
    }

    public override bool CheckVaild()
    {
        float fTime;
        return float.TryParse(m_szParam, out fTime);
    }
}

public class CTextBlockEventColor : CTextBlockEventBase
{
    public string Color
    {
        get;
        private set;
    }

    public override void Init(string a_szParam)
    {
        base.Init(a_szParam);
        Color = a_szParam;
    }

    public override void Fire()
    {
        base.Fire();
        Debug.Log("CTextBlockEventColor fire " + Color);
    }

    public override bool CheckVaild()
    {
        return Color.Length == 6;
    }
}

public class CTextBlock
{
    private static Dictionary<string, Type> m_mapEventConfig;

    static CTextBlock()
    {
        m_mapEventConfig = new Dictionary<string, Type>();
        m_mapEventConfig.Add(CConstTextParse.mc_szTagTypeTime, typeof(CTextBlockEventTime));
        m_mapEventConfig.Add(CConstTextParse.mc_szTagTypeColor, typeof(CTextBlockEventColor));
    }

    private List<CTextBlockEventBase> m_listEvent;
    private string m_szContent;

    public CTextBlock(string a_szContent, string a_szTag)
    {
        m_szContent = a_szContent;
        m_listEvent = new List<CTextBlockEventBase>();
        string[] arrTags = a_szTag.Split(CConstTextParse.mc_szTagEventSeparator);
        for (int i = 0; i < arrTags.Length; i++)
        {
            _ParseEvent(arrTags[i]);
        }
    }

    private void _ParseEvent(string a_szEvent)
    {
        if (string.IsNullOrEmpty(a_szEvent))
        {
            return;
        }

        string[] arrEventInfo = a_szEvent.Split(CConstTextParse.mc_szEventInfoSeparator);
        CTextBlockEventBase eventInfo = Activator.CreateInstance(m_mapEventConfig[arrEventInfo[0]], true) as CTextBlockEventBase;
        eventInfo.Init(arrEventInfo.Length == 2 ? arrEventInfo[1] : null);
        m_listEvent.Add(eventInfo);

    }

    public string GetContent()
    {
        if (string.IsNullOrEmpty(GetColor()))
        {
            return m_szContent;
        }
        else
        {
            return string.Format("<color=#{0}>{1}</color>", GetColor(), m_szContent);
        }
    }

    public float GetDelayTime()
    {
        for (int i = m_listEvent.Count - 1; i >= 0; i--)
        {
            CTextBlockEventTime timeEvent = m_listEvent[i] as CTextBlockEventTime;
            if (null != timeEvent)
            {
                return timeEvent.DelayTime;
            }
        }
        return CConstTextParse.mc_fDafaultTime;
    }

    public string GetColor()
    {
        for (int i = m_listEvent.Count - 1; i >= 0; i--)
        {
            CTextBlockEventColor colorEvent = m_listEvent[i] as CTextBlockEventColor;
            if (null != colorEvent)
            {
                return colorEvent.Color;
            }
        }
        return CConstTextParse.mc_szDafaultColor;
    }

    public void OnShow()
    {
        for (int i = 0; i < m_listEvent.Count; i++)
        {
            m_listEvent[i].Fire();
        }
    }
}

public class CTextData
{
    private string m_szOriginal;
    private List<CTextBlock> m_listBlock;
    private List<string> m_StackTag;

    public CTextData(string a_szOriginal)
    {
        m_szOriginal = a_szOriginal;
        m_listBlock = new List<CTextBlock>();
        m_StackTag = new List<string>();
        _ParseContent(m_szOriginal);
    }

    public List<CTextBlock> GetBlockList()
    {
        return m_listBlock;
    }

    public int GetBlockNum()
    {
        return m_listBlock.Count;
    }

    public CTextBlock GetBlockByIndex(int a_nIndex)
    {
        if (a_nIndex < 0 || a_nIndex >= GetBlockNum())
        {
            return null;
        }

        return m_listBlock[a_nIndex];
    }

    public string SubBlock(int a_nStartIndex, int a_nLength)
    {
        a_nStartIndex = Math.Max(0, Math.Min(GetBlockNum() - 1, a_nStartIndex));
        a_nLength = Math.Max(0, Math.Min(a_nLength, GetBlockNum() - a_nStartIndex));
        StringBuilder str = new StringBuilder();
        for (int i = 0; i < a_nLength; i++)
        {
            str.Append(m_listBlock[i + a_nStartIndex].GetContent());
        }
        return str.ToString();
    }

    public bool Contains(string a_szVal)
    {
        for (int i = 0; i < m_listBlock.Count; i++)
        {
            if (m_listBlock[i].GetContent().Contains(a_szVal))
            {
                return true;
            }
        }
        return false;
    }

    private void _ParseContent(string sz)
    {
        int nHeadStartIndex = sz.IndexOf(CConstTextParse.mc_szTagHeadStart);
        int nTailIndex = sz.LastIndexOf(CConstTextParse.mc_szTagTail);
        if (nHeadStartIndex == -1 || nTailIndex == -1)
        {
            _InsertParseWord(sz);
            return;
        }
        _InsertParseWord(sz.Substring(0, nHeadStartIndex));
        int nHeadParmStart = sz.IndexOf(CConstTextParse.mc_szTagParmStart, nHeadStartIndex);
        int nHeadEndIndex = sz.IndexOf(CConstTextParse.mc_szTagHeadEnd, nHeadStartIndex);
        _PushTag(sz.Substring(nHeadParmStart + 1, nHeadEndIndex - nHeadParmStart - 1));
        _ParseContent(sz.Substring(nHeadEndIndex + 1, nTailIndex - nHeadEndIndex - 1));
        _PopTag();
        _InsertParseWord(sz.Substring(nTailIndex + CConstTextParse.mc_szTagTail.Length, sz.Length - (nTailIndex + CConstTextParse.mc_szTagTail.Length)));
    }

    private void _PushTag(string a_szTag)
    {
        m_StackTag.Add(a_szTag);
    }

    private void _PopTag()
    {
        m_StackTag.RemoveAt(m_StackTag.Count - 1);
    }

    private void _InsertParseWord(string a_sz)
    {
        for (int i = 0; i < a_sz.Length; i++)
        {
            m_listBlock.Add(new CTextBlock(a_sz[i].ToString(), _GetEventStr()));
        }
    }

    private string _GetEventStr()
    {
        string szEvent = "";
        for (int i = 0; i < m_StackTag.Count; i++)
        {
            if (i > 0)
            {
                szEvent = szEvent + CConstTextParse.mc_szTagEventSeparator;
            }
            szEvent = szEvent + m_StackTag[i];
        }
        return szEvent;
    }
}
