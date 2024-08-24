using UnityEngine;
using System.Collections;

public class CROT_RoleObject
{
    GameObject m_goRole;

    public CROT_RoleObject(GameObject go)
    {
        GameCommon.ASSERT(go != null);
        m_goRole = go;
    }

    public Transform GetTransform() { return m_goRole.transform; }

    public void Update()
    {
        //RoleObject 驱动 UIHeadBar

        GameObject goUIHeadBar = GameObject.Find("UIHeadBar");
        GameCommon.ASSERT(goUIHeadBar != null);
        CROT_UIHeadBar stUIHeadBar = goUIHeadBar.GetComponent<CROT_UIHeadBar>();
        GameCommon.ASSERT(stUIHeadBar != null);

        Vector3 v3Pos = CROT_Main.GetInst().GetCameraMain().WorldToScreenPoint(GetTransform().position + Vector3.up * 3.99f);
        stUIHeadBar.UpdateByOuter(v3Pos);
    }
}