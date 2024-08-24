using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class TAC_Main : MonoBehaviour
{
    static TAC_Main m_inst = null;
    public static TAC_Main Inst
    {
        get
        {
            if (m_inst == null)
            {
                m_inst = GameCommon.GoFind<TAC_Main>();
            }
            return m_inst;
        }
    }

    ////////////////////////////////////////////////////

    public TAC_CircleConfig m_pCircleConfig;
    public Transform m_trPlatform;
    public TAC_Unit m_pUnitInst;
    List<TAC_Unit> m_lstRunningUnit = new List<TAC_Unit>();

    static int m_nStaticIdx = 0;

    void Awake()
    {
        m_pUnitInst.gameObject.SetActive(false);
    }

    static int AllocIdx() { return ++m_nStaticIdx; }
    public static int GetCurAllocIdx() { return m_nStaticIdx; }

    public void AddPoint(int nNumber)
    {
        TCK.CHECK(nNumber > 0);
        
        for (int i = 0; i < nNumber; i++)
        {
            AddPointBase();
        }
    }

    void AddPointBase()
    {
        TAC_Unit pZero = null;
        if (m_lstRunningUnit.Count > 0)
        {
            pZero = m_lstRunningUnit[m_lstRunningUnit.Count - 1];
        }

        GameObject goInst = GameObject.Instantiate(m_pUnitInst.gameObject);
        goInst.SetActive(true);
        goInst.transform.SetParent(m_trPlatform);
        goInst.transform.localPosition = Vector3.zero;
        goInst.transform.localRotation = Quaternion.Euler(Vector3.zero);
        goInst.transform.localScale = Vector3.one;

        TAC_Unit pUnit = goInst.GetComponent<TAC_Unit>();
        TCK.CHECK(pUnit != null);
        pUnit.SetBornIdx(AllocIdx());
        pUnit.name = "Unit " + pUnit.GetBornIdx();

        m_lstRunningUnit.Add(pUnit);

        Invoke("DoMathLogic", 0.5f);
    }
  
    void DoMathLogic()
    {
        /*
            按照倒序来遍历Unit，逐一修正坐标
        */
        m_pCircleConfig.ClearPointSaveValueInEveryCircle();

        for (int i = m_lstRunningUnit.Count - 1; i >= 0; i--)
        {
            TAC_Unit pUnit = m_lstRunningUnit[i];
            TCK.CHECK(pUnit != null);

            if (GetCurAllocIdx() == pUnit.GetBornIdx())
            {
                pUnit.transform.localPosition = new Vector3(
                    m_pCircleConfig.GetCircleZero().x,
                    0,
                    m_pCircleConfig.GetCircleZero().y
                    );
            }
            else
            {
                CorrectUnitPos(ref pUnit);
            }
        }
    }

    void CorrectUnitPos(ref TAC_Unit pUnit)
    {
        /*
            从第一环开始，尝试找到闲置空位
        */
        for (int iCircle = 1; iCircle < m_pCircleConfig.GetCircleNum(); iCircle++)
        {
            int iPoint_MinIdle = -1;
            Vector2 v2Unit = new Vector2(pUnit.transform.localPosition.x, pUnit.transform.localPosition.z);
            if (m_pCircleConfig.GetPointSaveValueInEveryCircle_MinIdle(iCircle, out iPoint_MinIdle, v2Unit))
            //if (m_pCircleConfig.GetPointSaveValueInEveryCircle_FirstIdle(iCircle, out iPoint_MinIdle))
            {
                Vector2 v2Pos = m_pCircleConfig.GetPosInCircle(iCircle, iPoint_MinIdle);
                pUnit.GetSimpleMoving().SetMovingTo(new Vector3(v2Pos.x, 0, v2Pos.y));

                m_pCircleConfig.SetPointSaveValueInEveryCircle(iCircle, iPoint_MinIdle, true);

                return;
            }
        }
        TCK.TERRO("TAC_Main.CorrectUnitPos pUnit -> "+ pUnit.GetBornIdx());
    }
    
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit))
            {
                TAC_Unit pUnit = hit.collider.gameObject.GetComponent<TAC_Unit>();
                if (pUnit != null)
                {
                    m_lstRunningUnit.Remove(pUnit);
                    GameObject.Destroy(pUnit.gameObject);

                    Invoke("DoMathLogic", 0.5f);
                }
            }
        }
    }
};