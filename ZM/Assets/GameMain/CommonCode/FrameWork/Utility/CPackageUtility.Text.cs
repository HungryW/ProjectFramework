
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
        /// 字符相关的实用函数。
        /// </summary>
        public static class Text
        {
            /// <summary>
            /// 将文本按行切分。
            /// </summary>
            /// <param name="text">要切分的文本。</param>
            /// <returns>按行切分后的文本。</returns>
            public static string[] SplitToLines(string text)
            {
                List<string> texts = new List<string>();
                int position = 0;
                string rowText = null;
                while ((rowText = ReadLine(text, ref position)) != null)
                {
                    texts.Add(rowText);
                }

                return texts.ToArray();
            }

            /// <summary>
            /// 根据类型和名称获取完整名称。
            /// </summary>
            /// <typeparam name="T">类型。</typeparam>
            /// <param name="name">名称。</param>
            /// <returns>完整名称。</returns>
            public static string GetFullName<T>(string name)
            {
                return GetFullName(typeof(T), name);
            }

            /// <summary>
            /// 根据类型和名称获取完整名称。
            /// </summary>
            /// <param name="type">类型。</param>
            /// <param name="name">名称。</param>
            /// <returns>完整名称。</returns>
            public static string GetFullName(Type type, string name)
            {
                if (type == null)
                {
                    throw new GameFrameworkException("Type is invalid.");
                }

                string typeName = type.FullName;
                return string.IsNullOrEmpty(name) ? typeName : Utility.Text.Format("{0}.{1}", typeName, name);
            }

            /// <summary>
            /// 获取用于编辑器显示的名称。
            /// </summary>
            /// <param name="fieldName">字段名称。</param>
            /// <returns>编辑器显示名称。</returns>
            public static string FieldNameForDisplay(string fieldName)
            {
                if (string.IsNullOrEmpty(fieldName))
                {
                    return string.Empty;
                }

                string str = Regex.Replace(fieldName, @"^m_", string.Empty);
                str = Regex.Replace(str, @"((?<=[a-z])[A-Z]|[A-Z](?=[a-z]))", @" $1").TrimStart();
                return str;
            }

            private static string ReadLine(string text, ref int position)
            {
                if (text == null)
                {
                    return null;
                }

                int length = text.Length;
                int offset = position;
                while (offset < length)
                {
                    char ch = text[offset];
                    switch (ch)
                    {
                        case '\r':
                        case '\n':
                            if (offset > position)
                            {
                                string line = text.Substring(position, offset - position);
                                position = offset + 1;
                                if ((ch == '\r') && (position < length) && (text[position] == '\n'))
                                {
                                    position++;
                                }

                                return line;
                            }

                            offset++;
                            position++;
                            break;

                        default:
                            offset++;
                            break;
                    }
                }

                if (offset > position)
                {
                    string str = text.Substring(position, offset - position);
                    position = offset;
                    return str;
                }

                return null;
            }

            /// <summary>
            /// Rijndael加密算法
            /// </summary>
            /// <param name="pString">待加密的明文</param>
            /// <param name="pKey">密钥,长度可以为:64位(byte[8]),128位(byte[16]),192位(byte[24]),256位(byte[32])</param>
            /// <param name="iv">iv向量,长度为128（byte[16])</param>
            /// <returns></returns>
            public static string RijndaelEncrypt(string pString, string pKey)
            {
                if (string.IsNullOrEmpty(pKey) || string.IsNullOrEmpty(pString))
                {
                    return pString;
                }
                //密钥
                byte[] keyArray = UTF8Encoding.UTF8.GetBytes(pKey);
                //待加密明文数组
                byte[] toEncryptArray = UTF8Encoding.UTF8.GetBytes(pString);

                //Rijndael解密算法
                RijndaelManaged rDel = new RijndaelManaged();
                rDel.Key = keyArray;
                rDel.Mode = CipherMode.ECB;
                rDel.Padding = PaddingMode.PKCS7;
                ICryptoTransform cTransform = rDel.CreateEncryptor();

                //返回加密后的密文
                byte[] resultArray = cTransform.TransformFinalBlock(toEncryptArray, 0, toEncryptArray.Length);
                return Convert.ToBase64String(resultArray, 0, resultArray.Length);
            }

            /// <summary>
            /// ijndael解密算法
            /// </summary>
            /// <param name="pString">待解密的密文</param>
            /// <param name="pKey">密钥,长度可以为:64位(byte[8]),128位(byte[16]),192位(byte[24]),256位(byte[32])</param>
            /// <param name="iv">iv向量,长度为128（byte[16])</param>
            /// <returns></returns>
            public static string RijndaelDecrypt(string pString, string pKey)
            {
                if (string.IsNullOrEmpty(pKey) || string.IsNullOrEmpty(pString))
                {
                    return pString;
                }
                //解密密钥
                byte[] keyArray = UTF8Encoding.UTF8.GetBytes(pKey);
                //待解密密文数组
                byte[] toEncryptArray = Convert.FromBase64String(pString);

                //Rijndael解密算法
                RijndaelManaged rDel = new RijndaelManaged();
                rDel.Key = keyArray;
                rDel.Mode = CipherMode.ECB;
                rDel.Padding = PaddingMode.PKCS7;
                ICryptoTransform cTransform = rDel.CreateDecryptor();

                //返回解密后的明文
                byte[] resultArray = cTransform.TransformFinalBlock(toEncryptArray, 0, toEncryptArray.Length);
                return UTF8Encoding.UTF8.GetString(resultArray);
            }
        }
    }
}
