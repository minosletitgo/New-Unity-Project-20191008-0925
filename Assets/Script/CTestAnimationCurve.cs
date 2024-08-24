using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CTestAnimationCurve : MonoBehaviour
{
    [SerializeField]
    UIWidget m_goMovingWidget;
    Vector3 m_v3MovingStartPos;
    float m_fMovingStartAlpha;

    [SerializeField]
    AnimationCurve m_stCurvePos;
    [SerializeField]
    AnimationCurve m_stCurveAlpha;

    [SerializeField]
    float m_fDuration = 1.0f;
    [SerializeField]
    Vector2 m_v2RangeX = new Vector2(0f, 1f);
    [SerializeField]
    Vector2 m_v2RangeY = new Vector2(-1f, 1f);

    [SerializeField]
    UIButton m_btnExecute;

    bool m_bIsUpdating = false;
    float m_fTimeStart;



    private void Awake()
    {
        m_bIsUpdating = false;
        m_v3MovingStartPos = m_goMovingWidget.transform.localPosition;
        m_fMovingStartAlpha = m_goMovingWidget.alpha;

        UIEventListener.Get(m_btnExecute.gameObject).onClick = OnClick_BtnExecute;        
    }

    void OnClick_BtnExecute(GameObject go)
    {
        m_bIsUpdating = true;

        m_fTimeStart = Time.time;
        m_goMovingWidget.gameObject.SetActive(true);
        m_goMovingWidget.transform.localPosition = m_v3MovingStartPos;
        m_goMovingWidget.alpha = m_fMovingStartAlpha;
    }
    
    float GetEvaluatePos(float fRatioX)
    {
        return m_stCurvePos.Evaluate(fRatioX);
    }

    float GetEvaluateAlpha(float fRatioX)
    {
        return m_stCurveAlpha.Evaluate(fRatioX);
    }

    private void Update()
    {
        if (m_bIsUpdating)
        {
            float fPassTime = Time.time - m_fTimeStart;
            if (fPassTime <= m_fDuration)
            {
                float fRatio = fPassTime / m_fDuration;

                float fX = Mathf.Lerp(m_v2RangeX.x, m_v2RangeX.y, fRatio);
                float fY = Mathf.Lerp(m_v2RangeY.x, m_v2RangeY.y, GetEvaluatePos(fRatio));
                m_goMovingWidget.transform.localPosition = new Vector3(
                    fX,
                    fY,
                    m_goMovingWidget.transform.localPosition.z
                    );

                m_goMovingWidget.alpha = Mathf.Lerp(0f, 1f, GetEvaluateAlpha(fRatio));
            }
            else
            {
                m_bIsUpdating = false;
            }
        }
    }
}
