using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CUITableTest : MonoBehaviour 
{
    public UITable m_tabMain;
    public CUITableTest_ItemA m_stChildItemA;
    public CUITableTest_ItemB m_stChildItemB;
       
    CUITableTest_ItemA m_stLastSelectedChildItemA;
    CUITableTest_ItemB m_stLastSelectedChildItemB;

    IEnumerator m_coDelayBaseClickItemA;


    /*
        01.Start 只建立标题Item（即，第一层Item） 
        02.点击标题Item才及时刷新出第二层Item
    */

    void Start()
    {
        m_stChildItemA.gameObject.SetActive(false);
        m_stChildItemB.gameObject.SetActive(false);
        RebuildChildItemA();
    }


    void RebuildChildItemA()
    {
        int nItemACount = Random.Range(5, 20);
        //int nItemACount = 1;
        CUITableTest_ItemA stWillSelectingItemA = null;
        for (int i = 0; i < nItemACount; i++)
        {
            GameObject goInst = GameObject.Instantiate(m_stChildItemA.gameObject);
            goInst.SetActive(true);
            goInst.transform.SetParent(m_tabMain.transform);
            goInst.transform.localPosition = Vector3.zero;
            goInst.transform.localRotation = Quaternion.Euler(Vector3.zero);
            goInst.transform.localScale = Vector3.one;

            CUITableTest_ItemA stItemA = goInst.GetComponent<CUITableTest_ItemA>();
            stItemA.SetValue(Random.Range(10, 10000));
            stItemA.m_dgOnClick = OnClick_ChildItemA;

            if(stWillSelectingItemA == null)
            {
                stWillSelectingItemA = stItemA;
            }
        }
        m_tabMain.repositionNow = true;

        //if (stWillSelectingItemA != null)
        //{
        //    BaseClickChildItemA(stWillSelectingItemA);
        //}
    }

    #region --------------------------------------------------------------------------------
    void BaseClickChildItemA(CUITableTest_ItemA _stItem)
    {
        Debug.Log("BaseClickChildItemA -> " + _stItem.GetValue());

        if (m_stLastSelectedChildItemA != null)
        {
            m_stLastSelectedChildItemA.SetSelecting(false);
        }
        if (m_stLastSelectedChildItemB != null)
        {
            m_stLastSelectedChildItemB.SetSelecting(false);
        }
        _stItem.SetSelecting(true);

        if (_stItem.IsCurTargetTweenFolding())
        {
            //当前_stItem为已经折叠状态
            if (m_coDelayBaseClickItemA != null)
            {
                StopCoroutine(m_coDelayBaseClickItemA);
            }
            m_coDelayBaseClickItemA = CoDelayBaseClickItemA(_stItem);
            StartCoroutine(m_coDelayBaseClickItemA);
        }
        else
        {
            _stItem.DoLockUIPlayTween(false);
            _stItem.DoPlayUIPlayTween();
            _stItem.DoLockUIPlayTween(true);
        }
    }

    IEnumerator CoDelayBaseClickItemA(CUITableTest_ItemA _stItem)
    {
        yield return new WaitForSeconds(1.5f);

        _stItem.DoLockUIPlayTween(false);
        _stItem.DoPlayUIPlayTween();

        //重建ChildItemB        
        for (int i = 0; i < _stItem.m_gridChildItem.transform.childCount; i++)
        {
            Transform trChild = _stItem.m_gridChildItem.transform.GetChild(i);
            DestroyObject(trChild.gameObject);
        }
        _stItem.m_gridChildItem.transform.DetachChildren();

        int nItemBCount = Random.Range(1, 8);
        //int nItemBCount = 3;
        CUITableTest_ItemB stWillSelectingItemB = null;
        for (int i = 0; i < nItemBCount; i++)
        {
            GameObject goInst = GameObject.Instantiate(m_stChildItemB.gameObject);
            goInst.SetActive(true);
            goInst.transform.SetParent(_stItem.m_gridChildItem.transform);
            goInst.transform.localPosition = Vector3.zero;
            goInst.transform.localRotation = Quaternion.Euler(Vector3.zero);
            goInst.transform.localScale = Vector3.one;

            CUITableTest_ItemB stItemB = goInst.GetComponent<CUITableTest_ItemB>();
            stItemB.SetValue(Random.Range(10, 10000));
            stItemB.m_dgOnClick = OnClick_ChildItemB;

            if (stWillSelectingItemB == null)
            {
                stWillSelectingItemB = stItemB;
            }
        }
        _stItem.m_gridChildItem.repositionNow = true;

        if (stWillSelectingItemB != null)
        {
            BaseClickChildItemB(stWillSelectingItemB);
        }

        m_stLastSelectedChildItemA = _stItem;

        _stItem.DoLockUIPlayTween(true);
    }
         
    void BaseClickChildItemB(CUITableTest_ItemB _stItem)
    {
        Debug.Log("BaseClickChildItemB -> " + _stItem.GetValue());

        if (m_stLastSelectedChildItemB != null)
        {
            m_stLastSelectedChildItemB.SetSelecting(false);
        }
        _stItem.SetSelecting(true);

        m_stLastSelectedChildItemB = _stItem;
    }
    #endregion --------------------------------------------------------------------------------





    void OnClick_ChildItemA(CUITableTest_ItemA _stItem)
    {
        BaseClickChildItemA(_stItem);
    }

    void OnClick_ChildItemB(CUITableTest_ItemB _stItem)
    {
        BaseClickChildItemB(_stItem);
    }


}
