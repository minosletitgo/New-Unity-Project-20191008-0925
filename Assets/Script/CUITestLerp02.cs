using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class CUITestLerp02 : MonoBehaviour
{
    public Camera m_Camera;
    public Transform m_trShipObject;
    public Transform m_trTargetPos;
    Vector2 m_v3CurrentPosInScreen;
    const float m_fConstSpeed = 5.1f;
    bool m_bIsUpdating = true;

    public GameObject m_goTestPoints;
    UISprite[] m_aryTestPoints;
    int m_nTestPointsId = 0;
    Vector3 m_v3TestPoint;
    List<Vector3> m_lstTestPoint = new List<Vector3>();
    bool m_bIsLerping = false;

    public UIButton m_btnAddTestPoints;
    public UILabel m_labPrintScreen;
    
    


    public enum EM_LerpType
    {
        Invalid = -1,

        Normal,
        History,

        Max,
    };
    EM_LerpType m_emLerpType = EM_LerpType.Invalid;

    float m_fLerpRate;
    const float m_fConstLerpRate_Slow = 3f;
    const float m_fConstLerpRate_Fast = 10f;
    const float m_fDisCloseEnough = 0.0111f;







    private void Awake()
    {
        m_aryTestPoints = m_goTestPoints.GetComponentsInChildren<UISprite>();
        GameCommon.ASSERT(m_aryTestPoints != null);
        GameCommon.ASSERT(m_aryTestPoints.Length > 0);

        m_nTestPointsId = 0;
        m_emLerpType = EM_LerpType.History;

        UIEventListener.Get(m_btnAddTestPoints.gameObject).onClick = OnClick_BtnAddTestPoints;
    }

    private void Start()
    {
        m_v3CurrentPosInScreen = m_Camera.WorldToScreenPoint(m_trShipObject.position);

        Debug.Log("m_v3PosInScreen = " + m_v3CurrentPosInScreen);

        GameCommon.ASSERT(m_v3CurrentPosInScreen.x >= 0 && m_v3CurrentPosInScreen.x <= Screen.width);
        GameCommon.ASSERT(m_v3CurrentPosInScreen.y >= 0 && m_v3CurrentPosInScreen.y <= Screen.height);
    }

    void OnClick_BtnAddTestPoints(GameObject go)
    {
        int nIndex = (m_nTestPointsId++) % m_aryTestPoints.Length;
        AddTestPoints(m_aryTestPoints[nIndex].transform.position);
    }

    void AddTestPoints(Vector3 v3Pos)
    {
        m_v3TestPoint = v3Pos;
        m_lstTestPoint.Add(v3Pos);

        m_fLerpRate = m_fConstLerpRate_Slow;

        m_bIsLerping = true;
    }
    
    void MovingTo(Vector3 v3PosInWorld)
    {
        m_trShipObject.position = v3PosInWorld;
        m_v3CurrentPosInScreen = m_Camera.WorldToScreenPoint(v3PosInWorld);
    }

    private void Update()
    {
        if (m_bIsUpdating)
        {
            if (!m_bIsLerping)
            {
                //MovingTo(m_Camera.ScreenToWorldPoint(new Vector2(m_v3CurrentPosInScreen.x + m_fConstSpeed, m_v3CurrentPosInScreen.y)));
                MovingTo(Vector3.Lerp(m_Camera.ScreenToWorldPoint(m_v3CurrentPosInScreen), m_trTargetPos.position, Time.smoothDeltaTime * 0.10f));

                if (m_v3CurrentPosInScreen.x >= 0 && m_v3CurrentPosInScreen.x <= Screen.width &&
                    m_v3CurrentPosInScreen.y >= 0 && m_v3CurrentPosInScreen.y <= Screen.height
                    )
                {
                    m_bIsUpdating = true;
                }
                else
                {
                    m_bIsUpdating = false;
                }
            }
            else
            {
                if (m_lstTestPoint.Count > 0)
                {
                    //取出队列中的第一个设为插值的目标
                    MovingTo(Vector3.Lerp(m_Camera.ScreenToWorldPoint(m_v3CurrentPosInScreen), m_lstTestPoint[0], Time.smoothDeltaTime * m_fLerpRate));

                    //位置足够接近，从队列中移除第一个，紧接着就是第二个
                    if (Vector3.Distance(m_Camera.ScreenToWorldPoint(m_v3CurrentPosInScreen), m_lstTestPoint[0]) <= m_fDisCloseEnough)
                    {
                        m_lstTestPoint.RemoveAt(0);
                    }

                    //如果同步队列过大，加快插值速率，使其更快到达目标点
                    if (m_lstTestPoint.Count > 10)
                    {
                        m_fLerpRate = m_fConstLerpRate_Fast;
                    }
                    else
                    {
                        m_fLerpRate = m_fConstLerpRate_Slow;
                    }
                }
                else
                {
                    m_bIsLerping = false;
                }
            }
        }
    }

    //private void UpdateXXXXXXX()
    //{
    //    if (m_bIsLerping)
    //    {
    //        switch (m_emLerpType)
    //        {
    //            case EM_LerpType.Normal:
    //                {
    //                    m_trShipObject.position = Vector3.Lerp(
    //                        m_trShipObject.position,
    //                        m_v3TestPoint,
    //                        Time.smoothDeltaTime * m_fLerpRate
    //                        );

    //                    if (m_trShipObject.position == m_v3TestPoint)
    //                    {
    //                        m_bIsLerping = false;
    //                    }
    //                }
    //                break;
    //            case EM_LerpType.History:
    //                {
    //                    if (m_lstTestPoint.Count > 0)
    //                    {
    //                        //取出队列中的第一个设为插值的目标
    //                        m_trShipObject.position = Vector3.Lerp(m_trShipObject.position, m_lstTestPoint[0], Time.smoothDeltaTime * m_fLerpRate);

    //                        //位置足够接近，从队列中移除第一个，紧接着就是第二个
    //                        if (Vector3.Distance(m_trShipObject.position, m_lstTestPoint[0]) <= m_fDisCloseEnough)
    //                        {
    //                            m_lstTestPoint.RemoveAt(0);
    //                        }

    //                        //如果同步队列过大，加快插值速率，使其更快到达目标点
    //                        if (m_lstTestPoint.Count > 10)
    //                        {
    //                            m_fLerpRate = m_fConstLerpRate_Fast;
    //                        }
    //                        else
    //                        {
    //                            m_fLerpRate = m_fConstLerpRate_Slow;
    //                        }
    //                    }
    //                    else
    //                    {
    //                        m_bIsLerping = false;
    //                    }
    //                }
    //                break;
    //        }
    //    }
    //}

    private void LateUpdate()
    {
        m_labPrintScreen.text = null;
        foreach (Vector3 _v3Pos in m_lstTestPoint)
        {
            m_labPrintScreen.text += _v3Pos.ToString();
            m_labPrintScreen.text += "\n";
        }
    }
}