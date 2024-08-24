using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class TAC_SimpleMoving : MonoBehaviour
{
    //float m_fSpeed = 0.5f;
    Vector3 m_v3Target;
    [SerializeField]
    bool m_bIsUpdate = false;

    Vector3 m_v3CurrentVelocity;
    [SerializeField]
    float m_fSmoothTime = 0.5f;

    public void SetMovingTo(Vector3 v3Target)
    {
        m_v3Target = v3Target;
        m_bIsUpdate = true;
    }

    void Update()
    {
        if (m_bIsUpdate)
        {
            float fDis = Vector3.Distance(transform.localPosition, m_v3Target);
            if (fDis <= 0.001f)
            {
                m_bIsUpdate = false;
            }

            //Vector3 v3Dir = (m_v3Target - transform.localPosition).normalized;
            //transform.localPosition = transform.localPosition + (Time.deltaTime* m_fSpeed);
            transform.localPosition = Vector3.SmoothDamp(transform.localPosition, m_v3Target, ref m_v3CurrentVelocity, m_fSmoothTime);
        }
    }
}