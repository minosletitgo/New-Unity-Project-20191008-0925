using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
    2020.08.09 ganlu

    (0,0) | (1,0) | (2,0) | (3,0) | (4,0) | (5,0)
    (0,1) | (1,1) | (2,1) | (3,1) | (4,1) | (5,1)
    (0,2) | (1,2) | (2,2) | (3,2) | (4,2) | (5,2)

    AStar算法类(六边形Node)
    
    1.将开始节点放入开放列表(开始节点的F和G值都视为0);
    2.重复一下步骤:
        A01.在开放列表中查找具有最小F值的节点,并把查找到的节点作为当前节点;
        A02.把当前节点从开放列表删除, 加入到封闭列表;
        A03.对当前节点[相邻]的每一个节点依次执行以下步骤:
            a.如果该相邻节点不可通行或者该相邻节点已经在封闭列表中,则什么操作也不执行,继续检验下一个节点;
            b.如果该相邻节点不在开放列表中,则将该节点添加到开放列表中, 并将该相邻节点的父节点设为当前节点,同时保存该相邻节点的G和F值;
            c.如果该相邻节点在开放列表中, 则判断若经由当前节点到达该相邻节点的G值是否小于原来保存的G值,若小于,则将该相邻节点的父节点设为当前节点,并重新设置该相邻节点的G和F值.
        A04.循环结束条件:当终点节点被加入到开放列表作为待检验节点时, 表示路径被找到,此时应终止循环;或者当开放列表为空,表明已无可以添加的新节点,而已检验的节点中没有终点节点则意味着路径无法被找到,此时也结束循环;
    3.从终点节点开始沿父节点遍历, 并保存整个遍历到的节点坐标,遍历所得的节点就是最后得到的路径;
*/

public class HexagonAStarHandler
{
    int m_nMaxNodeNumber_H = 0;
    int m_nMaxNodeNumber_V = 0;
    float m_fNodeInterval_Horizontal;
    float m_fNodeInterval_Vertical;

    Dictionary<string, AStarNode> m_mapAllNode = new Dictionary<string, AStarNode>();
    List<AStarNode> m_lstOpenNode = new List<AStarNode>();
    List<AStarNode> m_lstCloseNode = new List<AStarNode>();
    AStarNode m_stCurNode = null;


    public HexagonAStarHandler()
    {
        //Nothing
    }

    public void RebuildNode(int nMaxNodeNumber_H, int nMaxNodeNumber_V, float fNodeInterval_Horizontal, float fNodeInterval_Vertical)
    {
        TCK.CHECK(nMaxNodeNumber_H > 0);
        TCK.CHECK(nMaxNodeNumber_V > 0);

        TCK.CHECK(fNodeInterval_Horizontal > 0);
        TCK.CHECK(fNodeInterval_Vertical > 0);

        m_mapAllNode.Clear();

        for (int iV = 0; iV < nMaxNodeNumber_V; iV++)
        {
            for (int iH = 0; iH < nMaxNodeNumber_H; iH++)
            {
                Vector2 v2Pos = new Vector2(iH, iV);
                AStarNode stNode = new AStarNode(EM_NodeType.Road, v2Pos);
                stNode.SetValue(0, 0);
                m_mapAllNode.Add(HexagonAStarHandler.GetAStarNodePosKeyName(v2Pos), stNode);
            }
        }

        m_nMaxNodeNumber_H = nMaxNodeNumber_H;
        m_nMaxNodeNumber_V = nMaxNodeNumber_V;

        m_fNodeInterval_Horizontal = fNodeInterval_Horizontal;
        m_fNodeInterval_Vertical = fNodeInterval_Vertical;
    }

    public bool ChangeNodeType(Vector2 v2Pos, EM_NodeType emType)
    {
        TCK.CHECK(v2Pos.x >= 0 && v2Pos.y >= 0);
        TCK.CHECK(emType > EM_NodeType.Invalid && emType < EM_NodeType.Max);

        string strPosKeyName = HexagonAStarHandler.GetAStarNodePosKeyName(v2Pos);
        AStarNode stNode = null;
        if (m_mapAllNode.TryGetValue(strPosKeyName, out stNode))
        {
            stNode.SetNodeType(emType);
            return true;
        }
        return false;
    }

    public bool StartFindPath(Vector2 v2StartPos, Vector2 v2EndPos, ref List<Vector2> lstReturn)
    {
        Debug.LogWarning(string.Format("({0},{1}) -> ({2},{3})", v2StartPos.x, v2StartPos.y, v2EndPos.x, v2EndPos.y));

        TCK.CHECK(v2StartPos.x >= 0 && v2StartPos.x < m_nMaxNodeNumber_H);
        TCK.CHECK(v2EndPos.x >= 0 && v2EndPos.x < m_nMaxNodeNumber_H);
        if(v2StartPos == v2EndPos)
        {
            //起始点和目标点重合
            return false;
        }

        foreach (KeyValuePair<string, AStarNode> objPair in m_mapAllNode)
        {
            objPair.Value.SetValue(0, 0);
            objPair.Value.SetParentNode(null);
        }
        m_lstOpenNode.Clear();
        m_lstCloseNode.Clear();
        m_stCurNode = null;

        AStarNode stNodeStart = GetNode(v2StartPos);
        TCK.CHECK(stNodeStart != null, "AStarHandler.StartFindPath Error: stNodeStart -> "+ v2StartPos.ToString());

        AStarNode stNodeEnd = GetNode(v2EndPos);
        TCK.CHECK(stNodeEnd != null, "AStarHandler.StartFindPath Error: stNodeEnd -> " + v2EndPos.ToString());
        if(stNodeEnd.GetNodeType() == EM_NodeType.Obstacle)
        {
            //目标点位障碍物
            return false;
        }

        //1
        m_lstOpenNode.Add(stNodeStart);

        //2
        while (true)
        {
            //A01
            m_stCurNode = null;
            int nMinFValue = int.MaxValue;
            foreach (AStarNode _stNode in m_lstOpenNode)
            {
                if (_stNode.GetFValue() <= nMinFValue)
                {
                    m_stCurNode = _stNode;
                    nMinFValue = _stNode.GetFValue();
                }
            }
            TCK.CHECK(m_stCurNode != null);

            //A02
            m_lstOpenNode.Remove(m_stCurNode);
            m_lstCloseNode.Add(m_stCurNode);

            //A03
            List<Vector2> lstNeighbor = HexagonAStarHandler.GetAStarNodeNeighbor(m_stCurNode.GetPos(), m_nMaxNodeNumber_H, m_nMaxNodeNumber_V);
            TCK.CHECK(lstNeighbor.Count > 0);
            foreach (Vector2 _v2Pos in lstNeighbor)
            {
                AStarNode _stNode = GetNode(_v2Pos);
                TCK.CHECK(_stNode != null);

                //a
                if (_stNode.GetNodeType() == EM_NodeType.Obstacle || m_lstCloseNode.Exists(x => x.GetPos() == _stNode.GetPos()))
                {
                    continue;
                }

                //b
                if (!m_lstOpenNode.Exists(x => x.GetPos() == _stNode.GetPos()))
                {
                    m_lstOpenNode.Add(_stNode);
                    _stNode.SetParentNode(m_stCurNode);

                    if (_stNode.GetPos() == stNodeEnd.GetPos())
                    {
                        break;
                    }

                    int nGValue = 0;
                    int nHValue = 0;
                    HexagonAStarHandler.CalcNodeValue(_stNode.GetPos(), m_stCurNode.GetPos(), stNodeEnd.GetPos(), 
                        m_nMaxNodeNumber_H, m_nMaxNodeNumber_V,
                        m_fNodeInterval_Horizontal, m_fNodeInterval_Vertical,
                        ref nGValue, ref nHValue);
                    TCK.CHECK(nGValue > 0);
                    TCK.CHECK(nHValue > 0);
                    _stNode.SetValue(nGValue, nHValue);
                }
                else
                {
                    int nGValue = 0;
                    int nHValue = 0;
                    HexagonAStarHandler.CalcNodeValue(_stNode.GetPos(), m_stCurNode.GetPos(), stNodeEnd.GetPos(), 
                        m_nMaxNodeNumber_H, m_nMaxNodeNumber_V,
                        m_fNodeInterval_Horizontal, m_fNodeInterval_Vertical,
                        ref nGValue, ref nHValue);
                    TCK.CHECK(nGValue > 0);
                    TCK.CHECK(nHValue > 0);

                    if (nGValue + nHValue < _stNode.GetFValue())
                    {
                        _stNode.SetParentNode(m_stCurNode);
                        _stNode.SetValue(nGValue, nHValue);
                    }
                }
            }

            //A04
            if (m_lstOpenNode.Exists(x => x.GetPos() == stNodeEnd.GetPos()))
            {
                break;
            }
            if (m_lstOpenNode.Count <= 0)
            {
                break;
            }
        }

        //3.
        Debug.Log("Success");

        lstReturn.Clear();
        HexagonAStarHandler.RecursionPopPosIdx(stNodeEnd, ref lstReturn);
        lstReturn.Reverse();

        if (lstReturn.Count == 1 && lstReturn[0] == stNodeStart.GetPos())
        {
            //有且只有StartPos
            return false;
        }

        return true;
    }





    //-----------------------------------------Private-Begin-----------------------------------------
    public enum EM_NodeType
    {
        Invalid = -1,
        Road,
        Obstacle,
        Max,
    }

    public class AStarNode
    {
        public AStarNode(EM_NodeType emType, Vector2 v2Pos)
        {
            TCK.CHECK(emType > EM_NodeType.Invalid && emType < EM_NodeType.Max);
            TCK.CHECK(v2Pos.x >= 0 && v2Pos.y >= 0);

            m_emType = emType;
            m_v2Pos = v2Pos;
            m_nGValue = 0;
            m_nHValue = 0;
            m_stParentNode = null;
        }

        public void SetNodeType(EM_NodeType emType)
        {
            TCK.CHECK(emType > EM_NodeType.Invalid && emType < EM_NodeType.Max);
            m_emType = emType;
        }

        public EM_NodeType GetNodeType() { return m_emType; }

        public Vector2 GetPos() { return m_v2Pos; }

        public void SetValue(int nGValue, int nHValue)
        {
            TCK.CHECK(nGValue >= 0);
            TCK.CHECK(nHValue >= 0);
            m_nGValue = nGValue;
            m_nHValue = nHValue;
        }

        public int GetFValue() { return m_nGValue + m_nHValue; }

        public void SetParentNode(AStarNode _stNode)
        {
            m_stParentNode = _stNode;
        }

        public AStarNode GetParentNode() { return m_stParentNode; }

        EM_NodeType m_emType;
        Vector2 m_v2Pos;
        int m_nGValue;
        int m_nHValue;
        AStarNode m_stParentNode;
    }

    AStarNode GetNode(Vector2 v2Pos)
    {
        AStarNode stNode = null;
        if (m_mapAllNode.TryGetValue(HexagonAStarHandler.GetAStarNodePosKeyName(v2Pos), out stNode))
        {
            return stNode;
        }
        return null;
    }



    public static string GetAStarNodePosKeyName(Vector2 v2Pos)
    {
        return string.Format("({0},{1})", v2Pos.x, v2Pos.y);
    }

    public enum EM_NineCorner
    {
        Invalid = -1,

        /*
            九宫格坐标示意图
            (x-1,y-1) | (x+0,y-1) | (x+1,y-1)
            (x-1,y+0) | (x+0,y+0) | (x+1,y+0)
            (x-1,y+1) | (x+0,y+1) | (x+1,y+1)

            这里排除Center
        */

        TopLeft,
        Top,
        TopRight,
        Right,
        BottomRight,
        Bottom,
        BottomLeft,
        Left,

        Max,
    }

    public static Vector2 GetNineCorner(Vector2 v2Pos, EM_NineCorner emCorner)
    {
        //由[中心点]坐标，反向推导出[目标九宫格点]坐标
        TCK.CHECK(emCorner > EM_NineCorner.Invalid && emCorner < EM_NineCorner.Max);

        switch (emCorner)
        {
            case EM_NineCorner.TopLeft: return new Vector2(v2Pos.x - 1, v2Pos.y - 1);
            case EM_NineCorner.Top: return new Vector2(v2Pos.x + 0, v2Pos.y - 1);
            case EM_NineCorner.TopRight: return new Vector2(v2Pos.x + 1, v2Pos.y - 1);
            case EM_NineCorner.Right: return new Vector2(v2Pos.x + 1, v2Pos.y + 0);
            case EM_NineCorner.BottomRight: return new Vector2(v2Pos.x + 1, v2Pos.y + 1);
            case EM_NineCorner.Bottom: return new Vector2(v2Pos.x + 0, v2Pos.y + 1);
            case EM_NineCorner.BottomLeft: return new Vector2(v2Pos.x - 1, v2Pos.y + 1);
            case EM_NineCorner.Left: return new Vector2(v2Pos.x - 1, v2Pos.y + 0);
            default:
                {
                    Debug.LogError("AStarHandler.GetNineCorner Error: " + emCorner.ToString());
                    return Vector2.zero;
                }
        }
    }
    public static List<Vector2> GetAStarNodeNeighbor(Vector2 v2Pos, int nMaxNodeNumber_H, int nMaxNodeNumber_V)
    {
        //计算出任意一个[点坐标]，其周围的[邻居点坐标]
        TCK.CHECK(v2Pos.x >= 0 && v2Pos.x < nMaxNodeNumber_H);
        TCK.CHECK(v2Pos.y >= 0 && v2Pos.y < nMaxNodeNumber_V);

        List<Vector2> lstRet = new List<Vector2>();

        List<HexagonAStarHandler.EM_NineCorner> lstCornerValue = new List<EM_NineCorner>();
        for (HexagonAStarHandler.EM_NineCorner iCorner = HexagonAStarHandler.EM_NineCorner.Invalid + 1; iCorner < HexagonAStarHandler.EM_NineCorner.Max; iCorner++)
        {
            lstCornerValue.Add(iCorner);
        }

        if (v2Pos.x % 2 != 0)
        {
            //剔除九宫格的[左上角、右上角]即可
            lstCornerValue.Remove(EM_NineCorner.TopLeft);
            lstCornerValue.Remove(EM_NineCorner.TopRight);
        }
        else
        {
            //剔除九宫格的[左下角、右下角]即可
            lstCornerValue.Remove(EM_NineCorner.BottomLeft);
            lstCornerValue.Remove(EM_NineCorner.BottomRight);
        }

        foreach (HexagonAStarHandler.EM_NineCorner iCorner in lstCornerValue)
        {
            Vector2 v2Tmp = HexagonAStarHandler.GetNineCorner(v2Pos, iCorner);
            if (v2Tmp.x >= 0 && v2Tmp.x < nMaxNodeNumber_H && v2Tmp.y >= 0 && v2Tmp.y < nMaxNodeNumber_V)
            {
                lstRet.Add(v2Tmp);
            }
        }

        return lstRet;
    }

    public static bool IsAStarNodeAdjacent(Vector2 v2PosA, Vector2 v2PosB, int nMaxNodeNumber_H, int nMaxNodeNumber_V)
    {
        //判断[2点坐标]是否为[相邻关系]
        List<Vector2> lstNeighbor = HexagonAStarHandler.GetAStarNodeNeighbor(v2PosA, nMaxNodeNumber_H, nMaxNodeNumber_V);
        TCK.CHECK(lstNeighbor.Count > 0);

        return lstNeighbor.Exists(x => x == v2PosB);
    }

    public static bool IsAStarNodeAdjacentEx(Vector2 v2PosA, Vector2 v2PosB, int nMaxNodeNumber_H, int nMaxNodeNumber_V, ref EM_NineCorner emRetCorner)
    {
        TCK.CHECK(v2PosA.x >= 0 && v2PosA.x < nMaxNodeNumber_H && v2PosA.y >= 0 && v2PosA.y < nMaxNodeNumber_V,
            "IsAStarNodeAdjacentEx Error v2PosA: " + v2PosA.ToString());
        TCK.CHECK(v2PosB.x >= 0 && v2PosB.x < nMaxNodeNumber_H && v2PosB.y >= 0 && v2PosB.y < nMaxNodeNumber_V,
            "IsAStarNodeAdjacentEx Error v2PosB: " + v2PosB.ToString());

        //以v2PosA为中心，验证v2PosB是否为邻居之一
        int nDeltaX = (int)(v2PosA.x - v2PosB.x);
        nDeltaX = Mathf.Abs(nDeltaX);
        int nDeltaY = (int)(v2PosA.y - v2PosB.y);
        nDeltaY = Mathf.Abs(nDeltaY);

        if (nDeltaX > 1 || nDeltaY > 1)
        {
            //压根不在九宫格范围内
            return false;
        }

        if (v2PosA.x % 2 != 0)
        {
            //剔除九宫格的[左上角、右上角]即可
            Vector2 v2TopLeft = HexagonAStarHandler.GetNineCorner(v2PosA, EM_NineCorner.TopLeft);
            if (v2TopLeft == v2PosB)
            {
                return false;
            }

            Vector2 v2TopRight = HexagonAStarHandler.GetNineCorner(v2PosA, EM_NineCorner.TopRight);
            if (v2TopRight == v2PosB)
            {
                return false;
            }
        }
        else
        {
            //剔除九宫格的[左下角、右下角]即可
            Vector2 v2BottomLeft = HexagonAStarHandler.GetNineCorner(v2PosA, EM_NineCorner.BottomLeft);
            if (v2BottomLeft == v2PosB)
            {
                return false;
            }

            Vector2 v2BottomRight = HexagonAStarHandler.GetNineCorner(v2PosA, EM_NineCorner.BottomRight);
            if (v2BottomRight == v2PosB)
            {
                return false;
            }
        }

        emRetCorner = EM_NineCorner.Invalid;
        for (EM_NineCorner iCorner = EM_NineCorner.Invalid + 1; iCorner < EM_NineCorner.Max; iCorner++)
        {
            if (HexagonAStarHandler.GetNineCorner(v2PosA, iCorner) == v2PosB)
            {
                emRetCorner = iCorner;
                break;
            }
        }
        TCK.CHECK(emRetCorner > EM_NineCorner.Invalid && emRetCorner < EM_NineCorner.Max);

        return true;
    }

    public static void CalcNodeValue(Vector2 v2TargetPos, Vector2 v2StartPos, Vector2 v2EndPos, 
        int nMaxNodeNumber_H, int nMaxNodeNumber_V, 
        float fNodeInterval_Horizontal, float fNodeInterval_Vertical,
        ref int nGValue, ref int nHValue)
    {
        TCK.CHECK(v2TargetPos.x >= 0 && v2TargetPos.y >= 0);
        TCK.CHECK(v2StartPos.x >= 0 && v2StartPos.y >= 0);
        TCK.CHECK(v2EndPos.x >= 0 && v2EndPos.y >= 0);

        TCK.CHECK(v2TargetPos != v2StartPos && v2TargetPos != v2EndPos);
        TCK.CHECK(v2StartPos != v2EndPos);

        EM_NineCorner emRetCorner = EM_NineCorner.Invalid;
        TCK.CHECK(HexagonAStarHandler.IsAStarNodeAdjacentEx(v2TargetPos, v2StartPos, nMaxNodeNumber_H, nMaxNodeNumber_V, ref emRetCorner));

        Vector2 v2Position_Target = HexagonAStarHandler.CalcNodePosition(v2TargetPos, fNodeInterval_Horizontal, fNodeInterval_Vertical);
        Vector2 v2Position_Start = HexagonAStarHandler.CalcNodePosition(v2StartPos, fNodeInterval_Horizontal, fNodeInterval_Vertical);
        Vector2 v2Position_End = HexagonAStarHandler.CalcNodePosition(v2EndPos, fNodeInterval_Horizontal, fNodeInterval_Vertical);

        int nDeltaValueX_ToStartPos = (int)(v2TargetPos.x) - (int)(v2StartPos.x);
        nDeltaValueX_ToStartPos = Mathf.Abs(nDeltaValueX_ToStartPos);
        int nDeltaValueY_Start_Pos = (int)(v2TargetPos.y) - (int)(v2StartPos.y);
        nDeltaValueY_Start_Pos = Mathf.Abs(nDeltaValueY_Start_Pos);
        TCK.CHECK(nDeltaValueX_ToStartPos == 1 || nDeltaValueY_Start_Pos == 1);

        //计算G值
        nGValue = (int)Vector2.Distance(v2Position_Target, v2Position_Start);


        //计算H值
        //int nDeltaValueX_ToEndPos = (int)(v2TargetPos.x) - (int)(v2EndPos.x);
        //nDeltaValueX_ToEndPos = Mathf.Abs(nDeltaValueX_ToEndPos);
        //int nDeltaValueY_ToEndPos = (int)(v2TargetPos.y) - (int)(v2EndPos.y);
        //nDeltaValueY_ToEndPos = Mathf.Abs(nDeltaValueY_ToEndPos);
        //nHValue = nDeltaValueX_ToEndPos * 10 + nDeltaValueY_ToEndPos * 10;
        nHValue = (int)Vector2.Distance(v2Position_Target, v2Position_End);
    }

    public static void RecursionPopPosIdx(AStarNode stNodeEnd, ref List<Vector2> lstRet)
    {
        if (stNodeEnd == null)
        {
            return;
        }

        lstRet.Add(stNodeEnd.GetPos());
        RecursionPopPosIdx(stNodeEnd.GetParentNode(), ref lstRet);
    }

    public static Vector2 CalcNodePosition(Vector2 v2PosIdx, float fNodeInterval_Horizontal, float fNodeInterval_Vertical)
    {
        //计算平面坐标
        TCK.CHECK(v2PosIdx.x >= 0 && v2PosIdx.y >= 0);
        TCK.CHECK(fNodeInterval_Horizontal > 0);
        TCK.CHECK(fNodeInterval_Vertical > 0);

        //以[0,0]对应(0,0,0) 依次计算
        int nZeroIdx_Horizontal = 0;
        int nZeroIdx_Vertical = 0;

        float fRetX = (v2PosIdx.x - nZeroIdx_Horizontal) * fNodeInterval_Horizontal;

        float fRetY = (v2PosIdx.y - nZeroIdx_Vertical) * fNodeInterval_Vertical;
        fRetY = fRetY * (-1);
        if (((int)(v2PosIdx.x)) % 2 != 0)
        {
            //横向索引为奇数，则向下偏移
            fRetY = fRetY - fNodeInterval_Vertical / 2.0f;
        }

        return new Vector2(fRetX, fRetY);
    }
    //-----------------------------------------Private-End-----------------------------------------
}
