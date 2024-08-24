using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CUITableTest_ItemB : MonoBehaviour 
{
    public UILabel m_labText;
    public GameObject m_goSelecting;
    
    int m_nValue;
    public delegate void DGOnClick(CUITableTest_ItemB pThis);
    public DGOnClick m_dgOnClick;

    void Awake()
    {
        SetSelecting(false);

        UIEventListener.Get(gameObject).onClick =
        delegate (GameObject go)
        {
            if (m_dgOnClick != null)
            {
                m_dgOnClick(this);
            }
        };
    }

    public void SetValue(int nValue)
    {
        m_labText.text = nValue.ToString();
        m_nValue = nValue;
    }

    public int GetValue() { return m_nValue; }

    public void SetSelecting(bool bIsSelecting)
    {
        m_goSelecting.SetActive(bIsSelecting);
    }
}
