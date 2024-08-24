using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class CUITAC_Console : MonoBehaviour
{
    public UIButton m_btnQuitGame;
    public UIButton m_btnAddPoint_1;

    public UIInput m_itMoreNumber;
    public UIButton m_btnAddPoint_More;

    void Awake()
    {
        UIEventListener.Get(m_btnQuitGame.gameObject).onClick = delegate (GameObject go)
        {
            Application.Quit();
        };

        UIEventListener.Get(m_btnAddPoint_1.gameObject).onClick = delegate (GameObject go)
        {
            TAC_Main.Inst.AddPoint(1);
        };


        m_itMoreNumber.validation = UIInput.Validation.Integer;
        m_itMoreNumber.value = "5";
        UIEventListener.Get(m_btnAddPoint_More.gameObject).onClick = delegate (GameObject go)
        {
            int nNumber = int.Parse(m_itMoreNumber.value);
            if (nNumber > 0)
            {
                TAC_Main.Inst.AddPoint(nNumber);
            }
        };
    }
}