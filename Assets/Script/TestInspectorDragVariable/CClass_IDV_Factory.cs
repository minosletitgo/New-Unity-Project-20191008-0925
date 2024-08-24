using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class CClass_IDV_Factory : CClass_IDV_Building
{
    protected int m_nFactoryId;

    protected abstract void InstantiateCharacters();
}