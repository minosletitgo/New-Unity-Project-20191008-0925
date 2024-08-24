using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CUITestCoordinate2D : MonoBehaviour
{
    public UIInput m_itTargetScreenX;
    public UIInput m_itTargetScreenY;
    public UIButton m_btnRebuild;

    public UISprite m_sprZeroCenter;
    public UISprite m_sprLine;



    private void Awake()
    {
        m_itTargetScreenX.validation = UIInput.Validation.Integer;
        m_itTargetScreenX.value = Screen.width.ToString();
        m_itTargetScreenY.validation = UIInput.Validation.Integer;
        m_itTargetScreenY.value = Screen.height.ToString();

        UIEventListener.Get(m_btnRebuild.gameObject).onClick = OnClick_BtnRebuild;
    }

    void OnClick_BtnRebuild(GameObject go)
    {
        int nTargetScreenX = 0;
        if (!int.TryParse(m_itTargetScreenX.value, out nTargetScreenX))
        {
            return;
        }
        int nTargetScreenY = 0;
        if (!int.TryParse(m_itTargetScreenY.value, out nTargetScreenY))
        {
            return;
        }

        Vector2 v2ScreenPos_ZeroCenter = UICamera.mainCamera.WorldToScreenPoint(m_sprZeroCenter.transform.position);
        Vector2 v2ScreenPos_Target = new Vector2(nTargetScreenX, nTargetScreenY);
        Vector2 vLineDirection = v2ScreenPos_Target - v2ScreenPos_ZeroCenter;
        float fDegree = Vector2.SignedAngle(Vector2.right, vLineDirection);

        m_sprLine.transform.localRotation = Quaternion.Euler(0, 0, fDegree);
        m_sprLine.width = (int)Vector2.Distance(v2ScreenPos_ZeroCenter, v2ScreenPos_Target);

    }



}