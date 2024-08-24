using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public enum EM_TranslationLanguage
{
    auto,   //自动检测
    zh,     //中文
    en,     //英语
    yue,    //粤语
    wyw,    //文言文
    jp,     //日语
    kor,    //韩语
    fra,    //法语
    spa,    //西班牙语
    th,     //泰语
    ara,    //阿拉伯语
    ru,     //俄语
    pt,     //葡萄牙语
    de,     //德语
    it,     //意大利语
    el,     //希腊语
    nl,     //荷兰语
    pl,     //波兰语
    bul,    //保加利亚语
    est,    //爱沙尼亚语
    dan,    //丹麦语
    fin,    //芬兰语
    cs,     //捷克语
    rom,    //罗马尼亚语
    slo,    //斯洛文尼亚语
    swe,    //瑞典语
    hu,     //匈牙利语
    cht,    //繁体中文
    vie,    //越南语
}




public class CUIFanyiTool : MonoBehaviour
{
    public UIInput mItEnterWordsBefore;
    public UIInput mItEnterWordsAfter;
    public UIButton mBtnReqTranslate;


    public UIButton mBtnError_NoAssert;
    public UIButton mBtnError_Exception;
    public UIButton mBtnError_NoException;


    private void Awake()
    {
        UIEventListener.Get(mBtnReqTranslate.gameObject).onClick = OnClick_BtnReqTranslate;

        UIEventListener.Get(mBtnError_NoAssert.gameObject).onClick = OnClick_BtnError_NoAssert;
        UIEventListener.Get(mBtnError_Exception.gameObject).onClick = OnClick_BtnError_Exception;
        UIEventListener.Get(mBtnError_NoException.gameObject).onClick = OnClick_BtnError_NoException;
    }

    void OnClick_BtnReqTranslate(GameObject go)
    {
        string strBefore = mItEnterWordsBefore.value;
        if (string.IsNullOrEmpty(strBefore))
        {
            return;
        }

        EM_TranslationLanguage emFrom = EM_TranslationLanguage.en;
        EM_TranslationLanguage emTo = EM_TranslationLanguage.zh;

        Debug.Log("ReqTranslate: " + emFrom.ToString() + " -> " + emTo.ToString());
        bool bIsSucc = false;
        mItEnterWordsAfter.value = BaiduFanYiHelper.Instance.DoTranslationFromBaiduFanyi(
            strBefore,
            emFrom,
            emTo,
            out bIsSucc
            );
    }


    #region -------------------------------------TestButton---------------------------------------------

    void OnClick_BtnError_NoAssert(GameObject go)
    {
        Debug.Log("Debug.Log : Error_NoAssert");
        Debug.LogWarning("Debug.LogWarning : Error_NoAssert");
        Debug.LogError("Debug.LogError : Error_NoAssert");

        ST_Test[] arrayTest = new ST_Test[10];
        arrayTest[2].m_nValue = 11;

        Debug.Log("Debug.Log : Error_NoAssert End Line");
    }

    void OnClick_BtnError_Exception(GameObject go)
    {
        Debug.Log("Debug.Log : Error_Exception");
        Debug.LogWarning("Debug.LogWarning : Error_Exception");
        Debug.LogError("Debug.LogError : Error_Exception");

        ST_Test[] arrayTest = new ST_Test[10];
        GameCommon.ASSERT(arrayTest[2] != null);
        arrayTest[2].m_nValue = 11;

        Debug.Log("Debug.Log : Error_Exception End Line");
    }

    void OnClick_BtnError_NoException(GameObject go)
    {
        Debug.Log("Debug.Log : BtnError_NoException");
        Debug.LogWarning("Debug.LogWarning : BtnError_NoException");
        Debug.LogError("Debug.LogError : BtnError_NoException");

        ST_Test stTest = new ST_Test();
        stTest.m_nValue = 1;
        stTest = null;
        GameCommon.ASSERT_NoException(stTest != null, "OnClick_BtnDebug02");
        stTest.m_nValue = 2;
        Debug.Log("Debug.Log : Error_NoException End Line");
    }

    public class ST_Test
    {
        public int m_nValue = 0;
    };

    #endregion -------------------------------------TestButton---------------------------------------------
}
