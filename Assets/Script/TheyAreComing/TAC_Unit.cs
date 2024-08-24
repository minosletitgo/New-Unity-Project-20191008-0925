using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class TAC_Unit : MonoBehaviour
{
    [SerializeField]
    int m_nBornIdx = 0;

    Rigidbody m_pRigidbody;
    TAC_SimpleMoving m_pSimpleMoving;

    void Awake()
    {
        m_pRigidbody = gameObject.GetComponent<Rigidbody>();
        //TCK.CHECK(m_pRigidbody != null);

        m_pSimpleMoving = gameObject.AddMissingComponent<TAC_SimpleMoving>();
        TCK.CHECK(m_pSimpleMoving != null);
    }

    public void SetBornIdx(int nIdx) { m_nBornIdx = nIdx; }
    public int GetBornIdx() { return m_nBornIdx; }

    public Rigidbody GetRigidbody() { return m_pRigidbody; }
    public TAC_SimpleMoving GetSimpleMoving() { return m_pSimpleMoving; }

    void FixedUpdate()
    {
        transform.forward = Vector3.forward;
        transform.localPosition = new Vector3(transform.localPosition.x, 0, transform.localPosition.z);

        if (m_nBornIdx == TAC_Main.GetCurAllocIdx())
        {
            transform.localPosition = new Vector3(
                TAC_Main.Inst.m_pCircleConfig.GetCircleZero().x,
                0,
                TAC_Main.Inst.m_pCircleConfig.GetCircleZero().y
                );
        }
    }
}