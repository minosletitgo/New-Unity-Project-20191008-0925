using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CUIMainChatItem : MonoBehaviour
{
    public UISprite mSpWidthHeight;
    public UILabel mLabContent;

    private void Awake()
    {

    }

    public void SetFillData(string strContent)
    {
        mLabContent.text = strContent;

        //Vector2 v2Size = NGUIText.CalculatePrintedSize(strContent);
        //Vector2 v2Size = mLabContent.localSize;
        Vector2 v2Size = mLabContent.printedSize;

        v2Size.y += 31;

        mSpWidthHeight.height = (int)v2Size.y;
    }
}