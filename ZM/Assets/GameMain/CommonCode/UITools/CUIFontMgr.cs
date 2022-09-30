using GameFramework.Resource;
using System.Collections.Generic;
using UnityEngine;
using UnityGameFramework.Runtime;

namespace GameFrameworkPackage
{
    public enum CustomFont
    {
        BattleGreen,
        BattleRed
    }
    public class CUIFontMgr:ISingleton<CUIFontMgr>
    {
        private Dictionary<CustomFont, Font> m_Fonts;
        public CUIFontMgr()
        {
            m_Fonts = new Dictionary<CustomFont, Font>();
        }
        public void AddFont(CustomFont name,Font font)
        {
            m_Fonts.Add(name,font);
        }
        public Font GetCustomFont(CustomFont fontName)
        {
            return m_Fonts[fontName];
        }
    }
}
