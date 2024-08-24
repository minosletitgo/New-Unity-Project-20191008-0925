using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CUITestScaleParticles : MonoBehaviour
{
    public ParticlesScaleHelper m_psHelper;
    UIPanel m_stPanel;

    // Use this for initialization
    void Start ()
    {
        m_stPanel = NGUITools.FindInParents<UIPanel>(m_psHelper.gameObject);

        if (m_coChangeRenderQueue != null)
        {
            StopCoroutine(m_coChangeRenderQueue);
        }
        m_coChangeRenderQueue = CoChangeRenderQueue();
        StartCoroutine(m_coChangeRenderQueue);
    }
	
	// Update is called once per frame
	void Update ()
    {
		
	}

    IEnumerator m_coChangeRenderQueue;
    IEnumerator CoChangeRenderQueue()
    {
        while (true)
        {
            GameCommon.SetEffectRenderQueue(m_psHelper.gameObject, m_stPanel.startingRenderQueue - 10);

            yield return new WaitForSeconds(2.5f);

            GameCommon.SetEffectRenderQueue(m_psHelper.gameObject, m_stPanel.startingRenderQueue + 10);

            yield return new WaitForSeconds(2.5f);
        }
    }
}
