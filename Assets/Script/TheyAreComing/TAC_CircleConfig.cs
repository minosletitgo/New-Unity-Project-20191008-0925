using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class TAC_CircleConfig : MonoBehaviour
{
    public GameObject m_goDebugSphere;
    List<GameObject> m_lstDebugSphere = new List<GameObject>();

    public Vector2 m_v2ZeroPos;
    public float m_fOffsetBetweenCircle = 10f;
    public int m_nFirstCircleInnerPointNumber = 6;
    public int m_nMaxCircleNumber = 10;
    List<int> m_lstPointNumberInEveryCircle = new List<int>();
    Dictionary<int, List<bool>> m_mapPointSaveValueInEveryCircle = new Dictionary<int, List<bool>>();

    void Awake()
    {
        TCK.CHECK(m_lstPointNumberInEveryCircle != null);
        m_lstPointNumberInEveryCircle.Clear();
        for (int iCircle = 0; iCircle < m_nMaxCircleNumber; iCircle++)
        {
            m_lstPointNumberInEveryCircle.Add(iCircle * m_nFirstCircleInnerPointNumber);
        }

        for (int iCircle = 0; iCircle < m_lstPointNumberInEveryCircle.Count; iCircle++)
        {
            List<bool> lstSaveValue = new List<bool>();
            for (int iSv = 0; iSv < m_lstPointNumberInEveryCircle[iCircle]; iSv++)
            {
                lstSaveValue.Add(false);
            }
            m_mapPointSaveValueInEveryCircle.Add(iCircle, lstSaveValue);
        }
    }

    [ContextMenu("DebugPrintSphere")]
    void DebugPrintSphere()
    {
        if (!Application.isPlaying)
        {
            TCK.CHECK(m_lstPointNumberInEveryCircle != null);
            m_lstPointNumberInEveryCircle.Clear();
            for (int iCircle = 0; iCircle < m_nMaxCircleNumber; iCircle++)
            {
                m_lstPointNumberInEveryCircle.Add(iCircle * m_nFirstCircleInnerPointNumber);
            }

            for (int iCircle = 0; iCircle < m_lstPointNumberInEveryCircle.Count; iCircle++)
            {
                int nNumber = m_lstPointNumberInEveryCircle[iCircle];

                for (int iPoint = 0; iPoint < nNumber; iPoint++)
                {
                    GameObject goInst = GameObject.Instantiate(m_goDebugSphere.gameObject);
                    goInst.SetActive(true);
                    goInst.transform.SetParent(transform);
                    goInst.transform.localPosition = Vector3.zero;
                    goInst.transform.localRotation = Quaternion.Euler(Vector3.zero);
                    goInst.transform.localScale = Vector3.one;

                    Vector3 v3CalcPos = Vector3.zero;
                    if (iCircle == 0)
                    {
                        v3CalcPos = m_v2ZeroPos;
                    }
                    else
                    {
                        v3CalcPos = GetPosInCircle(iCircle, iPoint);
                    }

                    goInst.transform.localPosition = new Vector3(
                        v3CalcPos.x,
                        0,
                        v3CalcPos.y
                        );

                    m_lstDebugSphere.Add(goInst);
                }
            }
        }
    }

    [ContextMenu("DebugDestroySphere")]
    void DebugDestroySphere()
    {
        if (!Application.isPlaying)
        {
            for (int i = 0; i < m_lstDebugSphere.Count; i++)
            {
                GameObject.DestroyImmediate(m_lstDebugSphere[i]);
            }
            m_lstDebugSphere.Clear();
        }
    }

    public Vector2 GetCircleZero()
    {
        /*
            获取原点坐标
        */
        return m_v2ZeroPos;
    }

    public int GetCircleNum()
    {
        /*
            获取总环数
        */
        return m_lstPointNumberInEveryCircle.Count;
    }

    public int GetCirclePointNum(int iCircle)
    {
        /*
            获取任意环，其内置的点数量
        */
        TCK.CHECK(iCircle >= 0 && iCircle < m_lstPointNumberInEveryCircle.Count);
        return m_lstPointNumberInEveryCircle[iCircle];
    }

    static Vector2 CalcPointPosInCircle(Vector2 v2Zero, float fRadius, float fAngle)
    {
        /*
            已知原点，半径，均分角度，求目标点坐标
        */
        float fX = v2Zero.x + fRadius * Mathf.Cos(fAngle * Mathf.PI / 180.0f);
        float fY = v2Zero.y + fRadius * Mathf.Sin(fAngle * Mathf.PI / 180.0f);
        return new Vector2(fX, fY);
    }

    public Vector2 GetPosInCircle(int iCircle, int iPoint)
    {
        /*
            获取任意环，其内置任意点的坐标
        */
        TCK.CHECK(iCircle >= 0 && iCircle < GetCircleNum());
        if (iCircle > 0)
        {
            int nNumber = GetCirclePointNum(iCircle);
            TCK.CHECK(iPoint >= 0 && iPoint < nNumber);
            float fAngle = 360.0f / (float)nNumber;
            Vector2 v2Ret = CalcPointPosInCircle(m_v2ZeroPos, m_fOffsetBetweenCircle * iCircle, fAngle * iPoint);
            return v2Ret;
        }
        else
        {
            return m_v2ZeroPos;
        }
    }

    public void ClearPointSaveValueInEveryCircle()
    {
        foreach (KeyValuePair<int, List<bool>> pPair in m_mapPointSaveValueInEveryCircle)
        {
            for (int i = 0; i < pPair.Value.Count; i++)
            {
                pPair.Value[i] = false;
            }
        }
    }

    public void SetPointSaveValueInEveryCircle(int iCircle, int iPoint, bool bValue)
    {
        TCK.CHECK(iCircle >= 0 && iCircle < GetCircleNum());
        List<bool> lstSave = null;
        if (m_mapPointSaveValueInEveryCircle.TryGetValue(iCircle, out lstSave))
        {
            TCK.CHECK(iPoint >= 0 && iPoint < lstSave.Count);
            lstSave[iPoint] = bValue;
            return;
        }

        TCK.TERRO(string.Format("SetPointSaveValueInEveryCircle -> iCircle:{0}, iPoint:{1} ", iCircle, iPoint));
    }

    public bool GetPointSaveValueInEveryCircle(int iCircle, int iPoint)
    {
        TCK.CHECK(iCircle >= 0 && iCircle < GetCircleNum());
        List<bool> lstSave = null;
        if (m_mapPointSaveValueInEveryCircle.TryGetValue(iCircle, out lstSave))
        {
            TCK.CHECK(iPoint >= 0 && iPoint < lstSave.Count);
            return lstSave[iPoint];
        }

        TCK.TERRO(string.Format("GetPointSaveValueInEveryCircle -> iCircle:{0}, iPoint:{1} ", iCircle, iPoint));
        return false;
    }

    public bool GetPointSaveValueInEveryCircle_MinIdle(int iCircle, out int iPointOut, Vector2 v2RefPos)
    {
        /*
            计算出与参数坐标，最近的闲置点
        */
        TCK.CHECK(iCircle >= 0 && iCircle < GetCircleNum());
        List<bool> lstSave = null;
        iPointOut = -1;
        if (m_mapPointSaveValueInEveryCircle.TryGetValue(iCircle, out lstSave))
        {
            float fMinDis = float.MaxValue;
            for (int i = 0; i < lstSave.Count; i++)
            {
                Vector2 v2PosCur = GetPosInCircle(iCircle, i);
                float fTmpDis = Vector2.Distance(v2RefPos, v2PosCur);
                if (lstSave[i] == false && fTmpDis <= fMinDis)
                {
                    iPointOut = i;
                    fMinDis = fTmpDis;
                }
            }

            if (iPointOut >= 0)
            {
                return true;
            }

            return false;
        }

        TCK.TERRO(string.Format("GetPointSaveValueInEveryCircle_Idle -> iCircle:{0} ", iCircle));
        return false;
    }

    public bool GetPointSaveValueInEveryCircle_FirstIdle(int iCircle, out int iPointOut)
    {
        /*
            计算出第一个闲置点
        */
        TCK.CHECK(iCircle >= 0 && iCircle < GetCircleNum());
        List<bool> lstSave = null;
        iPointOut = -1;
        if (m_mapPointSaveValueInEveryCircle.TryGetValue(iCircle, out lstSave))
        {
            for (int i = 0; i < lstSave.Count; i++)
            {
                Vector2 v2PosCur = GetPosInCircle(iCircle, i);
                if (lstSave[i] == false)
                {
                    iPointOut = i;
                    break;
                }
            }

            if (iPointOut >= 0)
            {
                return true;
            }

            return false;
        }

        TCK.TERRO(string.Format("GetPointSaveValueInEveryCircle_Idle -> iCircle:{0} ", iCircle));
        return false;
    }
}