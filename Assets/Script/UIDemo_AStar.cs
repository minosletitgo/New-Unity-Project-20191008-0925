using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIDemo_AStar : MonoBehaviour
{
    public Camera m_stCamera;
    public GameObject m_goMapRoot;
    public UIDemo_AStar_CellItem m_stCellItem;

    public UIInput m_itCellNum_Horizontal;
    public UIInput m_itCellNum_Vertical;
    public UIButton m_btnRebuild;

    public UIInput m_itStartIdx_X;
    public UIInput m_itStartIdx_Y;
    public UIInput m_itEndIdx_X;
    public UIInput m_itEndIdx_Y;
    public UIButton m_btnDoing;


    int m_nCellNum_Horizontal = 10;
    int m_nCellNum_Vertical = 13;
    float m_fCellInterval_Horizontal = 70f;
    float m_fCellInterval_Vertical = 77f;


    List<UIDemo_AStar_CellItem> m_lstCellItem = new List<UIDemo_AStar_CellItem>();
    HexagonAStarHandler m_stAStarHandler;


    void Start()
    {
        m_stCellItem.gameObject.SetActive(false);
        m_btnRebuild.isEnabled = true;
        m_btnDoing.isEnabled = false;

        m_itCellNum_Horizontal.validation = UIInput.Validation.Integer;
        m_itCellNum_Vertical.validation = UIInput.Validation.Integer;
        UIEventListener.Get(m_btnRebuild.gameObject).onClick = delegate (GameObject go)
        {
            m_nCellNum_Horizontal = int.Parse(m_itCellNum_Horizontal.value);
            m_nCellNum_Horizontal = Mathf.Max(1, m_nCellNum_Horizontal);
            m_itCellNum_Horizontal.value = m_nCellNum_Horizontal.ToString();

            m_nCellNum_Vertical = int.Parse(m_itCellNum_Vertical.value);
            m_nCellNum_Vertical = Mathf.Max(1, m_nCellNum_Vertical);
            m_itCellNum_Vertical.value = m_nCellNum_Vertical.ToString();

            RebuildCell();

            m_btnDoing.isEnabled = true;
        };

        m_itStartIdx_X.validation = UIInput.Validation.Integer;
        m_itStartIdx_Y.validation = UIInput.Validation.Integer;
        m_itEndIdx_X.validation = UIInput.Validation.Integer;
        m_itEndIdx_Y.validation = UIInput.Validation.Integer;
        UIEventListener.Get(m_btnDoing.gameObject).onClick = delegate (GameObject go)
        {
            int nStartPosIdx_X = int.Parse(m_itStartIdx_X.value);
            nStartPosIdx_X = Mathf.Max(0, nStartPosIdx_X);
            m_itStartIdx_X.value = nStartPosIdx_X.ToString();

            int nStartPosIdx_Y = int.Parse(m_itStartIdx_Y.value);
            nStartPosIdx_Y = Mathf.Max(0, nStartPosIdx_Y);
            m_itStartIdx_Y.value = nStartPosIdx_Y.ToString();

            int nEndPosIdx_X = int.Parse(m_itEndIdx_X.value);
            nEndPosIdx_X = Mathf.Max(0, nEndPosIdx_X);
            m_itEndIdx_X.value = nEndPosIdx_X.ToString();

            int nEndPosIdx_Y = int.Parse(m_itEndIdx_Y.value);
            nEndPosIdx_Y = Mathf.Max(0, nEndPosIdx_Y);
            m_itEndIdx_Y.value = nEndPosIdx_Y.ToString();

            DoingAStar(nStartPosIdx_X, nStartPosIdx_Y, nEndPosIdx_X, nEndPosIdx_Y);
        };

        m_stAStarHandler = new HexagonAStarHandler();
    }

    void RebuildCell()
    {
        for (int i = 0; i < m_lstCellItem.Count; i++)
        {
            GameObject.Destroy(m_lstCellItem[i].gameObject);
        }
        m_lstCellItem.Clear();


        for (int iV = 0; iV < m_nCellNum_Vertical; iV++)
        {
            for (int iH = 0; iH < m_nCellNum_Horizontal; iH++)
            {
                GameObject goInst = GameObject.Instantiate(m_stCellItem.gameObject);
                goInst.name = string.Format("[{0},{1}]", iH, iV);
                goInst.SetActive(true);
                goInst.transform.SetParent(m_goMapRoot.transform);
                goInst.transform.localPosition = CalcCellPos(iH, iV);
                goInst.transform.localRotation = Quaternion.Euler(Vector3.zero);
                goInst.transform.localScale = Vector3.one;

                UIDemo_AStar_CellItem stCellItem = goInst.GetComponent<UIDemo_AStar_CellItem>();
                TCK.CHECK(stCellItem != null);
                stCellItem.Initialize(new Vector2(iH, iV));
                m_lstCellItem.Add(stCellItem);
            }
        }

        m_stAStarHandler.RebuildNode(m_nCellNum_Horizontal, m_nCellNum_Vertical, m_fCellInterval_Horizontal, m_fCellInterval_Vertical);
    }

    Vector3 CalcCellPos(int nPosIdx_X, int nPosIdx_Y)
    {
        if (nPosIdx_X < 0 || nPosIdx_Y < 0)
        {
            Debug.LogError(string.Format("CalcCellPos Failed: [{0},{1}]", nPosIdx_X, nPosIdx_Y));
            return Vector3.zero;
        }

        ////以[0,0]对应(0,0,0) 依次计算
        //int nZeroIdx_Horizontal = 0;
        //int nZeroIdx_Vertical = 0;

        //float fRetX = (nPosIdx_X - nZeroIdx_Horizontal) * m_fCellInterval_Horizontal;

        //float fRetY = (nPosIdx_Y - nZeroIdx_Vertical) * m_fCellInterval_Vertical;
        //fRetY = fRetY * (-1);
        //if (nPosIdx_X % 2 != 0)
        //{
        //    //横向索引为奇数，则向下偏移
        //    fRetY = fRetY - m_fCellInterval_Vertical / 2.0f;
        //}

        ////return new Vector3(fRetX, 0, fRetY);
        //return new Vector3(fRetX, fRetY, 0);

        Vector2 v2Calc = HexagonAStarHandler.CalcNodePosition(new Vector2(nPosIdx_X, nPosIdx_Y), m_fCellInterval_Horizontal, m_fCellInterval_Vertical);
        return new Vector3(v2Calc.x, v2Calc.y, 0);
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            //左键:显示邻居块
            Ray ray = m_stCamera.ScreenPointToRay(Input.mousePosition);
            RaycastHit hitInfo;
            if (Physics.Raycast(ray, out hitInfo))
            {
                UIDemo_AStar_CellItem stCellItemClick = hitInfo.collider.gameObject.GetComponent<UIDemo_AStar_CellItem>();

                if (stCellItemClick != null && m_nCellNum_Horizontal > 0 && m_nCellNum_Vertical > 0)
                {
                    for (int i = 0; i < m_lstCellItem.Count; i++)
                    {
                        m_lstCellItem[i].ShowDebugFlag(false);
                    }

                    //List<Vector2> lstNeighbor = AStarHandler.GetAStarNodeNeighbor(stCellItem.GetPosIdx(), m_nCellNum_Horizontal, m_nCellNum_Vertical);
                    //TCK.CHECK(lstNeighbor.Count > 0);

                    //foreach (Vector2 _v2Pos in lstNeighbor)
                    //{
                    //    UIDemo_AStar_CellItem _stNeighborCell = m_lstCellItem.Find(x => x.GetPosIdx() == _v2Pos);
                    //    TCK.CHECK(_stNeighborCell != null);
                    //    _stNeighborCell.ShowDebugFlag(true);
                    //}

                    foreach (UIDemo_AStar_CellItem _stItem in m_lstCellItem)
                    {
                        if (_stItem == stCellItemClick)
                        {
                            continue;
                        }

                        HexagonAStarHandler.EM_NineCorner emRetCorner = HexagonAStarHandler.EM_NineCorner.Invalid;
                        if (HexagonAStarHandler.IsAStarNodeAdjacentEx(stCellItemClick.GetPosIdx(), _stItem.GetPosIdx(), m_nCellNum_Horizontal, m_nCellNum_Vertical, ref emRetCorner))
                        {
                            _stItem.ShowDebugFlag(true);
                        }
                    }
                }
            }
        }
        else if (Input.GetMouseButtonDown(1))
        {
            //右键:设置障碍物
            Ray ray = m_stCamera.ScreenPointToRay(Input.mousePosition);
            RaycastHit hitInfo;
            if (Physics.Raycast(ray, out hitInfo))
            {
                UIDemo_AStar_CellItem stCellItem = hitInfo.collider.gameObject.GetComponent<UIDemo_AStar_CellItem>();
                if (stCellItem != null && m_nCellNum_Horizontal > 0 && m_nCellNum_Vertical > 0)
                {
                    stCellItem.SwitchState();
                    m_stAStarHandler.ChangeNodeType(stCellItem.GetPosIdx(), stCellItem.GetState());
                }                    
            }
        }
    }

    void DoingAStar(int nStartPosIdx_X, int nStartPosIdx_Y, int nEndPosIdx_X, int nEndPosIdx_Y)
    {
        for (int i = 0; i < m_lstCellItem.Count; i++)
        {
            m_lstCellItem[i].ShowDebugFlag(false);
        }

        List<Vector2> m_lstReturnPosIdx = new List<Vector2>();
        bool bIsSucc = m_stAStarHandler.StartFindPath(
            new Vector2(nStartPosIdx_X, nStartPosIdx_Y), 
            new Vector2(nEndPosIdx_X, nEndPosIdx_Y), 
            ref m_lstReturnPosIdx
            );

        Debug.LogWarning("DoingAStar : bIsSucc = " + bIsSucc.ToString());
        if (!bIsSucc)
        {
            return;
        }

        string strLog = "DoingAStar Return: ";
        strLog += "\n";
        for (int i = 0; i < m_lstReturnPosIdx.Count; i++)
        {
            strLog += string.Format("({0},{1})", m_lstReturnPosIdx[i].x, m_lstReturnPosIdx[i].y);
            strLog += " | ";
            if (i > 0 && i % 4 == 0)
            {
                strLog += "\n";
            }
        }
        Debug.LogWarning(strLog);


        for (int i = 0; i < m_lstReturnPosIdx.Count; i++)
        {
            UIDemo_AStar_CellItem _stItem = m_lstCellItem.Find(x => x.GetPosIdx() == m_lstReturnPosIdx[i]);
            TCK.CHECK(_stItem != null);
            _stItem.ShowDebugFlag(true);
        }
    }
}