using UnityEngine;
using System.Collections;

public class CUIHoldClickTrigger : MonoBehaviour
{
    public void TryAwake(BoxCollider boxClick)
    {
        //TCK.CHECK(boxClick != null);

        UIEventListener.Get(boxClick.gameObject).onPress += OnPress_BoxClick;
        UIEventListener.Get(boxClick.gameObject).onDrag += OnDrag_BoxClick;
    }

    public void TryAwake(GameObject objBox)
    {
        //TCK.CHECK(objBox != null);

        UIEventListener.Get(objBox.gameObject).onPress += OnPress_BoxClick;
        UIEventListener.Get(objBox.gameObject).onDrag += OnDrag_BoxClick;
    }

    object m_objData;
    public void SetFillData(object objData)
    {
        m_objData = objData;
    }


    public delegate void DGNormalClick(object objData);
    public DGNormalClick m_dgNormalClick;
    public delegate void DGHoldClick(bool bPressed, object objData);
    public DGHoldClick m_dgHoldClick;
    void OnPress_BoxClick(GameObject go, bool bPressed)
    {
        //Debug.Log("OnPress_BoxClick " + bPressed);

        m_fHoldTimeStart = 0f;

        if (bPressed)
        {
            m_fHoldTimeStart = Time.time;
            m_bOnDragRunning = false;
        }
        else
        {
            if (m_bHoldPress)
            {
                if (!m_bIgnoreOnDragRunning || !m_bOnDragRunning)
                {
                    if (m_dgHoldClick != null)
                    {
                        m_dgHoldClick(false, m_objData);
                    }
                }
                m_bHoldPress = false;
            }
            else
            {
                if (m_dgNormalClick != null)
                {
                    m_dgNormalClick(m_objData);
                }
            }
        }
    }

    bool m_bOnDragRunning = false;
    bool m_bIgnoreOnDragRunning = true;
    void OnDrag_BoxClick(GameObject go, Vector2 delta)
    {
        m_bOnDragRunning = true;
    }

    bool m_bHoldPress = false;
    float m_fHoldTimeStart = 0;
    void Update()
    {
        if (m_fHoldTimeStart > 0)
        {
            if (Time.time - m_fHoldTimeStart >= m_fHoldTime)
            {
                if (!m_bIgnoreOnDragRunning || !m_bOnDragRunning)
                {
                    if (m_dgHoldClick != null)
                    {
                        m_dgHoldClick(true, m_objData);
                    }
                }           

                m_bHoldPress = true;
                m_fHoldTimeStart = 0f;
            }
        }
    }

    public float m_fHoldTime = 0.03f;
    public void SetHoldTime(float fTime)
    {
        m_fHoldTime = fTime > 0 ? fTime : 0.03f;
    }
}