using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameFrameworkPackage
{
    public static class MathUtility
    {
        // Return a random integer number between min [inclusive] and max [inclusive]
        public static int TrueRandom(int a_nMin, int a_nMax)
        {
            Random.InitState(Mathf.RoundToInt(Random.value * 1000000000));
            return Random.Range(a_nMin, a_nMax + 1);
        }
        public static int RandomRange(this AnimationCurve thisCurve, int a_nMin, int a_nMax)
        {
            float fRandomVal = thisCurve.Evaluate(Random.value);
            int nMax = Mathf.Max(a_nMin, a_nMax);
            int nMin = Mathf.Min(a_nMin, a_nMax);
            return Mathf.RoundToInt(fRandomVal * (nMax - nMin) + nMin);
        }

        public static float CalcLnSuccessRate(int a_nCurNum, int a_nNeedNum)
        {
            float mc_fParam1 = 0.3886f;
            float mc_fParam2 = 0.109821f;
            float fVal = 10.0f * a_nCurNum / a_nNeedNum;
            return mc_fParam1 * Mathf.Log(fVal) + mc_fParam2;
        }

        public static int CeilToAbsBigInt(float a_fNum)
        {
            if (a_fNum < 0)
            {
                return Mathf.FloorToInt(a_fNum);
            }
            else
            {
                return Mathf.CeilToInt(a_fNum);
            }
        }

        public static bool IsEqual(float a_fA, float a_fB)
        {
            return Mathf.Abs(a_fA - a_fB) < 0.00001f;
        }

        public static Vector2 CalculationChangeVector2Degress(Vector2 a_v2Original, float a_fChangeDegress)
        {
            float fChangeRadinas = a_fChangeDegress * Mathf.Deg2Rad;
            float fSinChange = Mathf.Sin(fChangeRadinas);
            float fCosChange = Mathf.Cos(fChangeRadinas);
            float fNewX = a_v2Original.x * fCosChange + a_v2Original.y * fSinChange;
            float fNewY = a_v2Original.y * fCosChange - a_v2Original.x * fSinChange;
            Vector2 vNew = new Vector2(fNewX, fNewY).normalized;
            return vNew;
        }

        public static Vector2 CalcVector2DByDegress(float a_fDegress)
        {
            float fRadians = a_fDegress * Mathf.Deg2Rad;
            return new Vector2(Mathf.Cos(fRadians), Mathf.Sin(fRadians));
        }

        public static Vector2 GetFixInScreenPos(Camera a_camera, Vector2 a_v2Pos, Vector2 a_v2Size)
        {
            Vector2 v2Fix = Vector2.zero;
            float fScreenH = a_camera.orthographicSize * 2;
            float fScreenW = fScreenH * Screen.width * 1.0f / Screen.height;
            v2Fix.x = Mathf.Max(Mathf.Min((fScreenW - a_v2Size.x) / 2, a_v2Pos.x), (-fScreenW + a_v2Size.x) / 2);
            v2Fix.y = Mathf.Max(Mathf.Min((fScreenH - a_v2Size.y) / 2, a_v2Pos.y), (-fScreenH + a_v2Size.y) / 2);
            return v2Fix;
        }

        public static bool CheckPointInRect(Vector2 a_nRectPos, Vector2 a_v2RectSize, Vector2 a_v2Pos)
        {
            if (a_v2Pos.x > a_nRectPos.x + a_v2RectSize.x / 2)
            {
                return false;
            }
            if (a_v2Pos.x < a_nRectPos.x - a_v2RectSize.x / 2)
            {
                return false;
            }
            if (a_v2Pos.y > a_nRectPos.y + a_v2RectSize.y / 2)
            {
                return false;
            }
            if (a_v2Pos.y < a_nRectPos.y - a_v2RectSize.y / 2)
            {
                return false;
            }
            return true;
        }



        public static List<int> GetRandomList(int a_nMin, int a_nMax, int a_nNum)
        {
            List<int> listVal = new List<int>();
            if (a_nNum > a_nMax - a_nMin)
            {
                for (int i = a_nMin; i <= a_nMax; i++)
                {
                    listVal.Add(i);
                }
                return listVal;
            }

            Hashtable hashtable = new Hashtable();
            for (int i = 0; hashtable.Count < a_nNum; i++)
            {
                int nValue = TrueRandom(a_nMin, a_nMax);
                if (!hashtable.ContainsValue(nValue))
                {
                    hashtable.Add(nValue, nValue);
                    listVal.Add(nValue);
                }
            }

            return listVal;
        }

        public static int RandomValByProbability(List<int> listNum, List<float> listProbility, int a_nDefaultVal)
        {
            float fRandomProbility = Random.value;
            float fTotalProbility = 0;
            for (int i = 0; i < listProbility.Count; i++)
            {
                fTotalProbility += listProbility[i];
                if (fTotalProbility >= fRandomProbility)
                {
                    return listNum[i];
                }
            }
            return a_nDefaultVal;
        }

        public static int RandomValByWeight(List<int> listNum, List<int> listWeight, int a_nDefaultVal)
        {
            List<float> listProbability = new List<float>();
            int nTotalWeight = 0;
            foreach (int nWeight in listWeight)
            {
                nTotalWeight += nWeight;
            }

            foreach (int nWeight in listWeight)
            {
                listProbability.Add(1.0f * nWeight / nTotalWeight);
            }

            return RandomValByProbability(listNum, listProbability, a_nDefaultVal);
        }

        
    }

 
}


