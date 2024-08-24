using System;
using UnityEngine;

public class EarthTChunk
{
    EarthTHandler m_stHandler;
    EarthTHandler.EM_ChunkIndex m_nId;
    
    EarthTArea[] m_aryAreas = new EarthTArea[EarthTileHelper.GetOneChunkAreaMaxNum()];

    public class ST_NeighborChunk
    {
        EarthTHandler.EM_ChunkDirection emDir;
        EarthTChunk stChunk;

        /*
        针对[模型片]旋转:
        01.顺时针：正数(如若针对[UI片]，则刚好相反)
        02.顺时针：负数(如若针对[UI片]，则刚好相反)
        03.Y轴(如若针对[UI片]，则Z轴)
        */
        float fNeedRotateDegree;

        public ST_NeighborChunk(
            EarthTHandler.EM_ChunkDirection emDir, 
            EarthTChunk stChunk, 
            float fNeedRotateDegree
            )
        {
            GameCommon.ASSERT(EarthTileHelper.CheckChunkDirection(emDir));
            GameCommon.ASSERT(stChunk != null);

            this.emDir = emDir;
            this.stChunk = stChunk;
            this.fNeedRotateDegree = fNeedRotateDegree;
        }

        public EarthTHandler.EM_ChunkDirection GetDir() { return emDir; }
        public EarthTChunk GetChunk() { return stChunk; }
        public float GetNeedRotateDegree() { return fNeedRotateDegree; }
    };

    ST_NeighborChunk[] m_aryNeighborChunks = new ST_NeighborChunk[(int)(EarthTHandler.EM_ChunkDirection.Max)];











    #region HardCode(硬代码)

    bool m_bIsInitHardCode_NeighborChunks = false;
    public void InitHardCode_NeighborChunks()
    {
        GameCommon.ASSERT(m_stHandler != null);
        GameCommon.ASSERT(!m_bIsInitHardCode_NeighborChunks);

        switch (m_nId)
        {
            case EarthTHandler.EM_ChunkIndex.Zero:
                {
                    InitHardCode_NeighborChunks_AutoAlloc(
                        ref m_aryNeighborChunks,
                        EarthTHandler.EM_ChunkDirection.N,
                        EarthTHandler.EM_ChunkIndex.Four,
                        180
                        );
                    InitHardCode_NeighborChunks_AutoAlloc(
                        ref m_aryNeighborChunks,
                        EarthTHandler.EM_ChunkDirection.E,
                        EarthTHandler.EM_ChunkIndex.One,
                        0
                        );
                    InitHardCode_NeighborChunks_AutoAlloc(
                        ref m_aryNeighborChunks,
                        EarthTHandler.EM_ChunkDirection.S,
                        EarthTHandler.EM_ChunkIndex.Five,
                        180
                        );
                    InitHardCode_NeighborChunks_AutoAlloc(
                        ref m_aryNeighborChunks,
                        EarthTHandler.EM_ChunkDirection.W,
                        EarthTHandler.EM_ChunkIndex.Three,
                        0
                        );
                }
                break;
            case EarthTHandler.EM_ChunkIndex.One:
                {
                    InitHardCode_NeighborChunks_AutoAlloc(
                        ref m_aryNeighborChunks,
                        EarthTHandler.EM_ChunkDirection.N,
                        EarthTHandler.EM_ChunkIndex.Four,
                        -90
                        );
                    InitHardCode_NeighborChunks_AutoAlloc(
                        ref m_aryNeighborChunks,
                        EarthTHandler.EM_ChunkDirection.E,
                        EarthTHandler.EM_ChunkIndex.Two,
                        0
                        );
                    InitHardCode_NeighborChunks_AutoAlloc(
                        ref m_aryNeighborChunks,
                        EarthTHandler.EM_ChunkDirection.S,
                        EarthTHandler.EM_ChunkIndex.Five,
                        90
                        );
                    InitHardCode_NeighborChunks_AutoAlloc(
                        ref m_aryNeighborChunks,
                        EarthTHandler.EM_ChunkDirection.W,
                        EarthTHandler.EM_ChunkIndex.Zero,
                        0
                        );
                }
                break;
            case EarthTHandler.EM_ChunkIndex.Two:
                {
                    InitHardCode_NeighborChunks_AutoAlloc(
                        ref m_aryNeighborChunks,
                        EarthTHandler.EM_ChunkDirection.N,
                        EarthTHandler.EM_ChunkIndex.Four,
                        0
                        );
                    InitHardCode_NeighborChunks_AutoAlloc(
                        ref m_aryNeighborChunks,
                        EarthTHandler.EM_ChunkDirection.E,
                        EarthTHandler.EM_ChunkIndex.Three,
                        0
                        );
                    InitHardCode_NeighborChunks_AutoAlloc(
                        ref m_aryNeighborChunks,
                        EarthTHandler.EM_ChunkDirection.S,
                        EarthTHandler.EM_ChunkIndex.Five,
                        0
                        );
                    InitHardCode_NeighborChunks_AutoAlloc(
                        ref m_aryNeighborChunks,
                        EarthTHandler.EM_ChunkDirection.W,
                        EarthTHandler.EM_ChunkIndex.One,
                        0
                        );
                }
                break;
            case EarthTHandler.EM_ChunkIndex.Three:
                {
                    InitHardCode_NeighborChunks_AutoAlloc(
                        ref m_aryNeighborChunks,
                        EarthTHandler.EM_ChunkDirection.N,
                        EarthTHandler.EM_ChunkIndex.Four,
                        90
                        );
                    InitHardCode_NeighborChunks_AutoAlloc(
                        ref m_aryNeighborChunks,
                        EarthTHandler.EM_ChunkDirection.E,
                        EarthTHandler.EM_ChunkIndex.Zero,
                        0
                        );
                    InitHardCode_NeighborChunks_AutoAlloc(
                        ref m_aryNeighborChunks,
                        EarthTHandler.EM_ChunkDirection.S,
                        EarthTHandler.EM_ChunkIndex.Five,
                        -90
                        );
                    InitHardCode_NeighborChunks_AutoAlloc(
                        ref m_aryNeighborChunks,
                        EarthTHandler.EM_ChunkDirection.W,
                        EarthTHandler.EM_ChunkIndex.Two,
                        0
                        );
                }
                break;
            case EarthTHandler.EM_ChunkIndex.Four:
                {
                    InitHardCode_NeighborChunks_AutoAlloc(
                        ref m_aryNeighborChunks,
                        EarthTHandler.EM_ChunkDirection.N,
                        EarthTHandler.EM_ChunkIndex.Zero,
                        180
                        );
                    InitHardCode_NeighborChunks_AutoAlloc(
                        ref m_aryNeighborChunks,
                        EarthTHandler.EM_ChunkDirection.E,
                        EarthTHandler.EM_ChunkIndex.Three,
                        -90
                        );
                    InitHardCode_NeighborChunks_AutoAlloc(
                        ref m_aryNeighborChunks,
                        EarthTHandler.EM_ChunkDirection.S,
                        EarthTHandler.EM_ChunkIndex.Two,
                        0
                        );
                    InitHardCode_NeighborChunks_AutoAlloc(
                        ref m_aryNeighborChunks,
                        EarthTHandler.EM_ChunkDirection.W,
                        EarthTHandler.EM_ChunkIndex.One,
                        90
                        );                 
                }
                break;
            case EarthTHandler.EM_ChunkIndex.Five:
                {
                    InitHardCode_NeighborChunks_AutoAlloc(
                        ref m_aryNeighborChunks,
                        EarthTHandler.EM_ChunkDirection.N,
                        EarthTHandler.EM_ChunkIndex.Two,
                        0
                        );
                    InitHardCode_NeighborChunks_AutoAlloc(
                        ref m_aryNeighborChunks,
                        EarthTHandler.EM_ChunkDirection.E,
                        EarthTHandler.EM_ChunkIndex.Three,
                        90
                        );
                    InitHardCode_NeighborChunks_AutoAlloc(
                        ref m_aryNeighborChunks,
                        EarthTHandler.EM_ChunkDirection.S,
                        EarthTHandler.EM_ChunkIndex.Zero,
                        180
                        );
                    InitHardCode_NeighborChunks_AutoAlloc(
                        ref m_aryNeighborChunks,
                        EarthTHandler.EM_ChunkDirection.W,
                        EarthTHandler.EM_ChunkIndex.One,
                        -90
                        );
                }
                break;
            default:
                {
                    Debug.LogError("InitHardCode_NeighborChunks: " + m_nId.ToString());
                }
                break;
        }

        m_bIsInitHardCode_NeighborChunks = true;
    }

    void InitHardCode_NeighborChunks_AutoAlloc(
        ref ST_NeighborChunk[] aryNeighborChunks,
        EarthTHandler.EM_ChunkDirection emDir,
        EarthTHandler.EM_ChunkIndex emChunkIndex,
        float fNeedRotateDegree
        )
    {
        GameCommon.ASSERT(fNeedRotateDegree % 90 == 0);
        float fNeedRotateDegree_Abs = Math.Abs(fNeedRotateDegree);
        GameCommon.ASSERT(fNeedRotateDegree_Abs >= 0 &&
            fNeedRotateDegree_Abs <= 270);

        aryNeighborChunks[(int)(emDir)] = new ST_NeighborChunk(
            emDir,
            m_stHandler.GetChunk(emChunkIndex),
            fNeedRotateDegree
            );
    }

    bool m_bIsInitHardCode_NeighborAreas = false;
    public void InitHardCode_NeighborAreas()
    {
        GameCommon.ASSERT(!m_bIsInitHardCode_NeighborAreas);

        for (int i = 0; i < m_aryAreas.Length; i++)
        {
            m_aryAreas[i].InitHardCode_NeighborAreas();
        }

        m_bIsInitHardCode_NeighborAreas = true;
    }

    #endregion




    

    public EarthTChunk(EarthTHandler stHandler, EarthTHandler.EM_ChunkIndex nId)
    {
        GameCommon.ASSERT(EarthTileHelper.CheckChunkIndex(nId));

        for (int iX = 0; iX < EarthTHandler.m_nConstOneChunkAreaSideNum; iX++)
        {
            for (int iY = 0; iY < EarthTHandler.m_nConstOneChunkAreaSideNum; iY++)
            {
                int nAreaId = 0;
                EarthTArea stArea = new EarthTArea(stHandler, this, iX, iY, out nAreaId);
                GameCommon.ASSERT(nAreaId >= 0 && nAreaId < m_aryAreas.Length);
                m_aryAreas[nAreaId] = stArea;
            }
        }

        m_stHandler = stHandler;
        m_nId = nId;        
    }

    public EarthTHandler.EM_ChunkIndex GetId() { return m_nId; }

    public EarthTArea GetArea(int nAreaId)
    {
        GameCommon.ASSERT(nAreaId >= 0 && nAreaId < m_aryAreas.Length);

        return m_aryAreas[nAreaId];
    }

    public EarthTArea GetArea(int nIndexX, int nIndexY)
    {
        return GetArea(EarthTileHelper.GetAreaId(nIndexX, nIndexY));
    }

    public EarthTChunk.ST_NeighborChunk GetNeighborChunk(EarthTHandler.EM_ChunkDirection emDir)
    {
        GameCommon.ASSERT(EarthTileHelper.CheckChunkDirection(emDir));

        return m_aryNeighborChunks[(int)(emDir)];
    }

    public EarthTChunk.ST_NeighborChunk GetNeighborChunk(EarthTHandler.EM_ChunkDirection emDir, float fSelfRotatedDegree)
    {
        GameCommon.ASSERT(EarthTileHelper.CheckChunkDirection(emDir));

        int nRotate90DegreeTimes = EarthTileHelper.GetRotate90DegreeTimes(fSelfRotatedDegree);
        GameCommon.ASSERT(nRotate90DegreeTimes >= 0 && nRotate90DegreeTimes <= 3);

        int nMoveStep = 0;
        if (nRotate90DegreeTimes == 0)
        {
            nMoveStep = 0;
        }
        else if (nRotate90DegreeTimes == 1)
        {
            nMoveStep = 3;
        }
        else if (nRotate90DegreeTimes == 2)
        {
            nMoveStep = 2;
        }
        else if (nRotate90DegreeTimes == 3)
        {
            nMoveStep = 1;
        }

        int nRet = (int)(emDir + nMoveStep) % (int)(EarthTHandler.EM_ChunkDirection.Max);

        return m_aryNeighborChunks[nRet];
    }

    public EarthTChunk.ST_NeighborChunk GetNeighborChunk(
        EarthTHandler.EM_ChunkDirection emDir, 
        float fSelfRotatedDegree,
        out float fNeighborRotatedDegree
        )
    {
        GameCommon.ASSERT(EarthTileHelper.CheckChunkDirection(emDir));

        fNeighborRotatedDegree = 0;

        float fNeedRotateDegree_Clamp = 0;
        int nRotate90DegreeTimes = EarthTileHelper.GetRotate90DegreeTimes(fSelfRotatedDegree, out fNeedRotateDegree_Clamp);
        GameCommon.ASSERT(nRotate90DegreeTimes >= 0 && nRotate90DegreeTimes <= 3);

        int nMoveStep = 0;
        if (nRotate90DegreeTimes == 0)
        {
            nMoveStep = 0;
        }
        else if (nRotate90DegreeTimes == 1)
        {
            nMoveStep = 3;
        }
        else if (nRotate90DegreeTimes == 2)
        {
            nMoveStep = 2;
        }
        else if (nRotate90DegreeTimes == 3)
        {
            nMoveStep = 1;
        }

        int nRet = (int)(emDir + nMoveStep) % (int)(EarthTHandler.EM_ChunkDirection.Max);
        EarthTChunk.ST_NeighborChunk _stNeighborRet = m_aryNeighborChunks[nRet];
        GameCommon.ASSERT(_stNeighborRet != null);

        int nRotate90DegreeTimes_Neighbor = EarthTileHelper.GetRotate90DegreeTimes(_stNeighborRet.GetNeedRotateDegree());
        int nRotate90DegreeTimes_NeighborRet = (nRotate90DegreeTimes_Neighbor + nRotate90DegreeTimes) % 4;
        fNeighborRotatedDegree = EarthTileHelper.GetRotateClampDegree(nRotate90DegreeTimes_NeighborRet);
        return _stNeighborRet;
    }

    public EarthTChunk.ST_NeighborChunk GetNeighborChunk(EarthTHandler.EM_ChunkIndex emTryId)
    {
        for (EarthTHandler.EM_ChunkDirection _emDir = EarthTHandler.EM_ChunkDirection.Invalid + 1;
            _emDir < EarthTHandler.EM_ChunkDirection.Max;
            _emDir++
            )
        {
            EarthTChunk.ST_NeighborChunk _stNeighborChunk = GetNeighborChunk(_emDir);
            GameCommon.ASSERT(_stNeighborChunk != null);

            if (_stNeighborChunk.GetChunk().GetId() == emTryId)
            {
                return _stNeighborChunk;
            }
        }

        return null;
    }
};
