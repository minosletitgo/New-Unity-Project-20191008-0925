using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CUIMainChat : MonoBehaviour
{
    public UIInput mItRebuildChatCount;
    public UIButton mBtnRebuildChatCount;

    public UIInput mItAddChatCount;
    public UIButton mBtnAddChatCount;

    public UIInput mItSetAmountValue;
    public UIButton mBtnSetAmountValue;

    public UILabel mLabDebugPrint;

    public UIWrapContentEx mWrapChat;
    public CUIMainChatItem mInstChatItem;

    //模拟真实聊天内容(暂且以纯string为数据)
    List<string> mLstChatContent = new List<string>();

    





    private void Awake()
    {
        mItRebuildChatCount.value = null;
        mItRebuildChatCount.validation = UIInput.Validation.None;
        UIEventListener.Get(mBtnRebuildChatCount.gameObject).onClick = OnClick_BtnRebuildChatCount;

        mItAddChatCount.value = null;
        mItAddChatCount.validation = UIInput.Validation.None;
        UIEventListener.Get(mBtnAddChatCount.gameObject).onClick = OnClick_BtnAddChatCount;

        mItSetAmountValue.value = null;
        mItSetAmountValue.validation = UIInput.Validation.None;
        UIEventListener.Get(mBtnSetAmountValue.gameObject).onClick = OnClick_BtnSetAmountValue;

        {
            //UIWrapContentEx初始设置
            if (mWrapChat.transform.childCount != 1)
            {
                Debug.LogError("mWrapChat.transform.childCount != 1");
            }

            Transform trInstItem = mWrapChat.transform.GetChild(0);
            if (trInstItem != mInstChatItem.transform)
            {
                Debug.LogError("mWrapChat.transform.GetChild(0) != mInstChatItem");
            }
            
            const int nConstInstItem = 30;
            for (int i = 0; i < nConstInstItem; i++)
            {
                GameObject go = Instantiate(mInstChatItem.gameObject) as GameObject;
                go.name = "item_" + (i + 1);
                go.transform.parent = mWrapChat.transform;
                go.transform.localPosition = Vector3.zero;
                go.transform.localRotation = Quaternion.Euler(Vector3.zero);
                go.transform.localScale = Vector3.one;
            }

            mWrapChat.mIsDirectionAdd = false;
            mWrapChat.mDGOnFillingItem = OnWrapChatFillingItem;
            mWrapChat.mDGOnCalcItemCellSize = OnWrapChatCalcItemCellSize;
        }
    }

    private void Start()
    {
        mWrapChat.ClearVirtualChild();
    }

    private void Update()
    {
        mLabDebugPrint.text = mWrapChat.PrintDicChildToVitualIndex();
    }

    void OnWrapChatFillingItem(GameObject go, int realIndex)
    {
        CUIMainChatItem item = go.GetComponent<CUIMainChatItem>();

        string strValue = mLstChatContent[realIndex];
        item.SetFillData(strValue);
    }

    Bounds OnWrapChatCalcItemCellSize(GameObject go, int realIndex)
    {
        CUIMainChatItem item = go.GetComponent<CUIMainChatItem>();

        string strValue = mLstChatContent[realIndex];
        item.SetFillData(strValue);

        Bounds bd = NGUIMath.CalculateRelativeWidgetBounds(item.transform, false);
        return bd;
    }

    //Bounds GetAndFillingItem(CUIMainChatItem item,int realIndex)
    //{
    //    GameCommon.CHECK(realIndex >= 0 && realIndex < mLstChatContent.Count);

    //    string strValue = mLstChatContent[realIndex];
    //    item.SetFillData(strValue);

    //    Bounds bd = NGUIMath.CalculateRelativeWidgetBounds(item.transform, false);
    //    return bd;
    //}

    void AddChatCount(int nCount)
    {
        int indexStart = mLstChatContent.Count;
        for (int i = indexStart; i < indexStart + nCount; i++)
        {
            string strValue = null;
            strValue += string.Format("{0}: ", i);

            int nRandomWordCount = NGUITools.RandomRange(1, 300);
            //int nRandomWordCount = 36;
            //int nRandomWordCount = (i % 2 == 0) ? 35 : 15;
            for (int iR = 0; iR < nRandomWordCount; iR++)
            {
                strValue += "六";
            }

            mLstChatContent.Add(strValue);
        }
    }

    void OnClick_BtnRebuildChatCount(GameObject go)
    {
        int nRebuildChatCount = int.Parse(mItRebuildChatCount.value);
        if (nRebuildChatCount <= 0)
        {
            return;
        }

        mLstChatContent.Clear();
        AddChatCount(nRebuildChatCount);

        mWrapChat.ClearVirtualChild();
        mWrapChat.AddVirtualChild(nRebuildChatCount);
        mWrapChat.SetDragAmountValue(0.0f);
    }

    void OnClick_BtnAddChatCount(GameObject go)
    {
        int nAddChatCount = int.Parse(mItAddChatCount.value);
        if (nAddChatCount <= 0)
        {
            return;
        }

        AddChatCount(nAddChatCount);
        mWrapChat.AddVirtualChild(nAddChatCount);
        mWrapChat.SetDragAmountValue(mWrapChat.GetDragAmountValue());
    }

    void OnClick_BtnSetAmountValue(GameObject go)
    {
        float fAmountValue = float.Parse(mItSetAmountValue.value);
        if (fAmountValue < 0.0f)
        {
            return;
        }

        mWrapChat.SetDragAmountValue(fAmountValue);
    }
}