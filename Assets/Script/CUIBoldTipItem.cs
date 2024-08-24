using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CUIBoldTipItem : MonoBehaviour
{
    public TweenAlpha m_twAlpha;
    public UILabel m_labString;
    int m_nOnlyKey = 0;


    private void Awake()
    {
        m_twAlpha.SetOnFinished(OnTweenAlphaFinished);
        m_labString.text = null;
    }

    public delegate void DgOnTweenAlphaFinished(CUIBoldTipItem pThis);
    public DgOnTweenAlphaFinished m_dgOnAlphaFinished;
    void OnTweenAlphaFinished()
    {
        if (m_dgOnAlphaFinished != null)
        {
            m_dgOnAlphaFinished(this);
        }
        //Debug.Log("Finished: " + mLabText.text);
    }

    public void DoPlaying(string strText, int nOnlyKey)
    {
        m_nOnlyKey = nOnlyKey;

        m_twAlpha.ResetToBeginning();
        gameObject.SetActive(true);
        m_twAlpha.PlayForward();

        if (!string.IsNullOrEmpty(strText))
        {
            m_labString.text = strText;
        }
        else
        {
            m_labString.text = "NULL";
        }
    }

    public int GetOnlyKey() { return m_nOnlyKey; }
}