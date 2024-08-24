using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class CClass_IDV_Building : MonoBehaviour
{
    protected int m_nBuildingType;

    protected abstract void BuildingLevUpToNext();
}