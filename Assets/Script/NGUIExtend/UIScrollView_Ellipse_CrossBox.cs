using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIScrollView_Ellipse_CrossBox : MonoBehaviour
{
    /*    
        01.挂载在【CenterOnChild的参照物】上，监测OnTriggerEnter
        02.封装【Cross事件】
    */

    bool mIsCrossWatching = false;
    bool mIsCrossToLeft = false;

    public delegate void DGOnTriggerEnter(Collider other, bool bIsCrossToLeft);
    public DGOnTriggerEnter m_dgOnTriggerEnter;

    private void OnTriggerEnter(Collider other)
    {
        //Debug.Log("other = "+ other.name);
        if (mIsCrossWatching)
        {
            if (m_dgOnTriggerEnter != null)
            {
                m_dgOnTriggerEnter(other, mIsCrossToLeft);
            }            
        }
        else
        {
            EditorLOG.logWarn("OnTriggerEnter Missiong");
        }
    }

    public void Initialized(UIScrollView_Ellipse stSView)
    {
        GameCommon.ASSERT(stSView != null);
        UICenterOnChild_Ellipse stCenterOnChild = stSView.m_trRoot.GetComponent<UICenterOnChild_Ellipse>();
        GameCommon.ASSERT(stCenterOnChild != null);
        stCenterOnChild.onBeginSpringCallback = OnSViewBeginSpring;
        stSView.onMoveAbsoluteNotification = OnSViewMoveAbsolute;
    }

    void OnSViewBeginSpring(
        float _fAngleDegreeFrom, 
        float _fAngleDegreeTo, 
        UIScrollView_Ellipse.EM_RecenterEvent emREvent
        )
    {
        switch (emREvent)
        {
            case UIScrollView_Ellipse.EM_RecenterEvent.OnSViewDragFinished:
            case UIScrollView_Ellipse.EM_RecenterEvent.OnCustomClick:
            case UIScrollView_Ellipse.EM_RecenterEvent.OnCustomRefresh:
                {
                    float fDelta = Mathf.Abs(_fAngleDegreeFrom - _fAngleDegreeTo);
                    if (_fAngleDegreeFrom > _fAngleDegreeTo)
                    {
                        mIsCrossToLeft = true;
                    }
                    else
                    {
                        mIsCrossToLeft = false;
                    }
                    mIsCrossWatching = true;
                }
                break;
        }
    }

    void OnSViewMoveAbsolute(
        Vector3 _v3Absolute,
        float _fAngleDegreeFrom, 
        float _fAngleDegreeTo, 
        UIScrollView_Ellipse.EM_MoveAbsoluteEvent emMAEvent
        )
    {
        switch (emMAEvent)
        {
            case UIScrollView_Ellipse.EM_MoveAbsoluteEvent.OnSViewDrag:
                {
                    if (_v3Absolute.x > 0)
                    {
                        mIsCrossToLeft = false;
                    }
                    else
                    {
                        mIsCrossToLeft = true;
                    }
                    mIsCrossWatching = true;
                }
                break;
        }
    }

}