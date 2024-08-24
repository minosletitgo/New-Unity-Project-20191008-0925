using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CUILearnSkill_ItemMix : MonoBehaviour
{
    public GameObject mObjRootTitle;
    public UILabel mLabTitle;

    public GameObject mObjRootDetail;
    public GameObject mObjTakenRoot;
    public UILabel mLabName;
    public UIProgressBar mPrgCD;
    public UILabel mLabTimeRemain;
    public GameObject mObjHighlight;

    BoxCollider mBoxClick;
    CUIHoldClickTrigger mHoldClickTrigger;
    UIDragScrollView mDragScrollView;

    public enum EM_DragEvent
    {
        OnHoldPress,

        OnDragStart,
        OnDrag,
        OnDragOver,
        OnDragOut,
        OnDragEnd,
        OnDrop,
    };

    public delegate void DGDragEvent(
        CUILearnSkill_ItemMix pThis,
        EM_DragEvent emDragEvent,
        object objData
        );
    public DGDragEvent m_dgDragEvent = null;


    void Awake()
    {
        mObjHighlight.SetActive(false);

        mBoxClick = gameObject.GetComponent<BoxCollider>();
        mHoldClickTrigger = gameObject.GetComponent<CUIHoldClickTrigger>();
        mHoldClickTrigger.TryAwake(mBoxClick);
        mHoldClickTrigger.m_dgHoldClick = OnHoldClick;

        UIEventListener.Get(gameObject).onDragStart += OnDragStart_Self;
        UIEventListener.Get(gameObject).onDrag += OnDrag_Self;
        UIEventListener.Get(gameObject).onDragOver += OnDragOver_Self;
        UIEventListener.Get(gameObject).onDragOut += OnDragOut_Self;
        UIEventListener.Get(gameObject).onDragEnd += OnDragEnd_Self;
        UIEventListener.Get(gameObject).onDrop += OnDrop_Self;

        mDragScrollView = gameObject.GetComponent<UIDragScrollView>();
    }

    protected CUILearnSkill.ST_ItemData m_stData;
    public void SetFillData(CUILearnSkill.ST_ItemData stData, bool bIsCursor = false)
    {
        mObjRootTitle.SetActive(stData.GetState() == CUILearnSkill.EM_State.Title);
        mObjRootDetail.SetActive(stData.GetState() != CUILearnSkill.EM_State.Title);

        if (stData.GetState() == CUILearnSkill.EM_State.Title)
        {
            mLabTitle.text = stData.GetTitle();
            mBoxClick.enabled = false;
        }
        else
        {
            mLabName.text = stData.GetData().ToString();
            mBoxClick.enabled = true;
        }

        if (bIsCursor)
        {
            mBoxClick.enabled = false;
        }

        m_stData = stData;
    }

    public CUILearnSkill.ST_ItemData GetItemData() { return m_stData; }
    public void ClearFillData() { m_stData = null; }

    void Update()
    {
        if (m_stData != null && m_stData.GetState() != CUILearnSkill.EM_State.Title)
        {
            //计算时间
            int nSecondPassed = (int)(DateTime.Now - m_stData.GetTimeStart()).TotalSeconds;
            int nSecondRemain = m_stData.GetSecondAll() - nSecondPassed;
            float fProgress = (float)nSecondPassed / (float)m_stData.GetSecondAll();
            mPrgCD.value = fProgress;

            TimeSpan tsRemain = TimeSpan.FromSeconds(nSecondRemain);
            {
                //格式:25:31:20
                //小时:分钟:秒
                mLabTimeRemain.text = string.Format("{0}:{1}:{2}",
                    (tsRemain.Days * 24 + tsRemain.Hours).ToString("00"),
                    (tsRemain.Minutes).ToString("00"),
                    (tsRemain.Seconds).ToString("00")
                    );
            }            
        }
    }

    public void SetActiveTakenRoot(bool bActive)
    {
        mDragScrollView.enabled = bActive;
        mObjTakenRoot.SetActive(bActive);
    }

    public void SetActiveHighlight(bool bActive)
    {
        mObjHighlight.SetActive(bActive);
    }

    void OnHoldClick(bool bPressed, object objData)
    {
        //Debug.Log("......OnHoldClick........" + "bPressed:" + bPressed.ToString() + " -> " + gameObject.name);

        if (m_dgDragEvent != null)
        {
            m_dgDragEvent(this, EM_DragEvent.OnHoldPress, bPressed);
        }
    }

    void OnDragStart_Self(GameObject go)
    {
        //Debug.Log("......OnDragStart........" + go.name);

        if (m_dgDragEvent != null)
        {
            m_dgDragEvent(this, EM_DragEvent.OnDragStart, null);
        }
    }

    void OnDrag_Self(GameObject go, Vector2 delta)
    {
        //Debug.Log("......OnDrag........" + go.name);

        if (m_dgDragEvent != null)
        {
            m_dgDragEvent(this, EM_DragEvent.OnDrag, null);
        }
    }

    void OnDragOver_Self(GameObject go)
    {
        //Debug.Log("......OnDragOver........" + go.name + "|" + this.GetItemData().GetData());

        if (m_dgDragEvent != null)
        {
            m_dgDragEvent(this, EM_DragEvent.OnDragOver, null);
        }
    }

    void OnDragOut_Self(GameObject go)
    {
        //Debug.Log("......OnDragOut........" + go.name + "|" + this.GetItemData().GetData());

        if (m_dgDragEvent != null)
        {
            m_dgDragEvent(this, EM_DragEvent.OnDragOut, null);
        }
    }

    void OnDragEnd_Self(GameObject go)
    {
        //Debug.Log("......OnDragEnd........" + go.name);

        if (m_dgDragEvent != null)
        {
            m_dgDragEvent(this, EM_DragEvent.OnDragEnd, null);
        }
    }

    void OnDrop_Self(GameObject go, GameObject obj)
    {
        //Debug.Log("......OnDrop........" + obj.name + " -> " + go.name);

        if (m_dgDragEvent != null)
        {
            m_dgDragEvent(this, EM_DragEvent.OnDrop, null);
        }
    }
}