using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CUIWorldDragMapWindow : MonoBehaviour
{
    [SerializeField]
    Camera m_3dCamera;
    [SerializeField]
    float m_fCameraFieldOfView = 50f;
    [SerializeField]
    Vector2 m_v2CameraRangeX = new Vector2(-6.63f, 6.63f);
    [SerializeField]
    Vector2 m_v2CameraRangeY = new Vector2(-3.16f, 3.16f);
    [SerializeField]
    Vector2 m_v2CameraRangeZ = new Vector2(-2.26f, 0f);
    [SerializeField]
    UITexture m_texView;

    RenderTexture m_RenderTexture;

    [SerializeField]
    UIDragSpringOriginalFrame m_stUIDragSpring;


    List<Vector2> m_lstScreenCornerPoint = new List<Vector2>();
    enum EM_MapBoundary
    {
        Null,
        Left, Right, Top, Bottom,
        TopLeft, TopRight, BottomLeft, BottomRight
    };
    EM_MapBoundary m_emMapBoundaryValue = EM_MapBoundary.Null;


    [SerializeField]
    UIButton m_btnTest;







    private void Awake()
    {
        m_3dCamera.fieldOfView = m_fCameraFieldOfView;
        m_3dCamera.transform.localPosition = Vector3.zero;
        m_3dCamera.transform.eulerAngles = Vector3.zero;
        m_3dCamera.transform.localScale = Vector3.one;

        //const int nRTWidth = 1920;
        //const int nRTHeight = 1080;
        //int nRTWidth = Screen.currentResolution.width;
        //int nRTHeight = Screen.currentResolution.height;
        int nRTWidth = Screen.width;
        int nRTHeight = Screen.height;

        UIStretch stStretch = m_texView.GetComponent<UIStretch>();
        GameCommon.ASSERT(stStretch != null);
        stStretch.style = UIStretch.Style.FillKeepingRatio;
        stStretch.runOnlyOnce = false;
        stStretch.relativeSize = new Vector2(1, 1);
        stStretch.initialSize = new Vector2(nRTWidth, nRTHeight);
        stStretch.borderPadding = new Vector2(0, 0);

        m_RenderTexture = new RenderTexture(nRTWidth, nRTHeight, 24);
        m_RenderTexture.name = "render_texture";
        m_RenderTexture.format = RenderTextureFormat.ARGB32;
        m_texView.width = nRTWidth;
        m_texView.height = nRTHeight;
        m_3dCamera.targetTexture = m_RenderTexture;
        m_3dCamera.cullingMask = (1 << LayerMask.NameToLayer("Model"));
        m_texView.mainTexture = m_RenderTexture;

        BoxCollider boxTrigger = m_texView.GetComponent<BoxCollider>();
        GameCommon.ASSERT(boxTrigger != null);
        UIEventListener.Get(boxTrigger.gameObject).onScroll += OnScroll_BoxTrigger;

        UIDragSpringOriginalFrame stUIDragSpring = m_stUIDragSpring.GetComponent<UIDragSpringOriginalFrame>();
        GameCommon.ASSERT(stUIDragSpring != null);
        stUIDragSpring.Initialized(boxTrigger, MoveAbsolute);
        stUIDragSpring.onDragFinished += OnDragFinished;

        m_lstScreenCornerPoint.Add(Vector2.zero);
        m_lstScreenCornerPoint.Add(new Vector2(0, Screen.height));
        m_lstScreenCornerPoint.Add(new Vector2(Screen.width, Screen.height));
        m_lstScreenCornerPoint.Add(new Vector2(Screen.width, 0));

        UIEventListener.Get(m_btnTest.gameObject).onClick =
        delegate (GameObject go)
        {
            //Debug.Log(string.Format("[{0},{1}]", Screen.width, Screen.height));
            Debug.LogWarning(CheckViewIsOutBoundary().ToString());
        };
    }





















    void OnScroll_BoxTrigger(GameObject go, float delta)
    {
        //Debug.Log(delta);
        const float fConstSpeed = 0.1f;

        float fZ = m_3dCamera.transform.localPosition.z;
        if (delta > 0)
        {
            fZ += fConstSpeed;
        }
        else
        {
            fZ -= fConstSpeed;
        }
        fZ = Mathf.Clamp(fZ, m_v2CameraRangeZ.x, m_v2CameraRangeZ.y);

        m_3dCamera.transform.localPosition = new Vector3(
            m_3dCamera.transform.localPosition.x,
            m_3dCamera.transform.localPosition.y,
            fZ
            );
    }

    //void MoveAbsolute(Vector3 absolute, Vector2 v2Delta)
    //{
    //    Debug.Log(GameCommon.PrintVector3(absolute) + "  ,  " + GameCommon.PrintVector3(v2Delta));

    //    Vector3 v3Before = m_3dCamera.transform.localPosition;
    //    Vector3 v3After = v3Before + new Vector3(
    //        -1 * absolute.x,
    //        -1 * absolute.y,
    //        0
    //        );
    //    float fAfterX = Mathf.Clamp(v3After.x, m_v2CameraRangeX.x, m_v2CameraRangeX.y);
    //    float fAfterY = Mathf.Clamp(v3After.y, m_v2CameraRangeY.x, m_v2CameraRangeY.y);

    //    m_3dCamera.transform.localPosition = new Vector3(
    //        fAfterX,
    //        fAfterY,
    //        m_3dCamera.transform.localPosition.z
    //        );
    //}

    void MoveAbsolute(UIDragSpringOriginalFrame.EM_MessageEvent emEvent, Vector3 absolute, Vector2 v2Delta)
    {
        //Debug.Log(GameCommon.PrintVector3(absolute) + "  ,  " + GameCommon.PrintVector3(v2Delta));

        Vector3 v3Before = m_3dCamera.transform.localPosition;
        //Vector3 v3After = v3Before + new Vector3(
        //    -1 * absolute.x,
        //    -1 * absolute.y,
        //    0
        //    );
        Debug.Log("01. v3Before: " + GameCommon.PrintVector3(v3Before));
        m_3dCamera.transform.localPosition = v3Before + new Vector3(
            -1 * absolute.x,
            -1 * absolute.y,
            0
            );

        //if (CheckViewIsOutBoundary())
        //{
        //    m_3dCamera.transform.localPosition = v3Before;

        //    m_stUIDragSpring.StopMoving();
        //    Debug.Log("~~~~~~~~~~~~~~~~~~~StopMomentum~~~~~~~~~~~~~~~~~~~");
        //}
        m_emMapBoundaryValue = CheckViewIsOutBoundary();
        //Debug.Log("02. " + m_emMapBoundaryValue.ToString());
        switch (m_emMapBoundaryValue)
        {
            case EM_MapBoundary.Left:
            case EM_MapBoundary.Right:
                {
                    m_3dCamera.transform.localPosition = new Vector3(
                        v3Before.x,
                        m_3dCamera.transform.localPosition.y,
                        m_3dCamera.transform.localPosition.z
                        );
                    //m_stUIDragSpring.StopMoving();
                }
                break;
            case EM_MapBoundary.Top:
            case EM_MapBoundary.Bottom:
                {
                    m_3dCamera.transform.localPosition = new Vector3(                        
                        m_3dCamera.transform.localPosition.x,
                        v3Before.y,
                        m_3dCamera.transform.localPosition.z
                        );
                    //m_stUIDragSpring.StopMoving();
                }
                break;
            case EM_MapBoundary.TopLeft:
            case EM_MapBoundary.TopRight:
            case EM_MapBoundary.BottomLeft:
            case EM_MapBoundary.BottomRight:
                {
                    m_3dCamera.transform.localPosition = v3Before;
                    //m_stUIDragSpring.StopMoving();
                }
                break;
        }
        Debug.Log("03. v3Set: " + GameCommon.PrintVector3(m_3dCamera.transform.localPosition));
    }

    EM_MapBoundary CheckViewIsOutBoundary()
    {
        foreach (Vector2 _v2Point in m_lstScreenCornerPoint)
        {
            Ray ray = m_3dCamera.ScreenPointToRay(_v2Point);
            RaycastHit rayHit;
            bool bIsHitBoundary = Physics.Raycast(
                ray,
                out rayHit,
                1000,
                (1 << LayerMask.NameToLayer("MapBoundary"))
                );

            if (bIsHitBoundary)
            {
                Debug.Log("rayHit.collider.gameObject: " + rayHit.collider.gameObject.name);
                switch (rayHit.collider.gameObject.name)
                {
                    case "Boundary_L": return EM_MapBoundary.Left;
                    case "Boundary_R": return EM_MapBoundary.Right;
                    case "Boundary_T": return EM_MapBoundary.Top;
                    case "Boundary_B": return EM_MapBoundary.Bottom;
                    case "Boundary_TL": return EM_MapBoundary.TopLeft;
                    case "Boundary_TR": return EM_MapBoundary.TopRight;
                    case "Boundary_BL": return EM_MapBoundary.BottomLeft;
                    case "Boundary_BR": return EM_MapBoundary.BottomRight;
                    default:
                        {
                            Debug.Log("Null: "+ rayHit.collider.gameObject.name);
                            return EM_MapBoundary.Null;
                        }
                }
            }
        }

        string strDebug = string.Format("---> {0} ", GameCommon.PrintVector3(m_3dCamera.transform.localPosition));
        Debug.LogWarning(strDebug);
        return EM_MapBoundary.Null;
    }

    void OnDragFinished()
    {

    }
}
