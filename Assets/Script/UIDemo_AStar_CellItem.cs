using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIDemo_AStar_CellItem : MonoBehaviour
{
    public UISprite m_sprMain;
    public UISprite m_sprDebugFlag;
    public UILabel m_labPosIdx;

    Vector2 m_v2PosIdx = Vector2.zero;
    HexagonAStarHandler.EM_NodeType m_emCellState;

    public void Initialize(Vector2 v2PosIdx)
    {
        TCK.CHECK(v2PosIdx.x >= 0 && v2PosIdx.y >= 0);

        m_labPosIdx.text = string.Format("({0},{1})", v2PosIdx.x, v2PosIdx.y);
        m_v2PosIdx = v2PosIdx;
        SetState(HexagonAStarHandler.EM_NodeType.Road);
        ShowDebugFlag(false);
    }

    public Vector2 GetPosIdx() { return m_v2PosIdx; }

    public void SetState(HexagonAStarHandler.EM_NodeType emState)
    {
        TCK.CHECK(emState > HexagonAStarHandler.EM_NodeType.Invalid && emState < HexagonAStarHandler.EM_NodeType.Max);

        switch (emState)
        {
            case HexagonAStarHandler.EM_NodeType.Road:
                m_sprMain.spriteName = "Hexagon_03";
                break;
            case HexagonAStarHandler.EM_NodeType.Obstacle:
                m_sprMain.spriteName = "Hexagon_01";
                break;
        }
        m_emCellState = emState;
    }

    public HexagonAStarHandler.EM_NodeType GetState() { return m_emCellState; }

    public void SwitchState()
    {
        switch (m_emCellState)
        {
            case HexagonAStarHandler.EM_NodeType.Road:
                SetState(HexagonAStarHandler.EM_NodeType.Obstacle);
                break;
            case HexagonAStarHandler.EM_NodeType.Obstacle:
                SetState(HexagonAStarHandler.EM_NodeType.Road);
                break;
        }
    }

    public void ShowDebugFlag(bool bIsShow)
    {
        m_sprDebugFlag.gameObject.SetActive(bIsShow);
    }
}