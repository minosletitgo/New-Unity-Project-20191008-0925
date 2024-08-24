using UnityEngine;

public class CUITestOnlyTextTip : MonoBehaviour
{
    public CUIOnlyTextTipEx m_goTipEx;

    public UIInput m_itTipTitle;
    public UIInput m_itTipContent;
    public UIInput m_itTipMinWidth;
    public UIInput m_itTipMaxWidth;

    public UIButton m_btnClick;
    public UIButton m_btnClick_TL;
    public UIButton m_btnClick_TR;
    public UIButton m_btnClick_BL;
    public UIButton m_btnClick_BR;


    private void Awake()
    {
        m_itTipMaxWidth.validation = UIInput.Validation.Integer;
        m_goTipEx.UnShow();

        UIEventListener.Get(m_btnClick.gameObject).onClick = OnClick_BtnClick;
        UIEventListener.Get(m_btnClick_TL.gameObject).onClick = OnClick_BtnClick_TL;
        UIEventListener.Get(m_btnClick_TR.gameObject).onClick = OnClick_BtnClick_TR;
        UIEventListener.Get(m_btnClick_BL.gameObject).onClick = OnClick_BtnClick_BL;
        UIEventListener.Get(m_btnClick_BR.gameObject).onClick = OnClick_BtnClick_BR;
    }

    void OnClick_BtnClick(GameObject go)
    {
        int nMinWidth = int.Parse(m_itTipMinWidth.value);
        int nMaxWidth = int.Parse(m_itTipMaxWidth.value);        

        m_goTipEx.Show(
            m_itTipTitle.value,
            m_itTipContent.value,
            nMinWidth,
            nMaxWidth,
            CUIOnlyTextTipEx.EM_PosPivot.TopLeft
            );
    }

    void OnClick_BtnClick_TL(GameObject go)
    {
        int nMinWidth = int.Parse(m_itTipMinWidth.value);
        int nMaxWidth = int.Parse(m_itTipMaxWidth.value);

        Transform trPos = go.transform.Find("Sprite Point");
        GameCommon.ASSERT(trPos != null);
        Vector2 v2Pos = UICamera.mainCamera.WorldToScreenPoint(trPos.position);

        m_goTipEx.Show(
            m_itTipTitle.value,
            m_itTipContent.value,
            nMinWidth,
            nMaxWidth,
            CUIOnlyTextTipEx.EM_PosPivot.TopLeft,
            v2Pos
            );
    }

    void OnClick_BtnClick_TR(GameObject go)
    {
        int nMinWidth = int.Parse(m_itTipMinWidth.value);
        int nMaxWidth = int.Parse(m_itTipMaxWidth.value);

        Transform trPos = go.transform.Find("Sprite Point");
        GameCommon.ASSERT(trPos != null);
        Vector2 v2Pos = UICamera.mainCamera.WorldToScreenPoint(trPos.position);

        m_goTipEx.Show(
            m_itTipTitle.value,
            m_itTipContent.value,
            nMinWidth,
            nMaxWidth,
            CUIOnlyTextTipEx.EM_PosPivot.TopRight,
            v2Pos
            );
    }

    void OnClick_BtnClick_BL(GameObject go)
    {
        int nMinWidth = int.Parse(m_itTipMinWidth.value);
        int nMaxWidth = int.Parse(m_itTipMaxWidth.value);

        Transform trPos = go.transform.Find("Sprite Point");
        GameCommon.ASSERT(trPos != null);
        Vector2 v2Pos = UICamera.mainCamera.WorldToScreenPoint(trPos.position);

        m_goTipEx.Show(
            m_itTipTitle.value,
            m_itTipContent.value,
            nMinWidth,
            nMaxWidth,
            CUIOnlyTextTipEx.EM_PosPivot.BottomLeft,
            v2Pos
            );
    }

    void OnClick_BtnClick_BR(GameObject go)
    {
        int nMinWidth = int.Parse(m_itTipMinWidth.value);
        int nMaxWidth = int.Parse(m_itTipMaxWidth.value);

        Transform trPos = go.transform.Find("Sprite Point");
        GameCommon.ASSERT(trPos != null);
        Vector2 v2Pos = UICamera.mainCamera.WorldToScreenPoint(trPos.position);

        m_goTipEx.Show(
            m_itTipTitle.value,
            m_itTipContent.value,
            nMinWidth,
            nMaxWidth,
            CUIOnlyTextTipEx.EM_PosPivot.BottomRight,
            v2Pos
            );
    }
}