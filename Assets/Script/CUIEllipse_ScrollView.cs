using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CUIEllipse_ScrollView : MonoBehaviour
{
    public UIScrollView_Ellipse mSView;
    public UIInput mItSetAmountValue;
    public UIButton mBtnSetAmountValue;
    public UILabel mLabSelecting;

    UICenterOnChild_Ellipse mCeneterOCE;
    GameObject mGoCenterFinished;



    private void Awake()
    {
        mCeneterOCE = mSView.m_trRoot.GetComponent<UICenterOnChild_Ellipse>();
        GameCommon.ASSERT(mCeneterOCE != null);
        mCeneterOCE.onCenter = OnSViewCenter;
        mCeneterOCE.onFinished = OnSViewFinished;
        mSView.onSetAmount = OnSViewSetAmount;
        mSView.mBoxCross.m_dgOnTriggerEnter = OnSViewCrossBoxTriggerEnter;

        mItSetAmountValue.value = null;
        mItSetAmountValue.validation = UIInput.Validation.Float;
        UIEventListener.Get(mBtnSetAmountValue.gameObject).onClick = OnClick_BtnSetAmountValue;
    }

    void OnClick_BtnSetAmountValue(GameObject go)
    {
        float fAmountValue = float.Parse(mItSetAmountValue.value);
        if (fAmountValue < 0.0f)
        {
            return;
        }

        mSView.SetDragAmountDegreeValue(fAmountValue, true);
        mCeneterOCE.Recenter(UIScrollView_Ellipse.EM_RecenterEvent.OnCustomRefresh);
    }

    void OnSViewCenter(GameObject centeredObject)
    {
        mGoCenterFinished = centeredObject;
        if (mGoCenterFinished != null)
        {
            //Debug.Log("OnSViewCenter = " + mGoCenterFinished.name);
            //mLabSelecting.text = mGoCenterFinished.name;
        }
    }

    void OnSViewFinished()
    {
        if (mGoCenterFinished != null)
        {
            //Debug.Log("OnSViewFinished = " + mGoCenterFinished.name);
            //mLabSelecting.text = mGoCenterFinished.name;
        }
    }

    void OnSViewSetAmount(float fDegreeValue)
    {
        //Debug.Log("OnSViewSetAmount = "+ fDegreeValue);
        float fDegreeStep = 360.0f / (float)(mSView.GetChildCount());
        //Debug.Log("fDegreeStep = " + fDegreeStep);

        foreach (Transform trChild in mSView.EnumChildList())
        {
            if (!trChild.gameObject.activeSelf)
            {
                continue;
            }

            float fDegree = MathEllipseHelper.GetAngleDegreeInEllipse(trChild.gameObject, mSView);
            fDegree = MathEllipseHelper.DegreeClamp(fDegree);
            //Debug.Log("fDegree = "+ fDegree);

            float fAngleDegree_Center = MathEllipseHelper.GetAngleDegreeInEllipse(mCeneterOCE.m_goCenterPoint, mSView);
            fAngleDegree_Center = MathEllipseHelper.DegreeClamp(fAngleDegree_Center);

            float fAngleDegree_Child = MathEllipseHelper.GetAngleDegreeInEllipse(trChild.gameObject, mSView);
            fAngleDegree_Child = MathEllipseHelper.DegreeClamp(fAngleDegree_Child);

            float fAngleDegree_Delta = Mathf.Abs(fAngleDegree_Center - fAngleDegree_Child);
            if (fAngleDegree_Delta > 180)
            {
                fAngleDegree_Delta = 360 - fAngleDegree_Delta;
            }
            //Debug.Log("fAngleDegree_Delta = " + fAngleDegree_Delta + " | " + trChild.name);
            double fMultiTimes = (fAngleDegree_Delta / fDegreeStep);
            fMultiTimes = Math.Round(fMultiTimes, MidpointRounding.AwayFromZero);
            //Debug.Log("fAngleDegree_MultiTimes = " + fMultiTimes + " | " + trChild.name);

            //简单的缩放线性公式:
            float fScale = -0.2f * (float)fMultiTimes + 1.4f;

            trChild.localScale = new Vector3(fScale, fScale, 1f);
        }
    }

    void OnSViewCrossBoxTriggerEnter(Collider other, bool bIsCrossToLeft)
    {
        string strDebug = string.Format("{0}: 向{1}穿过", other.name, bIsCrossToLeft ? "左" : "右");
        mLabSelecting.text = strDebug;
    }
}