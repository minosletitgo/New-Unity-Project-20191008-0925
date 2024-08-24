using System;
using System.Collections;
using System.Collections.Generic;

public class EarthTHandler
{
    /*
    
          [4]
    [0][1][2][3]
          [5]
    
    90,91,92,93,94,95,96,97,98,99
    80,81,82,83,84,85,86,87,88,89
    70,71,72,73,74,75,76,77,78,79
    60,61,62,63,64,65,66,67,68,69
    50,51,52,53,54,55,56,57,58,59
    40,41,42,43,44,45,46,47,48,49
    30,31,32,33,34,35,36,37,38,39
    20,21,22,23,24,25,26,27,28,29
    10,11,12,13,14,15,16,17,18,19
    00,01,02,03,04,05,06,07,08,09

    */


    public const int m_nConstOneChunkAreaSideNum = 12;
    //public const float m_fConstOneChunkDimension = 49569.724f;
    //public const float m_fConstOneChunkDimension = 10000.0f;    
    public const float m_fConstOneChunkDimension = 49152.0f;

    public enum EM_ChunkIndex
    {
        Invalid = -1,
        Zero, One, Two, Three, Four, Five,
        Max,
    };

    public enum EM_ChunkDirection
    {
        Invalid = -1,
        N, E, S, W,
        Max,
    };

    public enum EM_AreaDirection
    {
        Invalid = -1,
        N, NE, E, SE, S, SW, W, NW,
        Max,
    };




    EarthTChunk[] m_aryChunks = new EarthTChunk[(int)(EarthTHandler.EM_ChunkIndex.Max)];

    public EarthTHandler()
    {
        for (int i = 0; i < m_aryChunks.Length; i++)
        {
            m_aryChunks[i] = new EarthTChunk(this, (EM_ChunkIndex)(i));
        }

        for (int i = 0; i < m_aryChunks.Length; i++)
        {
            m_aryChunks[i].InitHardCode_NeighborChunks();
        }

        for (int i = 0; i < m_aryChunks.Length; i++)
        {
            m_aryChunks[i].InitHardCode_NeighborAreas();
        }
    }

    public EarthTChunk GetChunk(EarthTHandler.EM_ChunkIndex nId)
    {
        GameCommon.ASSERT(EarthTileHelper.CheckChunkIndex(nId));

        return m_aryChunks[(int)(nId)];
    }
}