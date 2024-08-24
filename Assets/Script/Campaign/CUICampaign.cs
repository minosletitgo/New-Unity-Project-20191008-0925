using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class CUICampaign : MonoBehaviour
{
    public GameObject m_goCircleCenter;
    public GameObject m_goCirclePointInst;
    public GameObject m_goCircleParent;
    public UIInput m_itInputRadius;
    public UIButton m_btnMakeCircle;
    public UIButton m_btnRandomPoint;
    public GameObject m_goRandomPointBegin;
    public GameObject m_goRandomPointEnd;
    public GameObject m_goLineArrow;
    public GameObject m_goHotCircle;
    public UIButton m_btnReverseHotCircle;

    Vector2 m_v2RandomPoint;

    float m_fRadius;
    List<GameObject> m_lstCirclePoint = new List<GameObject>();

    void Awake()
    {
        m_itInputRadius.value = "500";
        m_itInputRadius.validation = UIInput.Validation.Float;
        m_goCirclePointInst.SetActive(false);
        m_goRandomPointBegin.SetActive(false);
        m_goRandomPointEnd.SetActive(false);
        m_goLineArrow.SetActive(false);
        m_goHotCircle.SetActive(false);
        UIEventListener.Get(m_btnMakeCircle.gameObject).onClick = delegate (GameObject go)
        {
            if (!string.IsNullOrEmpty(m_itInputRadius.value))
            {
                float fRadius = float.Parse(m_itInputRadius.value);
                if (fRadius > 0)
                {
                    DoMakeCircle(fRadius);
                    m_fRadius = fRadius;
                }
            }
        };

        UIEventListener.Get(m_btnRandomPoint.gameObject).onClick = delegate (GameObject go)
        {
            DoMakeRandomPoint(m_fRadius);
        };

        UIEventListener.Get(m_btnReverseHotCircle.gameObject).onClick = delegate (GameObject go)
        {
            Vector3 v3EulerAngles = m_goHotCircle.transform.localRotation.eulerAngles;
            m_goHotCircle.transform.localRotation = Quaternion.Euler(0, 0, v3EulerAngles.z - 180);
        };
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

    static Vector2 CalcTwoPointCenter(Vector2 v2Begin, Vector2 v2End)
    {
        float fX = (v2Begin.x + v2End.x) / 2.0f;
        float fY = (v2Begin.y + v2End.y) / 2.0f;
        return new Vector2(fX, fY);
    }

    void DoMakeCircle(float fRadius)
    {
        Vector3 v3CircleCenter = UICamera.mainCamera.WorldToScreenPoint(m_goCircleCenter.transform.position);
        //Debug.Log("v3Center = " + v3Center);

        for (int i = 0; i < m_lstCirclePoint.Count; i++)
        {
            GameObject.Destroy(m_lstCirclePoint[i]);
        }

        for (int i = 0; i < 360; i++)
        {
            if (i % 3 == 0)
            {
                Vector2 v2Pos = CalcPointPosInCircle(v3CircleCenter, fRadius, i);
                v2Pos = UICamera.mainCamera.ScreenToWorldPoint(v2Pos);

                GameObject goInst = GameObject.Instantiate(m_goCirclePointInst);
                goInst.SetActive(true);
                goInst.transform.SetParent(m_goCircleParent.transform);
                goInst.transform.position = v2Pos;
                goInst.transform.localRotation = Quaternion.Euler(Vector3.zero);
                goInst.transform.localScale = Vector3.one;

                m_lstCirclePoint.Add(goInst);
            }
        }
    }

    void DoMakeRandomPoint(float fRadius)
    {
        Vector3 v3CircleCenter = UICamera.mainCamera.WorldToScreenPoint(m_goCircleCenter.transform.position);

        float fRadiusTmp = (float)(NGUITools.RandomRange(1, (int)fRadius));
        float fAngleTmp1 = (float)(NGUITools.RandomRange(0, 360));
        float fAngleTmp2 = (float)(NGUITools.RandomRange(0, 360));

        Vector2 v2Begin_RP = CalcPointPosInCircle(v3CircleCenter, fRadiusTmp, fAngleTmp1);
        m_goRandomPointBegin.SetActive(true);
        m_goRandomPointBegin.transform.position = UICamera.mainCamera.ScreenToWorldPoint(v2Begin_RP);

        Vector2 v2End_RP = CalcPointPosInCircle(v3CircleCenter, fRadiusTmp, fAngleTmp2);
        m_goRandomPointEnd.SetActive(true);
        m_goRandomPointEnd.transform.position = UICamera.mainCamera.ScreenToWorldPoint(v2End_RP);

        Vector2 v3Center_RP = CalcTwoPointCenter(v2Begin_RP, v2End_RP);

        ////////////////////////////////////////////////////////////////////////

        m_goLineArrow.SetActive(true);
        m_goLineArrow.transform.position = UICamera.mainCamera.ScreenToWorldPoint(v3Center_RP);
        m_goHotCircle.SetActive(true);
        m_goHotCircle.transform.position = UICamera.mainCamera.ScreenToWorldPoint(v3CircleCenter);

        Vector2 v2Center = CalcPointPosInCircle(v3Center_RP, 0, 0);
        Vector2 v2Horizontal = CalcPointPosInCircle(v3Center_RP, fRadius, 0);
        Vector2 v3Line_BE = v2End_RP - v2Begin_RP;
        Vector2 v3Line_Horizontal = v2Horizontal - v2Center;

        float fSignedAngleBetween_BE = Vector2.SignedAngle(v3Line_Horizontal, v3Line_BE);
        float fAngleBetween_BE = Vector2.Angle(v3Line_Horizontal, v3Line_BE);
        Debug.LogFormat("fSignedAngleBetween_BE = {0}, fAngleBetween_BE = {1} ", fSignedAngleBetween_BE, fAngleBetween_BE);

        m_goLineArrow.transform.localRotation = Quaternion.Euler(0, 0, fSignedAngleBetween_BE);
        m_goHotCircle.transform.localRotation = Quaternion.Euler(0, 0, fSignedAngleBetween_BE - 90);
    }
}