
using GameFramework;
using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;

namespace GameFrameworkPackage
{
    public static partial class CPackageUtility
    {
        /// <summary>
        /// 路径相关的实用函数。
        /// </summary>
        public static class Path
        {
            public static string GetCombinePath(params string[] path)
            {
                if (path == null || path.Length < 1)
                {
                    return null;
                }

                string combinePath = path[0];
                for (int i = 1; i < path.Length; i++)
                {
                    combinePath = System.IO.Path.Combine(combinePath, path[i]);
                }

                return Utility.Path.GetRegularPath(combinePath);
            }
        }
    }
}
