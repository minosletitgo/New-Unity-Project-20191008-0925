using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*

    以屏幕2D坐标系为基础，搜集椭圆坐标点集合
 
    t = tan(θ) = (Y - Y0)/(X - X0)
    Y = t * (X - X0) + Y0

    Expression:  (X-X0)² = 1/（1/A² + t²/B²）
*/

public class MathEllipseHelper
{
    public static Vector3 GetScreenPostion(
        //Vector3 v3Center,//圆心
        Transform tranCenter,
        float fAxisA,//A轴（即长轴）长度
        float fAxisB,//B轴（即短轴）长度
        float fDegree,//该点在圆心下的基本夹角
        float m_fDegreeRotateOffsetX,//整体椭圆绕圆心的旋转偏移角度
        float m_fDegreeRotateOffsetY,
        float m_fDegreeRotateOffsetZ
        )
    {
        GameCommon.ASSERT(fAxisA > 0);
        GameCommon.ASSERT(fAxisB > 0);
        GameCommon.ASSERT(fDegree >= 0);
        //GameCommon.CHECK(fDegreeRotateOffset >= 0 && fDegreeRotateOffset <= 360);

        float fPointRatio = GameCommon.CalcScreenPointRatio(tranCenter);
        Vector3 v3Center = UICamera.mainCamera.WorldToScreenPoint(tranCenter.position);
        fAxisA = fAxisA * fPointRatio;
        fAxisB = fAxisB * fPointRatio;

        fDegree = MathEllipseHelper.DegreeClamp(fDegree);

        float fTan = Mathf.Tan((Mathf.PI / 180) * fDegree);
        float fExprR = 1.0f / (1 / Mathf.Pow(fAxisA, 2) + Mathf.Pow(fTan, 2) / Mathf.Pow(fAxisB, 2));
        float fExprL_Abs = Mathf.Sqrt(fExprR);

        float fX_01 = fExprL_Abs + v3Center.x;
        float fY_01 = fTan * (fX_01 - v3Center.x) + v3Center.y;
        if (fDegree == 90)
        {
            //强行修正
            fY_01 = v3Center.y + fAxisB;
        }
        if (fDegree == 270)
        {
            //强行修正
            fY_01 = v3Center.y - fAxisB;
        }

        float fX_02 = v3Center.x - fExprL_Abs;
        float fY_02 = fTan * (fX_02 - v3Center.x) + v3Center.y;
        if (fDegree == 90)
        {
            //强行修正
            fY_02 = v3Center.y + fAxisB;
        }
        if (fDegree == 270)
        {
            //强行修正
            fY_02 = v3Center.y - fAxisB;
        }

        Vector2 v2CalcPoint = DoCalcFilterQuadrant(
            v3Center, fDegree,
            fX_01, fY_01,
            fX_02, fY_02
            );

        Vector3 v3Center_World = UICamera.mainCamera.ScreenToWorldPoint(v3Center);

        Vector3 v3CalcPoint_World = UICamera.mainCamera.ScreenToWorldPoint(v2CalcPoint);
        if (m_fDegreeRotateOffsetX != 0)
        {
            v3CalcPoint_World = MathEllipseHelper.V3RotateAround02(
                v3CalcPoint_World, 
                v3Center_World,
                new Vector3(1, 0, 0), 
                m_fDegreeRotateOffsetX
                );
        }
        if (m_fDegreeRotateOffsetY != 0)
        {
            v3CalcPoint_World = MathEllipseHelper.V3RotateAround02(
                v3CalcPoint_World,
                v3Center_World,
                new Vector3(0, 1, 0), 
                m_fDegreeRotateOffsetY
                );
        }
        if (m_fDegreeRotateOffsetZ != 0)
        {
            v3CalcPoint_World = MathEllipseHelper.V3RotateAround02(
                v3CalcPoint_World,
                v3Center_World,
                new Vector3(0, 0, 2), 
                m_fDegreeRotateOffsetZ
                );
        }
        return UICamera.mainCamera.WorldToScreenPoint(v3CalcPoint_World);
    }

    static Vector2 DoCalcFilterQuadrant(
         Vector3 v3Center, float fDegree,
         float fX_01, float fY_01,
         float fX_02, float fY_02
         )
    {
        //以圆心为中心，划分四个象限
        if (fDegree >= 0 && fDegree <= 90)
        {
            if (fX_01 >= v3Center.x && fY_01 >= v3Center.y)
            {
                return new Vector2(fX_01, fY_01);
            }
            if (fX_02 >= v3Center.x && fY_02 >= v3Center.y)
            {
                return new Vector2(fX_02, fY_02);
            }
        }

        if (fDegree >= 90 && fDegree <= 180)
        {
            if (fX_01 <= v3Center.x && fY_01 >= v3Center.y)
            {
                return new Vector2(fX_01, fY_01);
            }
            if (fX_02 <= v3Center.x && fY_02 >= v3Center.y)
            {
                return new Vector2(fX_02, fY_02);
            }
        }

        if (fDegree >= 180 && fDegree <= 270)
        {
            if (fX_01 <= v3Center.x && fY_01 <= v3Center.y)
            {
                return new Vector2(fX_01, fY_01);
            }
            if (fX_02 <= v3Center.x && fY_02 <= v3Center.y)
            {
                return new Vector2(fX_02, fY_02);
            }
        }

        if (fDegree >= 270 && fDegree <= 360)
        {
            if (fX_01 >= v3Center.x && fY_01 <= v3Center.y)
            {
                return new Vector2(fX_01, fY_01);
            }
            if (fX_02 >= v3Center.x && fY_02 <= v3Center.y)
            {
                return new Vector2(fX_02, fY_02);
            }
        }

        Debug.LogError("DoCalcFilterQuadrant -> fDegree: " + fDegree);
        return Vector2.zero;
    }
    
    //以0~360为角度范畴
    public static float DegreeClamp(float fDegree)
    {
        if (fDegree == 0)
        {
            return 0;
        }

        if (fDegree > 0)
        {
            return fDegree % 360;
        }

        if (fDegree < 0)
        {
            return fDegree % 360 + 360;
        }

        return 0;
    }

    static public float SpringLerp(float from, float to, float strength, float deltaTime)
    {
        if (deltaTime > 1f) deltaTime = 1f;
        int ms = Mathf.RoundToInt(deltaTime * 1000f);
        deltaTime = 0.001f * strength;
        for (int i = 0; i < ms; ++i)
        {
            from = Mathf.Lerp(from, to, deltaTime);
        }
        return from;
    }

    public static float GetAngleDegreeInEllipse(GameObject goTarget, UIScrollView_Ellipse sView)
    {
        GameCommon.ASSERT(goTarget != null);
        GameCommon.ASSERT(sView != null);

        Vector3 v3Center = UICamera.mainCamera.WorldToScreenPoint(sView.m_goCenter.transform.position);
        Vector3 v3Center_World = UICamera.mainCamera.ScreenToWorldPoint(v3Center);

        Vector3 _v3PosZero = MathEllipseHelper.GetScreenPostion(
            sView.m_goCenter.transform,
            sView.m_fAxisA,
            sView.m_fAxisB,
            0,
            sView.m_fDegreeRotateOffsetX,
            sView.m_fDegreeRotateOffsetY,
            sView.m_fDegreeRotateOffsetZ
            );
        Vector3 _v2PosZeroDir = _v3PosZero - v3Center;

        //先把[目标点坐标]反向旋转回去
        Vector3 _v3PosTarget_World = goTarget.transform.position;
        if (sView.m_fDegreeRotateOffsetX != 0)
        {
            _v3PosTarget_World = MathEllipseHelper.V3RotateAround02(
                _v3PosTarget_World,
                v3Center_World,
                new Vector3(1, 0, 0),
                -1 * sView.m_fDegreeRotateOffsetX
                );
        }
        if (sView.m_fDegreeRotateOffsetY != 0)
        {
            _v3PosTarget_World = MathEllipseHelper.V3RotateAround02(
                _v3PosTarget_World,
                v3Center_World,
                new Vector3(0, 1, 0),
                -1 * sView.m_fDegreeRotateOffsetY
                );
        }
        if (sView.m_fDegreeRotateOffsetZ != 0)
        {
            _v3PosTarget_World = MathEllipseHelper.V3RotateAround02(
                _v3PosTarget_World,
                v3Center_World,
                new Vector3(0, 0, 1),
                -1 * sView.m_fDegreeRotateOffsetZ
                );
        }
        Vector3 _v3PosTarget_Screen = UICamera.mainCamera.WorldToScreenPoint(_v3PosTarget_World);
        Vector3 _v2PosTargetDir = _v3PosTarget_Screen - v3Center;

        //最终计算出真实角度
        float fAngleDegree = Vector2.SignedAngle(_v2PosZeroDir, _v2PosTargetDir);
        return fAngleDegree;
    }

    //public static Vector3 V3RotateAround(Vector3 v3Src, Vector3 v3Axis, float fDegree)
    //{
    //    //PS:切记[v3Src]为世界坐标（非屏幕坐标）
    //    Quaternion q = Quaternion.AngleAxis(fDegree, v3Axis);
    //    return q * v3Src;
    //}

    public static Vector3 V3RotateAround02(Vector3 position, Vector3 center, Vector3 axis, float angle)
    {
        //PS:切记[v3Src]为世界坐标（非屏幕坐标）
        Vector3 point = Quaternion.AngleAxis(angle, axis) * (position - center);
        Vector3 resultVec3 = center + point;
        return resultVec3;
    }

    public static float CheckAngleDegree(float fAngleDegree)
    {
        //PS: 0 ~ 180 保持不变
        //PS: 181 ~ 360 转化为 负数或0
        float fRet = fAngleDegree - 180;

        if (fRet > 0)
            return fRet - 180;

        return fRet + 180;
    }
}