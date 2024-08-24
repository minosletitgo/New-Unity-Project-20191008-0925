using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CUITestSomeCode02 : MonoBehaviour
{
    public UIPanel m_panel;
    public UITexture m_texBG;
    public GameObject m_goTarget;

	// Use this for initialization
	void Start ()
    {
        Test();
    }
	
	// Update is called once per frame
	void Update ()
    {
        Test();
    }

    [ContextMenu("Execute")]
    void Test()
    {
        //m_panel.baseClipRegion = new Vector4(
        //    m_panel.baseClipRegion.x,
        //    m_panel.baseClipRegion.y,
        //    (float)(NGUITools.screenSize.x),
        //    m_panel.baseClipRegion.w
        //    );

        float fPosX = -0.375f * (float)(m_texBG.width) + 0.02f * (float)(m_texBG.width);
        float fPosY = -1 * (float)m_texBG.height / 2.0f + 0.259f * (float)m_texBG.height + (0.0879f / 2.0f) * (float)m_texBG.height;

        m_goTarget.transform.localPosition = new Vector3(
            fPosX,
            fPosY,
            0
            );
    }
}
