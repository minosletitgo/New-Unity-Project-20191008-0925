using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LogicWorldEarthMapHelper : Singleton<LogicWorldEarthMapHelper>
{
    public LogicWorldEarthMapHelper()
    {
        //预留...
    }

    #region ########################DragMapWindow_OrthographicSize#################################

    public enum EM_OrthographicSize
    {
        INVALID = -1,

        Min,
        ShowPortAll,
        ShowPortLess,
        Max,

        MAXCOUNT,
    };
    float m_fOrthographicSize_Min;
    float m_fOrthographicSize_ShowPortAll;
    float m_fOrthographicSize_ShowPortLess;
    float m_fOrthographicSize_Max;


    public enum EM_OrthographicSizeLev
    {
        INVALID = -1,

        Default,
        Lev1,
        Lev2,
        Lev3,
        Lev4,
        Lev5,

        MAXCOUNT,
    };
    float m_fOrthographicSize_Default;
    float m_fOrthographicSize_Lev2;
    float m_fOrthographicSize_Lev3;
    float m_fOrthographicSize_Lev4;


    bool m_bIsInitOrthographicSize = false;
    public void InitOrthographicSize()
    {
        GameCommon.ASSERT(!m_bIsInitOrthographicSize);

        //一些摄像机伸缩范围        
        m_fOrthographicSize_Min = 0.6f/*GameConfig.Get(EDT_GAME_CONFIG.MAP2D_ORTHOGRAPHICSIZE_MIN).value*/;
        m_fOrthographicSize_ShowPortAll = 1.1f/*GameConfig.Get(EDT_GAME_CONFIG.MAP2D_ORTHOGRAPHICSIZE_SHOWPORTALL).value*/;
        m_fOrthographicSize_ShowPortLess = 1.7f/*GameConfig.Get(EDT_GAME_CONFIG.MAP2D_ORTHOGRAPHICSIZE_SHOWPORTLESS).value*/;
        m_fOrthographicSize_Max = 4.0f/*GameConfig.Get(EDT_GAME_CONFIG.MAP2D_ORTHOGRAPHICSIZE_MAX).value*/;
        GameCommon.ASSERT(m_fOrthographicSize_Min < m_fOrthographicSize_ShowPortAll);
        GameCommon.ASSERT(m_fOrthographicSize_ShowPortAll < m_fOrthographicSize_ShowPortLess);
        GameCommon.ASSERT(m_fOrthographicSize_ShowPortLess < m_fOrthographicSize_Max);

        m_fOrthographicSize_Default = 1.1f/*GameConfig.Get(EDT_GAME_CONFIG.MAP2D_ORTHOGRAPHICSIZE_DEFAULT).value*/;
        m_fOrthographicSize_Lev2 = 1.1f/*GameConfig.Get(EDT_GAME_CONFIG.MAP2D_ORTHOGRAPHICSIZE_LEVEL2).value*/;
        m_fOrthographicSize_Lev3 = 1.7f/*GameConfig.Get(EDT_GAME_CONFIG.MAP3D_ORTHOGRAPHICSIZE_LEVEL3).value*/;
        m_fOrthographicSize_Lev4 = 2.4f/*GameConfig.Get(EDT_GAME_CONFIG.MAP4D_ORTHOGRAPHICSIZE_LEVEL4).value*/;
        GameCommon.ASSERT(m_fOrthographicSize_Min < m_fOrthographicSize_Lev2);
        GameCommon.ASSERT(m_fOrthographicSize_Lev2 < m_fOrthographicSize_Lev3);
        GameCommon.ASSERT(m_fOrthographicSize_Lev3 < m_fOrthographicSize_Lev4);
        GameCommon.ASSERT(m_fOrthographicSize_Lev4 < m_fOrthographicSize_Max);
        GameCommon.ASSERT(
            m_fOrthographicSize_Default == m_fOrthographicSize_Min ||
            m_fOrthographicSize_Default == m_fOrthographicSize_Lev2 ||
            m_fOrthographicSize_Default == m_fOrthographicSize_Lev3 ||
            m_fOrthographicSize_Default == m_fOrthographicSize_Lev4 ||
            m_fOrthographicSize_Default == m_fOrthographicSize_Max
            );

        m_bIsInitOrthographicSize = true;
    }

    public float GetOrthographicSize(EM_OrthographicSize emSize)
    {
        switch (emSize)
        {
            case EM_OrthographicSize.Min: return m_fOrthographicSize_Min;
            case EM_OrthographicSize.ShowPortAll: return m_fOrthographicSize_ShowPortAll;
            case EM_OrthographicSize.ShowPortLess: return m_fOrthographicSize_ShowPortLess;
            case EM_OrthographicSize.Max: return m_fOrthographicSize_Max;
            default: Debug.LogError("GetOrthographicSize: " + emSize.ToString()); return 0;
        }
    }

    public EM_OrthographicSize GetOrthographicSize_ShowPort(float fSize)
    {
        if (fSize >= GetOrthographicSize(EM_OrthographicSize.Min) && fSize < GetOrthographicSize(EM_OrthographicSize.ShowPortAll))
        {
            //[Min,ShowPortAll)
            return EM_OrthographicSize.ShowPortAll;
        }

        if (fSize >= GetOrthographicSize(EM_OrthographicSize.ShowPortAll) && fSize < GetOrthographicSize(EM_OrthographicSize.ShowPortLess))
        {
            //[ShowPortAll,ShowPortLess)
            return EM_OrthographicSize.ShowPortLess;
        }

        return EM_OrthographicSize.INVALID;
    }

    public float GetOrthographicSizeLev(EM_OrthographicSizeLev emSizeLev)
    {
        switch (emSizeLev)
        {
            case EM_OrthographicSizeLev.Default: return m_fOrthographicSize_Default;
            case EM_OrthographicSizeLev.Lev1: return m_fOrthographicSize_Min;
            case EM_OrthographicSizeLev.Lev2: return m_fOrthographicSize_Lev2;
            case EM_OrthographicSizeLev.Lev3: return m_fOrthographicSize_Lev3;
            case EM_OrthographicSizeLev.Lev4: return m_fOrthographicSize_Lev4;
            case EM_OrthographicSizeLev.Lev5: return m_fOrthographicSize_Max;
            default: Debug.LogError("GetOrthographicSizeLev: " + emSizeLev.ToString()); return 0;
        }
    }

    public float CaclOrthographicSizeLev(float fSizeFrom, float fSizeTo)
    {
        GameCommon.ASSERT(fSizeFrom != fSizeTo);
        GameCommon.ASSERT(fSizeFrom >= GetOrthographicSize(EM_OrthographicSize.Min) &&
            fSizeFrom <= GetOrthographicSize(EM_OrthographicSize.Max));

        bool bIsAdd = (fSizeTo > fSizeFrom);

        EM_OrthographicSizeLev emFrom = EM_OrthographicSizeLev.INVALID;
        if (fSizeFrom == GetOrthographicSizeLev(EM_OrthographicSizeLev.Lev1))
        {
            emFrom = EM_OrthographicSizeLev.Lev1;
        }
        else if (fSizeFrom == GetOrthographicSizeLev(EM_OrthographicSizeLev.Lev2))
        {
            emFrom = EM_OrthographicSizeLev.Lev2;
        }
        else if (fSizeFrom == GetOrthographicSizeLev(EM_OrthographicSizeLev.Lev3))
        {
            emFrom = EM_OrthographicSizeLev.Lev3;
        }
        else if (fSizeFrom == GetOrthographicSizeLev(EM_OrthographicSizeLev.Lev4))
        {
            emFrom = EM_OrthographicSizeLev.Lev4;
        }
        else if (fSizeFrom == GetOrthographicSizeLev(EM_OrthographicSizeLev.Lev5))
        {
            emFrom = EM_OrthographicSizeLev.Lev5;
        }
        else
        {
            Debug.LogError("CaclOrthographicSizeLev: fSizeFrom: " + fSizeFrom);
        }

        if (bIsAdd)
        {
            if (emFrom == EM_OrthographicSizeLev.Lev5)
            {
                //Debug.Log("CaclOrthographicSizeLev: Lev5");
                return GetOrthographicSizeLev(EM_OrthographicSizeLev.Lev5);
            }
            else
            {
                //Debug.Log("CaclOrthographicSizeLev: " + (emFrom + 1).ToString());
                return GetOrthographicSizeLev(emFrom + 1);
            }
        }
        else
        {
            if (emFrom == EM_OrthographicSizeLev.Lev1)
            {
                //Debug.Log("CaclOrthographicSizeLev: Lev1");
                return GetOrthographicSizeLev(EM_OrthographicSizeLev.Lev1);
            }
            else
            {
                //Debug.Log("CaclOrthographicSizeLev: " + (emFrom - 1).ToString());
                return GetOrthographicSizeLev(emFrom - 1);
            }
        }
    }

    #endregion ########################DragMapWindow_OrthographicSize#################################
}