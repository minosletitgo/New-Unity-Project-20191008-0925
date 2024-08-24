using UnityEngine;
using System.Collections;

public class CROT_Main : MonoBehaviour
{
    static CROT_Main m_inst;    
    Camera m_CameraMain;
    
    CROT_RoleObject m_stMyRole;
    CROT_UIHeadBar m_stUIHeadBar;




    private void Awake()
    {
        m_inst = this;
        DontDestroyOnLoad(m_inst);

        m_CameraMain = GameObject.Find("Main Camera").GetComponent<Camera>();
        GameCommon.ASSERT(m_CameraMain != null);

        GameObject goMyRole = GameObject.Find("MyRole");
        GameCommon.ASSERT(goMyRole != null);
        m_stMyRole = new CROT_RoleObject(goMyRole);
        GameCommon.ASSERT(m_stMyRole != null);

        GameObject goUIHeadBar = GameObject.Find("UIHeadBar");
        GameCommon.ASSERT(goUIHeadBar != null);
        m_stUIHeadBar = goUIHeadBar.GetComponent<CROT_UIHeadBar>();
        GameCommon.ASSERT(m_stUIHeadBar != null);
    }

    public static CROT_Main GetInst() { return m_inst; }

    public Camera GetCameraMain() { return m_CameraMain; }


    void LateUpdate()
    {
        //Update.RoleObject
        m_stMyRole.Update();
    }
}