using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CUILearnSkill : MonoBehaviour
{
    public UIInput mItCountLearning;
    public UIInput mItCountWaiting;
    public UIButton mBtnRebuild;

    public UIScrollView mSView;
    public UIGrid mGrid;
    public CUILearnSkill_ItemMix mInstItemMix;
    List<CUILearnSkill_ItemMix> mLstUIItemMix = new List<CUILearnSkill_ItemMix>();

    List<ST_ItemData> mLstItemData = new List<ST_ItemData>();

    public CUILearnSkill_Cursor mCursor;
    CUILearnSkill_ItemMix mItemDragingBegin;
    CUILearnSkill_ItemMix mItemDragingEnd;
    CUILearnSkill_ItemMix mItemDragingHighlight;

    public enum EM_State
    {
        Null,
        Title,
        Learning,
        Waiting,
    };

    public class ST_ItemData
    {
        public ST_ItemData(EM_State emState, int nData, int nSecondAll, string strTitle)
        {
            this.emState = emState;
            this.nData = nData;
            this.nSecondAll = nSecondAll;
            this.strTitle = strTitle;

            this.dtTimeStart = DateTime.Now;
        }

        EM_State emState;
        int nData;
        int nSecondAll;
        string strTitle;
        DateTime dtTimeStart;

        public EM_State GetState() { return emState; }
        public int GetData() { return nData; }
        public int GetSecondAll() { return nSecondAll; }
        public string GetTitle() { return strTitle; }
        public DateTime GetTimeStart() { return dtTimeStart; }
        public int GetSortValue()
        {
            int nSortValue = 0;
            switch (GetState())
            {
                case EM_State.Title:
                    {
                        nSortValue = (int)(GetData()) * 100000;
                    }
                    break;
                case EM_State.Learning:
                    {
                        nSortValue = (int)(EM_State.Learning) * 100000 + GetData();
                    }
                    break;
                case EM_State.Waiting:
                    {
                        nSortValue = (int)(EM_State.Waiting) * 100000 + GetData();
                    }
                    break;
            }

            return nSortValue;
        }

        public void ChgStateTo(EM_State emState)
        {
            this.emState = emState;
        }
    };

    


    void Awake()
    {
        mItCountLearning.value = null;
        mItCountLearning.validation = UIInput.Validation.Integer;
        mItCountWaiting.value = null;
        mItCountWaiting.validation = UIInput.Validation.Integer;

        UIEventListener.Get(mBtnRebuild.gameObject).onClick = OnClick_BtnRebuild;
        mInstItemMix.gameObject.SetActive(false);

        mGrid.sorting = UIGrid.Sorting.None;

        mCursor.InitRootItem(mInstItemMix);

        ////Test
        //Transform trans1 = gameObject.transform.Find("UIGrid");
        //Transform trans2 = gameObject.transform.Find("DragPanel");
        //Transform trans3 = gameObject.transform.Find("List/DragPanel/UIGrid");
        //Debug.Log("trans1 = " + trans1.name);
        //Debug.Log("trans2 = " + trans2.name);
        //Debug.Log("trans3 = " + trans3.name);
    }

    int GetRandomData()
    {
        return NGUITools.RandomRange(1, 1000);
    }

    int GetRandomnSecondRemain()
    {
        //return NGUITools.RandomRange(60, 90000);
        return NGUITools.RandomRange(60, 500);
    }

    void OnClick_BtnRebuild(GameObject go)
    {
        for (int i = 0; i < mLstItemData.Count; i++)
        {
            mLstItemData[i] = null;
        }
        mLstItemData.Clear();

        mLstItemData.Add(new ST_ItemData(EM_State.Title, (int)(EM_State.Learning), 0, "学习中"));
        int nCountLearning = int.Parse(mItCountLearning.value);
        for (int i = 0; i < nCountLearning; i++)
        {
            mLstItemData.Add(new ST_ItemData(EM_State.Learning, GetRandomData(), GetRandomnSecondRemain(),null));
        }

        mLstItemData.Add(new ST_ItemData(EM_State.Title, (int)(EM_State.Waiting), 0, "等待中"));
        int nCountWaiting = int.Parse(mItCountWaiting.value);
        for (int i = 0; i < nCountWaiting; i++)
        {
            mLstItemData.Add(new ST_ItemData(EM_State.Waiting, GetRandomData(), GetRandomnSecondRemain(), null));
        }

        //保持[数据列表]的严格顺序
        mLstItemData.Sort(DoSortItemData);
        
        RefreshAllData();
    }

    int DoSortItemData(ST_ItemData stData1, ST_ItemData stData2)
    {
        return stData1.GetSortValue().CompareTo(stData2.GetSortValue());
    }
    
    void RefreshAllData()
    {
        mSView.verticalScrollBar.value = 0;

        int indexUIMix = 0;
        int indexData = 0;
        while (indexData < mLstItemData.Count)
        {
            ST_ItemData stData = mLstItemData[indexData];

            CUILearnSkill_ItemMix stUIItem = null;
            if (indexUIMix < mLstUIItemMix.Count)
            {
                stUIItem = mLstUIItemMix[indexUIMix];
            }
            else
            {
                GameObject go = Instantiate(mInstItemMix.gameObject) as GameObject;
                go.name = "item_" + indexData;
                go.transform.parent = mGrid.transform;
                go.transform.localPosition = Vector3.zero;
                go.transform.localRotation = Quaternion.Euler(Vector3.zero);
                go.transform.localScale = Vector3.one;
                stUIItem = go.GetComponent<CUILearnSkill_ItemMix>();
                stUIItem.m_dgDragEvent = OnItemDragEvent;

                mLstUIItemMix.Add(stUIItem);
            }

            stUIItem.gameObject.SetActive(true);
            stUIItem.SetFillData(stData);

            indexUIMix++;

            indexData++;
        }
        for (; indexUIMix < mLstUIItemMix.Count; indexUIMix++)
        {
            mLstUIItemMix[indexUIMix].gameObject.SetActive(false);
        }

        mGrid.repositionNow = true;
    }

    void OnItemDragEvent(
        CUILearnSkill_ItemMix pThis,
        CUILearnSkill_ItemMix.EM_DragEvent emDragEvent,
        object objData
        )
    {
        //Debug.Log("OnItemDragEvent: " + emDragEvent.ToString() + " -> " + pThis.name);

        switch (emDragEvent)
        {
            case CUILearnSkill_ItemMix.EM_DragEvent.OnHoldPress:
                {
                    if (pThis.GetItemData().GetState() == EM_State.Learning)
                    {
                        return;
                    }

                    bool bIsPress = (bool)objData;
                    if (bIsPress)
                    {
                        //按压[pThis区域]
                        pThis.SetActiveTakenRoot(false);
                        mCursor.BeginRunning(pThis.GetItemData());

                        //Debug.Log("Start: " + pThis.name);

                        mItemDragingBegin = pThis;
                        mItemDragingEnd = null;

                        //Test
                        if (mCoUpdateDragScrollBar != null)
                        {
                            StopCoroutine(mCoUpdateDragScrollBar);
                        }
                        mCoUpdateDragScrollBar = CoUpdateDragScrollBar(mLstItemData.Count);
                        StartCoroutine(mCoUpdateDragScrollBar);
                    }
                    else
                    {
                        //按压[pThis区域]后原地松起
                        Debug.Log("无效拖拽");
                        mCursor.EndRunning();
                        if (mItemDragingBegin != null)
                        {
                            mItemDragingBegin.SetActiveTakenRoot(true);
                        }

                        if (mItemDragingHighlight != null)
                        {
                            mItemDragingHighlight.SetActiveHighlight(false);
                        }

                        mItemDragingBegin = null;
                        mItemDragingEnd = null;
                    }            
                }
                break;
            case CUILearnSkill_ItemMix.EM_DragEvent.OnDragOver:
                {
                    if (mItemDragingBegin != null)
                    {
                        //拖拽进入[pThis区域]
                        if (mItemDragingHighlight != null)
                        {
                            mItemDragingHighlight.SetActiveHighlight(false);
                        }

                        if (pThis != mItemDragingBegin)
                        {
                            //自动移动[高亮元素]
                            mItemDragingHighlight = pThis;
                            mItemDragingHighlight.SetActiveHighlight(true);
                            //Debug.Log("mItemDragingHighlight: " + mItemDragingHighlight.GetItemData().GetData());
                        }
                    }                                
                }
                break;
            case CUILearnSkill_ItemMix.EM_DragEvent.OnDragOut:
                {
                    //拖拽离开[pThis区域]
                    if (pThis != mCursor.GetCacheItem() &&
                        pThis != mItemDragingBegin
                        )
                    {
                        //Debug.Log("EM_DragEvent.OnDragOut: " + pThis.GetItemData().GetData());
                        mItemDragingEnd = pThis;
                    }

                    if (pThis == mItemDragingBegin)
                    {
                        mItemDragingEnd = null;
                    }
                }
                break;
            case CUILearnSkill_ItemMix.EM_DragEvent.OnDragEnd:
                {
                    //拖拽结束（即开始结算）
                    if (mItemDragingBegin != null && mItemDragingEnd != null)
                    {
                        string strLog = string.Format("有效拖拽: {0}({1}) -> {2}({3})",
                            mItemDragingBegin.name, mItemDragingBegin.GetItemData().GetData().ToString(),
                            mItemDragingEnd.name, mItemDragingEnd.GetItemData().GetData().ToString()
                            );
                        Debug.Log(strLog);

                        //逻辑交互模拟....
                        DoTryChange(mItemDragingBegin, mItemDragingEnd);
                    }
                    else
                    {
                        Debug.Log("无效拖拽");
                    }

                    mCursor.EndRunning();
                    if (mItemDragingBegin != null)
                    {
                        mItemDragingBegin.SetActiveTakenRoot(true);
                    }

                    if (mItemDragingHighlight != null)
                    {
                        mItemDragingHighlight.SetActiveHighlight(false);
                    }

                    mItemDragingBegin = null;
                    mItemDragingEnd = null;
                }
                break;
        }
    }

    IEnumerator mCoUpdateDragScrollBar = null;
    IEnumerator CoUpdateDragScrollBar(int itemCount)
    {
        /*

        ******|*********|******
        ******|---------|******
        ******|         |******
        ******|         |******
        ******|---------|******
        ******|*********|******

        星花区域即可触发裁剪框自动滑动
        */

        while (mItemDragingBegin != null)
        {
            yield return new WaitForSeconds(0.001f);
            //mSView.verticalScrollBar.value += 0.0005f; //-> 100
            //mSView.verticalScrollBar.value += 0.001f; //-> 50
            //mSView.verticalScrollBar.value += 0.01f; //-> 10

            //float fSpeed = (float)Math.Log(itemCount) * -0.004f + 0.0196f;
            float fSpeed = 0.01f;
            //if (fSpeed <= 0)
            //{
            //    fSpeed = 0.001f;
            //}            

            Vector3 v3Mouse = Input.mousePosition;
            //Debug.Log("v3Mouse:" + v3Mouse);

            Vector3[] corners = mSView.panel.worldCorners;
            Vector3[] v3CornerScreen = new Vector3[corners.Length];
            for (int i = 0; i < corners.Length; i++)
            {
                v3CornerScreen[i] = UICamera.currentCamera.WorldToScreenPoint(corners[i]);
                //Debug.Log("v3CornerScreen:" + i + " -> " + v3CornerScreen[i]);
            }

            //if (v3Mouse.x > v3CornerScreen[1].x &&
            //    v3Mouse.x < v3CornerScreen[2].x
            //    )
            {
                bool bIsNeedFindHighlight = false;
                bool bIsFindHighlightToBottom = false;

                if (v3Mouse.y < v3CornerScreen[0].y)
                {
                    //"下区域",需要触发滑动值递增
                    if (mSView.shouldMoveVertically)
                    {
                        mSView.verticalScrollBar.value += fSpeed;
                    }                        

                    bIsNeedFindHighlight = true;
                    bIsFindHighlightToBottom = true;
                }

                if (v3Mouse.y > v3CornerScreen[2].y)
                {
                    //"上区域",需要触发滑动值递减
                    if (mSView.shouldMoveVertically)
                    {
                        mSView.verticalScrollBar.value -= fSpeed;
                    }

                    bIsNeedFindHighlight = true;
                    bIsFindHighlightToBottom = false;
                }

                if (bIsNeedFindHighlight && mItemDragingHighlight != null)
                {
                    int indexHighlightCur = mLstUIItemMix.FindIndex(x => x.GetItemData() == mItemDragingHighlight.GetItemData());
                    CUILearnSkill_ItemMix itemTarget = FindNearestClipItem(indexHighlightCur, bIsFindHighlightToBottom);
                    //Debug.Log("+ itemTarget = " + itemTarget.GetItemData().GetData());

                    if (itemTarget != null)
                    {
                        if (mItemDragingHighlight != null)
                        {
                            mItemDragingHighlight.SetActiveHighlight(false);
                        }

                        if (itemTarget != mItemDragingBegin)
                        {
                            //自动移动[高亮元素]
                            mItemDragingHighlight = itemTarget;
                            mItemDragingHighlight.SetActiveHighlight(true);
                            //Debug.Log("mItemDragingHighlight: " + mItemDragingHighlight.GetItemData().GetData());
                            //自动作为End
                            mItemDragingEnd = mItemDragingHighlight;
                        }
                    }                 
                }
            }
        }
    }

    CUILearnSkill_ItemMix FindNearestClipItem(int indexScr, bool bIsBottom)
    {
        //找出距离裁剪框最近的Item

        Vector3[] corners = mSView.panel.worldCorners;
        if (bIsBottom)
        {
            Vector3 v3CornerBottom = UICamera.currentCamera.WorldToScreenPoint(corners[0]);
            int indexTarget = 0;
            for (int i = indexScr; i < mLstUIItemMix.Count; i++)
            {
                CUILearnSkill_ItemMix itemCur = mLstUIItemMix[i] as CUILearnSkill_ItemMix;
                if (itemCur.GetItemData().GetState() == EM_State.Title)
                {
                    continue;
                }

                indexTarget = i;
                Vector3 v3ItemCenter = UICamera.currentCamera.WorldToScreenPoint(itemCur.transform.position);
                if (v3ItemCenter.y < v3CornerBottom.y)
                {
                    break;
                }
            }
            
            CUILearnSkill_ItemMix itemTarget = mLstUIItemMix[indexTarget] as CUILearnSkill_ItemMix;
            return itemTarget;
        }
        else
        {
            Vector3 v3CornerTop = UICamera.currentCamera.WorldToScreenPoint(corners[1]);
            int indexTarget = 0;
            for (int i = indexScr; i >= 0; i--)
            {
                CUILearnSkill_ItemMix itemCur = mLstUIItemMix[i] as CUILearnSkill_ItemMix;
                if (itemCur == null)
                {
                    continue;
                }

                indexTarget = i;
                Vector3 v3ItemCenter = UICamera.currentCamera.WorldToScreenPoint(itemCur.transform.position);
                if (v3ItemCenter.y > v3CornerTop.y)
                {
                    break;
                }
            }
            
            CUILearnSkill_ItemMix itemTarget = mLstUIItemMix[indexTarget] as CUILearnSkill_ItemMix;
            return itemTarget;
        }
    }

    void DoTryChange(CUILearnSkill_ItemMix itemBegin, CUILearnSkill_ItemMix itemEnd)
    {
        //客户端模拟插入动作
        //把itemBegin插入到itemEnd尾部

        int indexBegin = mLstItemData.FindIndex(x => x == itemBegin.GetItemData());
        int indexEnd = mLstItemData.FindIndex(x => x == itemEnd.GetItemData());

        if (itemEnd.GetItemData().GetState() == EM_State.Learning)
        {
            //替换操作
            ST_ItemData stBegin = mLstItemData[indexBegin];
            stBegin.ChgStateTo(EM_State.Learning);
            ST_ItemData stEnd = mLstItemData[indexEnd];
            stEnd.ChgStateTo(EM_State.Waiting);

            mLstItemData.RemoveAt(indexBegin);
            mLstItemData.RemoveAt(indexEnd);

            mLstItemData.Insert(indexEnd, stBegin);
            mLstItemData.Insert(indexBegin, stEnd);
        }
        else
        {
            if (indexBegin > indexEnd)
            {
                if (indexBegin == indexEnd + 1)
                {
                    return;
                }
                mLstItemData.Remove(itemBegin.GetItemData());
                mLstItemData.Insert(indexEnd + 1, itemBegin.GetItemData());
            }
            else
            {
                mLstItemData.Insert(indexEnd + 1, itemBegin.GetItemData());
                mLstItemData.RemoveAt(indexBegin);
            }
        }

        float fScrollBarValue = mSView.verticalScrollBar.value;
        RefreshAllData();
        mSView.verticalScrollBar.value = fScrollBarValue;
    }
}


