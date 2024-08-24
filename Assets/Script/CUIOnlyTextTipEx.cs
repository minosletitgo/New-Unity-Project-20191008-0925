using UnityEngine;

public class CUIOnlyTextTipEx : MonoBehaviour
{
    public GameObject m_goMovingRoot;
    public UISprite m_sprWidthHeight;

    public UIGridEx m_gridData;
    public UILabel m_labTitle;
    public UILabel m_labContent;

    public enum EM_PosPivot
    {
        TopLeft,
        TopRight,
        BottomLeft,
        BottomRight,
    };

    //以下是一些【偏移】参数
    public const int m_nConstTextOffset2WH_X = 15;
    public const int m_nConstTextOffset2WH_Y = 15;



    private void Awake()
    {
        GameCommon.ASSERT(m_sprWidthHeight.pivot == UIWidget.Pivot.TopLeft);

        GameCommon.ASSERT(m_gridData.hideInactive);
        GameCommon.ASSERT(!m_gridData.mIsDirectionAdd);
        GameCommon.ASSERT(m_gridData.arrangement == UIGrid.Arrangement.Vertical);

        GameCommon.ASSERT(m_labTitle.pivot == UIWidget.Pivot.TopLeft);
        GameCommon.ASSERT(m_labTitle.overflowMethod == UILabel.Overflow.ResizeHeight);
        GameCommon.ASSERT(m_labTitle.overflowMethod == UILabel.Overflow.ResizeHeight);

        GameCommon.ASSERT(m_labContent.pivot == UIWidget.Pivot.TopLeft);
        GameCommon.ASSERT(m_labContent.overflowMethod == UILabel.Overflow.ResizeHeight);
        GameCommon.ASSERT(m_labContent.overflowMethod == UILabel.Overflow.ResizeHeight);

        UICameraEventHelper.Instance.m_dgOnEvent += OnUICameraEvent;
    }



    public void Show(
        string strTitle,
        string strContent,
        int nMinWidth,
        int nMaxWidth,
        EM_PosPivot emPivot = EM_PosPivot.TopLeft,
        Vector2 vFixPos = new Vector2()
        )
    {
        GameCommon.ASSERT(!string.IsNullOrEmpty(strContent));
        GameCommon.ASSERT(nMaxWidth > 0);

        gameObject.SetActive(true);

        m_labTitle.width = nMaxWidth;
        m_labTitle.text = strTitle;
        m_labTitle.width = Mathf.Min((int)m_labTitle.printedSize.x, nMaxWidth);
        m_labTitle.gameObject.SetActive(!string.IsNullOrEmpty(strTitle));

        m_labContent.width = nMaxWidth;
        m_labContent.text = strContent;
        m_labContent.width = Mathf.Min((int)m_labContent.printedSize.x, nMaxWidth);
        m_labTitle.gameObject.SetActive(!string.IsNullOrEmpty(strTitle));

        m_gridData.Reposition();

        Bounds bd = NGUIMath.CalculateRelativeWidgetBounds(m_gridData.transform, false);
        m_sprWidthHeight.width = (int)bd.size.x + m_nConstTextOffset2WH_X * 2;
        m_sprWidthHeight.height = (int)bd.size.y + m_nConstTextOffset2WH_Y * 2;

        Vector2 vPosReal;
        if (vFixPos == Vector2.zero)
        {
            vPosReal = UICamera.lastEventPosition;
        }
        else
        {
            vPosReal = vFixPos;
        }

        //推导出【左上锚点】
        switch (emPivot)
        {
            case EM_PosPivot.TopLeft:
                {
                    //Nothing
                }
                break;
            case EM_PosPivot.TopRight:
                {
                    int nRealWidth = m_sprWidthHeight.width;
                    nRealWidth = (int)((float)nRealWidth * GameCommon.CalcScreenPointRatio(this.transform));
                    vPosReal.x = vPosReal.x - nRealWidth;
                }
                break;
            case EM_PosPivot.BottomLeft:
                {
                    int nRealHeight = m_sprWidthHeight.height;
                    nRealHeight = (int)((float)nRealHeight * GameCommon.CalcScreenPointRatio(this.transform));
                    vPosReal.y = vPosReal.y + nRealHeight;
                }
                break;
            case EM_PosPivot.BottomRight:
                {
                    int nRealWidth = m_sprWidthHeight.width;
                    nRealWidth = (int)((float)nRealWidth * GameCommon.CalcScreenPointRatio(this.transform));
                    vPosReal.x = vPosReal.x - nRealWidth;

                    int nRealHeight = m_sprWidthHeight.height;
                    nRealHeight = (int)((float)nRealHeight * GameCommon.CalcScreenPointRatio(this.transform));
                    vPosReal.y = vPosReal.y + nRealHeight;
                }
                break;
        }
        
        CalculateTransform(vPosReal);
    }

    public void UnShow()
    {
        gameObject.SetActive(false);
    }

    public bool IsShow()
    {
        return gameObject.activeSelf;
    }

    void CalculateTransform(Vector2 vFixPos = new Vector2())
    {
        Vector3 mPos = Vector3.zero;

        if (vFixPos == Vector2.zero)
        {
            // Since the screen can be of different than expected size, we want to convert
            // mouse coordinates to view space, then convert that to world position.
            mPos = UICamera.lastEventPosition;
            Debug.Log("UICamera.lastEventPosition = " + UICamera.lastEventPosition);
        }
        else
        {
            mPos = vFixPos;
        }

        mPos.x = Mathf.Clamp01(mPos.x / Screen.width);
        mPos.y = Mathf.Clamp01(mPos.y / Screen.height);

        Vector3 mSize = new Vector3(m_sprWidthHeight.width, m_sprWidthHeight.height);

        // Calculate the ratio of the camera's target orthographic size to current screen size
        float activeSize = UICamera.currentCamera.orthographicSize / m_goMovingRoot.transform.parent.lossyScale.y;
        float ratio = (Screen.height * 0.5f) / activeSize;

        // Calculate the maximum on-screen size of the tooltip window
        Vector2 max = new Vector2(ratio * mSize.x / Screen.width, ratio * mSize.y / Screen.height);

        // Limit the tooltip to always be visible
        mPos.x = Mathf.Min(mPos.x, 1f - max.x);
        mPos.y = Mathf.Max(mPos.y, max.y);

        m_goMovingRoot.transform.position = UICamera.currentCamera.ViewportToWorldPoint(mPos);
    }

    void OnUICameraEvent(GameObject go, UICameraEventHelper.EM_Event emEvent, object obj)
    {
        if (emEvent == UICameraEventHelper.EM_Event.OnPress &&
            obj != null && (bool)(obj) == true
            )
        {
            if (!IsShow())
            {
                return;
            }

            if (go == null)
            {
                UnShow();
            }
            else if (go != m_sprWidthHeight.gameObject)
            {
                UnShow();
            }
        }
    }
}