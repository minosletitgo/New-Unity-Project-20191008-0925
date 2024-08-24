using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CUIScrollViewCenter : MonoBehaviour
{
    public UIScrollView m_sv;
    public UIPanel m_pal;
    public UIGrid m_grid;
    UICenterOnChild m_stCenter;

    public CUIScrollViewCenterItem m_instUIItem;
    public int m_nTestItemCount = 10;
    List<CUIScrollViewCenterItem> m_lstUIItems = new List<CUIScrollViewCenterItem>();


    private void Awake()
    {
        m_stCenter = m_grid.GetComponent<UICenterOnChild>();
        GameCommon.ASSERT(m_stCenter != null);
        m_stCenter.onFinished += OnCenterFinished;

        m_pal.onClipMove += OnClipMove;

        for (int i = 0; i < m_nTestItemCount; i++)
        {
            GameObject goInst = Object.Instantiate(m_instUIItem.gameObject);
            GameCommon.ASSERT(goInst != null);
            goInst.transform.SetParent(m_grid.transform);
            goInst.transform.localPosition = Vector3.zero;
            goInst.transform.localRotation = Quaternion.Euler(Vector3.zero);
            goInst.transform.localScale = Vector3.one;

            CUIScrollViewCenterItem item = goInst.GetComponent<CUIScrollViewCenterItem>();
            GameCommon.ASSERT(item != null);
            item.gameObject.name = "item_" + i;
            item.m_labName.text = "Name " + i;

            m_lstUIItems.Add(item);
        }
        m_grid.repositionNow = true;

        m_instUIItem.gameObject.SetActive(false);
    }

    private void Start()
    {
        if (m_coFirstCenterOn != null)
        {
            StopCoroutine(m_coFirstCenterOn);
        }
        m_coFirstCenterOn = CoFirstCenterOn();
        StartCoroutine(m_coFirstCenterOn);
    }

    IEnumerator m_coFirstCenterOn;
    IEnumerator CoFirstCenterOn()
    {
        yield return new WaitForSeconds(0.2f);
        m_stCenter.CenterOn(m_lstUIItems[0].transform);
    }

    void OnCenterFinished()
    {
        if (m_stCenter.centeredObject != null)
        {
            Debug.Log("OnCenterFinished: " + m_stCenter.centeredObject.name);
        }
        else
        {
            Debug.Log("OnCenterFinished: ?");
        }
    }

    void OnClipMove(UIPanel panel, Vector2 v2Delta)
    {
        if (m_stCenter.centeredObject != null)
        {
            Debug.Log("OnClipMove: " + m_stCenter.centeredObject.name);
        }
        else
        {
            Debug.Log("OnClipMove: ?");
        }
    }

}