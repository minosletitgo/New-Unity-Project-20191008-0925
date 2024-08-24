using UnityEngine;
using System.Collections;

public class CROT_UIHeadBar : MonoBehaviour
{
    Transform m_trRoot;
    CROT_RoleObject m_stRoleObject;

    private void Awake()
    {
        m_trRoot = gameObject.transform;
    }
    
    public void InitRoleObject(CROT_RoleObject stRoleObject)
    {
        GameCommon.ASSERT(stRoleObject != null);

        m_stRoleObject = stRoleObject;
    }

    public void UpdateByOuter(Vector2 v2ScreenPos)
    {
        if (UICamera.currentCamera != null)
        {
            v2ScreenPos.x = Mathf.Clamp01(v2ScreenPos.x / Screen.width);
            v2ScreenPos.y = Mathf.Clamp01(v2ScreenPos.y / Screen.height);
            m_trRoot.position = UICamera.currentCamera.ViewportToWorldPoint(v2ScreenPos);
        }
    }
}