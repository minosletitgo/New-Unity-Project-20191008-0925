using UnityEngine;
using System.Collections;

public class CROT_Camera : MonoBehaviour
{
    enum EM_CameraMoveTo
    {
        Invalid = -1,

        ToCenter,
        ToUp,
        ToDown,
        ToLeft,
        ToRight,

        Max,
    };
    EM_CameraMoveTo m_emCameraMoveTo = EM_CameraMoveTo.Invalid;

    public UIButton m_btnMoveCameraToZero;
    public UIButton m_btnMoveCameraToUp;
    public UIButton m_btnMoveCameraToDown;
    public UIButton m_btnMoveCameraToLeft;
    public UIButton m_btnMoveCameraToRight;



    private void Awake()
    {
        UIEventListener.Get(m_btnMoveCameraToZero.gameObject).parameter = EM_CameraMoveTo.ToCenter;
        UIEventListener.Get(m_btnMoveCameraToZero.gameObject).onPress = OnPress_BtnMoveCamera;
        UIEventListener.Get(m_btnMoveCameraToUp.gameObject).parameter = EM_CameraMoveTo.ToUp;
        UIEventListener.Get(m_btnMoveCameraToUp.gameObject).onPress = OnPress_BtnMoveCamera;
        UIEventListener.Get(m_btnMoveCameraToDown.gameObject).parameter = EM_CameraMoveTo.ToDown;
        UIEventListener.Get(m_btnMoveCameraToDown.gameObject).onPress = OnPress_BtnMoveCamera;
        UIEventListener.Get(m_btnMoveCameraToLeft.gameObject).parameter = EM_CameraMoveTo.ToLeft;
        UIEventListener.Get(m_btnMoveCameraToLeft.gameObject).onPress = OnPress_BtnMoveCamera;
        UIEventListener.Get(m_btnMoveCameraToRight.gameObject).parameter = EM_CameraMoveTo.ToRight;
        UIEventListener.Get(m_btnMoveCameraToRight.gameObject).onPress = OnPress_BtnMoveCamera;
    }

    void OnPress_BtnMoveCamera(GameObject go, bool bIsPress)
    {
        if (bIsPress)
        {
            m_emCameraMoveTo = (EM_CameraMoveTo)UIEventListener.Get(go).parameter;
        }
        else
        {
            m_emCameraMoveTo = EM_CameraMoveTo.Invalid;
        }
    }

    void LateUpdate()
    {
        if (m_emCameraMoveTo > EM_CameraMoveTo.Invalid)
        {
            const float fMoveStep = 9.55f;

            switch (m_emCameraMoveTo)
            {
                case EM_CameraMoveTo.ToCenter:
                    {
                        transform.transform.localPosition = Vector3.zero;
                    }
                    break;
                case EM_CameraMoveTo.ToUp:
                    {
                        transform.transform.localPosition += new Vector3(0, Time.deltaTime * fMoveStep, 0);
                    }
                    break;
                case EM_CameraMoveTo.ToDown:
                    {
                        transform.transform.localPosition += new Vector3(0, -Time.deltaTime * fMoveStep, 0);
                    }
                    break;
                case EM_CameraMoveTo.ToLeft:
                    {
                        transform.transform.localPosition += new Vector3(-Time.deltaTime * fMoveStep, 0, 0);
                    }
                    break;
                case EM_CameraMoveTo.ToRight:
                    {
                        transform.transform.localPosition += new Vector3(Time.deltaTime * fMoveStep, 0, 0);
                    }
                    break;
            }
        }
    }

}