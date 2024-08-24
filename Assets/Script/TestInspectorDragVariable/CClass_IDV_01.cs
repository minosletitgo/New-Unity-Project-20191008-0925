using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//[NonSerialized]
//[Serializable]
public class CClass_IDV_01 : CClass_IDV
{
    public int m_nValue;

    protected override void Awake()
    {
        base.Awake();
        Debug.Log("CClass_IDV_01.Awake ");
    }

    public override void PerformAction()
    {

    }
}