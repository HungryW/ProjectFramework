using GameFramework;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameFrameworkPackage
{
    public static class STLUtility
    {

        public static void Reverse<T>(ref T[] a_arry)
        {
            T temp;
            for (int i = 0; i < a_arry.Length / 2; i++)
            {
                temp = a_arry[i];
                a_arry[i] = a_arry[a_arry.Length - i - 1];
                a_arry[a_arry.Length - i - 1] = temp;
            }
        }

        public static void SafeSort<T>(this List<T> a_list, Comparison<T> comparison)
        {
            if (a_list != null && a_list.Count > 1)
            {
                a_list.Sort(comparison);
            }
        }

        public static T RandomOne<T>(this List<T> a_list)
        {
            if (a_list == null || a_list.Count == 0)
            {
                return default(T);
            }
            int n = UnityEngine.Random.Range(0, a_list.Count);
            return a_list[n];
        }

        public static void AddRangeNoReapt<T>(this List<T> a_list, List<T> a_listAdd)
        {
            Dictionary<T, bool> mapTemp = new Dictionary<T, bool>();
            List<T> listTemp = new List<T>();
            foreach (var item in a_list)
            {
                if (!mapTemp.ContainsKey(item))
                {
                    mapTemp.Add(item, true);
                    listTemp.Add(item);
                }
            }

            foreach (var item in a_listAdd)
            {
                if (!mapTemp.ContainsKey(item))
                {
                    mapTemp.Add(item, true);
                    listTemp.Add(item);
                }
            }
            a_list.Clear();
            a_list.AddRange(listTemp);
        }

        public static void Exlude<T>(this List<T> a_list, List<T> a_listExlude)
        {
            List<T> listTemp = new List<T>();
            foreach (var item in a_list)
            {
                if (!a_listExlude.Contains(item))
                {
                    listTemp.Add(item);
                }
            }
            a_list.Clear();
            a_list.AddRange(listTemp);
        }

        public static void Unque<T>(this List<T> a_list)
        {
            Dictionary<T, bool> mapTemp = new Dictionary<T, bool>();
            List<T> listTemp = new List<T>();
            foreach (var item in a_list)
            {
                if (!mapTemp.ContainsKey(item))
                {
                    mapTemp.Add(item, true);
                    listTemp.Add(item);
                }
            }

            a_list.Clear();
            a_list.AddRange(listTemp);
        }

        public static List<T> GetRandomItemList<T>(this List<T> a_list, int a_nNum)
        {
            List<T> listItem = new List<T>();
            if (a_list == null || a_list.Count == 0)
            {
                return listItem;
            }
            List<int> listIdx = GetUnqueRandomNumList(0, a_list.Count - 1, a_nNum);
            foreach (int nIdx in listIdx)
            {
                listItem.Add(a_list[nIdx]);
            }
            return listItem;
        }

        public static List<int> GetUnqueRandomNumList(int a_nMinValue, int a_nMaxValue, int a_nNeedNum)
        {
            List<int> listResult = new List<int>();
            if (a_nNeedNum > Mathf.Abs(a_nMaxValue - a_nMinValue))
            {
                for (int i = Mathf.Min(a_nMinValue, a_nMaxValue); i <= Mathf.Max(a_nMinValue, a_nMaxValue); i++)
                {
                    listResult.Add(i);
                }
                return listResult;
            }

            int sum = Mathf.Abs(a_nMaxValue - a_nMinValue) + 1;//计算数组范围
            int site = sum;//设置索引范围
            int[] index = new int[sum];
            int temp = 0;
            for (int i = a_nMinValue; i <= a_nMaxValue; i++)
            {
                index[temp] = i;
                temp++;
            }
            for (int i = 0; i < a_nNeedNum; i++)
            {
                int id = UnityEngine.Random.Range(0, site);
                listResult.Add(index[id]);
                index[id] = index[site - 1];//因id随机到的数已经获取到了，用最后的一个数来替换它
                site--;//缩小索引范围
            }
            return listResult;
        }

        public static void UnSort<T>(this List<T> a_list)
        {
            int currentIndex;
            T tempValue;
            for (int i = 0; i < a_list.Count; i++)
            {
                currentIndex = UnityEngine.Random.Range(0, a_list.Count - i);
                tempValue = a_list[currentIndex];
                a_list[currentIndex] = a_list[a_list.Count - 1 - i];
                a_list[a_list.Count - 1 - i] = tempValue;
            }
        }

        public static string RemoveLast(this string a_sz)
        {
            if (a_sz.Length == 0)
            {
                return a_sz;
            }
            return a_sz.Remove(a_sz.Length - 1, 1);
        }


        public static List<string> SplitToStringList(this string a_sz, char a_cSpector)
        {
            List<string> listVal = new List<string>();
            string[] arrSz = a_sz.Split(a_cSpector);
            for (int i = 0; i < arrSz.Length; i++)
            {
                if(!string.IsNullOrWhiteSpace(arrSz[i]))
                {
                    listVal.Add(arrSz[i]);
                }
            }
            return listVal;
        }

        public static List<int> SplitStringToIntList(this string a_sz, char a_cSpector)
        {
            List<int> listVal = new List<int>();
            string[] arrSz = a_sz.Split(a_cSpector);
            int nVal;
            for (int i = 0; i < arrSz.Length; i++)
            {
                if (int.TryParse(arrSz[i], out nVal))
                {
                    listVal.Add(nVal);
                }
            }
            return listVal;
        }

        public static List<float> SplitStringToFloatList(this string a_sz, char a_cSpector)
        {
            List<float> listVal = new List<float>();
            string[] arrSz = a_sz.Split(a_cSpector);
            float fVal;
            for (int i = 0; i < arrSz.Length; i++)
            {
                if (float.TryParse(arrSz[i], out fVal))
                {
                    listVal.Add(fVal);
                }
            }
            return listVal;
        }

        public static int RandomValByProbability(this string a_sz, int a_nDenominator, int a_nDefaultVal)
        {
            string[] arrProbility = a_sz.Split(';');
            List<int> listNum = new List<int>();
            List<float> listProbility = new List<float>();
            for (int i = 0; i < arrProbility.Length; i++)
            {
                string[] arrItem = arrProbility[i].Split(',');
                if (arrItem.Length != 2)
                {
                    continue;
                }
                listNum.Add(int.Parse(arrItem[0]));
                listProbility.Add(1.0f * int.Parse(arrItem[1]) / a_nDenominator);
            }
            return MathUtility.RandomValByProbability(listNum, listProbility, a_nDefaultVal);
        }

        public static int RandomValByProbability(this string a_sz, int a_nDefaultVal)
        {
            int Denominator = a_sz.ParseTotalWeight();
            return RandomValByProbability(a_sz, Denominator, a_nDefaultVal);
        }

        public static int ParseTotalWeight(this string a_sz)
        {
            string[] arrProbility = a_sz.Split(';');
            int Denominator = 0;
            for (int i = 0; i < arrProbility.Length; i++)
            {
                string[] arrItem = arrProbility[i].Split(',');
                if (arrItem.Length != 2)
                {
                    continue;
                }
                Denominator += int.Parse(arrItem[1]);
            }
            return Denominator;
        }

        public static void ParseRandomValParams(this string a_sz, List<int> a_listVal, List<float> a_listProbility)
        {
            a_listVal.Clear();
            a_listProbility.Clear();
            string[] arrProbility = a_sz.Split(';');
            int Denominator = 0;
            for (int i = 0; i < arrProbility.Length; i++)
            {
                string[] arrItem = arrProbility[i].Split(',');
                if (arrItem.Length != 2)
                {
                    continue;
                }
                Denominator += int.Parse(arrItem[1]);
            }
            for (int i = 0; i < arrProbility.Length; i++)
            {
                string[] arrItem = arrProbility[i].Split(',');
                if (arrItem.Length != 2)
                {
                    continue;
                }
                a_listVal.Add(int.Parse(arrItem[0]));
                a_listProbility.Add(1.0f * int.Parse(arrItem[1]) / Denominator);
            }
        }

        public static string NumberToChinese(this int number)
        {
            string[] UNITS = { "", "十", "百", "千", "万", "十", "百", "千", "亿", };
            string[] NUMS = { "零", "一", "二", "三", "四", "五", "六", "七", "八", "九" };
            if (number == 0)
            {
                return NUMS[0];
            }
            string results = "";
            for (int i = number.ToString().Length - 1; i >= 0; i--)
            {
                int r = (int)(number / (Mathf.Pow(10, i)));
                results += NUMS[r % 10] + UNITS[i];
            }
            results = results.Replace("零十", "零")
                             .Replace("零百", "零")
                             .Replace("零千", "零");

            if (results.StartsWith("一十"))
            {
                results = results.Substring(1);
            }
            while (results.EndsWith("零") && results.Length > 0)
            {
                results = results.RemoveLast();
            }
            return results;
        }

        /// <summary>
        /// 获得从n个不同元素中任意选取m个元素的组合的所有组合形式的列表
        /// </summary>
        /// <param name="elements">供组合选择的元素</param>
        /// <param name="m">组合中选取的元素个数</param>
        /// <returns>返回一个包含列表的列表，包含的每一个列表就是每一种组合可能</returns>
        public static List<List<T>> GetCombinationList<T>(this List<T> elements, int m)
        {
            List<List<T>> result = new List<List<T>>();//存放返回的列表
            List<List<T>> temp = null; //临时存放从下一级递归调用中返回的结果
            List<T> oneList = null; //存放每次选取的第一个元素构成的列表，当只需选取一个元素时，用来存放剩下的元素分别取其中一个构成的列表；
            T oneElment; //每次选取的元素
            List<T> source = new List<T>(elements); //将传递进来的元素列表拷贝出来进行处理，防止后续步骤修改原始列表，造成递归返回后原始列表被修改；
            int n = 0; //待处理的元素个数

            if (elements != null)
            {
                n = elements.Count;
            }
            if (n == m && m != 1)//n=m时只需将剩下的元素作为一个列表全部输出
            {
                result.Add(source);
                return result;
            }
            if (m == 1)  //只选取一个时，将列表中的元素依次列出
            {
                foreach (T el in source)
                {
                    oneList = new List<T>();
                    oneList.Add(el);
                    result.Add(oneList);
                    oneList = null;
                }
                return result;
            }

            for (int i = 0; i <= n - m; i++)
            {
                oneElment = source[0];
                source.RemoveAt(0);
                temp = GetCombinationList(source, m - 1);
                for (int j = 0; j < temp.Count; j++)
                {
                    oneList = new List<T>();
                    oneList.Add(oneElment);
                    oneList.AddRange(temp[j]);
                    result.Add(oneList);
                    oneList = null;
                }
            }


            return result;
        }

        public static string GetChangeValDesc(this int a_nVal)
        {
            if (a_nVal >= 0)
            {
                return Utility.Text.Format("+{0}", a_nVal);
            }
            else
            {
                return Utility.Text.Format("—{0}", -a_nVal);
            }
        }
    }
}




