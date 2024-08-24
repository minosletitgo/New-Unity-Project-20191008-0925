using UnityEngine;

public class UIToggle_Simple : MonoBehaviour
{
    public UILabel m_labTitle;
    public GameObject m_goForeground;
    public GameObject m_goBackground;
    
    public GameObject m_goRedRemind;
    public UILabel m_labRedRemind;


    private void Awake()
    {
        GameCommon.ASSERT(m_goForeground != null, "UIToggle_Simple: m_goForeground == null");
    }

    public void SetTitle(string strTitle)
    {
        if (m_labTitle != null)
        {
            m_labTitle.text = strTitle;
        }
    }

    public void SetActiveValue(bool bActive)
    {
        m_goForeground.gameObject.SetActive(bActive);
        if (m_goBackground != null)
        {
            m_goBackground.gameObject.SetActive(!bActive);
        }
    }

    public void SetRedRemind(bool bRemind, string strValue = null)
    {
        if (m_goRedRemind != null)
        {
            m_goRedRemind.SetActive(bRemind);
        }

        if (bRemind && m_labRedRemind != null)
        {
            m_labRedRemind.text = strValue;
        }
    }
}