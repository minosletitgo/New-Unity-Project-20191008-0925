using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CUIDebugConsoleWindow : MonoBehaviour
{
    [SerializeField]
    TweenPosition mTwOpen;
    [SerializeField]
    UIInput mUIInput_InputCommand;
    [SerializeField]
    public UIButton mUIButton_BtnConfirm;
    [SerializeField]
    public UIButton mUIButton_BtnUp;
    [SerializeField]
    public UIButton mUIButton_BtnDown;
    
    
    List<string> mLstInputValueCache = new List<string>();
    int mIndexCurSelect = -1;




    private void Awake()
    {
        mUIInput_InputCommand.onReturnKey = UIInput.OnReturnKey.Submit;
        EventDelegate.Add(mUIInput_InputCommand.onSubmit, OnInputCommandSubmit);

        UIEventListener.Get(mUIButton_BtnConfirm.gameObject).onClick = OnClick_BtnConfirm;
        UIEventListener.Get(mUIButton_BtnUp.gameObject).onClick = OnClick_BtnUp;
        UIEventListener.Get(mUIButton_BtnDown.gameObject).onClick = OnClick_BtnDown;
        
        GameCommon.ASSERT(mTwOpen != null);
    }
    
    public void OpenWindow()
    {
        gameObject.SetActive(true);

        mTwOpen.ResetToBeginning();
        mTwOpen.PlayForward();

        UICamera.selectedObject = mUIInput_InputCommand.gameObject;
    }

    public void CloseWindow()
    {
        gameObject.SetActive(false);
    }

    public bool IsWindowOpened() { return gameObject.activeSelf; }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            GoToLastInputValue();
        }
        else if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            GoToNextInputValue();
        }
    }

    void GoToLastInputValue()
    {
        if (this.IsWindowOpened() &&
            mLstInputValueCache.Count > 0 &&
            UICamera.selectedObject == mUIInput_InputCommand.gameObject
            )
        {
            if (mIndexCurSelect == -1)
            {
                mIndexCurSelect = mLstInputValueCache.Count - 1;
            }
            else if (mIndexCurSelect >= 1)
            {
                mIndexCurSelect--;
            }
            mUIInput_InputCommand.value = mLstInputValueCache[mIndexCurSelect];
        }
    }

    void GoToNextInputValue()
    {
        if (this.IsWindowOpened() &&
            mLstInputValueCache.Count > 0 &&
            UICamera.selectedObject == mUIInput_InputCommand.gameObject
            )
        {
            if (mIndexCurSelect == -1)
            {
                mIndexCurSelect = mLstInputValueCache.Count - 1;
            }
            else if (mIndexCurSelect < (mLstInputValueCache.Count - 1))
            {
                mIndexCurSelect++;
            }
            mUIInput_InputCommand.value = mLstInputValueCache[mIndexCurSelect];
        }
    }

    void OnClick_BtnConfirm(GameObject go)
    {
        OnInputCommandSubmit();
        CloseWindow();
    }

    void OnClick_BtnUp(GameObject go)
    {
        UICamera.selectedObject = mUIInput_InputCommand.gameObject;
        GoToLastInputValue();
    }

    void OnClick_BtnDown(GameObject go)
    {
        UICamera.selectedObject = mUIInput_InputCommand.gameObject;
        GoToNextInputValue();
    }

    void OnInputCommandSubmit()
    {
        string strInput = mUIInput_InputCommand.value;
        if (!ProcGMCMD(strInput))
        {
            if (!string.IsNullOrEmpty(strInput))
            {
                Debug.LogWarning("Invaild InputCommand String !!!");
            }
        }

        if (!string.IsNullOrEmpty(strInput))
        {
            if (mLstInputValueCache.Count >= 10)
            {
                mLstInputValueCache.RemoveAt(0);
            }
            mLstInputValueCache.Add(strInput);
        }

        mUIInput_InputCommand.value = null;
        mIndexCurSelect = -1;
    }

    public bool ProcGMCMD(string strInput)
    {
        if (strInput.Length <= 0)
        {
            return false;
        }

        char[] charSeparators = new char[] { ' ' };
        string[] strSplits = strInput.Split(charSeparators, System.StringSplitOptions.RemoveEmptyEntries);

        if (strSplits.Length <= 0)
        {
            return false;
        }

        if (strSplits[0].StartsWith("/"))
        {
            if (!ProcGMCMD_Client(strSplits, strInput))
            {
                //GameCommon.ShowMsgBoxWindow(
                //    "ProcGMCMD_Client",
                //    "ProcGMCMD_Client.Failed:\n" + strInput,
                //    MsgBoxWindow.EM_FormFlag.BtnOK
                //    );
            }
        }
        else
        {
            //GameCommon.ShowMsgBoxWindow(
            //    "ProcGMCMD",
            //    "ProcGMCMD.Failed:\n" + strInput,
            //    MsgBoxWindow.EM_FormFlag.BtnOK
            //    );
        }

        return true;
    }

    bool ProcGMCMD_Client(string[] strSplits, string strInput)
    {
        try
        {
            string strCMD = strSplits[0].Substring(1);
            switch (strCMD)
            {
                case "001":
                    {
                        /* 
                            /001 15
                        */

                        if (strSplits.Length == 2)
                        {
                            float fDis = float.Parse(strSplits[1]);
                            bool bIsNew = false;
                            var stWindow = CSimpleUIManage.Instance.GetUI<CUIWorldDragMapWindow02>(CSimpleUIManage.EM_UIName.WorldDragMapWindow02, out bIsNew);
                            stWindow.m_fMultiTouchMinDis = fDis;
                        }
                    }
                    break;
                case "002":
                    {
                        /* 
                            /002 2
                        */

                        //if (strSplits.Length == 2)
                        //{
                        //    float fTimeInterval = float.Parse(strSplits[1]);
                        //    bool bIsNew = false;
                        //    var stWindow = CSimpleUIManage.Instance.GetUI<CUIWorldDragMapWindow02>(CSimpleUIManage.EM_UIName.WorldDragMapWindow02, out bIsNew);
                        //    stWindow.m_fMultiTouchMinTimeInerval = fTimeInterval;
                        //}
                    }
                    break;
                default:
                    {
                        //无效Code
                        return false;
                    }
                    //break;
            }
        }
        catch (Exception e)
        {
            Debug.LogWarning("DebugConsoleWindow.ProcGMCMD.Exception -> " + e);
            //LOG.logWarn("DebugConsoleWindow.ProcGMCMD.Exception -> " + e);
        }

        return true;
    }
}