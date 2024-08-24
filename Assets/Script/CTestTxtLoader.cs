using UnityEngine;
using System;
using System.Collections;

public class CTestTxtLoader:MonoBehaviour
{
    private void Start()
    {
        CTBLInfo_Demo.Inst.LoadIconInfo("config/tbl/Icon");
    }
}