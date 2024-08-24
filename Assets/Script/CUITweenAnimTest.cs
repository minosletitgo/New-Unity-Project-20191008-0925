using UnityEngine;
using System.Collections;

public class CUITweenAnimTest : MonoBehaviour
{
    public GameObject m_goRoot;
    CUITweenAnimTest_Item[] m_aryTweenAnimItems;

    public UIButton m_btnRandomScreenPos;
    public UIButton m_btnShow;
    public UIButton m_btnUnShow;




    private void Awake()
    {
        m_aryTweenAnimItems = m_goRoot.GetComponentsInChildren<CUITweenAnimTest_Item>(true);
        GameCommon.ASSERT(m_aryTweenAnimItems != null);
        GameCommon.ASSERT(m_aryTweenAnimItems.Length > 0);

        UIEventListener.Get(m_btnRandomScreenPos.gameObject).onClick =
        delegate (GameObject go)
        {
            DoRandomScreenPos();
        };

        UIEventListener.Get(m_btnShow.gameObject).onClick =
        delegate (GameObject go)
        {
            DoShow();
        };

        UIEventListener.Get(m_btnUnShow.gameObject).onClick =
        delegate (GameObject go)
        {
            DoUnShow();
        };
    }

    void DoRandomScreenPos()
    {
        Vector2 v2AnixX = new Vector2(
            0 + 100.0f, Screen.width - 100.0f
            );
        Vector2 v2AnixY = new Vector2(
            0 + 100.0f, Screen.height - 100.0f
            );

        for (int i = 0; i < m_aryTweenAnimItems.Length; i++)
        {
            CUITweenAnimTest_Item _item = m_aryTweenAnimItems[i];
            GameCommon.ASSERT(_item != null);

            int _nX = NGUITools.RandomRange((int)v2AnixX.x, (int)v2AnixX.y);
            int _nY = NGUITools.RandomRange((int)v2AnixY.x, (int)v2AnixY.y);

            _item.transform.position = UICamera.currentCamera.ScreenToWorldPoint(
                new Vector2(_nX, _nY)
                );
        }
    }

    void DoShow()
    {
        if (m_coShow != null)
        {
            StopCoroutine(m_coShow);
        }
        m_coShow = CoShow();
        StartCoroutine(m_coShow);
    }

    IEnumerator m_coShow;
    IEnumerator CoShow()
    {
        float fDelayInterval = 0.02f;
        for (int i = 0; i < m_aryTweenAnimItems.Length; i++)
        {
            CUITweenAnimTest_Item _item = m_aryTweenAnimItems[i];
            GameCommon.ASSERT(_item != null);

            _item.Show();

            yield return new WaitForSeconds(fDelayInterval);
        }
    }

    void DoUnShow()
    {
        for (int i = 0; i < m_aryTweenAnimItems.Length; i++)
        {
            CUITweenAnimTest_Item _item = m_aryTweenAnimItems[i];
            GameCommon.ASSERT(_item != null);

            _item.UnShow();
        }
    }
}