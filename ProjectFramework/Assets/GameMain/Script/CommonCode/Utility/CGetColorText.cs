
using GameFramework;
using System.Collections.Generic;
using UnityEngine;

namespace Logic
{
    public enum ETxtColor
    {
        Invalid = -1,
        Normal = 0,
        Red = 1,
        Green = 2,
        Gray = 3,
        Orange = 4,
        BrilliantYellow = 5,
        BrilliantGreen = 6,
        BrilliantRed = 7,
        DarkRed = 8,
        White = 9,
    }

    public static class CColorTxtExtend
    {
        private static Dictionary<ETxtColor, string> ms_mapColorConfig = new Dictionary<ETxtColor, string>();

        private static void _InitColorConfig()
        {
            if (ms_mapColorConfig.Count > 0)
            {
                return;
            }
            ms_mapColorConfig.Add(ETxtColor.Normal, "6C321BFF");
            ms_mapColorConfig.Add(ETxtColor.Red, "E91A1AFF");
            ms_mapColorConfig.Add(ETxtColor.Green, "218724FF");
            ms_mapColorConfig.Add(ETxtColor.Gray, "B2B2B2FF");
            ms_mapColorConfig.Add(ETxtColor.Orange, "D17B02FF");
            ms_mapColorConfig.Add(ETxtColor.BrilliantYellow, "FFF4D2FF");
            ms_mapColorConfig.Add(ETxtColor.BrilliantGreen, "51D248FF");
            ms_mapColorConfig.Add(ETxtColor.BrilliantRed, "FE5F5FFF");
            ms_mapColorConfig.Add(ETxtColor.DarkRed, "800000FF");
            ms_mapColorConfig.Add(ETxtColor.White, "FFFFFFFF");
        }

        public static string GetColorText(this string a_szTxt, ETxtColor a_eColor)
        {
            if (a_eColor == ETxtColor.Invalid)
            {
                return a_szTxt;
            }
            CColorTxtExtend._InitColorConfig();
            string szColor = ms_mapColorConfig.ContainsKey(a_eColor) ? ms_mapColorConfig[a_eColor] : ms_mapColorConfig[ETxtColor.Normal];
            return Utility.Text.Format("<color=#{0}>{1}</color>", szColor, a_szTxt);
        }

        public static string GetColorText(this string a_szTxt, string a_szColor)
        {
            return Utility.Text.Format("<color=#{0}>{1}</color>", a_szColor, a_szTxt);
        }

    }
}
