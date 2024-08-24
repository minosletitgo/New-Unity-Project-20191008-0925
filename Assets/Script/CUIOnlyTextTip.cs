using UnityEngine;

public class CUIOnlyTextTip : MonoBehaviour
{
    public GameObject m_goMovingRoot;
    public UISprite m_sprWidthHeight;

    public UIGridEx m_gridData;
    public UILabel m_labTitle;
    public UILabel m_labContent;

    //以下是一些【偏移】参数
    public const int m_nConstTextOffset2WH_X = 15;
    public const int m_nConstTextOffset2WH_Y = 15;
    public const int m_nConstTextWH_Border_L = 10;
    public const int m_nConstTextWH_Border_R = 10;
    public const int m_nConstTextWH_Border_T = 10;
    public const int m_nConstTextWH_Border_B = 10;

    //以下是一些【最大宽度】(理论上应该被功能模块来持有该参数)示例:
    public const int m_nConstMaxWidth_RoleInfo = 400;





    private void Awake()
    {
        GameCommon.ASSERT(m_sprWidthHeight.pivot == UIWidget.Pivot.TopLeft);

        GameCommon.ASSERT(m_gridData.hideInactive);
        GameCommon.ASSERT(!m_gridData.mIsDirectionAdd);
        GameCommon.ASSERT(m_gridData.arrangement == UIGrid.Arrangement.Vertical);

        GameCommon.ASSERT(m_labTitle.pivot == UIWidget.Pivot.TopLeft);
        GameCommon.ASSERT(m_labTitle.overflowMethod == UILabel.Overflow.ResizeHeight);
        GameCommon.ASSERT(m_labContent.pivot == UIWidget.Pivot.TopLeft);
        GameCommon.ASSERT(m_labContent.overflowMethod == UILabel.Overflow.ResizeHeight);

        UICameraEventHelper.Instance.m_dgOnEvent += OnUICameraEvent;
    }


    public void Show(string strTitle, string strContent, int nMaxWidth, Vector2 vFixPos = new Vector2())
    {
        GameCommon.ASSERT(!string.IsNullOrEmpty(strContent));
        GameCommon.ASSERT(nMaxWidth > 0);

        gameObject.SetActive(true);

        m_labTitle.width = nMaxWidth;
        m_labTitle.text = strTitle;
        m_labTitle.gameObject.SetActive(!string.IsNullOrEmpty(strTitle));

        m_labContent.width = nMaxWidth;
        m_labContent.text = strContent;

        m_gridData.Reposition();

        Bounds bd = NGUIMath.CalculateRelativeWidgetBounds(m_gridData.transform, false);
        m_sprWidthHeight.width = (int)bd.size.x + m_nConstTextOffset2WH_X * 2;
        m_sprWidthHeight.height = (int)bd.size.y + m_nConstTextOffset2WH_Y * 2;

        CalculateTransform(vFixPos);
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
            if (IsShow())
            {
                UnShow();
            }
        }
    }
}