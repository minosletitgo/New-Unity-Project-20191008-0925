using UnityEngine;
using System.Collections.Generic;

public class UICenterOnChild_Ellipse : MonoBehaviour
{
    public delegate void OnCenterCallback(GameObject centeredObject);

    public float springStrength = 8f;

    public SpringEllipseDegree.OnFinished onFinished;

    public OnCenterCallback onCenter;

    public GameObject m_goCenterPoint;

    UIScrollView_Ellipse mScrollView;
    GameObject mCenteredObject;

    public GameObject centeredObject { get { return mCenteredObject; } }

    public delegate void OnBeginSpringCallback(
        float _fAngleDegreeFrom, 
        float _fAngleDegreeTo, 
        UIScrollView_Ellipse.EM_RecenterEvent emREvent
        );
    public OnBeginSpringCallback onBeginSpringCallback;


    void Start() { Recenter(UIScrollView_Ellipse.EM_RecenterEvent.OnSOChildStart); }
    void OnEnable() { if (mScrollView) { mScrollView.centerOnChild = this; Recenter(UIScrollView_Ellipse.EM_RecenterEvent.OnSOChildEnable); } }
    void OnDisable() { if (mScrollView) mScrollView.centerOnChild = null; }
    void OnDragFinished() { if (enabled) Recenter(UIScrollView_Ellipse.EM_RecenterEvent.OnSViewDragFinished); }

    [ContextMenu("Execute")]
    public void Recenter(UIScrollView_Ellipse.EM_RecenterEvent emREvent)
    {
        if (m_goCenterPoint == null)
        {
            Debug.LogWarning("UICenterOnChild_Ellipse.Recenter Missing - m_goCenterPoint");
            enabled = false;
            return;
        }

        if (mScrollView == null)
        {
            mScrollView = NGUITools.FindInParents<UIScrollView_Ellipse>(gameObject);

            if (mScrollView == null)
            {
                Debug.LogWarning(GetType() + " requires " + typeof(UIScrollView_Ellipse) + " on a parent object in order to work", this);
                enabled = false;
                return;
            }
            else
            {
                if (mScrollView)
                {
                    mScrollView.centerOnChild = this;
                    mScrollView.onDragFinished += OnDragFinished;
                }
            }
        }

        Transform trans = transform;
        if (trans.childCount == 0) return;

        float min = float.MaxValue;
        Transform closest = null;
        foreach (Transform t in mScrollView.EnumChildList())
        {
            if (!t.gameObject.activeInHierarchy) continue;
            float sqrDist = Vector3.SqrMagnitude(t.position - m_goCenterPoint.transform.position);

            if (sqrDist < min)
            {
                min = sqrDist;
                closest = t;
            }
        }

        CenterOn(closest, emREvent);
    }

    public void CenterOn(Transform target, UIScrollView_Ellipse.EM_RecenterEvent emREvent)
    {
        if (target != null && mScrollView != null)
        {
            mCenteredObject = target.gameObject;
            
            float fAngleBefore = MathEllipseHelper.GetAngleDegreeInEllipse(target.gameObject, mScrollView);
            if (fAngleBefore < 0)
            {
                fAngleBefore = 360.0f + fAngleBefore;
            }

            float fAngleAfter = MathEllipseHelper.GetAngleDegreeInEllipse(m_goCenterPoint, mScrollView);
            if (fAngleAfter < 0)
            {
                fAngleAfter = 360.0f + fAngleAfter;
            }

            float fAngleDelta = fAngleAfter - fAngleBefore;
            //Debug.Log(" " + fAngleAfter + " - " + fAngleAfter + " = " + fAngleDelta);
            float fAngleTarget = mScrollView.GetDragAmountDegreeValue() + fAngleDelta;
#if UNITY_EDITOR
            //Debug.Log("fAngleTarget = "+ fAngleTarget);
            //fAngleTarget = MathEllipseHelper.DegreeClamp(fAngleTarget);
            //Debug.Log("fAngleTarget After = " + fAngleTarget);
            //Debug.Log("Begin; " + mScrollView.GetDragAmountDegreeValue() + " -> " + fAngleTarget + " | " + target.name);
            //float fSpringAngleDelta = Mathf.Abs(mScrollView.GetDragAmountDegreeValue() - fAngleTarget);
            //if (mScrollView.GetDragAmountDegreeValue() > fAngleTarget)
            //{
            //    EditorLOG.logWarn(string.Format("顺时针: fSpringAngleDelta: {0} | {1}", fSpringAngleDelta, emREvent.ToString()));
            //}
            //else
            //{
            //    EditorLOG.logWarn(string.Format("逆时针: fSpringAngleDelta: {0} | {1}", fSpringAngleDelta, emREvent.ToString()));
            //}
#endif
            if (onBeginSpringCallback != null)
            {
                onBeginSpringCallback(mScrollView.GetDragAmountDegreeValue(), fAngleTarget, emREvent);
            }

            SpringEllipseDegree.Stop(mScrollView.gameObject);
            SpringEllipseDegree.Begin(
                mScrollView.gameObject,
                fAngleTarget,
                springStrength
                ).onFinished = onFinished;
        }
        else
        {
            mCenteredObject = null;
        }

        if (onCenter != null) onCenter(mCenteredObject);
    }
}