using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CUICircleScrollView_Item : MonoBehaviour
{
    public UILabel mLabText;


    int mIndexData = -1;
    public void SetFillData(int indexData)
    {
        mLabText.text = indexData.ToString("00");

        mIndexData = indexData;
    }

    public int GetFillData() { return mIndexData; }
}