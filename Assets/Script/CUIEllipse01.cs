using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CUIEllipse01 : MonoBehaviour
{
    public UIInput m_itAxis2A;
    public UIInput m_itAxis2B;
    public UIInput m_itOffsetDegree;
    public UIButton m_btnRebuild;    

    public UIInput m_itMovingPointDegree;
    public UIButton m_btnMoving;

    public UISprite m_sprCenter;
    public UISprite m_sprLineAxis2A;
    public UISprite m_sprLineAxis2B;
    public GameObject m_goDrawRoot;
    public UIWidget m_sprDrawPointInst;
    List<UIWidget> m_lstDrawPoint = new List<UIWidget>();
    List<Vector2> m_v2DrawPoint = new List<Vector2>();

    public GameObject m_goMovingTest;




    private void Awake()
    {
        m_itAxis2A.validation = UIInput.Validation.Integer;
        m_itAxis2B.validation = UIInput.Validation.Integer;
        m_itOffsetDegree.validation = UIInput.Validation.Float;
        UIEventListener.Get(m_btnRebuild.gameObject).onClick = OnClick_BtnRebuild;

        m_itMovingPointDegree.validation = UIInput.Validation.Float;        
        UIEventListener.Get(m_btnMoving.gameObject).onClick = OnClick_BtnMoving;

        m_sprDrawPointInst.gameObject.SetActive(false);
    }

    void OnClick_BtnRebuild(GameObject go)
    {
        int nAxis2A = int.Parse(m_itAxis2A.value);
        int nAxis2B = int.Parse(m_itAxis2B.value);

        if (nAxis2A < 0 || nAxis2B < 0)
        {
            return;
        }

        float fMovingPointDegree = float.Parse(m_itMovingPointDegree.value);
        if (fMovingPointDegree < 0.0f)
        {
            fMovingPointDegree = 0.0f;
            m_itMovingPointDegree.value = fMovingPointDegree.ToString();
        }
        if (fMovingPointDegree > 360.0f)
        {
            fMovingPointDegree = 360.0f;
            m_itMovingPointDegree.value = fMovingPointDegree.ToString();
        }

        m_sprLineAxis2A.width = nAxis2A;
        m_sprLineAxis2B.width = nAxis2B;

        //DoCalcDraw();
        DoCalcDraw02();
    }

    void OnClick_BtnMoving(GameObject go)
    {
        int nAxis2A = int.Parse(m_itAxis2A.value);
        int nAxis2B = int.Parse(m_itAxis2B.value);

        if (nAxis2A < 0 || nAxis2B < 0)
        {
            return;
        }

        float fMovingPointDegree = float.Parse(m_itMovingPointDegree.value);
        if (fMovingPointDegree < 0.0f)
        {
            fMovingPointDegree = 0.0f;
            m_itMovingPointDegree.value = fMovingPointDegree.ToString();
        }
        if (fMovingPointDegree > 360.0f)
        {
            fMovingPointDegree = 360.0f;
            m_itMovingPointDegree.value = fMovingPointDegree.ToString();
        }

        m_sprLineAxis2A.width = nAxis2A;
        m_sprLineAxis2B.width = nAxis2B;

        DoCalcMovingPoint(fMovingPointDegree);
    }

    //void DoCalcDraw()
    //{
    //    /*
        
    //    Expr:  ( (1 - X-X0)² / A² ) * B² = ( Y -Y0 )²

    //    */
    //    Vector3 v3Center = UICamera.mainCamera.WorldToScreenPoint(m_sprCenter.transform.position);
    //    float fAxisA = (float)m_sprLineAxis2A.width / 2.0f;
    //    float fAxisB = (float)m_sprLineAxis2B.width / 2.0f;

    //    //以遍历X轴为准
    //    float fX_Min = v3Center.x - (float)m_sprLineAxis2A.width / 2.0f;
    //    float fX_Max = v3Center.x + (float)m_sprLineAxis2A.width / 2.0f;
    //    const float fX_Step = 2.0f;

    //    m_v2DrawPoint.Clear();
    //    for (float iX = fX_Min; iX <= fX_Max; iX += fX_Step)
    //    {
    //        float fExprL = (1 - Mathf.Pow((iX - v3Center.x), 2) / Mathf.Pow(fAxisA, 2)) * Mathf.Pow(fAxisB, 2);
    //        float fExprR_Abs = Mathf.Sqrt(fExprL);
    //        float fY_01 = fExprR_Abs + v3Center.y;
    //        float fY_02 = v3Center.y - fExprR_Abs;
    //        m_v2DrawPoint.Add(new Vector2(iX, fY_01));
    //        m_v2DrawPoint.Add(new Vector2(iX, fY_02));
    //    }

    //    DoBaseDraw();

    //    //测试一个点
    //    float fffff = Mathf.Sin((Mathf.PI / 180) * 180);
    //    Debug.Log("fffff = " + fffff);
    //}

    void DoCalcDraw02()
    {
        Vector3 v3Center = UICamera.mainCamera.WorldToScreenPoint(m_sprCenter.transform.position);
        float fAxisA = (float)m_sprLineAxis2A.width / 2.0f;
        float fAxisB = (float)m_sprLineAxis2B.width / 2.0f;

        m_v2DrawPoint.Clear();

        //以角度（0 ~ 360）为准
        const float fDegree_Begin = 0;
        const float fDegree_End = 360;
        for (float iD = fDegree_Begin; iD <= fDegree_End; iD++)
        {
            m_v2DrawPoint.Add(DoCalcDraw02BasePoint_ByX(iD));
        }

        DoBaseDraw();
    }

    Vector2 DoCalcFilterQuadrant(
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

        Debug.LogError("DoCalcDraw02BasePoint -> fDegree: " + fDegree);
        return Vector2.zero;
    }

    Vector3 DoCalcDraw02BasePoint_ByX(float fDegree)
    {
        /*
        
        t = tan(θ) = (Y - Y0)/(X - X0)
        Y = T * (X - X0) + Y0

        Expr:  (X-X0)² = 1/（1/A² + t²/B²）
                       
        */

        //Vector3 v3Center = UICamera.mainCamera.WorldToScreenPoint(m_sprCenter.transform.position);
        float fAxisA = (float)m_sprLineAxis2A.width / 2.0f;
        float fAxisB = (float)m_sprLineAxis2B.width / 2.0f;

        return MathEllipseHelper.GetScreenPostion(
            m_sprCenter.transform, 
            fAxisA, 
            fAxisB, 
            fDegree,
            0,
            0,
            float.Parse(m_itOffsetDegree.value)
            );
    }

    //Vector2 DoCalcDraw02BasePoint_ByY(float fDegree)
    //{
    //    /*
        
    //    t = tan(θ) = (Y - Y0)/(X - X0)
    //    X = （Y - Y0）/T + X0

    //    Expr:  (Y - Y0)² = B²/（1 +  ( B²/(A²*T²) )）
                       
    //    */

    //    Vector3 v3Center = UICamera.mainCamera.WorldToScreenPoint(m_sprCenter.transform.position);
    //    float fAxisA = (float)m_sprLineAxis2A.width / 2.0f;
    //    float fAxisB = (float)m_sprLineAxis2B.width / 2.0f;

    //    GameCommon.CHECK(fDegree >= 0 && fDegree <= 360);
    //    if (fDegree == 360)
    //    {
    //        fDegree = 0;
    //    }

    //    {
    //        float fTan = Mathf.Tan((Mathf.PI / 180) * fDegree);
    //        float fExprR = Mathf.Pow(fAxisB, 2) / (1 + (Mathf.Pow(fAxisB, 2) / (Mathf.Pow(fAxisA, 2) * Mathf.Pow(fTan, 2))));
    //        float fExprL_Abs = Mathf.Sqrt(fExprR);

    //        float fY_01 = fExprL_Abs + v3Center.y;
    //        float fX_01 = 0;
    //        if (fTan == 0.0)
    //        {
    //            fX_01 = v3Center.x;
    //        }
    //        else
    //        {
    //            fX_01 = (fY_01 - v3Center.y) / fTan + v3Center.x;
    //        }

    //        float fY_02 = fExprL_Abs - v3Center.x;
    //        float fX_02 = 0f;
    //        if (fTan == 0.0f)
    //        {
    //            fX_02 = v3Center.x;
    //        }
    //        else
    //        {
    //            fX_02 = (fY_02 - v3Center.y) / fTan + v3Center.x;
    //        }

    //        return DoCalcFilterQuadrant(
    //            v3Center, fDegree,
    //            fX_01, fY_01,
    //            fX_02, fY_02
    //            );
    //    }
    //}

    void DoBaseDraw()
    {
        int nIndexData = 0;
        int nIndexUI = 0;
        while (nIndexData < m_v2DrawPoint.Count)
        {
            Vector2 _v2PosScreen = m_v2DrawPoint[nIndexData];
            UIWidget _sprItem = null;
            if (nIndexData >= m_lstDrawPoint.Count)
            {
                GameObject go = Instantiate(m_sprDrawPointInst.gameObject) as GameObject;
                go.name = "item_" + (nIndexData + 1);
                go.transform.parent = m_goDrawRoot.transform;
                go.transform.localPosition = Vector3.zero;
                go.transform.localRotation = Quaternion.Euler(Vector3.zero);
                go.transform.localScale = Vector3.one;
                _sprItem = go.GetComponent<UIWidget>();
                GameCommon.ASSERT(_sprItem);
                m_lstDrawPoint.Add(_sprItem);
            }
            else
            {
                _sprItem = m_lstDrawPoint[nIndexUI];
                GameCommon.ASSERT(_sprItem);
            }

            _sprItem.gameObject.SetActive(true);
            _sprItem.transform.position = UICamera.mainCamera.ScreenToWorldPoint(_v2PosScreen);

            nIndexUI++;
            nIndexData++;
        }
        for (; nIndexUI < m_lstDrawPoint.Count; nIndexUI++)
        {
            m_lstDrawPoint[nIndexUI].gameObject.SetActive(false);
        }
    }

    void DoCalcMovingPoint(float fDegree)
    {
        m_goMovingTest.SetActive(true);
        m_goMovingTest.transform.position = UICamera.mainCamera.ScreenToWorldPoint(
            DoCalcDraw02BasePoint_ByX(fDegree)
            );

        if(m_coMoving != null)
        {
            StopCoroutine(m_coMoving);
        }
        m_coMoving = CoMoving(fDegree);
        StartCoroutine(m_coMoving);
    }

    IEnumerator m_coMoving = null;
    IEnumerator CoMoving(float fDegreeBegin)
    {
        int nIndex = 0;
        while (true)
        {
            if (nIndex > 360)
            {
                break;
            }

            m_goMovingTest.transform.position = UICamera.mainCamera.ScreenToWorldPoint(
                DoCalcDraw02BasePoint_ByX(fDegreeBegin)
                );

            if (nIndex == 0)
            {
                yield return new WaitForSeconds(1.0f);
            }

            fDegreeBegin = MathEllipseHelper.DegreeClamp(fDegreeBegin + 1);
            nIndex++;

            yield return new WaitForEndOfFrame();
        }
    }
}
