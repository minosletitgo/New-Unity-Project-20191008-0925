using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class EarthTileHelper
{
    public static bool CheckChunkIndex(EarthTHandler.EM_ChunkIndex emId)
    {
        if (emId > EarthTHandler.EM_ChunkIndex.Invalid && emId < EarthTHandler.EM_ChunkIndex.Max)
        {
            return true;
        }
        return false;
    }

    public static bool CheckAreaIndex(int nAreaId)
    {
        if (nAreaId >= 0 && nAreaId < GetOneChunkAreaMaxNum())
        {
            return true;
        }
        return false;
    }

    public static bool CheckAreaIndexXY(int nAreaIndexXY)
    {
        if (nAreaIndexXY >= 0 && nAreaIndexXY < EarthTHandler.m_nConstOneChunkAreaSideNum)
        {
            return true;
        }
        return false;
    }

    public static bool CheckChunkDirection(EarthTHandler.EM_ChunkDirection emDir)
    {
        if (emDir > EarthTHandler.EM_ChunkDirection.Invalid && emDir < EarthTHandler.EM_ChunkDirection.Max)
        {
            return true;
        }
        return false;
    }

    public static bool CheckAreaDirection(EarthTHandler.EM_AreaDirection emDir)
    {
        if (emDir > EarthTHandler.EM_AreaDirection.Invalid && emDir < EarthTHandler.EM_AreaDirection.Max)
        {
            return true;
        }
        return false;
    }

    public static float GetOneChunkAreaDimension()
    {
        return (float)(EarthTHandler.m_fConstOneChunkDimension) / (float)(EarthTHandler.m_nConstOneChunkAreaSideNum);
    }

    public static int GetOneChunkAreaMaxNum()
    {
        return EarthTHandler.m_nConstOneChunkAreaSideNum * EarthTHandler.m_nConstOneChunkAreaSideNum;
    }

    public static int GetAreaId(int nIndexX, int nIndexY)
    {
        return nIndexX + nIndexY * EarthTHandler.m_nConstOneChunkAreaSideNum;
    }

    public static void GetAreaIndex(int nAreaId, out int nIndexX, out int nIndexY)
    {
        GameCommon.ASSERT(CheckAreaIndex(nAreaId));

        nIndexY = nAreaId / EarthTHandler.m_nConstOneChunkAreaSideNum;
        nIndexX = nAreaId % EarthTHandler.m_nConstOneChunkAreaSideNum;
    }

    public static int GetAreaIndexX(float fPosInChunk_X)
    {
        GameCommon.ASSERT(
            fPosInChunk_X >= 0 &&
            fPosInChunk_X <= EarthTHandler.m_fConstOneChunkDimension, "fPosInChunk_X = " + fPosInChunk_X
            );
        if (fPosInChunk_X == EarthTHandler.m_fConstOneChunkDimension)
        {
            return EarthTHandler.m_nConstOneChunkAreaSideNum - 1;
        }
        return (int)(fPosInChunk_X / (float)(EarthTileHelper.GetOneChunkAreaDimension()));
    }

    public static int GetAreaIndexY(float fPosInChunk_Y)
    {
        GameCommon.ASSERT(
            fPosInChunk_Y >= 0 &&
            fPosInChunk_Y <= EarthTHandler.m_fConstOneChunkDimension, "fPosInChunk_Y = " + fPosInChunk_Y
            );
        if (fPosInChunk_Y == EarthTHandler.m_fConstOneChunkDimension)
        {
            return EarthTHandler.m_nConstOneChunkAreaSideNum - 1;
        }
        return (int)(fPosInChunk_Y / (float)(EarthTileHelper.GetOneChunkAreaDimension()));
    }

    public static int GetRotate90DegreeTimes(float fNeedRotateDegree, out float fNeedRotateDegree_Clamp)
    {
        fNeedRotateDegree_Clamp = GetClampRotateDegree(fNeedRotateDegree);

        //度数: 0 ~ 270
        //顺时针旋转90度的次数: 0 ~ 3
        int nRotate90DegreeTimes = (int)(fNeedRotateDegree_Clamp / 90.0f);

        return nRotate90DegreeTimes;
    }

    public static int GetRotate90DegreeTimes(float fNeedRotateDegree)
    {
        float fNeedRotateDegree_Clamp = GetClampRotateDegree(fNeedRotateDegree);

        //度数: 0 ~ 270
        //顺时针旋转90度的次数: 0 ~ 3
        int nRotate90DegreeTimes = (int)(fNeedRotateDegree_Clamp / 90.0f);

        return nRotate90DegreeTimes;
    }

    public static float GetClampRotateDegree(float fNeedRotateDegree)
    {
        GameCommon.ASSERT(fNeedRotateDegree % 90 == 0);
        fNeedRotateDegree = fNeedRotateDegree % 360;
        float fNeedRotateDegree_Abs = Math.Abs(fNeedRotateDegree);
        GameCommon.ASSERT(fNeedRotateDegree_Abs >= 0 &&
            fNeedRotateDegree_Abs <= 270);

        float fNeedRotateDegree_Clamp = 0;
        if (fNeedRotateDegree == 0)
        {
            fNeedRotateDegree_Clamp = 0;
        }

        if (fNeedRotateDegree > 0)
        {
            fNeedRotateDegree_Clamp = fNeedRotateDegree % 360;
        }

        if (fNeedRotateDegree < 0)
        {
            fNeedRotateDegree_Clamp = fNeedRotateDegree % 360 + 360;
        }

        return fNeedRotateDegree_Clamp;
    }

    public static float GetRotateClampDegree(int nRotate90DegreeTimes)
    {
        GameCommon.ASSERT(nRotate90DegreeTimes >= 0 && nRotate90DegreeTimes <= 3);
        if (nRotate90DegreeTimes == 0) return 0;
        if (nRotate90DegreeTimes == 1) return 90;
        if (nRotate90DegreeTimes == 2) return 180;
        if (nRotate90DegreeTimes == 3) return 270;
        return 0;
    }

    public static EarthTArea GetTargetArea(
        EarthTChunk.ST_NeighborChunk stNeighborChunk,
        int nAreaSideIndex
        )
    {
        GameCommon.ASSERT(stNeighborChunk != null);

        EarthTChunk stChunk = stNeighborChunk.GetChunk();
        GameCommon.ASSERT(stChunk != null);

        float fNeedRotateDegree_Clamp = 0;
        int nRotate90DegreeTimes = EarthTileHelper.GetRotate90DegreeTimes(
            stNeighborChunk.GetNeedRotateDegree(),
            out fNeedRotateDegree_Clamp
            );

        GameCommon.ASSERT(nRotate90DegreeTimes >= 0 && nRotate90DegreeTimes <= 3);

        //取点使用索引
        GameCommon.ASSERT(nAreaSideIndex >= 0 && nAreaSideIndex < EarthTHandler.m_nConstOneChunkAreaSideNum);

        int nIndexX_Ret = -1;
        int nIndexY_Ret = -1;
        if (nRotate90DegreeTimes == 0)
        {
            switch (stNeighborChunk.GetDir())
            {
                case EarthTHandler.EM_ChunkDirection.N:
                    {
                        //2 -> 4
                        nIndexX_Ret = nAreaSideIndex;
                        nIndexY_Ret = 0;
                    }
                    break;
                case EarthTHandler.EM_ChunkDirection.E:
                    {
                        //2 -> 3
                        nIndexX_Ret = 0;
                        nIndexY_Ret = nAreaSideIndex;
                    }
                    break;
                case EarthTHandler.EM_ChunkDirection.S:
                    {
                        //2 -> 5
                        nIndexX_Ret = nAreaSideIndex;
                        nIndexY_Ret = EarthTHandler.m_nConstOneChunkAreaSideNum - 1;
                    }
                    break;
                case EarthTHandler.EM_ChunkDirection.W:
                    {
                        //2 -> 1
                        nIndexX_Ret = EarthTHandler.m_nConstOneChunkAreaSideNum - 1;
                        nIndexY_Ret = nAreaSideIndex;
                    }
                    break;
            }
        }
        else if (nRotate90DegreeTimes == 1)
        {
            switch (stNeighborChunk.GetDir())
            {
                case EarthTHandler.EM_ChunkDirection.N:
                    {
                        //3 -> 4
                        nIndexX_Ret = EarthTHandler.m_nConstOneChunkAreaSideNum - 1;
                        nIndexY_Ret = nAreaSideIndex;
                    }
                    break;
                case EarthTHandler.EM_ChunkDirection.E:
                    {
                        //5 -> 3
                        nIndexX_Ret = EarthTHandler.m_nConstOneChunkAreaSideNum - nAreaSideIndex - 1;
                        nIndexY_Ret = 0;
                    }
                    break;
                case EarthTHandler.EM_ChunkDirection.S:
                    {
                        //1 -> 5
                        nIndexX_Ret = 0;
                        nIndexY_Ret = nAreaSideIndex;
                    }
                    break;
                case EarthTHandler.EM_ChunkDirection.W:
                    {
                        //4 -> 1
                        nIndexX_Ret = EarthTHandler.m_nConstOneChunkAreaSideNum - nAreaSideIndex - 1;
                        nIndexY_Ret = EarthTHandler.m_nConstOneChunkAreaSideNum - 1;
                    }
                    break;
            }
        }
        else if (nRotate90DegreeTimes == 2)
        {
            switch (stNeighborChunk.GetDir())
            {
                case EarthTHandler.EM_ChunkDirection.N:
                    {
                        //0 -> 4
                        nIndexX_Ret = EarthTHandler.m_nConstOneChunkAreaSideNum - nAreaSideIndex - 1;
                        nIndexY_Ret = EarthTHandler.m_nConstOneChunkAreaSideNum - 1;
                    }
                    break;
                case EarthTHandler.EM_ChunkDirection.E:
                    {
                        //NoWay
                        GameCommon.ASSERT(false, "nRotateCircleTimes == 2 , EM_ChunkDirection.E");
                    }
                    break;
                case EarthTHandler.EM_ChunkDirection.S:
                    {
                        //0 -> 5
                        nIndexX_Ret = EarthTHandler.m_nConstOneChunkAreaSideNum - nAreaSideIndex - 1;
                        nIndexY_Ret = 0;
                    }
                    break;
                case EarthTHandler.EM_ChunkDirection.W:
                    {
                        //NoWay
                        GameCommon.ASSERT(false, "nRotateCircleTimes == 2 , EM_ChunkDirection.W");
                    }
                    break;
            }
        }
        else if (nRotate90DegreeTimes == 3)
        {
            switch (stNeighborChunk.GetDir())
            {
                case EarthTHandler.EM_ChunkDirection.N:
                    {
                        //1 -> 4
                        nIndexX_Ret = 0;
                        nIndexY_Ret = EarthTHandler.m_nConstOneChunkAreaSideNum - nAreaSideIndex - 1;
                    }
                    break;
                case EarthTHandler.EM_ChunkDirection.E:
                    {
                        //4 -> 3
                        nIndexX_Ret = nAreaSideIndex;
                        nIndexY_Ret = EarthTHandler.m_nConstOneChunkAreaSideNum - 1;
                    }
                    break;
                case EarthTHandler.EM_ChunkDirection.S:
                    {
                        //3 -> 5
                        nIndexX_Ret = EarthTHandler.m_nConstOneChunkAreaSideNum - 1;
                        nIndexY_Ret = EarthTHandler.m_nConstOneChunkAreaSideNum - nAreaSideIndex - 1;
                    }
                    break;
                case EarthTHandler.EM_ChunkDirection.W:
                    {
                        //5 -> 1
                        nIndexX_Ret = nAreaSideIndex;
                        nIndexY_Ret = 0;
                    }
                    break;
            }
        }

        EarthTArea stAreaRet = stChunk.GetArea(nIndexX_Ret, nIndexY_Ret);
        GameCommon.ASSERT(stAreaRet != null);

        return stAreaRet;
    }










    #region [地块坐标系]
    public static void GetChunkAreaByPos(
        EarthTHandler.EM_ChunkIndex emChunkIndex,
        Vector2 v2PosInChunk,
        out int nAreaId,
        out Vector2 v2OffsetToArea
        )
    {
        GameCommon.ASSERT(EarthTileHelper.CheckChunkIndex(emChunkIndex));

        GameCommon.ASSERT(v2PosInChunk.x >= 0 && v2PosInChunk.x <= EarthTHandler.m_fConstOneChunkDimension, "v2PosInChunk.x = " + v2PosInChunk.x);
        GameCommon.ASSERT(v2PosInChunk.y >= 0 && v2PosInChunk.y <= EarthTHandler.m_fConstOneChunkDimension, "v2PosInChunk.y = " + v2PosInChunk.y);

        int nAreaIndexX = EarthTileHelper.GetAreaIndexX(v2PosInChunk.x);
        GameCommon.ASSERT(EarthTileHelper.CheckAreaIndexXY(nAreaIndexX), "nAreaIndexX = " + nAreaIndexX);

        int nAreaIndexY = EarthTileHelper.GetAreaIndexX(v2PosInChunk.y);
        GameCommon.ASSERT(EarthTileHelper.CheckAreaIndexXY(nAreaIndexY), "nAreaIndexY = " + nAreaIndexY);

        nAreaId = GetAreaId(nAreaIndexX, nAreaIndexY);

        //计算相对于[当前Area的左下0点]偏移
        v2OffsetToArea = Vector2.zero;
        v2OffsetToArea.x = v2PosInChunk.x - nAreaIndexX * EarthTileHelper.GetOneChunkAreaDimension();
        GameCommon.ASSERT(v2OffsetToArea.x >= 0);
        v2OffsetToArea.y = v2PosInChunk.y - nAreaIndexY * EarthTileHelper.GetOneChunkAreaDimension();
        GameCommon.ASSERT(v2OffsetToArea.y >= 0);
    }

    public static void GetPosOffsetInChunkDimension(
        float fPosOffsetX, float fPosOffsetY,
        float fRotatedDegree, ref Vector2 v2Pos
        )
    {
        float fNeedRotateDegree_Clamp = 0;
        int nRotate90DegreeTimes = GetRotate90DegreeTimes(fRotatedDegree, out fNeedRotateDegree_Clamp);
        GameCommon.ASSERT(nRotate90DegreeTimes >= 0 && nRotate90DegreeTimes <= 3);

        v2Pos = Vector2.zero;

        if (nRotate90DegreeTimes == 0)
        {
            if (fPosOffsetX >= 0)
            {
                v2Pos.x = fPosOffsetX;
            }
            else
            {
                v2Pos.x = EarthTHandler.m_fConstOneChunkDimension + fPosOffsetX;
            }

            if (fPosOffsetY >= 0)
            {
                v2Pos.y = fPosOffsetY;
            }
            else
            {
                v2Pos.y = EarthTHandler.m_fConstOneChunkDimension + fPosOffsetY;
            }
        }
        else if (nRotate90DegreeTimes == 1)
        {
            if (fPosOffsetX >= 0)
            {
                v2Pos.y = fPosOffsetX;
            }
            else
            {
                v2Pos.y = EarthTHandler.m_fConstOneChunkDimension + fPosOffsetX;
            }

            if (fPosOffsetY >= 0)
            {
                v2Pos.x = EarthTHandler.m_fConstOneChunkDimension - fPosOffsetY;
            }
            else
            {
                v2Pos.x = -fPosOffsetY;
            }
        }
        else if (nRotate90DegreeTimes == 2)
        {
            if (fPosOffsetX >= 0)
            {
                v2Pos.x = EarthTHandler.m_fConstOneChunkDimension - fPosOffsetX;
            }
            else
            {
                v2Pos.x = -fPosOffsetX;
            }

            if (fPosOffsetY >= 0)
            {
                v2Pos.y = EarthTHandler.m_fConstOneChunkDimension - fPosOffsetY;
            }
            else
            {
                v2Pos.y = -fPosOffsetY;
            }
        }
        else if (nRotate90DegreeTimes == 3)
        {
            if (fPosOffsetX >= 0)
            {
                v2Pos.y = EarthTHandler.m_fConstOneChunkDimension - fPosOffsetX;
            }
            else
            {
                v2Pos.y = -fPosOffsetX;
            }

            if (fPosOffsetY >= 0)
            {
                v2Pos.x = fPosOffsetY;
            }
            else
            {
                v2Pos.x = EarthTHandler.m_fConstOneChunkDimension - (-fPosOffsetY);
            }
        }
    }
    #endregion








    #region [2D全局坐标系]
    public static void GetChunkAreaBy2DPosition(
        float fX, float fY, EarthTHandler stHandler,
        out EarthTHandler.EM_ChunkIndex emChunkId,
        out float fChunkRotatedDegree,
        out Vector2 v2PosInOneChunk
        )
    {
        fX = fX % (EarthTHandler.m_fConstOneChunkDimension * 4);
        fY = fY % (EarthTHandler.m_fConstOneChunkDimension * 4);

        GameCommon.ASSERT(stHandler != null);

        //从横纵移动Chunk的次数
        int nMoveChunkToH = (int)(fX / EarthTHandler.m_fConstOneChunkDimension);
        float fMoveChunkToH_Offset = fX % EarthTHandler.m_fConstOneChunkDimension;
        if (fMoveChunkToH_Offset >= 0)
        {
            nMoveChunkToH = (int)(Mathf.Abs(fX) / EarthTHandler.m_fConstOneChunkDimension);
        }
        else
        {
            nMoveChunkToH = (int)(Mathf.Abs(fX) / EarthTHandler.m_fConstOneChunkDimension);
            nMoveChunkToH = nMoveChunkToH * -1;
            nMoveChunkToH = nMoveChunkToH - 1;

            fMoveChunkToH_Offset = EarthTHandler.m_fConstOneChunkDimension + fMoveChunkToH_Offset;
        }

        //从纵纵移动Chunk的次数
        int nMoveChunkToV = (int)(fY / EarthTHandler.m_fConstOneChunkDimension);
        float fMoveChunkToV_Offset = fY % EarthTHandler.m_fConstOneChunkDimension;
        if (fMoveChunkToV_Offset >= 0)
        {
            nMoveChunkToV = (int)(Mathf.Abs(fY) / EarthTHandler.m_fConstOneChunkDimension);
        }
        else
        {
            nMoveChunkToV = (int)(Mathf.Abs(fY) / EarthTHandler.m_fConstOneChunkDimension);
            nMoveChunkToV = nMoveChunkToV * -1;
            nMoveChunkToV = nMoveChunkToV - 1;

            fMoveChunkToV_Offset = EarthTHandler.m_fConstOneChunkDimension + fMoveChunkToV_Offset;
        }

        EarthTChunk stChunk_Tmp = stHandler.GetChunk(EarthTHandler.EM_ChunkIndex.Zero);
        GameCommon.ASSERT(stChunk_Tmp != null);
        float fCenterRotatedDegree = 0;

        for (int _iTimes = 0; _iTimes < Mathf.Abs(nMoveChunkToH); _iTimes++)
        {
            EarthTChunk.ST_NeighborChunk _stNeighborChunk = null;
            if (nMoveChunkToH > 0)
            {
                _stNeighborChunk = stChunk_Tmp.GetNeighborChunk(EarthTHandler.EM_ChunkDirection.E, fCenterRotatedDegree);
            }
            else
            {
                _stNeighborChunk = stChunk_Tmp.GetNeighborChunk(EarthTHandler.EM_ChunkDirection.W, fCenterRotatedDegree);
            }

            GameCommon.ASSERT(_stNeighborChunk != null);
            stChunk_Tmp = _stNeighborChunk.GetChunk();
            GameCommon.ASSERT(stChunk_Tmp != null);
            fCenterRotatedDegree += _stNeighborChunk.GetNeedRotateDegree();
        }
        for (int _iTimes = 0; _iTimes < Mathf.Abs(nMoveChunkToV); _iTimes++)
        {
            EarthTChunk.ST_NeighborChunk _stNeighborChunk = null;
            if (nMoveChunkToV > 0)
            {
                _stNeighborChunk = stChunk_Tmp.GetNeighborChunk(EarthTHandler.EM_ChunkDirection.N, fCenterRotatedDegree);
            }
            else
            {
                _stNeighborChunk = stChunk_Tmp.GetNeighborChunk(EarthTHandler.EM_ChunkDirection.S, fCenterRotatedDegree);
            }

            GameCommon.ASSERT(_stNeighborChunk != null);
            stChunk_Tmp = _stNeighborChunk.GetChunk();
            GameCommon.ASSERT(stChunk_Tmp != null);
            fCenterRotatedDegree += _stNeighborChunk.GetNeedRotateDegree();
        }

        GameCommon.ASSERT(stChunk_Tmp != null);

        emChunkId = stChunk_Tmp.GetId();
        fChunkRotatedDegree = fCenterRotatedDegree;

        v2PosInOneChunk = Vector2.zero;
        EarthTileHelper.GetPosOffsetInChunkDimension(
            fMoveChunkToH_Offset, fMoveChunkToV_Offset,
            fCenterRotatedDegree, ref v2PosInOneChunk
            );
    }

    public static Vector2 GetDirectionBy2DPosition(
        float fXInBigEarth_1, float fYInBigEarth_1,
        float fXInBigEarth_2, float fYInBigEarth_2,
        EarthTHandler stHandler
        )
    {
        Vector2 v2PosWithTileCoordinate_1 = GetTileCoordinate(
            fXInBigEarth_1, fYInBigEarth_1,
            stHandler);

        Vector2 v2PosWithTileCoordinate_2 = GetTileCoordinate(
            fXInBigEarth_1, fYInBigEarth_1,
            fXInBigEarth_2, fYInBigEarth_2,
            stHandler);

        return v2PosWithTileCoordinate_2 - v2PosWithTileCoordinate_1;
    }

    public static Vector2 GetDirectionBy2DPosition_Smart(
        float fXInBigEarth_1, float fYInBigEarth_1,
        float fXInBigEarth_2, float fYInBigEarth_2,
        EarthTHandler stHandler
        )
    {
        //PS: 自动调整 [ChunkId小] 指向 [ChunkId大] , 即可确保有完整的[正序方向]和[逆序方向]

        //[X,Y] -> [i,X,Y]
        EarthTHandler.EM_ChunkIndex emChunkId_1;
        float fChunkRotatedDegree_1;
        Vector2 v2PosInOneChunk_1;
        EarthTileHelper.GetChunkAreaBy2DPosition(
            fXInBigEarth_1, fYInBigEarth_1, stHandler,
            out emChunkId_1,
            out fChunkRotatedDegree_1,
            out v2PosInOneChunk_1
            );
        GameCommon.ASSERT(EarthTileHelper.CheckChunkIndex(emChunkId_1));
        EarthTChunk stChunk_1 = stHandler.GetChunk(emChunkId_1);
        GameCommon.ASSERT(stChunk_1 != null);
        GameCommon.ASSERT(v2PosInOneChunk_1.x >= 0);
        GameCommon.ASSERT(v2PosInOneChunk_1.y >= 0);

        //[X,Y] -> [i,X,Y]
        EarthTHandler.EM_ChunkIndex emChunkId_2;
        float fChunkRotatedDegree_2;
        Vector2 v2PosInOneChunk_2;
        EarthTileHelper.GetChunkAreaBy2DPosition(
            fXInBigEarth_2, fYInBigEarth_2, stHandler,
            out emChunkId_2,
            out fChunkRotatedDegree_2,
            out v2PosInOneChunk_2
            );
        GameCommon.ASSERT(EarthTileHelper.CheckChunkIndex(emChunkId_2));
        EarthTChunk stChunk_2 = stHandler.GetChunk(emChunkId_2);
        GameCommon.ASSERT(stChunk_2 != null);
        GameCommon.ASSERT(v2PosInOneChunk_2.x >= 0);
        GameCommon.ASSERT(v2PosInOneChunk_2.y >= 0);


        Vector2 v2PosWithTileCoordinate_1;
        Vector2 v2PosWithTileCoordinate_2;

        if (emChunkId_1 == emChunkId_2)
        {
            v2PosWithTileCoordinate_1 = GetTileCoordinate(
                fXInBigEarth_1, fYInBigEarth_1,
                stHandler);

            v2PosWithTileCoordinate_2 = GetTileCoordinate(
                fXInBigEarth_1, fYInBigEarth_1,
                fXInBigEarth_2, fYInBigEarth_2,
                stHandler);
        }
        else
        {
            if (emChunkId_1 < emChunkId_2)
            {
                v2PosWithTileCoordinate_1 = GetTileCoordinate(
                    fXInBigEarth_1, fYInBigEarth_1,
                    stHandler);

                v2PosWithTileCoordinate_2 = GetTileCoordinate(
                    fXInBigEarth_1, fYInBigEarth_1,
                    fXInBigEarth_2, fYInBigEarth_2,
                    stHandler);
            }
            else
            {
                v2PosWithTileCoordinate_2 = GetTileCoordinate(
                    fXInBigEarth_2, fYInBigEarth_2,
                    stHandler);

                v2PosWithTileCoordinate_1 = GetTileCoordinate(
                    fXInBigEarth_2, fYInBigEarth_2,
                    fXInBigEarth_1, fYInBigEarth_1,
                    stHandler);
            }
        }

        Vector2 v2RetDir = v2PosWithTileCoordinate_2 - v2PosWithTileCoordinate_1;

        return v2RetDir.normalized;
    }
    #endregion


    public static float GetDeltaDisInChunk(
        Vector2 v2PosInChunk,
        float fChunkRotatedDegree,
        EarthTHandler.EM_ChunkDirection emDirTargetLine
        )
    {
        //PS: 得到[目标方向Line]距离

        GameCommon.ASSERT(v2PosInChunk.x >= 0);
        GameCommon.ASSERT(v2PosInChunk.y >= 0);

        float fNeedRotateDegree_Clamp = 0;
        int nRotate90DegreeTimes = EarthTileHelper.GetRotate90DegreeTimes(
            fChunkRotatedDegree,
            out fNeedRotateDegree_Clamp
            );

        GameCommon.ASSERT(nRotate90DegreeTimes >= 0 && nRotate90DegreeTimes <= 3);

        GameCommon.ASSERT(EarthTileHelper.CheckChunkDirection(emDirTargetLine));

        if (nRotate90DegreeTimes == 0)
        {
            switch (emDirTargetLine)
            {
                case EarthTHandler.EM_ChunkDirection.N: return EarthTHandler.m_fConstOneChunkDimension - v2PosInChunk.y;
                case EarthTHandler.EM_ChunkDirection.E: return EarthTHandler.m_fConstOneChunkDimension - v2PosInChunk.x;
                case EarthTHandler.EM_ChunkDirection.S: return v2PosInChunk.y;
                case EarthTHandler.EM_ChunkDirection.W: return v2PosInChunk.x;
            }
        }
        else if (nRotate90DegreeTimes == 1)
        {
            switch (emDirTargetLine)
            {
                case EarthTHandler.EM_ChunkDirection.N: return v2PosInChunk.x;
                case EarthTHandler.EM_ChunkDirection.E: return EarthTHandler.m_fConstOneChunkDimension - v2PosInChunk.y;
                case EarthTHandler.EM_ChunkDirection.S: return EarthTHandler.m_fConstOneChunkDimension - v2PosInChunk.x;
                case EarthTHandler.EM_ChunkDirection.W: return v2PosInChunk.y;
            }
        }
        else if (nRotate90DegreeTimes == 2)
        {
            switch (emDirTargetLine)
            {
                case EarthTHandler.EM_ChunkDirection.N: return v2PosInChunk.y;
                case EarthTHandler.EM_ChunkDirection.E: return v2PosInChunk.x;
                case EarthTHandler.EM_ChunkDirection.S: return EarthTHandler.m_fConstOneChunkDimension - v2PosInChunk.y;
                case EarthTHandler.EM_ChunkDirection.W: return EarthTHandler.m_fConstOneChunkDimension - v2PosInChunk.x;
            }
        }
        else if (nRotate90DegreeTimes == 3)
        {
            switch (emDirTargetLine)
            {
                case EarthTHandler.EM_ChunkDirection.N: return EarthTHandler.m_fConstOneChunkDimension - v2PosInChunk.x;
                case EarthTHandler.EM_ChunkDirection.E: return v2PosInChunk.y;
                case EarthTHandler.EM_ChunkDirection.S: return v2PosInChunk.x;
                case EarthTHandler.EM_ChunkDirection.W: return EarthTHandler.m_fConstOneChunkDimension - v2PosInChunk.y;
            }
        }

        return 0;
    }

    public static float GetDistanceBy2DPosition(
        float fXInBigEarth_1, float fYInBigEarth_1,
        float fXInBigEarth_2, float fYInBigEarth_2,
        EarthTHandler stHandler
        )
    {
        GameCommon.ASSERT(stHandler != null);

        //[X,Y] -> [i,X,Y]
        EarthTHandler.EM_ChunkIndex emChunkId_1;
        float fChunkRotatedDegree_1;
        Vector2 v2PosInOneChunk_1;
        EarthTileHelper.GetChunkAreaBy2DPosition(
            fXInBigEarth_1, fYInBigEarth_1, stHandler,
            out emChunkId_1,
            out fChunkRotatedDegree_1,
            out v2PosInOneChunk_1
            );
        GameCommon.ASSERT(EarthTileHelper.CheckChunkIndex(emChunkId_1));
        EarthTChunk stChunk_1 = stHandler.GetChunk(emChunkId_1);
        GameCommon.ASSERT(stChunk_1 != null);
        GameCommon.ASSERT(v2PosInOneChunk_1.x >= 0);
        GameCommon.ASSERT(v2PosInOneChunk_1.y >= 0);

        //[X,Y] -> [i,X,Y]
        EarthTHandler.EM_ChunkIndex emChunkId_2;
        float fChunkRotatedDegree_2;
        Vector2 v2PosInOneChunk_2;
        EarthTileHelper.GetChunkAreaBy2DPosition(
            fXInBigEarth_2, fYInBigEarth_2, stHandler,
            out emChunkId_2,
            out fChunkRotatedDegree_2,
            out v2PosInOneChunk_2
            );
        GameCommon.ASSERT(EarthTileHelper.CheckChunkIndex(emChunkId_2));
        EarthTChunk stChunk_2 = stHandler.GetChunk(emChunkId_2);
        GameCommon.ASSERT(stChunk_2 != null);
        GameCommon.ASSERT(v2PosInOneChunk_2.x >= 0);
        GameCommon.ASSERT(v2PosInOneChunk_2.y >= 0);

        if (emChunkId_1 == emChunkId_2)
        {
            return Vector2.Distance(v2PosInOneChunk_1, v2PosInOneChunk_2);
        }
        else
        {
            float fRet = 0;
            float fDeltaDisH = 0;
            float fDeltaDisV = 0;

            //PS: 把[1]摆正，计算[2]之于[1]的关系

            EarthTChunk.ST_NeighborChunk stNeighborChunk = stChunk_1.GetNeighborChunk(stChunk_2.GetId());
            if (stNeighborChunk != null)
            {
                switch (stNeighborChunk.GetDir())
                {
                    case EarthTHandler.EM_ChunkDirection.N:
                        {
                            float fDirLineL_1 = GetDeltaDisInChunk(v2PosInOneChunk_1, 0, EarthTHandler.EM_ChunkDirection.W);
                            float fDirLineR_1 = GetDeltaDisInChunk(v2PosInOneChunk_1, 0, EarthTHandler.EM_ChunkDirection.E);
                            float fDirLineL_2 = GetDeltaDisInChunk(v2PosInOneChunk_2, stNeighborChunk.GetNeedRotateDegree(), EarthTHandler.EM_ChunkDirection.W);
                            float fDirLineR_2 = GetDeltaDisInChunk(v2PosInOneChunk_2, stNeighborChunk.GetNeedRotateDegree(), EarthTHandler.EM_ChunkDirection.E);

                            if (fDirLineL_1 <= fDirLineL_2)
                            {
                                fDeltaDisH = EarthTHandler.m_fConstOneChunkDimension - (fDirLineL_1 + fDirLineR_2);
                            }
                            else
                            {
                                fDeltaDisH = EarthTHandler.m_fConstOneChunkDimension - (fDirLineL_2 + fDirLineR_1);
                            }
                            GameCommon.ASSERT(fDeltaDisH >= 0);



                            float fDirLineU_1 = GetDeltaDisInChunk(v2PosInOneChunk_1, 0, EarthTHandler.EM_ChunkDirection.N);
                            float fDirLineB_2 = GetDeltaDisInChunk(v2PosInOneChunk_2, stNeighborChunk.GetNeedRotateDegree(), EarthTHandler.EM_ChunkDirection.S);

                            fDeltaDisV = fDirLineU_1 + fDirLineB_2;
                            GameCommon.ASSERT(fDeltaDisV >= 0);
                        }
                        break;
                    case EarthTHandler.EM_ChunkDirection.E:
                        {
                            float fDirLineR_1 = GetDeltaDisInChunk(v2PosInOneChunk_1, 0, EarthTHandler.EM_ChunkDirection.E);
                            float fDirLineL_2 = GetDeltaDisInChunk(v2PosInOneChunk_2, stNeighborChunk.GetNeedRotateDegree(), EarthTHandler.EM_ChunkDirection.W);

                            fDeltaDisH = fDirLineR_1 + fDirLineL_2;
                            GameCommon.ASSERT(fDeltaDisH >= 0);



                            float fDirLineU_1 = GetDeltaDisInChunk(v2PosInOneChunk_1, 0, EarthTHandler.EM_ChunkDirection.N);
                            float fDirLineB_1 = GetDeltaDisInChunk(v2PosInOneChunk_1, 0, EarthTHandler.EM_ChunkDirection.S);
                            float fDirLineU_2 = GetDeltaDisInChunk(v2PosInOneChunk_2, stNeighborChunk.GetNeedRotateDegree(), EarthTHandler.EM_ChunkDirection.N);
                            float fDirLineB_2 = GetDeltaDisInChunk(v2PosInOneChunk_2, stNeighborChunk.GetNeedRotateDegree(), EarthTHandler.EM_ChunkDirection.S);

                            if (fDirLineU_1 <= fDirLineU_2)
                            {
                                fDeltaDisV = EarthTHandler.m_fConstOneChunkDimension - (fDirLineU_1 + fDirLineB_2);
                            }
                            else
                            {
                                fDeltaDisV = EarthTHandler.m_fConstOneChunkDimension - (fDirLineU_2 + fDirLineB_1);
                            }
                            GameCommon.ASSERT(fDeltaDisV >= 0);
                        }
                        break;
                    case EarthTHandler.EM_ChunkDirection.S:
                        {
                            float fDirLineL_1 = GetDeltaDisInChunk(v2PosInOneChunk_1, 0, EarthTHandler.EM_ChunkDirection.W);
                            float fDirLineR_1 = GetDeltaDisInChunk(v2PosInOneChunk_1, 0, EarthTHandler.EM_ChunkDirection.E);
                            float fDirLineL_2 = GetDeltaDisInChunk(v2PosInOneChunk_2, stNeighborChunk.GetNeedRotateDegree(), EarthTHandler.EM_ChunkDirection.W);
                            float fDirLineR_2 = GetDeltaDisInChunk(v2PosInOneChunk_2, stNeighborChunk.GetNeedRotateDegree(), EarthTHandler.EM_ChunkDirection.E);

                            if (fDirLineL_1 > fDirLineL_2)
                            {
                                fDeltaDisH = EarthTHandler.m_fConstOneChunkDimension - (fDirLineR_1 + fDirLineL_2);
                            }
                            else
                            {
                                fDeltaDisH = EarthTHandler.m_fConstOneChunkDimension - (fDirLineL_1 + fDirLineR_2);
                            }
                            GameCommon.ASSERT(fDeltaDisH >= 0);


                            float fDirLineB_1 = GetDeltaDisInChunk(v2PosInOneChunk_1, 0, EarthTHandler.EM_ChunkDirection.S);
                            float fDirLineU_2 = GetDeltaDisInChunk(v2PosInOneChunk_2, stNeighborChunk.GetNeedRotateDegree(), EarthTHandler.EM_ChunkDirection.N);

                            fDeltaDisV = fDirLineB_1 + fDirLineU_2;
                            GameCommon.ASSERT(fDeltaDisV >= 0);
                        }
                        break;
                    case EarthTHandler.EM_ChunkDirection.W:
                        {
                            float fDirLineL_1 = GetDeltaDisInChunk(v2PosInOneChunk_1, 0, EarthTHandler.EM_ChunkDirection.W);
                            float fDirLineR_2 = GetDeltaDisInChunk(v2PosInOneChunk_2, stNeighborChunk.GetNeedRotateDegree(), EarthTHandler.EM_ChunkDirection.E);

                            fDeltaDisH = fDirLineL_1 + fDirLineR_2;
                            GameCommon.ASSERT(fDeltaDisH >= 0);



                            float fDirLineU_1 = GetDeltaDisInChunk(v2PosInOneChunk_1, 0, EarthTHandler.EM_ChunkDirection.N);
                            float fDirLineB_1 = GetDeltaDisInChunk(v2PosInOneChunk_1, 0, EarthTHandler.EM_ChunkDirection.S);
                            float fDirLineU_2 = GetDeltaDisInChunk(v2PosInOneChunk_2, stNeighborChunk.GetNeedRotateDegree(), EarthTHandler.EM_ChunkDirection.N);
                            float fDirLineB_2 = GetDeltaDisInChunk(v2PosInOneChunk_2, stNeighborChunk.GetNeedRotateDegree(), EarthTHandler.EM_ChunkDirection.S);

                            if (fDirLineU_1 > fDirLineU_2)
                            {
                                fDeltaDisV = EarthTHandler.m_fConstOneChunkDimension - (fDirLineB_1 + fDirLineU_2);
                            }
                            else
                            {
                                fDeltaDisV = EarthTHandler.m_fConstOneChunkDimension - (fDirLineU_1 + fDirLineB_2);
                            }
                            GameCommon.ASSERT(fDeltaDisV >= 0);
                        }
                        break;
                }

                fRet = (float)(Math.Sqrt(Math.Pow(fDeltaDisH, 2) + Math.Pow(fDeltaDisV, 2)));

                return fRet;
            }
            else
            {
                //PS: 对于[正方体魔方],既然不是[相连接的邻居],则必然是[相隔一块的邻居]
                //PS: 把[1]摆正，计算[2]之于[1]的关系

                //PS: 假设取其N方向找
                float fRetN = 0;
                {
                    EarthTChunk stChunk_Tmp = stChunk_1;
                    float fCenterRotatedDegree = 0;

                    for (int _iTimes = 0; _iTimes < 2; _iTimes++)
                    {
                        EarthTChunk.ST_NeighborChunk _stNeighborChunk = stChunk_Tmp.GetNeighborChunk(EarthTHandler.EM_ChunkDirection.N, fCenterRotatedDegree);
                        GameCommon.ASSERT(_stNeighborChunk != null);
                        stChunk_Tmp = _stNeighborChunk.GetChunk();
                        GameCommon.ASSERT(stChunk_Tmp != null);
                        fCenterRotatedDegree += _stNeighborChunk.GetNeedRotateDegree();
                    }

                    GameCommon.ASSERT(stChunk_Tmp != null);
                    GameCommon.ASSERT(stChunk_Tmp.GetId() == stChunk_2.GetId());

                    //PS: 此时直接认定[2]是[1]的N,只是中间夹着一个Chunk
                    {
                        float fDirLineL_1 = GetDeltaDisInChunk(v2PosInOneChunk_1, 0, EarthTHandler.EM_ChunkDirection.W);
                        float fDirLineR_1 = GetDeltaDisInChunk(v2PosInOneChunk_1, 0, EarthTHandler.EM_ChunkDirection.E);
                        float fDirLineL_2 = GetDeltaDisInChunk(v2PosInOneChunk_2, fCenterRotatedDegree % 360, EarthTHandler.EM_ChunkDirection.W);
                        float fDirLineR_2 = GetDeltaDisInChunk(v2PosInOneChunk_2, fCenterRotatedDegree % 360, EarthTHandler.EM_ChunkDirection.E);

                        if (fDirLineL_1 <= fDirLineL_2)
                        {
                            fDeltaDisH = EarthTHandler.m_fConstOneChunkDimension - (fDirLineL_1 + fDirLineR_2);
                        }
                        else
                        {
                            fDeltaDisH = EarthTHandler.m_fConstOneChunkDimension - (fDirLineR_1 + fDirLineL_2);
                        }

                        GameCommon.ASSERT(fDeltaDisH >= 0);



                        float fDirLineU_1 = GetDeltaDisInChunk(v2PosInOneChunk_1, 0, EarthTHandler.EM_ChunkDirection.N);
                        float fDirLineB_2 = GetDeltaDisInChunk(v2PosInOneChunk_2, fCenterRotatedDegree % 360, EarthTHandler.EM_ChunkDirection.S);

                        fDeltaDisV = fDirLineU_1 + fDirLineB_2;
                        fDeltaDisV += EarthTHandler.m_fConstOneChunkDimension;
                        GameCommon.ASSERT(fDeltaDisV >= 0);
                    }

                    fRetN = (float)(Math.Sqrt(Math.Pow(fDeltaDisH, 2) + Math.Pow(fDeltaDisV, 2)));
                }

                //PS: 假设取其S方向找
                float fRetS = 0;
                {
                    EarthTChunk stChunk_Tmp = stChunk_1;
                    float fCenterRotatedDegree = fChunkRotatedDegree_1;

                    for (int _iTimes = 0; _iTimes < 2; _iTimes++)
                    {
                        EarthTChunk.ST_NeighborChunk _stNeighborChunk = stChunk_Tmp.GetNeighborChunk(EarthTHandler.EM_ChunkDirection.S, fCenterRotatedDegree);
                        GameCommon.ASSERT(_stNeighborChunk != null);
                        stChunk_Tmp = _stNeighborChunk.GetChunk();
                        GameCommon.ASSERT(stChunk_Tmp != null);
                        fCenterRotatedDegree += _stNeighborChunk.GetNeedRotateDegree();
                    }

                    GameCommon.ASSERT(stChunk_Tmp != null);
                    GameCommon.ASSERT(stChunk_Tmp.GetId() == stChunk_2.GetId());

                    //PS: 此时直接认定[2]是[1]的S,只是中间夹着一个Chunk
                    {
                        float fDirLineL_1 = GetDeltaDisInChunk(v2PosInOneChunk_1, 0, EarthTHandler.EM_ChunkDirection.W);
                        float fDirLineR_1 = GetDeltaDisInChunk(v2PosInOneChunk_1, 0, EarthTHandler.EM_ChunkDirection.E);
                        float fDirLineL_2 = GetDeltaDisInChunk(v2PosInOneChunk_2, fCenterRotatedDegree % 360, EarthTHandler.EM_ChunkDirection.W);
                        float fDirLineR_2 = GetDeltaDisInChunk(v2PosInOneChunk_2, fCenterRotatedDegree % 360, EarthTHandler.EM_ChunkDirection.E);

                        if (fDirLineL_1 <= fDirLineL_2)
                        {
                            fDeltaDisH = EarthTHandler.m_fConstOneChunkDimension - (fDirLineL_1 + fDirLineR_2);
                        }
                        else
                        {
                            fDeltaDisH = EarthTHandler.m_fConstOneChunkDimension - (fDirLineR_1 + fDirLineL_2);
                        }

                        GameCommon.ASSERT(fDeltaDisH >= 0);



                        float fDirLineB_1 = GetDeltaDisInChunk(v2PosInOneChunk_1, 0, EarthTHandler.EM_ChunkDirection.S);
                        float fDirLineU_2 = GetDeltaDisInChunk(v2PosInOneChunk_2, fCenterRotatedDegree % 360, EarthTHandler.EM_ChunkDirection.N);

                        fDeltaDisV = fDirLineB_1 + fDirLineU_2;
                        fDeltaDisV += EarthTHandler.m_fConstOneChunkDimension;
                        GameCommon.ASSERT(fDeltaDisV >= 0);
                    }

                    fRetS = (float)(Math.Sqrt(Math.Pow(fDeltaDisH, 2) + Math.Pow(fDeltaDisV, 2)));
                }

                fRet = Math.Min(fRetN, fRetS);

                return fRet;
            }
        }
    }

    public static float GetDistanceBy2DPositionWithTileCoordinate(
        float fXInBigEarth_1, float fYInBigEarth_1,
        float fXInBigEarth_2, float fYInBigEarth_2,
        EarthTHandler stHandler
        )
    {
        GameCommon.ASSERT(stHandler != null);

        Vector2 v2PosTileCoordinate_1 = GetTileCoordinate(
            fXInBigEarth_1, fYInBigEarth_1,
            stHandler);
        Vector2 v2PosTileCoordinate_2 = GetTileCoordinate(
            fXInBigEarth_1, fYInBigEarth_1,
            fXInBigEarth_2, fYInBigEarth_2,
            stHandler);

        return Vector2.Distance(v2PosTileCoordinate_1, v2PosTileCoordinate_2);
    }

    public static Vector2 GetTileCoordinate(
        float fXInBigEarth_Base, float fYInBigEarth_Base,
        EarthTHandler stHandler
        )
    {
        GameCommon.ASSERT(stHandler != null);

        Vector2 v2Ret = Vector2.zero;

        //[X,Y] -> [i,X,Y]
        EarthTHandler.EM_ChunkIndex emChunkId_Base;
        float fChunkRotatedDegree_Base;
        Vector2 v2PosInOneChunk_Base;
        EarthTileHelper.GetChunkAreaBy2DPosition(
            fXInBigEarth_Base, fYInBigEarth_Base, stHandler,
            out emChunkId_Base,
            out fChunkRotatedDegree_Base,
            out v2PosInOneChunk_Base
            );
        GameCommon.ASSERT(EarthTileHelper.CheckChunkIndex(emChunkId_Base));
        EarthTChunk stChunk_Base = stHandler.GetChunk(emChunkId_Base);
        GameCommon.ASSERT(stChunk_Base != null);
        GameCommon.ASSERT(v2PosInOneChunk_Base.x >= 0);
        GameCommon.ASSERT(v2PosInOneChunk_Base.y >= 0);


        //以stChunk_Base的摆正角度 建立坐标系

        float fNeedRotateDegree_Clamp = 0;
        int nRotate90DegreeTimes = EarthTileHelper.GetRotate90DegreeTimes(
            fChunkRotatedDegree_Base,
            out fNeedRotateDegree_Clamp
            );

        GameCommon.ASSERT(nRotate90DegreeTimes >= 0 && nRotate90DegreeTimes <= 3);

        //if (nRotate90DegreeTimes == 0)
        //{
        //    v2Ret = v2PosInOneChunk_Base;            
        //}
        //else if (nRotate90DegreeTimes == 1)
        //{
        //    v2Ret.x = v2PosInOneChunk_Base.y;
        //    v2Ret.y = EarthTHandler.m_fConstOneChunkDimension - v2PosInOneChunk_Base.x;
        //}
        //else if (nRotate90DegreeTimes == 2)
        //{
        //    v2Ret.x = EarthTHandler.m_fConstOneChunkDimension - v2PosInOneChunk_Base.x;
        //    v2Ret.y = EarthTHandler.m_fConstOneChunkDimension - v2PosInOneChunk_Base.y;
        //}
        //else if (nRotate90DegreeTimes == 3)
        //{
        //    v2Ret.x = EarthTHandler.m_fConstOneChunkDimension - v2PosInOneChunk_Base.y;
        //    v2Ret.y = v2PosInOneChunk_Base.x;
        //}
        v2Ret = v2PosInOneChunk_Base;
        return v2Ret;
    }

    public static Vector2 GetTileCoordinate(
        float fXInBigEarth_Base, float fYInBigEarth_Base,
        float fXInBigEarth_Target, float fYInBigEarth_Target,
        EarthTHandler stHandler
        )
    {
        GameCommon.ASSERT(stHandler != null);

        Vector2 v2Ret = Vector2.zero;

        //[X,Y] -> [i,X,Y]
        EarthTHandler.EM_ChunkIndex emChunkId_Base;
        float fChunkRotatedDegree_Base;
        Vector2 v2PosInOneChunk_Base;
        EarthTileHelper.GetChunkAreaBy2DPosition(
            fXInBigEarth_Base, fYInBigEarth_Base, stHandler,
            out emChunkId_Base,
            out fChunkRotatedDegree_Base,
            out v2PosInOneChunk_Base
            );
        GameCommon.ASSERT(EarthTileHelper.CheckChunkIndex(emChunkId_Base));
        EarthTChunk stChunk_Base = stHandler.GetChunk(emChunkId_Base);
        GameCommon.ASSERT(stChunk_Base != null);
        GameCommon.ASSERT(v2PosInOneChunk_Base.x >= 0);
        GameCommon.ASSERT(v2PosInOneChunk_Base.y >= 0);



        //[X,Y] -> [i,X,Y]
        EarthTHandler.EM_ChunkIndex emChunkId_Target;
        float fChunkRotatedDegree_Target;
        Vector2 v2PosInOneChunk_Target;
        EarthTileHelper.GetChunkAreaBy2DPosition(
            fXInBigEarth_Target, fYInBigEarth_Target, stHandler,
            out emChunkId_Target,
            out fChunkRotatedDegree_Target,
            out v2PosInOneChunk_Target
            );
        GameCommon.ASSERT(EarthTileHelper.CheckChunkIndex(emChunkId_Target));
        EarthTChunk stChunk_Target = stHandler.GetChunk(emChunkId_Target);
        GameCommon.ASSERT(stChunk_Target != null);
        GameCommon.ASSERT(v2PosInOneChunk_Target.x >= 0);
        GameCommon.ASSERT(v2PosInOneChunk_Target.y >= 0);

        if (emChunkId_Base == emChunkId_Target)
        {
            return GetTileCoordinate(fXInBigEarth_Target, fYInBigEarth_Target, stHandler);
        }
        else
        {
            //PS:把[Target]在[Base坐标系]下摆放出来

            EarthTChunk.ST_NeighborChunk stNeighborChunk = stChunk_Base.GetNeighborChunk(stChunk_Target.GetId());
            if (stNeighborChunk != null)
            {
                float fDirLineL = GetDeltaDisInChunk(v2PosInOneChunk_Target, stNeighborChunk.GetNeedRotateDegree(), EarthTHandler.EM_ChunkDirection.W);
                float fDirLineR = GetDeltaDisInChunk(v2PosInOneChunk_Target, stNeighborChunk.GetNeedRotateDegree(), EarthTHandler.EM_ChunkDirection.E);
                float fDirLineU = GetDeltaDisInChunk(v2PosInOneChunk_Target, stNeighborChunk.GetNeedRotateDegree(), EarthTHandler.EM_ChunkDirection.N);
                float fDirLineB = GetDeltaDisInChunk(v2PosInOneChunk_Target, stNeighborChunk.GetNeedRotateDegree(), EarthTHandler.EM_ChunkDirection.S);

                switch (stNeighborChunk.GetDir())
                {
                    case EarthTHandler.EM_ChunkDirection.N:
                        {
                            v2Ret.x = fDirLineL;
                            v2Ret.y = EarthTHandler.m_fConstOneChunkDimension + fDirLineB;
                        }
                        break;
                    case EarthTHandler.EM_ChunkDirection.E:
                        {
                            v2Ret.x = EarthTHandler.m_fConstOneChunkDimension + fDirLineL;
                            v2Ret.y = fDirLineB;
                        }
                        break;
                    case EarthTHandler.EM_ChunkDirection.S:
                        {
                            v2Ret.x = fDirLineL;
                            v2Ret.y = -fDirLineU;
                        }
                        break;
                    case EarthTHandler.EM_ChunkDirection.W:
                        {
                            v2Ret.x = -fDirLineR;
                            v2Ret.y = fDirLineB;
                        }
                        break;
                }

                return v2Ret;
            }
            else
            {
                //PS: 对于[正方体魔方],既然不是[相连接的邻居],则必然是[相隔一块的邻居]
                //PS：[Target]在[Base坐标系]下的任意四个方向之一,即对立面
                //PS: 计算出每一个对立面的结果（优先取距离最近的，距离相等则取N方向）

                Vector2 v2Ret_Base = GetTileCoordinate(fXInBigEarth_Base, fYInBigEarth_Base, stHandler);

                Vector2[] v2AryRet = new Vector2[(int)(EarthTHandler.EM_ChunkDirection.Max)];
                float[] fAryDis = new float[(int)(EarthTHandler.EM_ChunkDirection.Max)];
                int iR_Min = 0;
                float fDis_Min = float.MaxValue;

                for (int iR = 0; iR < v2AryRet.Length; iR++)
                {
                    EarthTHandler.EM_ChunkDirection emDir = (EarthTHandler.EM_ChunkDirection)(iR);

                    v2AryRet[iR] = new Vector2(0, 0);

                    EarthTChunk stChunk_Tmp = stChunk_Base;
                    float fCenterRotatedDegree = 0;

                    for (int _iTimes = 0; _iTimes < 2; _iTimes++)
                    {
                        EarthTChunk.ST_NeighborChunk _stNeighborChunk = stChunk_Tmp.GetNeighborChunk(emDir, fCenterRotatedDegree);
                        GameCommon.ASSERT(_stNeighborChunk != null);
                        stChunk_Tmp = _stNeighborChunk.GetChunk();
                        GameCommon.ASSERT(stChunk_Tmp != null);
                        fCenterRotatedDegree += _stNeighborChunk.GetNeedRotateDegree();
                    }

                    GameCommon.ASSERT(stChunk_Tmp != null);
                    GameCommon.ASSERT(stChunk_Tmp.GetId() == stChunk_Target.GetId());

                    //PS: 此时直接认定[Target]是[Base]的[emDir],只是中间夹着一个Chunk

                    float fDirLineL = GetDeltaDisInChunk(v2PosInOneChunk_Target, fCenterRotatedDegree, EarthTHandler.EM_ChunkDirection.W);
                    float fDirLineR = GetDeltaDisInChunk(v2PosInOneChunk_Target, fCenterRotatedDegree, EarthTHandler.EM_ChunkDirection.E);
                    float fDirLineU = GetDeltaDisInChunk(v2PosInOneChunk_Target, fCenterRotatedDegree, EarthTHandler.EM_ChunkDirection.N);
                    float fDirLineB = GetDeltaDisInChunk(v2PosInOneChunk_Target, fCenterRotatedDegree, EarthTHandler.EM_ChunkDirection.S);

                    switch (emDir)
                    {
                        case EarthTHandler.EM_ChunkDirection.N:
                            {
                                v2AryRet[iR].x = fDirLineL;
                                v2AryRet[iR].y = EarthTHandler.m_fConstOneChunkDimension + fDirLineB;
                                v2AryRet[iR].y += EarthTHandler.m_fConstOneChunkDimension;
                            }
                            break;
                        case EarthTHandler.EM_ChunkDirection.E:
                            {
                                v2AryRet[iR].x = EarthTHandler.m_fConstOneChunkDimension + fDirLineL;
                                v2AryRet[iR].x += EarthTHandler.m_fConstOneChunkDimension;
                                v2AryRet[iR].y = fDirLineB;
                            }
                            break;
                        case EarthTHandler.EM_ChunkDirection.S:
                            {
                                v2AryRet[iR].x = fDirLineL;
                                v2AryRet[iR].y = -fDirLineU;
                                v2AryRet[iR].y -= EarthTHandler.m_fConstOneChunkDimension;
                            }
                            break;
                        case EarthTHandler.EM_ChunkDirection.W:
                            {
                                v2AryRet[iR].x = -fDirLineR;
                                v2AryRet[iR].x -= EarthTHandler.m_fConstOneChunkDimension;
                                v2AryRet[iR].y = fDirLineB;
                            }
                            break;
                    }

                    fAryDis[iR] = Vector2.Distance(v2Ret_Base, v2AryRet[iR]);

                    if (fAryDis[iR] < fDis_Min)
                    {
                        fDis_Min = fAryDis[iR];
                        iR_Min = iR;
                    }
                }

                return v2AryRet[iR_Min];
            }
        }
    }













    //20190415
    public static bool TryMovingTo(
        EarthTHandler.EM_ChunkIndex emChunkId,
        Vector2 v2PosInOneChunk,
        float fChunkRotatedDegree,
        Vector2 v2MoveLocalDir,
        float fMoveDis,
        EarthTHandler stHandler,
        out EarthTHandler.EM_ChunkIndex emChunkId_Ret,
        out Vector2 v2PosInOneChunk_Ret,
        out float fChunkRotatedDegree_Ret,
        out Vector2 v2MoveLocalDir_Ret
        )
    {
        GameCommon.ASSERT(EarthTileHelper.CheckChunkIndex(emChunkId));

        GameCommon.ASSERT(v2PosInOneChunk.x >= 0 && v2PosInOneChunk.x <= EarthTHandler.m_fConstOneChunkDimension, "v2PosInChunk.x = " + v2PosInOneChunk.x);
        GameCommon.ASSERT(v2PosInOneChunk.y >= 0 && v2PosInOneChunk.y <= EarthTHandler.m_fConstOneChunkDimension, "v2PosInChunk.y = " + v2PosInOneChunk.y);

        float fNeedRotateDegree_Clamp = 0;
        int nRotate90DegreeTimes = EarthTileHelper.GetRotate90DegreeTimes(
            fChunkRotatedDegree,
            out fNeedRotateDegree_Clamp
            );

        GameCommon.ASSERT(nRotate90DegreeTimes >= 0 && nRotate90DegreeTimes <= 3);

        GameCommon.ASSERT(stHandler != null);


        emChunkId_Ret = EarthTHandler.EM_ChunkIndex.Invalid;
        v2PosInOneChunk_Ret = Vector2.zero;
        fChunkRotatedDegree_Ret = 0;
        v2MoveLocalDir_Ret = Vector2.zero;


        Vector2 v2TryMoveDis = v2MoveLocalDir * fMoveDis;
        float fX_New = v2PosInOneChunk.x + v2TryMoveDis.x;
        float fY_New = v2PosInOneChunk.y + v2TryMoveDis.y;
        if (fX_New >= 0 && fX_New <= EarthTHandler.m_fConstOneChunkDimension &&
            fY_New >= 0 && fY_New <= EarthTHandler.m_fConstOneChunkDimension
            )
        {
            emChunkId_Ret = emChunkId;
            v2PosInOneChunk_Ret.x = fX_New;
            v2PosInOneChunk_Ret.y = fY_New;
            fChunkRotatedDegree_Ret = fChunkRotatedDegree;
            v2MoveLocalDir_Ret = v2MoveLocalDir;
            //IsCrossChunk
            return false;
        }

        //从横纵移动Chunk的次数
        int nMoveChunkToH = 0;
        float fMoveChunkToH_Offset = 0;
        if (fX_New >= 0)
        {
            nMoveChunkToH = (int)(Mathf.Abs(fX_New) / EarthTHandler.m_fConstOneChunkDimension);

            fMoveChunkToH_Offset = fX_New % EarthTHandler.m_fConstOneChunkDimension;
        }
        else
        {
            nMoveChunkToH = (int)(Mathf.Abs(fX_New) / EarthTHandler.m_fConstOneChunkDimension);
            nMoveChunkToH = nMoveChunkToH * -1;
            nMoveChunkToH = nMoveChunkToH - 1;

            fMoveChunkToH_Offset = EarthTHandler.m_fConstOneChunkDimension + (fX_New % EarthTHandler.m_fConstOneChunkDimension);
        }

        //从纵纵移动Chunk的次数
        int nMoveChunkToV = 0;
        float fMoveChunkToV_Offset = 0;
        if (fY_New >= 0)
        {
            nMoveChunkToV = (int)(Mathf.Abs(fY_New) / EarthTHandler.m_fConstOneChunkDimension);

            fMoveChunkToV_Offset = fY_New % EarthTHandler.m_fConstOneChunkDimension;
        }
        else
        {
            nMoveChunkToV = (int)(Mathf.Abs(fY_New) / EarthTHandler.m_fConstOneChunkDimension);
            nMoveChunkToV = nMoveChunkToV * -1;
            nMoveChunkToV = nMoveChunkToV - 1;

            fMoveChunkToV_Offset = EarthTHandler.m_fConstOneChunkDimension + (fY_New % EarthTHandler.m_fConstOneChunkDimension);
        }

        //摆正Chunk开始位移
        emChunkId_Ret = emChunkId;
        EarthTChunk stChunk_Tmp = stHandler.GetChunk(emChunkId_Ret);
        GameCommon.ASSERT(stChunk_Tmp != null);
        float fCenterRotatedDegree = 0;

        for (int _iTimes = 0; _iTimes < Mathf.Abs(nMoveChunkToH); _iTimes++)
        {
            EarthTChunk.ST_NeighborChunk _stNeighborChunk = null;
            if (nMoveChunkToH > 0)
            {
                _stNeighborChunk = stChunk_Tmp.GetNeighborChunk(EarthTHandler.EM_ChunkDirection.E, fCenterRotatedDegree);
            }
            else
            {
                _stNeighborChunk = stChunk_Tmp.GetNeighborChunk(EarthTHandler.EM_ChunkDirection.W, fCenterRotatedDegree);
            }

            GameCommon.ASSERT(_stNeighborChunk != null);
            stChunk_Tmp = _stNeighborChunk.GetChunk();
            GameCommon.ASSERT(stChunk_Tmp != null);
            fCenterRotatedDegree += _stNeighborChunk.GetNeedRotateDegree();
        }
        for (int _iTimes = 0; _iTimes < Mathf.Abs(nMoveChunkToV); _iTimes++)
        {
            EarthTChunk.ST_NeighborChunk _stNeighborChunk = null;
            if (nMoveChunkToV > 0)
            {
                _stNeighborChunk = stChunk_Tmp.GetNeighborChunk(EarthTHandler.EM_ChunkDirection.N, fCenterRotatedDegree);
            }
            else
            {
                _stNeighborChunk = stChunk_Tmp.GetNeighborChunk(EarthTHandler.EM_ChunkDirection.S, fCenterRotatedDegree);
            }

            GameCommon.ASSERT(_stNeighborChunk != null);
            stChunk_Tmp = _stNeighborChunk.GetChunk();
            GameCommon.ASSERT(stChunk_Tmp != null);
            fCenterRotatedDegree += _stNeighborChunk.GetNeedRotateDegree();
        }

        GameCommon.ASSERT(stChunk_Tmp != null);

        emChunkId_Ret = stChunk_Tmp.GetId();

        fChunkRotatedDegree_Ret = GetClampRotateDegree(fCenterRotatedDegree) + fChunkRotatedDegree;
        fChunkRotatedDegree_Ret = GetClampRotateDegree(fChunkRotatedDegree_Ret);

        v2PosInOneChunk_Ret = Vector2.zero;
        EarthTileHelper.GetPosOffsetInChunkDimension(
            fMoveChunkToH_Offset, fMoveChunkToV_Offset,
            fCenterRotatedDegree, ref v2PosInOneChunk_Ret
            );

        v2MoveLocalDir_Ret = GetRotatedDirInChunk(fCenterRotatedDegree, v2MoveLocalDir);

        return true;
    }

    public static Vector2 GetRotatedDirInChunk(
        float fTargetChunkRotatedDegree,
        Vector2 v2LocalMoveDir
        )
    {
        //PS: 01.摆正初始Chunk以[v2LocalMoveDir]方向，跨入到一个已经转动[fTargetChunkRotatedDegree]的目标Chunk中
        //PS: 02.最终计算出该[v2LocalMoveDir]在[目标Chunk]中的LocalMoveDir

        float fNeedRotateDegree_Clamp = 0;
        int nRotate90DegreeTimes = EarthTileHelper.GetRotate90DegreeTimes(
            fTargetChunkRotatedDegree,
            out fNeedRotateDegree_Clamp
            );

        GameCommon.ASSERT(nRotate90DegreeTimes >= 0 && nRotate90DegreeTimes <= 3);

        if (nRotate90DegreeTimes == 0)
        {
            return v2LocalMoveDir;
        }

        float fDirX_Abs = Math.Abs(v2LocalMoveDir.x);
        float fDirY_Abs = Math.Abs(v2LocalMoveDir.y);

        if (v2LocalMoveDir.x >= 0 && v2LocalMoveDir.y >= 0)
        {
            if (nRotate90DegreeTimes == 1)
            {
                return new Vector2(-fDirY_Abs, fDirX_Abs);
            }
            else if (nRotate90DegreeTimes == 2)
            {
                return new Vector2(-fDirX_Abs, -fDirY_Abs);
            }
            else if (nRotate90DegreeTimes == 3)
            {
                return new Vector2(fDirY_Abs, -fDirX_Abs);
            }
        }

        if (v2LocalMoveDir.x < 0 && v2LocalMoveDir.y >= 0)
        {
            if (nRotate90DegreeTimes == 1)
            {
                return new Vector2(-fDirY_Abs, -fDirX_Abs);
            }
            else if (nRotate90DegreeTimes == 2)
            {
                return new Vector2(fDirX_Abs, -fDirY_Abs);
            }
            else if (nRotate90DegreeTimes == 3)
            {
                return new Vector2(fDirY_Abs, fDirX_Abs);
            }
        }

        if (v2LocalMoveDir.x < 0 && v2LocalMoveDir.y < 0)
        {
            if (nRotate90DegreeTimes == 1)
            {
                return new Vector2(fDirY_Abs, -fDirX_Abs);
            }
            else if (nRotate90DegreeTimes == 2)
            {
                return new Vector2(fDirX_Abs, fDirY_Abs);
            }
            else if (nRotate90DegreeTimes == 3)
            {
                return new Vector2(-fDirY_Abs, fDirX_Abs);
            }
        }

        if (v2LocalMoveDir.x >= 0 && v2LocalMoveDir.y < 0)
        {
            if (nRotate90DegreeTimes == 1)
            {
                return new Vector2(fDirY_Abs, fDirX_Abs);
            }
            else if (nRotate90DegreeTimes == 2)
            {
                return new Vector2(-fDirX_Abs, fDirY_Abs);
            }
            else if (nRotate90DegreeTimes == 3)
            {
                return new Vector2(-fDirY_Abs, -fDirX_Abs);
            }
        }

        Debug.LogError("GetRotatedDirInChunk: " + fTargetChunkRotatedDegree + " | " + v2LocalMoveDir);
        return Vector2.zero;
    }

    public static Vector2 GetShipRotatedDirInChunk(
        float fTargetChunkRotatedDegree,
        Vector2 v2LocalMoveDir
        )
    {
        //PS: 01.摆正初始Chunk以[v2LocalMoveDir]为方向
        //PS: 02.整个画面旋转[fTargetChunkRotatedDegree],求出新的Dir(原点即旋转前的原点)

        float fNeedRotateDegree_Clamp = 0;
        int nRotate90DegreeTimes = EarthTileHelper.GetRotate90DegreeTimes(
            fTargetChunkRotatedDegree,
            out fNeedRotateDegree_Clamp
            );

        GameCommon.ASSERT(nRotate90DegreeTimes >= 0 && nRotate90DegreeTimes <= 3);

        if (nRotate90DegreeTimes == 0)
        {
            return v2LocalMoveDir;
        }

        float fDirX_Abs = Math.Abs(v2LocalMoveDir.x);
        float fDirY_Abs = Math.Abs(v2LocalMoveDir.y);

        if (v2LocalMoveDir.x >= 0 && v2LocalMoveDir.y >= 0)
        {
            if (nRotate90DegreeTimes == 1)
            {
                return new Vector2(fDirY_Abs, -fDirX_Abs);
            }
            else if (nRotate90DegreeTimes == 2)
            {
                return new Vector2(-fDirX_Abs, -fDirY_Abs);
            }
            else if (nRotate90DegreeTimes == 3)
            {
                return new Vector2(-fDirY_Abs, fDirX_Abs);
            }
        }

        if (v2LocalMoveDir.x < 0 && v2LocalMoveDir.y >= 0)
        {
            if (nRotate90DegreeTimes == 1)
            {
                return new Vector2(fDirY_Abs, fDirX_Abs);
            }
            else if (nRotate90DegreeTimes == 2)
            {
                return new Vector2(fDirX_Abs, -fDirY_Abs);
            }
            else if (nRotate90DegreeTimes == 3)
            {
                return new Vector2(-fDirY_Abs, -fDirX_Abs);
            }
        }

        if (v2LocalMoveDir.x < 0 && v2LocalMoveDir.y < 0)
        {
            if (nRotate90DegreeTimes == 1)
            {
                return new Vector2(-fDirY_Abs, fDirX_Abs);
            }
            else if (nRotate90DegreeTimes == 2)
            {
                return new Vector2(fDirX_Abs, fDirY_Abs);
            }
            else if (nRotate90DegreeTimes == 3)
            {
                return new Vector2(fDirY_Abs, -fDirX_Abs);
            }
        }

        if (v2LocalMoveDir.x >= 0 && v2LocalMoveDir.y < 0)
        {
            if (nRotate90DegreeTimes == 1)
            {
                return new Vector2(-fDirY_Abs, -fDirX_Abs);
            }
            else if (nRotate90DegreeTimes == 2)
            {
                return new Vector2(-fDirX_Abs, fDirY_Abs);
            }
            else if (nRotate90DegreeTimes == 3)
            {
                return new Vector2(fDirY_Abs, fDirX_Abs);
            }
        }

        Debug.LogError("GetShipRotatedDirInChunk: " + fTargetChunkRotatedDegree + " | " + v2LocalMoveDir);
        return Vector2.zero;
    }

    public static void GetChunkAreaByOverflowPosition(
        EarthTHandler.EM_ChunkIndex emChunkId,
        float fChunkRotatedDegree,
        Vector2 v2PosInOneChunk,
        EarthTHandler stHandler,
        out EarthTHandler.EM_ChunkIndex emChunkId_Ret,
        out float fChunkRotatedDegree_Ret,
        out Vector2 v2PosInOneChunk_Ret
        )
    {
        //PS: 通过溢出坐标数值，计算非溢出真实坐标数值
        GameCommon.ASSERT(stHandler != null);

        float fX = v2PosInOneChunk.x;
        float fY = v2PosInOneChunk.y;

        //从横纵移动Chunk的次数
        int nMoveChunkToH = (int)(fX / EarthTHandler.m_fConstOneChunkDimension);
        float fMoveChunkToH_Offset = fX % EarthTHandler.m_fConstOneChunkDimension;
        if (fMoveChunkToH_Offset >= 0)
        {
            nMoveChunkToH = (int)(Mathf.Abs(fX) / EarthTHandler.m_fConstOneChunkDimension);
        }
        else
        {
            nMoveChunkToH = (int)(Mathf.Abs(fX) / EarthTHandler.m_fConstOneChunkDimension);
            nMoveChunkToH = nMoveChunkToH * -1;
            nMoveChunkToH = nMoveChunkToH - 1;

            fMoveChunkToH_Offset = EarthTHandler.m_fConstOneChunkDimension + fMoveChunkToH_Offset;
        }

        //从纵纵移动Chunk的次数
        int nMoveChunkToV = (int)(fY / EarthTHandler.m_fConstOneChunkDimension);
        float fMoveChunkToV_Offset = fY % EarthTHandler.m_fConstOneChunkDimension;
        if (fMoveChunkToV_Offset >= 0)
        {
            nMoveChunkToV = (int)(Mathf.Abs(fY) / EarthTHandler.m_fConstOneChunkDimension);
        }
        else
        {
            nMoveChunkToV = (int)(Mathf.Abs(fY) / EarthTHandler.m_fConstOneChunkDimension);
            nMoveChunkToV = nMoveChunkToV * -1;
            nMoveChunkToV = nMoveChunkToV - 1;

            fMoveChunkToV_Offset = EarthTHandler.m_fConstOneChunkDimension + fMoveChunkToV_Offset;
        }

        //准备模拟移动
        EarthTChunk stChunk_Tmp = stHandler.GetChunk((EarthTHandler.EM_ChunkIndex)(emChunkId));
        GameCommon.ASSERT(stChunk_Tmp != null);
        float fCenterRotatedDegree = fChunkRotatedDegree;

        for (int _iTimes = 0; _iTimes < Mathf.Abs(nMoveChunkToH); _iTimes++)
        {
            EarthTChunk.ST_NeighborChunk _stNeighborChunk = null;
            if (nMoveChunkToH > 0)
            {
                _stNeighborChunk = stChunk_Tmp.GetNeighborChunk(EarthTHandler.EM_ChunkDirection.E, fCenterRotatedDegree);
            }
            else
            {
                _stNeighborChunk = stChunk_Tmp.GetNeighborChunk(EarthTHandler.EM_ChunkDirection.W, fCenterRotatedDegree);
            }

            GameCommon.ASSERT(_stNeighborChunk != null);
            stChunk_Tmp = _stNeighborChunk.GetChunk();
            GameCommon.ASSERT(stChunk_Tmp != null);
            fCenterRotatedDegree += _stNeighborChunk.GetNeedRotateDegree();
        }
        for (int _iTimes = 0; _iTimes < Mathf.Abs(nMoveChunkToV); _iTimes++)
        {
            EarthTChunk.ST_NeighborChunk _stNeighborChunk = null;
            if (nMoveChunkToV > 0)
            {
                _stNeighborChunk = stChunk_Tmp.GetNeighborChunk(EarthTHandler.EM_ChunkDirection.N, fCenterRotatedDegree);
            }
            else
            {
                _stNeighborChunk = stChunk_Tmp.GetNeighborChunk(EarthTHandler.EM_ChunkDirection.S, fCenterRotatedDegree);
            }

            GameCommon.ASSERT(_stNeighborChunk != null);
            stChunk_Tmp = _stNeighborChunk.GetChunk();
            GameCommon.ASSERT(stChunk_Tmp != null);
            fCenterRotatedDegree += _stNeighborChunk.GetNeedRotateDegree();
        }

        GameCommon.ASSERT(stChunk_Tmp != null);

        emChunkId_Ret = stChunk_Tmp.GetId();
        fChunkRotatedDegree_Ret = fCenterRotatedDegree;
        v2PosInOneChunk_Ret = Vector2.zero;
        EarthTileHelper.GetPosOffsetInChunkDimension(
            fMoveChunkToH_Offset, fMoveChunkToV_Offset,
            fCenterRotatedDegree, ref v2PosInOneChunk_Ret
            );
    }

    public static string GetAreaTileName(EarthTHandler.EM_ChunkIndex emChunkId, int nAreaId)
    {
        //tile_0_108
        return string.Format("tile_{0}_{1}", (int)(emChunkId), nAreaId);
    }

    public static string GetAreaTileName(EarthTArea stAreaData)
    {
        GameCommon.ASSERT(stAreaData != null);
        return GetAreaTileName(stAreaData.GetParentChunk().GetId(), stAreaData.GetId());
    }

    public static string GetAreaHitMeshCollideName(EarthTHandler.EM_ChunkIndex emChunkId, int nAreaId)
    {
        //Mesh_tile_5_collide_136
        return string.Format("Mesh_tile_{0}_collide_{1}", (int)(emChunkId), nAreaId);
    }

    public static string GetAreaHitMeshCollideName(EarthTArea stAreaData)
    {
        GameCommon.ASSERT(stAreaData != null);
        return GetAreaHitMeshCollideName(stAreaData.GetParentChunk().GetId(), stAreaData.GetId());
    }

    #region IXY坐标系下获取方向
    public static Vector2 GetTileCoordinate(
        EarthTHandler.EM_ChunkIndex emChunkId_Base,
        float fChunkRotatedDegree_Base,
        Vector2 v2PosInOneChunk_Base,
        EarthTHandler stHandler
        )
    {
        GameCommon.ASSERT(stHandler != null);

        Vector2 v2Ret = Vector2.zero;

        GameCommon.ASSERT(EarthTileHelper.CheckChunkIndex(emChunkId_Base));
        EarthTChunk stChunk_Base = stHandler.GetChunk(emChunkId_Base);
        GameCommon.ASSERT(stChunk_Base != null);
        GameCommon.ASSERT(v2PosInOneChunk_Base.x >= 0);
        GameCommon.ASSERT(v2PosInOneChunk_Base.y >= 0);


        //以stChunk_Base的摆正角度 建立坐标系

        float fNeedRotateDegree_Clamp = 0;
        int nRotate90DegreeTimes = EarthTileHelper.GetRotate90DegreeTimes(
            fChunkRotatedDegree_Base,
            out fNeedRotateDegree_Clamp
            );

        GameCommon.ASSERT(nRotate90DegreeTimes >= 0 && nRotate90DegreeTimes <= 3);

        v2Ret = v2PosInOneChunk_Base;
        return v2Ret;
    }

    public static Vector2 GetTileCoordinate(
        EarthTHandler.EM_ChunkIndex emChunkId_Base,
        float fChunkRotatedDegree_Base,
        Vector2 v2PosInOneChunk_Base,
        EarthTHandler.EM_ChunkIndex emChunkId_Target,
        float fChunkRotatedDegree_Target,
        Vector2 v2PosInOneChunk_Target,
        EarthTHandler stHandler
        )
    {
        GameCommon.ASSERT(stHandler != null);

        Vector2 v2Ret = Vector2.zero;

        GameCommon.ASSERT(EarthTileHelper.CheckChunkIndex(emChunkId_Base));
        EarthTChunk stChunk_Base = stHandler.GetChunk(emChunkId_Base);
        GameCommon.ASSERT(stChunk_Base != null);
        GameCommon.ASSERT(v2PosInOneChunk_Base.x >= 0);
        GameCommon.ASSERT(v2PosInOneChunk_Base.y >= 0);

        GameCommon.ASSERT(EarthTileHelper.CheckChunkIndex(emChunkId_Target));
        EarthTChunk stChunk_Target = stHandler.GetChunk(emChunkId_Target);
        GameCommon.ASSERT(stChunk_Target != null);
        GameCommon.ASSERT(v2PosInOneChunk_Target.x >= 0);
        GameCommon.ASSERT(v2PosInOneChunk_Target.y >= 0);

        if (emChunkId_Base == emChunkId_Target)
        {
            return GetTileCoordinate(
                emChunkId_Target,
                fChunkRotatedDegree_Target,
                v2PosInOneChunk_Target,
                stHandler
                );
        }
        else
        {
            //PS:把[Target]在[Base坐标系]下摆放出来

            EarthTChunk.ST_NeighborChunk stNeighborChunk = stChunk_Base.GetNeighborChunk(stChunk_Target.GetId());
            if (stNeighborChunk != null)
            {
                float fDirLineL = GetDeltaDisInChunk(v2PosInOneChunk_Target, stNeighborChunk.GetNeedRotateDegree(), EarthTHandler.EM_ChunkDirection.W);
                float fDirLineR = GetDeltaDisInChunk(v2PosInOneChunk_Target, stNeighborChunk.GetNeedRotateDegree(), EarthTHandler.EM_ChunkDirection.E);
                float fDirLineU = GetDeltaDisInChunk(v2PosInOneChunk_Target, stNeighborChunk.GetNeedRotateDegree(), EarthTHandler.EM_ChunkDirection.N);
                float fDirLineB = GetDeltaDisInChunk(v2PosInOneChunk_Target, stNeighborChunk.GetNeedRotateDegree(), EarthTHandler.EM_ChunkDirection.S);

                switch (stNeighborChunk.GetDir())
                {
                    case EarthTHandler.EM_ChunkDirection.N:
                        {
                            v2Ret.x = fDirLineL;
                            v2Ret.y = EarthTHandler.m_fConstOneChunkDimension + fDirLineB;
                        }
                        break;
                    case EarthTHandler.EM_ChunkDirection.E:
                        {
                            v2Ret.x = EarthTHandler.m_fConstOneChunkDimension + fDirLineL;
                            v2Ret.y = fDirLineB;
                        }
                        break;
                    case EarthTHandler.EM_ChunkDirection.S:
                        {
                            v2Ret.x = fDirLineL;
                            v2Ret.y = -fDirLineU;
                        }
                        break;
                    case EarthTHandler.EM_ChunkDirection.W:
                        {
                            v2Ret.x = -fDirLineR;
                            v2Ret.y = fDirLineB;
                        }
                        break;
                }

                return v2Ret;
            }
            else
            {
                //PS: 对于[正方体魔方],既然不是[相连接的邻居],则必然是[相隔一块的邻居]
                //PS：[Target]在[Base坐标系]下的任意四个方向之一,即对立面
                //PS: 计算出每一个对立面的结果（优先取距离最近的，距离相等则取N方向）

                Vector2 v2Ret_Base = GetTileCoordinate(
                    emChunkId_Base,
                    fChunkRotatedDegree_Base,
                    v2PosInOneChunk_Base,
                    stHandler
                    );

                Vector2[] v2AryRet = new Vector2[(int)(EarthTHandler.EM_ChunkDirection.Max)];
                float[] fAryDis = new float[(int)(EarthTHandler.EM_ChunkDirection.Max)];
                int iR_Min = 0;
                float fDis_Min = float.MaxValue;

                for (int iR = 0; iR < v2AryRet.Length; iR++)
                {
                    EarthTHandler.EM_ChunkDirection emDir = (EarthTHandler.EM_ChunkDirection)(iR);

                    v2AryRet[iR] = new Vector2(0, 0);

                    EarthTChunk stChunk_Tmp = stChunk_Base;
                    float fCenterRotatedDegree = 0;

                    for (int _iTimes = 0; _iTimes < 2; _iTimes++)
                    {
                        EarthTChunk.ST_NeighborChunk _stNeighborChunk = stChunk_Tmp.GetNeighborChunk(emDir, fCenterRotatedDegree);
                        GameCommon.ASSERT(_stNeighborChunk != null);
                        stChunk_Tmp = _stNeighborChunk.GetChunk();
                        GameCommon.ASSERT(stChunk_Tmp != null);
                        fCenterRotatedDegree += _stNeighborChunk.GetNeedRotateDegree();
                    }

                    GameCommon.ASSERT(stChunk_Tmp != null);
                    GameCommon.ASSERT(stChunk_Tmp.GetId() == stChunk_Target.GetId());

                    //PS: 此时直接认定[Target]是[Base]的[emDir],只是中间夹着一个Chunk

                    float fDirLineL = GetDeltaDisInChunk(v2PosInOneChunk_Target, fCenterRotatedDegree, EarthTHandler.EM_ChunkDirection.W);
                    float fDirLineR = GetDeltaDisInChunk(v2PosInOneChunk_Target, fCenterRotatedDegree, EarthTHandler.EM_ChunkDirection.E);
                    float fDirLineU = GetDeltaDisInChunk(v2PosInOneChunk_Target, fCenterRotatedDegree, EarthTHandler.EM_ChunkDirection.N);
                    float fDirLineB = GetDeltaDisInChunk(v2PosInOneChunk_Target, fCenterRotatedDegree, EarthTHandler.EM_ChunkDirection.S);

                    switch (emDir)
                    {
                        case EarthTHandler.EM_ChunkDirection.N:
                            {
                                v2AryRet[iR].x = fDirLineL;
                                v2AryRet[iR].y = EarthTHandler.m_fConstOneChunkDimension + fDirLineB;
                                v2AryRet[iR].y += EarthTHandler.m_fConstOneChunkDimension;
                            }
                            break;
                        case EarthTHandler.EM_ChunkDirection.E:
                            {
                                v2AryRet[iR].x = EarthTHandler.m_fConstOneChunkDimension + fDirLineL;
                                v2AryRet[iR].x += EarthTHandler.m_fConstOneChunkDimension;
                                v2AryRet[iR].y = fDirLineB;
                            }
                            break;
                        case EarthTHandler.EM_ChunkDirection.S:
                            {
                                v2AryRet[iR].x = fDirLineL;
                                v2AryRet[iR].y = -fDirLineU;
                                v2AryRet[iR].y -= EarthTHandler.m_fConstOneChunkDimension;
                            }
                            break;
                        case EarthTHandler.EM_ChunkDirection.W:
                            {
                                v2AryRet[iR].x = -fDirLineR;
                                v2AryRet[iR].x -= EarthTHandler.m_fConstOneChunkDimension;
                                v2AryRet[iR].y = fDirLineB;
                            }
                            break;
                    }

                    fAryDis[iR] = Vector2.Distance(v2Ret_Base, v2AryRet[iR]);

                    if (fAryDis[iR] < fDis_Min)
                    {
                        fDis_Min = fAryDis[iR];
                        iR_Min = iR;
                    }
                }

                return v2AryRet[iR_Min];
            }
        }
    }

    public static Vector2 GetDirectionByIXY(
        EarthTHandler.EM_ChunkIndex emChunkId_Base,
        float fChunkRotatedDegree_Base,
        Vector2 v2PosInOneChunk_Base,
        EarthTHandler.EM_ChunkIndex emChunkId_Target,
        float fChunkRotatedDegree_Target,
        Vector2 v2PosInOneChunk_Target,
        EarthTHandler stHandler
        )
    {
        Vector2 v2PosWithTileCoordinate_1 = GetTileCoordinate(
            emChunkId_Base, fChunkRotatedDegree_Base, v2PosInOneChunk_Base,
            stHandler
            );

        Vector2 v2PosWithTileCoordinate_2 = GetTileCoordinate(
            emChunkId_Base, fChunkRotatedDegree_Base, v2PosInOneChunk_Base,
            emChunkId_Target, fChunkRotatedDegree_Target, v2PosInOneChunk_Target,
            stHandler
            );

        return v2PosWithTileCoordinate_2 - v2PosWithTileCoordinate_1;
    }

    #endregion IXY坐标系下获取方向



    #region Coordinate+Rotate
    public static Vector2 CalcCoordinateWithRotate(
        int nRotate90DegreeTimes,
        Vector2 v2CoordinateAfterRotate,
        float fBeforeRotateSizeX, float fBeforeRotateSizeY
        )
    {
        /*
            从[旋转后坐标系]下的坐标，反算到[不旋转坐标系]下的坐标
        */

        GameCommon.ASSERT(nRotate90DegreeTimes >= 0 && nRotate90DegreeTimes <= 3);
        GameCommon.ASSERT(fBeforeRotateSizeX > 0);
        GameCommon.ASSERT(fBeforeRotateSizeY > 0);

        if (nRotate90DegreeTimes == 0)
        {
            return v2CoordinateAfterRotate;
        }
        else if (nRotate90DegreeTimes == 1)
        {
            return new Vector2(
                fBeforeRotateSizeX - v2CoordinateAfterRotate.y,
                v2CoordinateAfterRotate.x
                );
        }
        else if (nRotate90DegreeTimes == 2)
        {
            return new Vector2(
                fBeforeRotateSizeX - v2CoordinateAfterRotate.x,
                fBeforeRotateSizeY - v2CoordinateAfterRotate.y
                );
        }
        else if (nRotate90DegreeTimes == 3)
        {
            return new Vector2(
                v2CoordinateAfterRotate.y,
                fBeforeRotateSizeY - v2CoordinateAfterRotate.x
                );
        }

        return Vector2.zero;
    }

    public static Vector2 CalcCoordinateWithRotate_Opposite(
        int nRotate90DegreeTimes,
        Vector2 v2CoordinateBeforeRotate,
        float fBeforeRotateSizeX, float fBeforeRotateSizeY
        )
    {
        /*
            从[不旋转坐标系]下的坐标，反算到[旋转后坐标系]下的坐标
        */

        GameCommon.ASSERT(nRotate90DegreeTimes >= 0 && nRotate90DegreeTimes <= 3);
        GameCommon.ASSERT(fBeforeRotateSizeX > 0);
        GameCommon.ASSERT(fBeforeRotateSizeY > 0);

        if (nRotate90DegreeTimes == 0)
        {
            return v2CoordinateBeforeRotate;
        }
        else if (nRotate90DegreeTimes == 1)
        {
            return new Vector2(
                v2CoordinateBeforeRotate.y,
                fBeforeRotateSizeX - v2CoordinateBeforeRotate.x
                );
        }
        else if (nRotate90DegreeTimes == 2)
        {
            return new Vector2(
                fBeforeRotateSizeX - v2CoordinateBeforeRotate.x,
                fBeforeRotateSizeY - v2CoordinateBeforeRotate.y
                );
        }
        else if (nRotate90DegreeTimes == 3)
        {
            return new Vector2(
                fBeforeRotateSizeY - v2CoordinateBeforeRotate.y,
                v2CoordinateBeforeRotate.x
                );
        }

        return Vector2.zero;
    }

    public static Vector2 GetCoordinateSizeWithRotate(
        int nRotate90DegreeTimes,
        Vector2 v2CoordinateSizeBeforeRotate
        )
    {
        /*
            从[不旋转坐标系]下的坐标轴XY尺寸，反算到[旋转后坐标系]下的坐标轴XY尺寸
        */

        GameCommon.ASSERT(nRotate90DegreeTimes >= 0 && nRotate90DegreeTimes <= 3);
        GameCommon.ASSERT(v2CoordinateSizeBeforeRotate.x > 0);
        GameCommon.ASSERT(v2CoordinateSizeBeforeRotate.y > 0);

        if (nRotate90DegreeTimes == 0)
        {
            return v2CoordinateSizeBeforeRotate;
        }
        else if (nRotate90DegreeTimes == 1)
        {
            return new Vector2(
                v2CoordinateSizeBeforeRotate.y,
                v2CoordinateSizeBeforeRotate.x
                );
        }
        else if (nRotate90DegreeTimes == 2)
        {
            return v2CoordinateSizeBeforeRotate;
        }
        else if (nRotate90DegreeTimes == 3)
        {
            return new Vector2(
                v2CoordinateSizeBeforeRotate.y,
                v2CoordinateSizeBeforeRotate.x
                );
        }

        return v2CoordinateSizeBeforeRotate;
    }
    #endregion Coordinate+Rotate
};