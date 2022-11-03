using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameFramework;
using System.IO;

namespace GameFrameworkPackage
{
    public static class SystemUtility
    {
        public static void SafeInvoke(this Action a_fn)
        {
            if (a_fn != null)
            {
                a_fn.Invoke();
            }
        }

        /// <summary>
        /// 得到现在的秒数时间戳
        /// </summary>
        /// <returns></returns>
        public static long GetCurTimeStamp()
        {
            return CTimeStampMgr.Instance.GetCurTimeStampSed();
        }
        /// <summary>
        /// DateTime转为秒数时间戳
        /// </summary>
        /// <param name="dateTime"></param>
        /// <returns></returns>
        public static long DateTimeToTimeStamp(DateTime dateTime)
        {
            TimeSpan st = dateTime - new DateTime(1970, 1, 1, 0, 0, 0);
            return Convert.ToInt64(st.TotalSeconds);
        }
        /// <summary>
        /// 秒数时间戳转为DateTime
        /// </summary>
        /// <param name="timeStamp"></param>
        /// <returns></returns>
        public static DateTime TimeStampToDateTime(long timeStamp)
        {
            DateTime dateTime = new DateTime(1970, 1, 1, 0, 0, 0);
            return dateTime.AddSeconds(timeStamp);
        }
        /// <summary>
        /// 毫秒数转为时分秒
        /// </summary>
        /// <param name="a_millseconds"></param>
        /// <returns></returns>
        public static string FormatMillTime(long a_millseconds)
        {
            return FormatTime((long)(a_millseconds * 0.001));
        }

        /// <summary>
        /// 秒数转为时分秒
        /// </summary>
        /// <param name="seconds"></param>
        /// <returns></returns>
        public static string FormatTime(long seconds)
        {
            return FormatTimeBySuffix(seconds, "h", "m", "s");
        }

        public static string FormatTimeBySuffix(long seconds, string a_szHSuffix, string a_szMSuffix, string a_szSSuffix)
        {
            TimeSpan ts = new TimeSpan(0, 0, Convert.ToInt32(seconds));
            string str = "";
            if ((int)ts.TotalHours > 0)
            {
                if (ts.Minutes > 0 || ts.Seconds > 0)
                {
                    str = Utility.Text.Format("{0}{1}{2}{3}{4}{5}", ((int)ts.TotalHours).ToString(""), a_szHSuffix, ts.Minutes.ToString("00"), a_szMSuffix, ts.Seconds.ToString("00"), a_szSSuffix);
                }
                else
                {
                    str = Utility.Text.Format("{0}{1}", ((int)ts.TotalHours).ToString(""), a_szHSuffix);
                }
            }
            if ((int)ts.TotalHours == 0 && ts.Minutes > 0)
            {
                if (ts.Seconds > 0)
                {
                    str = Utility.Text.Format("{0}{1}{2}{3}", ts.Minutes.ToString(), a_szMSuffix, ts.Seconds.ToString("00"), a_szSSuffix);
                }
                else
                {
                    str = Utility.Text.Format("{0}{1}", ts.Minutes.ToString(), a_szMSuffix);
                }
            }
            if ((int)ts.TotalHours == 0 && ts.Minutes == 0)
            {
                str = Utility.Text.Format("{0}{1}", ts.Seconds.ToString(), a_szSSuffix);
            }

            return str;
        }

        public static void DebugStream(this MemoryStream stream)
        {
#if DEBUG
            string sz = "";
            byte[] arr = stream.ToArray();
            for (int i = 0; i < arr.Length; i++)
            {
                sz += arr[i];
                sz += " ";
            }
            UnityEngine.Debug.Log(string.Format("content ={0}", sz));
            UnityEngine.Debug.Log(string.Format("Length ={0}", stream.Length));
            UnityEngine.Debug.Log(string.Format("Position ={0}", stream.Position));
#endif
        }
    }

}
