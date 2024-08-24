using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CClass_IDV_02 : CClass_IDV
{
    public int m_nValue;

    protected override void Awake()
    {
        base.Awake();
        Debug.Log("CClass_IDV_02.Awake ");
    }

    public override void PerformAction()
    {

    }
}