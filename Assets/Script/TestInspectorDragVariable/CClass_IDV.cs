using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class CClass_IDV : MonoBehaviour
{
    protected virtual void Awake()
    {
        Debug.Log("CClass_IDV.Awake ");
    }

    public abstract void PerformAction();
}