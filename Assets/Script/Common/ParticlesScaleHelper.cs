using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ParticlesScaleHelper : MonoBehaviour
{
    //default scale size;
    public float m_fScaleSize = 1.0f;
    private List<float> m_lstInitialSizes = new List<float>();
    public int m_nRenderQueue = 3000;


    void Awake()
    {
        // Save off all the initial scale values at start.
        ParticleSystem[] aryParticles = gameObject.GetComponentsInChildren<ParticleSystem>();
        foreach (ParticleSystem particle in aryParticles)
        {
            m_lstInitialSizes.Add(particle.main.startSizeMultiplier);
            ParticleSystemRenderer renderer = particle.GetComponent<ParticleSystemRenderer>();
            if (renderer)
            {
                m_lstInitialSizes.Add(renderer.lengthScale);
                m_lstInitialSizes.Add(renderer.velocityScale);
            }
        }
    }

    void Start()
    {
        CalcToScale();
        CalcToRenderQueue();
    }

    [ContextMenu("Execute_Scale")]
    void CalcToScale()
    {
        Vector3 vLocalScale = new Vector3(m_fScaleSize, m_fScaleSize, m_fScaleSize);

        // Scale all the particle components based on parent.
        int arrayIndex = 0;
        ParticleSystem[] aryParticles = gameObject.GetComponentsInChildren<ParticleSystem>();
        foreach (ParticleSystem particle in aryParticles)
        {
            //particle.startSize = initialSizes[arrayIndex++] * gameObject.transform.localScale.magnitude;
            ParticleSystem.MainModule main = particle.main;
            main.startSizeMultiplier = m_lstInitialSizes[arrayIndex++] * /*vLocalScale.magnitude*/m_fScaleSize;
            ParticleSystemRenderer renderer = particle.GetComponent<ParticleSystemRenderer>();
            if (renderer)
            {
                renderer.lengthScale = m_lstInitialSizes[arrayIndex++] * /*vLocalScale.magnitude*/m_fScaleSize;
                renderer.velocityScale = m_lstInitialSizes[arrayIndex++] * /*vLocalScale.magnitude*/m_fScaleSize;
            }
        }
    }

    [ContextMenu("Execute_RenderQueue")]
    void CalcToRenderQueue()
    {
        GameCommon.SetEffectRenderQueue(gameObject, m_nRenderQueue);

        Renderer[] aryRender = gameObject.GetComponentsInChildren<Renderer>(true);
        if (aryRender != null)
        {
            foreach (var r in aryRender)
            {
                if (r != null && r.sharedMaterial != null)
                {
                    r.sharedMaterial.renderQueue = m_nRenderQueue;
                }
            }
        }
    }
}