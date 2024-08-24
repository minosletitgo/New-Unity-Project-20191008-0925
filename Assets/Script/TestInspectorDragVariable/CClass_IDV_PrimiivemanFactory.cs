using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ITest
{
    int GetIncreaseId();
}

public class CClass_IDV_PrimiivemanFactory : CClass_IDV_Factory, ITest
{
    private void Start()
    {
        BuildingLevUpToNext();
        InstantiateCharacters();
    }

    protected override void BuildingLevUpToNext()
    {
        Debug.Log("CClass_IDV_PrimiivemanFactory.BuildingLevUpToNext ");
    }

    protected override void InstantiateCharacters()
    {
        Debug.Log("CClass_IDV_PrimiivemanFactory.InstantiateCharacters ");
    }

    static int m_nIncreaseId = 0;
    public int GetIncreaseId()
    {
        return ++m_nIncreaseId;
    }
}