
public class EarthTArea
{
    EarthTHandler m_stHandler;
    int m_nId;
    int m_nIndexX;
    int m_nIndexY;

    EarthTChunk m_stParentChunk;
    EarthTArea[] m_aryNeighborAreas = new EarthTArea[(int)(EarthTHandler.EM_AreaDirection.Max)];
    float[] m_aryNeighborAreasNeedRotateDegree = new float[(int)(EarthTHandler.EM_AreaDirection.Max)];



    #region HardCode(硬代码)

    bool m_bIsInitHardCode_NeighborAreas = false;
    public void InitHardCode_NeighborAreas()
    {
        GameCommon.ASSERT(m_stHandler != null);
        GameCommon.ASSERT(!m_bIsInitHardCode_NeighborAreas);
        GameCommon.ASSERT(m_stParentChunk != null);


        //通用算法
        {
            for (EarthTHandler.EM_AreaDirection emAreaDir = (EarthTHandler.EM_AreaDirection.Invalid + 1);
                emAreaDir < EarthTHandler.EM_AreaDirection.Max;
                emAreaDir++
                )
            {
                switch (emAreaDir)
                {
                    case EarthTHandler.EM_AreaDirection.N:
                        {
                            if (m_nIndexY < EarthTHandler.m_nConstOneChunkAreaSideNum - 1)
                            {
                                /*                                
                                90,91,92,93,94,95,96,97,98,99
                                -----------------------------
                               |80,81,82,83,84,85,86,87,88,89|
                               |70,71,72,73,74,75,76,77,78,79|
                               |60,61,62,63,64,65,66,67,68,69|
                               |50,51,52,53,54,55,56,57,58,59|
                               |40,41,42,43,44,45,46,47,48,49|
                               |30,31,32,33,34,35,36,37,38,39|
                               |20,21,22,23,24,25,26,27,28,29|
                               |10,11,12,13,14,15,16,17,18,19|
                               |00,01,02,03,04,05,06,07,08,09|          
                                -----------------------------
                                */

                                InitHardCode_NeighborAreas_AutoAlloc(
                                    ref m_aryNeighborAreas,
                                    ref m_aryNeighborAreasNeedRotateDegree,
                                    emAreaDir,
                                    m_stParentChunk,
                                    m_nIndexX + 0,
                                    m_nIndexY + 1
                                    );
                            }
                            else if (m_nIndexY == EarthTHandler.m_nConstOneChunkAreaSideNum - 1)
                            {
                                /*                                
                                -----------------------------
                               |90,91,92,93,94,95,96,97,98,99|
                                -----------------------------
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

                                EarthTChunk.ST_NeighborChunk stNeighborChunk = m_stParentChunk.GetNeighborChunk(EarthTHandler.EM_ChunkDirection.N);
                                GameCommon.ASSERT(stNeighborChunk != null);

                                InitHardCode_NeighborAreas_AutoCale(
                                    ref m_aryNeighborAreas,
                                    ref m_aryNeighborAreasNeedRotateDegree,
                                    emAreaDir,
                                    stNeighborChunk,
                                    m_nIndexX
                                    );
                            }
                        }
                        break;

                    case EarthTHandler.EM_AreaDirection.NE:
                        {
                            if (m_nIndexX < EarthTHandler.m_nConstOneChunkAreaSideNum - 1)
                            {
                                if (m_nIndexY < EarthTHandler.m_nConstOneChunkAreaSideNum - 1)
                                {
                                    /*                           
                                    90,91,92,93,94,95,96,97,98,99
                                    --------------------------
                                   |80,81,82,83,84,85,86,87,88|89
                                   |70,71,72,73,74,75,76,77,78|79
                                   |60,61,62,63,64,65,66,67,68|69
                                   |50,51,52,53,54,55,56,57,58|59
                                   |40,41,42,43,44,45,46,47,48|49
                                   |30,31,32,33,34,35,36,37,38|39
                                   |20,21,22,23,24,25,26,27,28|29
                                   |10,11,12,13,14,15,16,17,18|19
                                   |00,01,02,03,04,05,06,07,08|09
                                    --------------------------
                                    */

                                    InitHardCode_NeighborAreas_AutoAlloc(
                                        ref m_aryNeighborAreas,
                                        ref m_aryNeighborAreasNeedRotateDegree,
                                        emAreaDir,
                                        m_stParentChunk,
                                        m_nIndexX + 1,
                                        m_nIndexY + 1
                                        );
                                }
                                else
                                {
                                    /*
                                    --------------------------
                                   |90,91,92,93,94,95,96,97,98|99
                                    --------------------------
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

                                    EarthTChunk.ST_NeighborChunk stNeighborChunk = m_stParentChunk.GetNeighborChunk(EarthTHandler.EM_ChunkDirection.N);
                                    GameCommon.ASSERT(stNeighborChunk != null);

                                    InitHardCode_NeighborAreas_AutoCale(
                                        ref m_aryNeighborAreas,
                                        ref m_aryNeighborAreasNeedRotateDegree,
                                        emAreaDir,
                                        stNeighborChunk,
                                        m_nIndexX + 1
                                        );
                                }
                            }
                            else
                            {
                                if (m_nIndexY < EarthTHandler.m_nConstOneChunkAreaSideNum - 1)
                                {
                                    /*
                                    90,91,92,93,94,95,96,97,98,99
                                                               --
                                    80,81,82,83,84,85,86,87,88|89|
                                    70,71,72,73,74,75,76,77,78|79|
                                    60,61,62,63,64,65,66,67,68|69|
                                    50,51,52,53,54,55,56,57,58|59|
                                    40,41,42,43,44,45,46,47,48|49|
                                    30,31,32,33,34,35,36,37,38|39|
                                    20,21,22,23,24,25,26,27,28|29|
                                    10,11,12,13,14,15,16,17,18|19|
                                    00,01,02,03,04,05,06,07,08|09|
                                                               --
                                    */

                                    EarthTChunk.ST_NeighborChunk stNeighborChunk = m_stParentChunk.GetNeighborChunk(EarthTHandler.EM_ChunkDirection.E);
                                    GameCommon.ASSERT(stNeighborChunk != null);

                                    InitHardCode_NeighborAreas_AutoCale(
                                        ref m_aryNeighborAreas,
                                        ref m_aryNeighborAreasNeedRotateDegree,
                                        emAreaDir,
                                        stNeighborChunk,
                                        m_nIndexY + 1
                                        );
                                }
                                else
                                {
                                    /*
                                                               --
                                    90,91,92,93,94,95,96,97,98|99|
                                                               --
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

                                    m_aryNeighborAreas[(int)(emAreaDir)] = null;
                                    m_aryNeighborAreasNeedRotateDegree[(int)(emAreaDir)] = 0;
                                }
                            }
                        }
                        break;

                    case EarthTHandler.EM_AreaDirection.E:
                        {
                            if (m_nIndexX < EarthTHandler.m_nConstOneChunkAreaSideNum - 1)
                            {
                                /*                          
                                 --------------------------                          
                                |90,91,92,93,94,95,96,97,98|99
                                |80,81,82,83,84,85,86,87,88|89
                                |70,71,72,73,74,75,76,77,78|79
                                |60,61,62,63,64,65,66,67,68|69
                                |50,51,52,53,54,55,56,57,58|59
                                |40,41,42,43,44,45,46,47,48|49
                                |30,31,32,33,34,35,36,37,38|39
                                |20,21,22,23,24,25,26,27,28|29
                                |10,11,12,13,14,15,16,17,18|19
                                |00,01,02,03,04,05,06,07,08|09
                                 --------------------------
                                */

                                InitHardCode_NeighborAreas_AutoAlloc(
                                    ref m_aryNeighborAreas,
                                    ref m_aryNeighborAreasNeedRotateDegree,
                                    emAreaDir,
                                    m_stParentChunk,
                                    m_nIndexX + 1,
                                    m_nIndexY + 0
                                    );
                            }
                            else
                            {
                                /*                          
                                                           --
                                90,91,92,93,94,95,96,97,98|99|
                                80,81,82,83,84,85,86,87,88|89|
                                70,71,72,73,74,75,76,77,78|79|
                                60,61,62,63,64,65,66,67,68|69|
                                50,51,52,53,54,55,56,57,58|59|
                                40,41,42,43,44,45,46,47,48|49|
                                30,31,32,33,34,35,36,37,38|39|
                                20,21,22,23,24,25,26,27,28|29|
                                10,11,12,13,14,15,16,17,18|19|
                                00,01,02,03,04,05,06,07,08|09|
                                                           --
                                */

                                EarthTChunk.ST_NeighborChunk stNeighborChunk = m_stParentChunk.GetNeighborChunk(EarthTHandler.EM_ChunkDirection.E);
                                GameCommon.ASSERT(stNeighborChunk != null);

                                InitHardCode_NeighborAreas_AutoCale(
                                    ref m_aryNeighborAreas,
                                    ref m_aryNeighborAreasNeedRotateDegree,
                                    emAreaDir,
                                    stNeighborChunk,
                                    m_nIndexY
                                    );
                            }
                        }
                        break;

                    case EarthTHandler.EM_AreaDirection.SE:
                        {
                            if (m_nIndexX < EarthTHandler.m_nConstOneChunkAreaSideNum - 1)
                            {
                                if (m_nIndexY > 0)
                                {
                                    /*                           
                                    --------------------------
                                   |90,91,92,93,94,95,96,97,98|99                            
                                   |80,81,82,83,84,85,86,87,88|89
                                   |70,71,72,73,74,75,76,77,78|79
                                   |60,61,62,63,64,65,66,67,68|69
                                   |50,51,52,53,54,55,56,57,58|59
                                   |40,41,42,43,44,45,46,47,48|49
                                   |30,31,32,33,34,35,36,37,38|39
                                   |20,21,22,23,24,25,26,27,28|29
                                   |10,11,12,13,14,15,16,17,18|19
                                    --------------------------
                                    00,01,02,03,04,05,06,07,08,09
                                    */

                                    InitHardCode_NeighborAreas_AutoAlloc(
                                        ref m_aryNeighborAreas,
                                        ref m_aryNeighborAreasNeedRotateDegree,
                                        emAreaDir,
                                        m_stParentChunk,
                                        m_nIndexX + 1,
                                        m_nIndexY - 1
                                        );
                                }
                                else
                                {
                                    /*                                                               
                                    90,91,92,93,94,95,96,97,98,99                            
                                    80,81,82,83,84,85,86,87,88,89
                                    70,71,72,73,74,75,76,77,78,79
                                    60,61,62,63,64,65,66,67,68,69
                                    50,51,52,53,54,55,56,57,58,59
                                    40,41,42,43,44,45,46,47,48,49
                                    30,31,32,33,34,35,36,37,38,39
                                    20,21,22,23,24,25,26,27,28,29
                                    10,11,12,13,14,15,16,17,18,19
                                    --------------------------
                                   |00,01,02,03,04,05,06,07,08|09
                                    --------------------------
                                    */

                                    EarthTChunk.ST_NeighborChunk stNeighborChunk = m_stParentChunk.GetNeighborChunk(EarthTHandler.EM_ChunkDirection.S);
                                    GameCommon.ASSERT(stNeighborChunk != null);

                                    InitHardCode_NeighborAreas_AutoCale(
                                        ref m_aryNeighborAreas,
                                        ref m_aryNeighborAreasNeedRotateDegree,
                                        emAreaDir,
                                        stNeighborChunk,
                                        m_nIndexX + 1
                                        );
                                }
                            }
                            else
                            {
                                if (m_nIndexY > 0)
                                {
                                    /*                           
                                                               --
                                    90,91,92,93,94,95,96,97,98|99|                            
                                    80,81,82,83,84,85,86,87,88|89|
                                    70,71,72,73,74,75,76,77,78|79|
                                    60,61,62,63,64,65,66,67,68|69|
                                    50,51,52,53,54,55,56,57,58|59|
                                    40,41,42,43,44,45,46,47,48|49|
                                    30,31,32,33,34,35,36,37,38|39|
                                    20,21,22,23,24,25,26,27,28|29|
                                    10,11,12,13,14,15,16,17,18|19|
                                                               --
                                    00,01,02,03,04,05,06,07,08,09
                                    */

                                    EarthTChunk.ST_NeighborChunk stNeighborChunk = m_stParentChunk.GetNeighborChunk(EarthTHandler.EM_ChunkDirection.E);
                                    GameCommon.ASSERT(stNeighborChunk != null);

                                    InitHardCode_NeighborAreas_AutoCale(
                                        ref m_aryNeighborAreas,
                                        ref m_aryNeighborAreasNeedRotateDegree,
                                        emAreaDir,
                                        stNeighborChunk,
                                        m_nIndexY - 1
                                        );
                                }
                                else
                                {
                                    /*
                                    90,91,92,93,94,95,96,97,98,99,                            
                                    80,81,82,83,84,85,86,87,88,89,
                                    70,71,72,73,74,75,76,77,78,79,
                                    60,61,62,63,64,65,66,67,68,69,
                                    50,51,52,53,54,55,56,57,58,59,
                                    40,41,42,43,44,45,46,47,48,49,
                                    30,31,32,33,34,35,36,37,38,39,
                                    20,21,22,23,24,25,26,27,28,29,
                                    10,11,12,13,14,15,16,17,18,19,
                                                               --
                                    00,01,02,03,04,05,06,07,08|09|
                                                               --
                                    */

                                    m_aryNeighborAreas[(int)(emAreaDir)] = null;
                                    m_aryNeighborAreasNeedRotateDegree[(int)(emAreaDir)] = 0;
                                }
                            }
                        }
                        break;

                    case EarthTHandler.EM_AreaDirection.S:
                        {
                            if (m_nIndexY > 0)
                            {
                                /*                           
                                -----------------------------
                               |90,91,92,93,94,95,96,97,98,99|
                               |80,81,82,83,84,85,86,87,88,89|
                               |70,71,72,73,74,75,76,77,78,79|
                               |60,61,62,63,64,65,66,67,68,69|
                               |50,51,52,53,54,55,56,57,58,59|
                               |40,41,42,43,44,45,46,47,48,49|
                               |30,31,32,33,34,35,36,37,38,39|
                               |20,21,22,23,24,25,26,27,28,29|
                               |10,11,12,13,14,15,16,17,18,19|
                                -----------------------------
                                00,01,02,03,04,05,06,07,08,09
                                */

                                InitHardCode_NeighborAreas_AutoAlloc(
                                    ref m_aryNeighborAreas,
                                    ref m_aryNeighborAreasNeedRotateDegree,
                                    emAreaDir,
                                    m_stParentChunk,
                                    m_nIndexX + 0,
                                    m_nIndexY - 1
                                    );
                            }
                            else
                            {
                                /*                           
                                90,91,92,93,94,95,96,97,98,99
                                80,81,82,83,84,85,86,87,88,89
                                70,71,72,73,74,75,76,77,78,79
                                60,61,62,63,64,65,66,67,68,69
                                50,51,52,53,54,55,56,57,58,59
                                40,41,42,43,44,45,46,47,48,49
                                30,31,32,33,34,35,36,37,38,39
                                20,21,22,23,24,25,26,27,28,29
                                10,11,12,13,14,15,16,17,18,19
                                -----------------------------
                               |00,01,02,03,04,05,06,07,08,09|
                                -----------------------------
                                */

                                EarthTChunk.ST_NeighborChunk stNeighborChunk = m_stParentChunk.GetNeighborChunk(EarthTHandler.EM_ChunkDirection.S);
                                GameCommon.ASSERT(stNeighborChunk != null);

                                InitHardCode_NeighborAreas_AutoCale(
                                    ref m_aryNeighborAreas,
                                    ref m_aryNeighborAreasNeedRotateDegree,
                                    emAreaDir,
                                    stNeighborChunk,
                                    m_nIndexX
                                    );
                            }
                        }
                        break;

                    case EarthTHandler.EM_AreaDirection.SW:
                        {
                            if (m_nIndexX > 0)
                            {
                                if (m_nIndexY > 0)
                                {
                                    /*                           
                                      --------------------------
                                   90|91,92,93,94,95,96,97,98,99|
                                   80|81,82,83,84,85,86,87,88,89|
                                   70|71,72,73,74,75,76,77,78,79|
                                   60|61,62,63,64,65,66,67,68,69|
                                   50|51,52,53,54,55,56,57,58,59|
                                   40|41,42,43,44,45,46,47,48,49|
                                   30|31,32,33,34,35,36,37,38,39|
                                   20|21,22,23,24,25,26,27,28,29|
                                   10|11,12,13,14,15,16,17,18,19|
                                      --------------------------
                                   00,01,02,03,04,05,06,07,08,09
                                    */

                                    InitHardCode_NeighborAreas_AutoAlloc(
                                        ref m_aryNeighborAreas,
                                        ref m_aryNeighborAreasNeedRotateDegree,
                                        emAreaDir,
                                        m_stParentChunk,
                                        m_nIndexX - 1,
                                        m_nIndexY - 1
                                        );
                                }
                                else
                                {
                                    /*
                                   90,91,92,93,94,95,96,97,98,99,
                                   80,81,82,83,84,85,86,87,88,89,
                                   70,71,72,73,74,75,76,77,78,79,
                                   60,61,62,63,64,65,66,67,68,69,
                                   50,51,52,53,54,55,56,57,58,59,
                                   40,41,42,43,44,45,46,47,48,49,
                                   30,31,32,33,34,35,36,37,38,39,
                                   20,21,22,23,24,25,26,27,28,29,
                                   10,11,12,13,14,15,16,17,18,19,
                                      --------------------------
                                   00|01,02,03,04,05,06,07,08,09|
                                      --------------------------
                                    */

                                    EarthTChunk.ST_NeighborChunk stNeighborChunk = m_stParentChunk.GetNeighborChunk(EarthTHandler.EM_ChunkDirection.S);
                                    GameCommon.ASSERT(stNeighborChunk != null);

                                    InitHardCode_NeighborAreas_AutoCale(
                                        ref m_aryNeighborAreas,
                                        ref m_aryNeighborAreasNeedRotateDegree,
                                        emAreaDir,
                                        stNeighborChunk,
                                        m_nIndexX - 1
                                        );
                                }
                            }
                            else
                            {
                                if (m_nIndexY > 0)
                                {
                                    /*                           
                                    --
                                   |90|91,92,93,94,95,96,97,98,99
                                   |80|81,82,83,84,85,86,87,88,89
                                   |70|71,72,73,74,75,76,77,78,79
                                   |60|61,62,63,64,65,66,67,68,69
                                   |50|51,52,53,54,55,56,57,58,59
                                   |40|41,42,43,44,45,46,47,48,49
                                   |30|31,32,33,34,35,36,37,38,39
                                   |20|21,22,23,24,25,26,27,28,29
                                   |10|11,12,13,14,15,16,17,18,19
                                    --
                                    00,01,02,03,04,05,06,07,08,09
                                    */

                                    EarthTChunk.ST_NeighborChunk stNeighborChunk = m_stParentChunk.GetNeighborChunk(EarthTHandler.EM_ChunkDirection.W);
                                    GameCommon.ASSERT(stNeighborChunk != null);

                                    InitHardCode_NeighborAreas_AutoCale(
                                        ref m_aryNeighborAreas,
                                        ref m_aryNeighborAreasNeedRotateDegree,
                                        emAreaDir,
                                        stNeighborChunk,
                                        m_nIndexY - 1
                                        );
                                }
                                else
                                {
                                    /*                           
                                    --
                                    90,91,92,93,94,95,96,97,98,99
                                    80,81,82,83,84,85,86,87,88,89
                                    70,71,72,73,74,75,76,77,78,79
                                    60,61,62,63,64,65,66,67,68,69
                                    50,51,52,53,54,55,56,57,58,59
                                    40,41,42,43,44,45,46,47,48,49
                                    30,31,32,33,34,35,36,37,38,39
                                    20,21,22,23,24,25,26,27,28,29
                                    10,11,12,13,14,15,16,17,18,19
                                    --
                                   |00|01,02,03,04,05,06,07,08,09
                                    --
                                    */

                                    m_aryNeighborAreas[(int)(emAreaDir)] = null;
                                    m_aryNeighborAreasNeedRotateDegree[(int)(emAreaDir)] = 0;
                                }
                            }
                        }
                        break;

                    case EarthTHandler.EM_AreaDirection.W:
                        {
                            if (m_nIndexX > 0)
                            {
                                /*                          
                                    --------------------------                          
                                 90|91,92,93,94,95,96,97,98,99|
                                 80|81,82,83,84,85,86,87,88,89|
                                 70|71,72,73,74,75,76,77,78,79|
                                 60|61,62,63,64,65,66,67,68,69|
                                 50|51,52,53,54,55,56,57,58,59|
                                 40|41,42,43,44,45,46,47,48,49|
                                 30|31,32,33,34,35,36,37,38,39|
                                 20|21,22,23,24,25,26,27,28,29|
                                 10|11,12,13,14,15,16,17,18,19|
                                 00|01,02,03,04,05,06,07,08,09|
                                    --------------------------                          
                                */

                                InitHardCode_NeighborAreas_AutoAlloc(
                                    ref m_aryNeighborAreas,
                                    ref m_aryNeighborAreasNeedRotateDegree,
                                    emAreaDir,
                                    m_stParentChunk,
                                    m_nIndexX - 1,
                                    m_nIndexY + 0
                                    );
                            }
                            else
                            {
                                /*                          
                                  --
                                 |90|91,92,93,94,95,96,97,98,99
                                 |80|81,82,83,84,85,86,87,88,89
                                 |70|71,72,73,74,75,76,77,78,79
                                 |60|61,62,63,64,65,66,67,68,69
                                 |50|51,52,53,54,55,56,57,58,59
                                 |40|41,42,43,44,45,46,47,48,49
                                 |30|31,32,33,34,35,36,37,38,39
                                 |20|21,22,23,24,25,26,27,28,29
                                 |10|11,12,13,14,15,16,17,18,19
                                 |00|01,02,03,04,05,06,07,08,09
                                  --
                                */

                                EarthTChunk.ST_NeighborChunk stNeighborChunk = m_stParentChunk.GetNeighborChunk(EarthTHandler.EM_ChunkDirection.W);
                                GameCommon.ASSERT(stNeighborChunk != null);

                                InitHardCode_NeighborAreas_AutoCale(
                                    ref m_aryNeighborAreas,
                                    ref m_aryNeighborAreasNeedRotateDegree,
                                    emAreaDir,
                                    stNeighborChunk,
                                    m_nIndexY
                                    );
                            }
                        }
                        break;

                    case EarthTHandler.EM_AreaDirection.NW:
                        {
                            if (m_nIndexX > 0)
                            {
                                if (m_nIndexY < EarthTHandler.m_nConstOneChunkAreaSideNum - 1)
                                {
                                    /*
                                    90,91,92,93,94,95,96,97,98,99                            
                                       --------------------------
                                    80|81,82,83,84,85,86,87,88,89|
                                    70|71,72,73,74,75,76,77,78,79|
                                    60|61,62,63,64,65,66,67,68,69|
                                    50|51,52,53,54,55,56,57,58,59|
                                    40|41,42,43,44,45,46,47,48,49|
                                    30|31,32,33,34,35,36,37,38,39|
                                    20|21,22,23,24,25,26,27,28,29|
                                    10|11,12,13,14,15,16,17,18,19|                            
                                    00|01,02,03,04,05,06,07,08,09|
                                       --------------------------
                                    */

                                    InitHardCode_NeighborAreas_AutoAlloc(
                                        ref m_aryNeighborAreas,
                                        ref m_aryNeighborAreasNeedRotateDegree,
                                        emAreaDir,
                                        m_stParentChunk,
                                        m_nIndexX - 1,
                                        m_nIndexY + 1
                                        );
                                }
                                else
                                {
                                    /*
                                       --------------------------
                                    90|91,92,93,94,95,96,97,98,99|
                                       --------------------------
                                    80,81,82,83,84,85,86,87,88,89
                                    70,71,72,73,74,75,76,77,78,79
                                    60,61,62,63,64,65,66,67,68,69
                                    50,51,52,53,54,55,56,57,58,59
                                    40,41,42,43,44,45,46,47,48,49
                                    30,31,32,33,34,35,36,37,38,39
                                    20,21,22,23,24,25,26,27,28,29
                                    10,11,12,13,14,15,16,17,18,19                            
                                    00,01,02,03,04,05,06,07,08,09
                                       --------------------------
                                    */

                                    EarthTChunk.ST_NeighborChunk stNeighborChunk = m_stParentChunk.GetNeighborChunk(EarthTHandler.EM_ChunkDirection.N);
                                    GameCommon.ASSERT(stNeighborChunk != null);

                                    InitHardCode_NeighborAreas_AutoCale(
                                        ref m_aryNeighborAreas,
                                        ref m_aryNeighborAreasNeedRotateDegree,
                                        emAreaDir,
                                        stNeighborChunk,
                                        m_nIndexX - 1
                                        );
                                }
                            }
                            else
                            {
                                if (m_nIndexY < EarthTHandler.m_nConstOneChunkAreaSideNum - 1)
                                {
                                    /*
                                     90,91,92,93,94,95,96,97,98,99                            
                                     --
                                    |80|81,82,83,84,85,86,87,88,89
                                    |70|71,72,73,74,75,76,77,78,79
                                    |60|61,62,63,64,65,66,67,68,69
                                    |50|51,52,53,54,55,56,57,58,59
                                    |40|41,42,43,44,45,46,47,48,49
                                    |30|31,32,33,34,35,36,37,38,39
                                    |20|21,22,23,24,25,26,27,28,29
                                    |10|11,12,13,14,15,16,17,18,19                            
                                    |00|01,02,03,04,05,06,07,08,09
                                     --
                                    */

                                    EarthTChunk.ST_NeighborChunk stNeighborChunk = m_stParentChunk.GetNeighborChunk(EarthTHandler.EM_ChunkDirection.W);
                                    GameCommon.ASSERT(stNeighborChunk != null);

                                    InitHardCode_NeighborAreas_AutoCale(
                                        ref m_aryNeighborAreas,
                                        ref m_aryNeighborAreasNeedRotateDegree,
                                        emAreaDir,
                                        stNeighborChunk,
                                        m_nIndexY + 1
                                        );
                                }
                                else
                                {
                                    /*
                                     --
                                    |90|91,92,93,94,95,96,97,98,99                            
                                     --
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

                                    m_aryNeighborAreas[(int)(emAreaDir)] = null;
                                    m_aryNeighborAreasNeedRotateDegree[(int)(emAreaDir)] = 0;
                                }
                            }
                        }
                        break;
                }
            }
        }

        m_bIsInitHardCode_NeighborAreas = true;
    }

    void InitHardCode_NeighborAreas_AutoAlloc(
        ref EarthTArea[] aryNeighborAreas,
        ref float[] aryNeighborAreasNeedRotate,
        EarthTHandler.EM_AreaDirection emDir,
        EarthTChunk stChunk,
        int nIndexX,
        int nIndexY
        )
    {
        GameCommon.ASSERT(EarthTileHelper.CheckAreaDirection(emDir));
        EarthTArea stArea = stChunk.GetArea(EarthTileHelper.GetAreaId(nIndexX, nIndexY));
        GameCommon.ASSERT(stArea != null);

        aryNeighborAreas[(int)(emDir)] = stArea;
        aryNeighborAreasNeedRotate[(int)(emDir)] = 0;
    }

    void InitHardCode_NeighborAreas_AutoCale(
        ref EarthTArea[] aryNeighborAreas,
        ref float[] aryNeighborAreasNeedRotate,
        EarthTHandler.EM_AreaDirection emDir,
        EarthTChunk.ST_NeighborChunk stNeighborChunk,
        int nAreaSideIndex
        )
    {
        GameCommon.ASSERT(EarthTileHelper.CheckAreaDirection(emDir));
        GameCommon.ASSERT(stNeighborChunk != null);
        EarthTArea stArea = EarthTileHelper.GetTargetArea(stNeighborChunk, nAreaSideIndex);
        GameCommon.ASSERT(stArea != null);

        aryNeighborAreas[(int)(emDir)] = stArea;
        aryNeighborAreasNeedRotate[(int)(emDir)] = stNeighborChunk.GetNeedRotateDegree();
    }

    #endregion











    public EarthTArea(EarthTHandler stHandler, EarthTChunk stParentChunk, int nIndexX, int nIndexY, out int nId)
    {
        GameCommon.ASSERT(nIndexX >= 0 && nIndexX < EarthTHandler.m_nConstOneChunkAreaSideNum);
        GameCommon.ASSERT(nIndexY >= 0 && nIndexY < EarthTHandler.m_nConstOneChunkAreaSideNum);

        m_nId = EarthTileHelper.GetAreaId(nIndexX, nIndexY);
        m_nIndexX = nIndexX;
        m_nIndexY = nIndexY;

        m_stHandler = stHandler;
        m_stParentChunk = stParentChunk;
        nId = m_nId;
    }

    public int GetId() { return m_nId; }

    public EarthTChunk GetParentChunk()
    {
        return m_stParentChunk;
    }

    public EarthTArea GetNeighborArea(EarthTHandler.EM_AreaDirection emDir, out float fNeedRotateDegree)
    {
        GameCommon.ASSERT(EarthTileHelper.CheckAreaDirection(emDir));
        fNeedRotateDegree = m_aryNeighborAreasNeedRotateDegree[(int)(emDir)];
        return m_aryNeighborAreas[(int)(emDir)];
    }
};