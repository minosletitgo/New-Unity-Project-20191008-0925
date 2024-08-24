using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameCommon
{
    static public void ASSERT(bool isTrue, string strLogError = null)
    {
        if (!isTrue)
        {
            if (string.IsNullOrEmpty(strLogError))
            {
                strLogError = "GameCommon.CHECK";
            }

            Debug.LogError(strLogError);
            //GameCommonLog.Instance.Log(strLogError);
            throw new Exception(strLogError);
        }
    }

    static public void ASSERT_NoException(bool isTrue, string strLogError = null)
    {
        if (!isTrue)
        {
            if (string.IsNullOrEmpty(strLogError))
            {
                strLogError = "GameCommon.ASSERT_NoException";
            }

            Debug.LogError(strLogError);
            //GameCommonLog.Instance.Log(strLogError);
        }
    }

    static public bool TryGetEnumFromString<T>(string enumString, ref T enumValue)
    {
        Type enumType = typeof(T);
        if (Enum.IsDefined(enumType, enumString))
        {
            enumValue = (T)Enum.Parse(enumType, enumString);
            return true;
        }
        return false;
    }

    static public T GetEnumFromString<T>(string enumString, T defaultEnumValue)
    {
        T enumValue = defaultEnumValue;
        TryGetEnumFromString<T>(enumString, ref enumValue);
        return enumValue;
    }

    static public T GetEnumFromString<T>(string enumString)
    {
        return GetEnumFromString<T>(enumString, default(T));
    }

    public static T GoFind<T>()
    {
        //UnityEngine.Object 不允许直接 强转为T
        //所以加入中间转换为System.Object
        return (T)((object)(UnityEngine.Object.FindObjectOfType(typeof(T))));
    }

    public static string ColorCodeTrans_RGB2Hex(byte r, byte g, byte b)
    {
        //255 0 255 -> #FF00FF

        string strRet = null;
        strRet += "#";
        strRet += r.ToString("X2");
        strRet += g.ToString("X2");
        strRet += b.ToString("X2");

        return strRet;
    }

    public static byte ColorCodeTrans_Hex2RGB_R(string strHex)
    {
        //#FF00FF -> 255

        GameCommon.ASSERT(strHex.Length == 7);

        string strHex_Get = strHex.Substring(1, 2);

        byte byRet = (byte)Int32.Parse(strHex_Get, System.Globalization.NumberStyles.HexNumber);
        return byRet;
    }

    public static byte ColorCodeTrans_Hex2RGB_G(string strHex)
    {
        //#FF00FF -> 0

        GameCommon.ASSERT(strHex.Length == 7);

        string strHex_Get = strHex.Substring(3, 2);

        byte byRet = (byte)Int32.Parse(strHex_Get, System.Globalization.NumberStyles.HexNumber);
        return byRet;
    }

    public static byte ColorCodeTrans_Hex2RGB_B(string strHex)
    {
        //#FF00FF -> 255

        GameCommon.ASSERT(strHex.Length == 7);

        string strHex_Get = strHex.Substring(5, 2);

        byte byRet = (byte)Int32.Parse(strHex_Get, System.Globalization.NumberStyles.HexNumber);
        return byRet;
    }

    public static void SetEffectRenderQueue(GameObject goEffect, int nRenderQueue)
    {
        Renderer[] aryRender = goEffect.GetComponentsInChildren<Renderer>(true);
        if (aryRender != null)
        {
            foreach (var r in aryRender)
            {
                if (r != null && r.sharedMaterial != null)
                {
                    r.sharedMaterial.renderQueue = nRenderQueue;
                }
            }
        }
    }

    public static float CalcScreenPointRatio(Transform trTarget)
    {
        //为了适配所有分辨率

        // Calculate the ratio of the camera's target orthographic size to current screen size
#if UNITY_EDITOR
        float activeSize = UICamera.mainCamera.orthographicSize / trTarget.parent.lossyScale.y;
#else
        float activeSize = UICamera.currentCamera.orthographicSize / trTarget.parent.lossyScale.y;
#endif
        float ratio = (Screen.height * 0.5f) / activeSize;
        return ratio;
    }
    
    public static float TransEulerAnglesToRotation(float fValue)
    {
        //PS: localEulerAngles -> Inspector.localRotation
        GameCommon.ASSERT(fValue >= 0.0f && fValue <= 360.0f);

        if (fValue >= 0.0f && fValue <= 180.0f)
        {
            return fValue;
        }
        else
        {
            return -1.0f * (360.0f - fValue);
        }
    }

    public static string TransEulerAnglesToString(Vector3 v3EulerAngles)
    {
        string strRet = string.Format("({0},{1},{2})", 
            TransEulerAnglesToRotation(v3EulerAngles.x).ToString("0.000"),
            TransEulerAnglesToRotation(v3EulerAngles.y).ToString("0.000"),
            TransEulerAnglesToRotation(v3EulerAngles.z).ToString("0.000")
            );
        return strRet;
    }
    
    public static Vector3 TransEulerAnglesToVector3(Vector3 v3EulerAngles)
    {
        return new Vector3(
            TransEulerAnglesToRotation(v3EulerAngles.x),
            TransEulerAnglesToRotation(v3EulerAngles.y),
            TransEulerAnglesToRotation(v3EulerAngles.z)
            );
    }    

    public static long BuildOnlyLongValue(int nValue1, int nValue2)
    {
        long lRet = nValue1;
        lRet <<= 32;
        lRet += nValue2;
        return lRet;
    }

    public static bool IsNotEqualToZero(float fValue)
    {
        //是否不等于0
        if (Math.Abs(fValue) > 0.000001f)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    public static string PrintVector2(Vector2 v2)
    {
        return string.Format("({0} , {1})",
            v2.x.ToString("0.00"),
            v2.y.ToString("0.00")
            );
    }

    public static string PrintVector3(Vector3 v3)
    {
        return string.Format("({0} , {1} , {2})",
            v3.x.ToString("0.00"),
            v3.y.ToString("0.00"),
            v3.z.ToString("0.00")
            );
    }
    
    public static float GetMinValueInVector2(Vector2 v2Target)
    {
        return Math.Min(v2Target.x, v2Target.y);
    }

    public static float GetMaxValueInVector2(Vector2 v2Target)
    {
        return Math.Max(v2Target.x, v2Target.y);
    }    
}