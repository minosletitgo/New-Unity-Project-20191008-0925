using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CUIEllipse02_Grid_RotationX : MonoBehaviour
{
    public UIGrid_Ellipse mGrid;

    public GameObject m_goTestPoint;

    public UIButton mBtnRotation;





    private void Awake()
    {
        UIEventListener.Get(mBtnRotation.gameObject).onClick = OnClick_BtnRotation;
    }

    void OnClick_BtnRotation(GameObject go)
    {
        Vector3 v3Center = UICamera.mainCamera.WorldToScreenPoint(mGrid.m_goCenter.transform.position);
        Vector3 v3Center_World = UICamera.mainCamera.ScreenToWorldPoint(v3Center);

        Vector3 _v3PosZero = MathEllipseHelper.GetScreenPostion(
            mGrid.m_goCenter.transform,
            mGrid.m_fAxisA,
            mGrid.m_fAxisB,
            0,
            0,
            0,
            0
            );
        Vector3 _v3PosZeroDir = _v3PosZero - v3Center;

        Vector3 v3Before = UICamera.mainCamera.WorldToScreenPoint(m_goTestPoint.transform.position);
        Vector3 v3Before_World = UICamera.mainCamera.ScreenToWorldPoint(v3Before);
        //Vector3 v3After_World = MathEllipseHelper.V3RotateAround(v3Before_World, new Vector3(0, 0, 1), 30);
        Vector3 v3After_World = MathEllipseHelper.V3RotateAround02(v3Before_World, v3Center_World, new Vector3(0, 0, 1), 30);
        m_goTestPoint.transform.position = v3After_World;
        Vector3 v3After_Screen = UICamera.mainCamera.WorldToScreenPoint(v3After_World);
        Debug.Log("v3After_Screen = " + v3After_Screen);
    }
}