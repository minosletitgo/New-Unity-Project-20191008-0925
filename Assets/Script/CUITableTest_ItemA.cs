using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CUITableTest_ItemA : MonoBehaviour 
{
    public UILabel m_labText;
    public GameObject m_goSelecting;
    public UIGrid m_gridChildItem;

    public delegate void DGOnClick(CUITableTest_ItemA pThis);
    public DGOnClick m_dgOnClick;

    int m_nValue;
    UIPlayTween m_stUIPlayTween;
    bool m_bIsFolding = true;

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

        m_nValue = 0;
        m_stUIPlayTween = gameObject.GetComponent<UIPlayTween>();
        m_stUIPlayTween.enabled = false;
        m_bIsFolding = true;
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

    public bool IsCurTargetTweenFolding()
    {
        return m_bIsFolding;
    }
    
    public void DoLockUIPlayTween(bool bIsLock)
    {
        m_stUIPlayTween.enabled = !bIsLock;
    }

    public void DoPlayUIPlayTween()
    {
        m_stUIPlayTween.Play(true);
        m_bIsFolding = !m_bIsFolding;
    }
}
