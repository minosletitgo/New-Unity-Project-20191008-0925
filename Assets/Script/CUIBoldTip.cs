using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CUIBoldTip : MonoBehaviour
{
    public GameObject m_goCacheRoot;
    public UIGrid m_gridPlayingRoot;
    public CUIBoldTipItem m_itemInsTip;
    public List<CUIBoldTipItem> m_lstItemPlaying = new List<CUIBoldTipItem>();
    public List<CUIBoldTipItem> m_lstItemCache = new List<CUIBoldTipItem>();
    static int m_sTempNumber = 0;


    public UIButton m_btnAddTip;

    private void Awake()
    {
        m_gridPlayingRoot.sorting = UIGrid.Sorting.Custom;
        m_gridPlayingRoot.onCustomSort = DoSort2Grid;
        m_itemInsTip.gameObject.SetActive(false);

        if (m_btnAddTip != null)
        {
            UIEventListener.Get(m_btnAddTip.gameObject).onClick = OnClick_BtnAddTip;
        }
    }

    int DoSort2Grid(Transform trans1, Transform trans2)
    {
        CUIBoldTipItem item1 = trans1.GetComponent<CUIBoldTipItem>();
        CUIBoldTipItem item2 = trans2.GetComponent<CUIBoldTipItem>();
        return item2.GetOnlyKey().CompareTo(item1.GetOnlyKey());
    }

    void OnClick_BtnAddTip(GameObject go)
    {
        List<string> lstString = new List<string>();
        lstString.Add("一");
        lstString.Add("二");
        lstString.Add("三");
        lstString.Add("四");
        lstString.Add("五");
        lstString.Add("六");
        lstString.Add("七");
        lstString.Add("八");
        lstString.Add("九");
        lstString.Add("十");
        lstString.Add("1");
        lstString.Add("2");
        lstString.Add("3");
        lstString.Add("4");
        lstString.Add("5");
        lstString.Add("6");
        lstString.Add("7");
        lstString.Add("8");
        lstString.Add("9");
        int nCount = NGUITools.RandomRange(5, 20);
        string strTipValue = null;
        strTipValue += string.Format("【{0}】: ", m_sTempNumber);
        for (int i = 0; i < nCount; i++)
        {
            strTipValue += lstString[NGUITools.RandomRange(0, lstString.Count - 1)];
        }

        AddTip(strTipValue);
    }

    public void AddTip(string strTip)
    {
        CUIBoldTipItem item = GetIdleTipItem();
        item.DoPlaying(strTip, m_sTempNumber++);
        BaseAdd2PlayingContainer(item);
        m_gridPlayingRoot.Reposition();
    }

    CUIBoldTipItem GetIdleTipItem()
    {
        CUIBoldTipItem itemRet = null;
        if (m_lstItemCache.Count > 0)
        {
            itemRet = m_lstItemCache[0];
            m_lstItemCache.RemoveAt(0);
        }
        else
        {
            GameObject go = Instantiate(m_itemInsTip.gameObject);
            itemRet = go.GetComponent<CUIBoldTipItem>();
            itemRet.transform.SetParent(m_gridPlayingRoot.transform);
            itemRet.transform.localPosition = Vector3.zero;
            itemRet.transform.localScale = Vector3.one;
            itemRet.transform.localRotation = Quaternion.Euler(Vector3.zero);
            itemRet.m_dgOnAlphaFinished = OnTipItemTweenAlphaFinished;
        }

        GameCommon.ASSERT(itemRet != null);
        return itemRet;
    }

    void BaseAdd2PlayingContainer(CUIBoldTipItem item)
    {
        item.transform.SetParent(m_gridPlayingRoot.transform);
        m_lstItemPlaying.Add(item);
    }

    void BaseAdd2CacheContainer(CUIBoldTipItem item)
    {
        item.transform.SetParent(m_goCacheRoot.transform);
        m_lstItemPlaying.Remove(item);
        m_lstItemCache.Add(item);
    }

    void OnTipItemTweenAlphaFinished(CUIBoldTipItem pThis)
    {
        pThis.gameObject.SetActive(false);
        BaseAdd2CacheContainer(pThis);

        if (m_lstItemPlaying.Count <= 0)
        {
            m_sTempNumber = 0;
        }
    }
}