using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CBezierUtils
{
    /// <summary>
    /// 根据T值，计算贝塞尔曲线上面相对应的点
    /// </summary>
    /// <param name="t"></param>T值
    /// <param name="p0"></param>起始点
    /// <param name="p1"></param>控制点
    /// <param name="p2"></param>目标点
    /// <returns></returns>根据T值计算出来的贝赛尔曲线点
    private static Vector3 CalculateCubicBezierPoint(float t, Vector3 p0, Vector3 p1, Vector3 p2)
    {
        float u = 1 - t;
        float tt = t * t;
        float uu = u * u;

        Vector3 p = uu * p0;
        p += 2 * u * t * p1;
        p += tt * p2;

        return p;
    }

    /// <summary>
    /// 获取存储贝塞尔曲线点的数组
    /// </summary>
    /// <param name="startPoint"></param>起始点
    /// <param name="controlPoint"></param>控制点
    /// <param name="endPoint"></param>目标点
    /// <param name="segmentNum"></param>采样点的数量
    /// <returns></returns>存储贝塞尔曲线点的数组
    public static Vector3[] GetBeizerList(Vector3 startPoint, Vector3 controlPoint, Vector3 endPoint, int segmentNum)
    {
        Vector3[] path = new Vector3[segmentNum];
        for (int i = 1; i <= segmentNum; i++)
        {
            float t = i / (float)segmentNum;
            Vector3 pixel = CalculateCubicBezierPoint(t, startPoint,
                controlPoint, endPoint);
            path[i - 1] = pixel;
            Debug.Log(path[i - 1]);
        }
        return path;
    }

    public static Vector3[] GetBeizerListByControlPosScale(Vector3 startPoint, Vector3 endPoint, float a_fControlPosOffsetDirectScale, float a_fControlPosOffsetNormalScale, int segmentNum)
    {
        Vector3 controlPoint = GetControlPos(startPoint, endPoint, a_fControlPosOffsetDirectScale, a_fControlPosOffsetNormalScale);
        Vector3[] path = new Vector3[segmentNum];

        for (int i = 1; i <= segmentNum; i++)
        {
            float t = i / (float)segmentNum;
            Vector3 pixel = CalculateCubicBezierPoint(t, startPoint,
                controlPoint, endPoint);
            path[i - 1] = pixel;
        }
        return path;
    }



    public static Vector3 GetControlPos(Vector3 a_v3StartPos, Vector3 a_v3EndPos, float a_fDirectScale, float a_fNormalScale)
    {
        Vector3 v3Offset = a_v3EndPos - a_v3StartPos;
        Vector3 v3NormalOffset = new Vector3(v3Offset.y, -v3Offset.x, v3Offset.z);
        Vector3 v3DirectPos = v3Offset.normalized * a_fDirectScale + a_v3StartPos;
        Vector3 v3ContorlPos = v3DirectPos + a_fNormalScale * v3NormalOffset;
        return v3ContorlPos;
    }
}