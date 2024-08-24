using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CUIWorldDragMapWindow02 : MonoBehaviour
{
    [SerializeField]
    Camera m_3dCamera;
    [SerializeField]
    UITexture m_texView;
    [SerializeField]
    MeshRenderer m_redPlane;
    BoxCollider m_boxBoxTexView;

    RenderTexture m_RenderTexture;
    UIDragSpringOriginalFrame m_stUIDragSpring;
    EarthTHandler m_stEarthTileHandler;


    enum EM_CommonCornerPoint
    {
        Invalid = -1,
        BottomLeft, TopRight,
        Max,
    };

    [SerializeField]
    GameObject[] m_aryMapGlobalCornerInWorld;
    [SerializeField]
    GameObject[] m_aryMapLogicCornerInWorld;


    Vector2[] m_aryCornerVectorInScreen = new Vector2[(int)(CUIWorldDragMapWindow02.EM_CommonCornerPoint.Max)];

    Vector2 m_v2TouchOldPos1;
    Vector2 m_v2TouchOldPos2;
    public float m_fMultiTouchMinDis = 10.0f;
    [SerializeField]
    UILabel m_labMapLogicRotated90Times;

    EarthTHandler.EM_ChunkIndex m_emMapCurChunkId;
    int m_nMapCurLogicRotated90Times = 0;

    Dictionary<EarthTHandler.EM_ChunkIndex, int> m_mMapLogicRotated90Times;


    [SerializeField]
    Vector2 m_v2MapLogicSize_BeforeRotate = new Vector2(2324, 1960);
    [SerializeField]
    UILabel m_labMapLogicSize_BeforeRotate;

    //[SerializeField]
    Vector2 m_v2MapLogicSize_AfterRotate = new Vector2(0, 0);
    [SerializeField]
    UILabel m_labMapLogicSize_AfterRotate;

    [SerializeField]
    UILabel m_labCurrentScheme;


    [SerializeField]
    UIInput m_itCameraGoToLogicX_AfterRotate;
    [SerializeField]
    UIInput m_itCameraGoToLogicY_AfterRotate;
    [SerializeField]
    UIButton m_btnCameraGoTo_AfterRotate;

    [SerializeField]
    UIInput m_itCameraGoToLogicX_BeforeRotate;
    [SerializeField]
    UIInput m_itCameraGoToLogicY_BeforeRotate;
    [SerializeField]
    UIButton m_btnCameraGoTo_BeforeRotate;

    [SerializeField]
    UIButton m_btnJumpToN;
    [SerializeField]
    UIButton m_btnJumpToE;
    [SerializeField]
    UIButton m_btnJumpToS;
    [SerializeField]
    UIButton m_btnJumpToW;
    Dictionary<EarthTHandler.EM_ChunkDirection, UIButton> m_mButtonJumpTo;

    //OpenAnimation
    bool m_bIsRuningJumpAnim = false;
    ST_JumpAnimParam m_stJumpAnimParam;
    float m_fRuningJumpAnimCostTime = 0;

    class ST_CustomChunkIndex
    {
        //定制跳转目标唯一指向
        public ST_CustomChunkIndex(EarthTHandler.EM_ChunkIndex emId, int nRotated90Times)
        {
            this.emId = emId;
            this.nRotated90Times = nRotated90Times;
        }

        public EarthTHandler.EM_ChunkIndex GetId() { return emId; }
        public int GetRotated90Times() { return nRotated90Times; }

        EarthTHandler.EM_ChunkIndex emId;
        int nRotated90Times;
    }
    Dictionary<EarthTHandler.EM_ChunkIndex, List<ST_CustomChunkIndex>> m_mCustomChunkTargetOnly;


    //打开窗体参数(即跳转动画参数)
    public class ST_JumpAnimParam
    {
        public ST_JumpAnimParam(
            bool bIsOpenOpr,
            EarthTHandler.EM_ChunkIndex emId,
            Vector2 v2PosRatioInChunk,
            float fOrthographicSizeFrom,
            float fOrthographicSizeTo
            )
        {
            CopyValueFrom(bIsOpenOpr, emId, v2PosRatioInChunk, fOrthographicSizeFrom, fOrthographicSizeTo);
        }

        public void CopyValueFrom(
            bool bIsOpenOpr,
            EarthTHandler.EM_ChunkIndex emId,
            Vector2 v2PosRatioInChunk,
            float fOrthographicSizeFrom,
            float fOrthographicSizeTo
            )
        {
            GameCommon.ASSERT(EarthTileHelper.CheckChunkIndex(emId));
            GameCommon.ASSERT(v2PosRatioInChunk.x >= 0f && v2PosRatioInChunk.x <= 1f);
            GameCommon.ASSERT(v2PosRatioInChunk.y >= 0f && v2PosRatioInChunk.y <= 1f);
            GameCommon.ASSERT(
                fOrthographicSizeFrom >= LogicWorldEarthMapHelper.Instance.GetOrthographicSize(LogicWorldEarthMapHelper.EM_OrthographicSize.Min) &&
                fOrthographicSizeFrom <= LogicWorldEarthMapHelper.Instance.GetOrthographicSize(LogicWorldEarthMapHelper.EM_OrthographicSize.Max)
                );
            GameCommon.ASSERT(
                fOrthographicSizeTo >= LogicWorldEarthMapHelper.Instance.GetOrthographicSize(LogicWorldEarthMapHelper.EM_OrthographicSize.Min) &&
                fOrthographicSizeTo <= LogicWorldEarthMapHelper.Instance.GetOrthographicSize(LogicWorldEarthMapHelper.EM_OrthographicSize.Max)
                );

            this.bIsOpenOpr = bIsOpenOpr;
            this.emId = emId;
            this.v2PosRatioInChunk = v2PosRatioInChunk;
            this.fOrthographicSizeFrom = fOrthographicSizeFrom;
            this.fOrthographicSizeTo = fOrthographicSizeTo;
        }

        public bool IsParamValid()
        {
            if (EarthTileHelper.CheckChunkIndex(emId))
            {
                return true;
            }

            if (v2PosRatioInChunk.x >= 0f && v2PosRatioInChunk.x <= 1f &&
                v2PosRatioInChunk.y >= 0f && v2PosRatioInChunk.y <= 1f
                )
            {
                return true;
            }

            return false;
        }

        public bool IsOpenOpr() { return bIsOpenOpr; }
        public EarthTHandler.EM_ChunkIndex GetId() { return emId; }
        public Vector2 GetPosRatioInChunk() { return v2PosRatioInChunk; }
        public float GetOrthographicSizeFrom() { return fOrthographicSizeFrom; }
        public float GetOrthographicSizeTo() { return fOrthographicSizeTo; }

        bool bIsOpenOpr;
        EarthTHandler.EM_ChunkIndex emId;
        Vector2 v2PosRatioInChunk;
        float fOrthographicSizeFrom;
        float fOrthographicSizeTo;
    };




    [SerializeField]
    UIButton m_btnSelfLocation;
    [SerializeField]
    UILabel m_labDebugLog;


    [SerializeField]
    CUIBoldTip m_stUIBoldTip;
    [SerializeField]
    UIButton m_btnGMOpen;









    private void Awake()
    {
        GameCommon.ASSERT(m_3dCamera.orthographic);

        int nRTWidth = Screen.width;
        int nRTHeight = Screen.height;

        UIStretch stStretch = m_texView.GetComponent<UIStretch>();
        GameCommon.ASSERT(stStretch != null);
        stStretch.style = UIStretch.Style.FillKeepingRatio;
        stStretch.runOnlyOnce = false;
        stStretch.relativeSize = new Vector2(1, 1);
        stStretch.initialSize = new Vector2(nRTWidth, nRTHeight);
        stStretch.borderPadding = new Vector2(0, 0);

        if (m_RenderTexture == null)
        {
            m_RenderTexture = new RenderTexture(nRTWidth, nRTHeight, 24);
        }
        m_RenderTexture.name = "render_texture";
        m_RenderTexture.format = RenderTextureFormat.ARGB32;
        m_texView.width = nRTWidth;
        m_texView.height = nRTHeight;
        m_3dCamera.targetTexture = m_RenderTexture;
        m_3dCamera.cullingMask = (1 << LayerMask.NameToLayer("Model"));
        m_texView.mainTexture = m_RenderTexture;

        m_boxBoxTexView = m_texView.GetComponent<BoxCollider>();
        GameCommon.ASSERT(m_boxBoxTexView != null);
        UIEventListener.Get(m_boxBoxTexView.gameObject).onClick += OnClick_BoxTexView;
        UIEventListener.Get(m_boxBoxTexView.gameObject).onDragEnd += OnDragEnd_BoxTexView;

        m_stUIDragSpring = m_3dCamera.GetComponent<UIDragSpringOriginalFrame>();
        GameCommon.ASSERT(m_stUIDragSpring != null);
        m_stUIDragSpring.Initialized(m_boxBoxTexView, OnMoveAbsolute);
        m_stUIDragSpring.InitRestrictWithinBounds(m_3dCamera.transform, OnIsWithinBounds, OnCalcNearestWithinBoundsLocalPos);
        m_stUIDragSpring.onDragFinished += OnDragFinised;
        m_stUIDragSpring.SetMultiTouchDisabled(true);

        if (m_stEarthTileHandler == null)
        {
            m_stEarthTileHandler = new EarthTHandler();
        }       

        GameCommon.ASSERT(m_aryMapGlobalCornerInWorld != null);
        GameCommon.ASSERT(m_aryMapGlobalCornerInWorld.Length == (int)(EM_CommonCornerPoint.Max));
        GameCommon.ASSERT(m_aryMapLogicCornerInWorld != null);
        GameCommon.ASSERT(m_aryMapLogicCornerInWorld.Length == (int)(EM_CommonCornerPoint.Max));

        m_aryCornerVectorInScreen[(int)(CUIWorldDragMapWindow02.EM_CommonCornerPoint.BottomLeft)] = new Vector2(0, 0);
        m_aryCornerVectorInScreen[(int)(CUIWorldDragMapWindow02.EM_CommonCornerPoint.TopRight)] = new Vector2(Screen.width, Screen.height);
        
        GameCommon.ASSERT(m_nMapCurLogicRotated90Times >= 0 && m_nMapCurLogicRotated90Times <= 3);

        if (m_mMapLogicRotated90Times == null)
        {
            m_mMapLogicRotated90Times = new Dictionary<EarthTHandler.EM_ChunkIndex, int>();
            m_mMapLogicRotated90Times.Add(EarthTHandler.EM_ChunkIndex.Zero, 3);
            m_mMapLogicRotated90Times.Add(EarthTHandler.EM_ChunkIndex.One, 3);
            m_mMapLogicRotated90Times.Add(EarthTHandler.EM_ChunkIndex.Two, 0);
            m_mMapLogicRotated90Times.Add(EarthTHandler.EM_ChunkIndex.Three, 1);
            m_mMapLogicRotated90Times.Add(EarthTHandler.EM_ChunkIndex.Four, 2);
            m_mMapLogicRotated90Times.Add(EarthTHandler.EM_ChunkIndex.Five, 0);
        }

        m_v2MapLogicSize_AfterRotate = EarthTileHelper.GetCoordinateSizeWithRotate(m_nMapCurLogicRotated90Times, m_v2MapLogicSize_BeforeRotate);

        if (m_mButtonJumpTo == null)
        {
            m_mButtonJumpTo = new Dictionary<EarthTHandler.EM_ChunkDirection, UIButton>();
        }
        m_mButtonJumpTo[EarthTHandler.EM_ChunkDirection.N] = m_btnJumpToN;
        m_mButtonJumpTo[EarthTHandler.EM_ChunkDirection.E] = m_btnJumpToE;
        m_mButtonJumpTo[EarthTHandler.EM_ChunkDirection.S] = m_btnJumpToS;
        m_mButtonJumpTo[EarthTHandler.EM_ChunkDirection.W] = m_btnJumpToW;
        foreach (KeyValuePair<EarthTHandler.EM_ChunkDirection, UIButton> pValue in m_mButtonJumpTo)
        {
            UIEventListener.Get(pValue.Value.gameObject).parameter = pValue.Key;
            UIEventListener.Get(pValue.Value.gameObject).onClick = OnClick_BtnJumpTo;
            pValue.Value.gameObject.SetActive(false);
        }

        if (m_mCustomChunkTargetOnly == null)
        {
            m_mCustomChunkTargetOnly = new Dictionary<EarthTHandler.EM_ChunkIndex, List<ST_CustomChunkIndex>>();
            {
                List<ST_CustomChunkIndex> lstChunkIndex = new List<ST_CustomChunkIndex>();
                lstChunkIndex.Add(new ST_CustomChunkIndex(EarthTHandler.EM_ChunkIndex.Five, 0));
                lstChunkIndex.Add(new ST_CustomChunkIndex(EarthTHandler.EM_ChunkIndex.Three, 1));
                lstChunkIndex.Add(new ST_CustomChunkIndex(EarthTHandler.EM_ChunkIndex.Four, 2));
                lstChunkIndex.Add(new ST_CustomChunkIndex(EarthTHandler.EM_ChunkIndex.One, 3));
                m_mCustomChunkTargetOnly.Add(EarthTHandler.EM_ChunkIndex.Two, lstChunkIndex);
            }
            {
                List<ST_CustomChunkIndex> lstChunkIndex = new List<ST_CustomChunkIndex>();
                lstChunkIndex.Add(new ST_CustomChunkIndex(EarthTHandler.EM_ChunkIndex.Five, 0));
                lstChunkIndex.Add(new ST_CustomChunkIndex(EarthTHandler.EM_ChunkIndex.Three, 1));
                lstChunkIndex.Add(new ST_CustomChunkIndex(EarthTHandler.EM_ChunkIndex.Four, 2));
                lstChunkIndex.Add(new ST_CustomChunkIndex(EarthTHandler.EM_ChunkIndex.One, 3));
                m_mCustomChunkTargetOnly.Add(EarthTHandler.EM_ChunkIndex.Zero, lstChunkIndex);
            }
        }

        //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~I'm CutLine After Logic~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~




        UIEventListener.Get(m_btnSelfLocation.gameObject).onClick = OnClick_BtnSelfLocation;

        m_labMapLogicSize_BeforeRotate.text = string.Format("BeforeRotate: [{0},{1}]", m_v2MapLogicSize_BeforeRotate.x, m_v2MapLogicSize_BeforeRotate.y);
                
        m_labMapLogicRotated90Times.text = null;
        m_labMapLogicSize_AfterRotate.text = string.Format("AfterRotate: [{0},{1}]", m_v2MapLogicSize_AfterRotate.x, m_v2MapLogicSize_AfterRotate.y);

        m_itCameraGoToLogicX_AfterRotate.validation = UIInput.Validation.Float;
        m_itCameraGoToLogicY_AfterRotate.validation = UIInput.Validation.Float;
        UIEventListener.Get(m_btnCameraGoTo_AfterRotate.gameObject).onClick = OnClick_BtnCameraGoTo_AfterRotate;

        m_itCameraGoToLogicX_BeforeRotate.validation = UIInput.Validation.Float;
        m_itCameraGoToLogicY_BeforeRotate.validation = UIInput.Validation.Float;
        UIEventListener.Get(m_btnCameraGoTo_BeforeRotate.gameObject).onClick = OnClick_BtnCameraGoTo_BeforeRotate;


        //一些摄像机伸缩范围     
        LogicWorldEarthMapHelper.Instance.InitOrthographicSize();



        m_btnGMOpen.gameObject.SetActive(true);
        UIEventListener.Get(m_btnGMOpen.gameObject).onClick =
        delegate (GameObject go)
        {
            bool bIsNew = false;
            var stWindow = CSimpleUIManage.Instance.GetUI<CUIDebugConsoleWindow>(CSimpleUIManage.EM_UIName.DebugConsoleWindow, out bIsNew);
            if (!bIsNew)
            {
                if (!stWindow.IsWindowOpened())
                {
                    stWindow.OpenWindow();
                }
                else
                {
                    stWindow.CloseWindow();
                }
            }
            else
            {
                stWindow.OpenWindow();
            }
        };
    }

    private void Start()
    {
        StartCoroutine(CoStart());
    }

    IEnumerator CoStart()
    {
        yield return new WaitForSeconds(0.5f);

        m_stJumpAnimParam = new ST_JumpAnimParam(
            true,
            EarthTHandler.EM_ChunkIndex.Five,
            new Vector2(0.5f, 0.5f),
            LogicWorldEarthMapHelper.Instance.GetOrthographicSize(LogicWorldEarthMapHelper.EM_OrthographicSize.Min),
            LogicWorldEarthMapHelper.Instance.GetOrthographicSize(LogicWorldEarthMapHelper.EM_OrthographicSize.Max)
            );

        JumpAnimEarthTileTo(
            true,
            m_stJumpAnimParam.GetId(),
            m_stJumpAnimParam.GetPosRatioInChunk(),
            m_stJumpAnimParam.GetOrthographicSizeFrom(),
            m_stJumpAnimParam.GetOrthographicSizeTo()
            );
    }

    private void Update()
    {
        m_labCurrentScheme.text = "CurrentScheme: " + UICamera.currentScheme.ToString();
    }

    private void LateUpdate()
    {
        if (m_bIsRuningJumpAnim)
        {
            GameCommon.ASSERT(m_stJumpAnimParam != null);

            bool bIsOSizeSpringDone = false;
            float fOSizeBefore = m_3dCamera.orthographicSize;
            float fOSizeAfter = NGUIMath.SpringLerp(
                fOSizeBefore,
                m_stJumpAnimParam.GetOrthographicSizeTo(),
                9.0f,
                RealTime.deltaTime
                );
            if (Math.Abs(fOSizeAfter - fOSizeBefore) < 0.001f)
            {
                fOSizeAfter = m_stJumpAnimParam.GetOrthographicSizeTo();
                bIsOSizeSpringDone = true;
            }

            bool bIsPosRatioSpringDone = false;
            Vector2 v2PosRatioBefore = Vector2.zero;
            Vector2 v2PosRatioAfter = Vector2.zero;
            if (!m_stJumpAnimParam.IsOpenOpr())
            {
                if (m_emMapCurChunkId == m_stJumpAnimParam.GetId())
                {
                    v2PosRatioBefore = GetMapCurPosRatio();

                    v2PosRatioAfter = NGUIMath.SpringLerp(
                        v2PosRatioBefore,
                        m_stJumpAnimParam.GetPosRatioInChunk(),
                        2.0f,
                        RealTime.deltaTime
                        );
                    //Debug.Log("v2PosRatio: "+ v2PosRatioAfter + " , "+ v2PosRatioBefore);
                    if ((v2PosRatioAfter - v2PosRatioBefore).sqrMagnitude < 0.001f)
                    {
                        v2PosRatioAfter = m_stJumpAnimParam.GetPosRatioInChunk();
                        bIsPosRatioSpringDone = true;
                    }
                }
                else
                {
                    v2PosRatioAfter = m_stJumpAnimParam.GetPosRatioInChunk();
                    bIsPosRatioSpringDone = true;
                }
            }
            else
            {
                v2PosRatioAfter = m_stJumpAnimParam.GetPosRatioInChunk();
                bIsPosRatioSpringDone = true;
            }

            if (Time.time - m_fRuningJumpAnimCostTime >= 2.5f)
            {
                //安全跳出
                fOSizeAfter = m_stJumpAnimParam.GetOrthographicSizeTo();
                v2PosRatioAfter = m_stJumpAnimParam.GetPosRatioInChunk();

                bIsOSizeSpringDone = true;
                bIsPosRatioSpringDone = true;
            }

            //Debug.Log("m_bIsRuningJumpAnim = " + bIsOSizeSpringDone + ","+ bIsPosRatioSpringDone);
            if (bIsOSizeSpringDone && bIsPosRatioSpringDone)
            {
                m_bIsRuningJumpAnim = false;
            }

            RefreshEarthTileTo(
                m_stJumpAnimParam.GetId(),
                v2PosRatioAfter,
                m_mMapLogicRotated90Times[m_stJumpAnimParam.GetId()],
                fOSizeAfter
                );
        }
        else
        {
            float fWheelDelta = 0f;

            if (Input.touchCount == 0)
            {
                //UnityEditor OR PC
                fWheelDelta = Input.GetAxis("Mouse ScrollWheel");
            }
            else if (Input.touchCount == 1)
            {
                //UIDragSpringOriginalFrame 接管单指事件
            }
            else if (Input.touchCount == 2)
            {
                if (Input.GetTouch(0).phase == TouchPhase.Moved ||
                    Input.GetTouch(1).phase == TouchPhase.Moved
                    )
                {
                    Vector2 v2TouchPos1 = Input.GetTouch(0).position;
                    Vector2 v2TouchPos2 = Input.GetTouch(1).position;

                    if (m_v2TouchOldPos1 == Vector2.zero ||
                        m_v2TouchOldPos2 == Vector2.zero
                        )
                    {
                        m_v2TouchOldPos1 = v2TouchPos1;
                        m_v2TouchOldPos2 = v2TouchPos2;
                        return;
                    }

                    float fDisCur = Vector2.Distance(v2TouchPos1, v2TouchPos2);
                    float fDisOld = Vector2.Distance(m_v2TouchOldPos1, m_v2TouchOldPos2);
                    float fDisDelta = fDisCur - fDisOld;
                    
                    if (Mathf.Abs(fDisDelta) > m_fMultiTouchMinDis)
                    {
                        PrintDebugLog(string.Format("fDisDelta: {0}", fDisDelta));
                        fWheelDelta = (fDisDelta > 0) ? -0.1f : 0.1f;

                        m_v2TouchOldPos1 = v2TouchPos1;
                        m_v2TouchOldPos2 = v2TouchPos2;
                    }

                    m_stUIDragSpring.StopMoving();
                }
            }

            //if (fWheelDelta != 0.0f)
            if (Math.Abs(fWheelDelta) > 0.000001f)
            {
                ScrollWheel(fWheelDelta);
            }

            //if (Input.GetMouseButtonUp(0) || Input.GetMouseButtonUp(1))
            //{
            //    PrintDebugLog(string.Format("GetMouseButtonUp(0)"));
            //    m_v2TouchOldPos1 = Vector2.zero;
            //    m_v2TouchOldPos2 = Vector2.zero;
            //}
        }
    }

    //void ScrollWheel(float delta)
    //{
    //    //Debug.Log(delta);
    //    //PrintDebugLog(string.Format("ScrollWheel: {0}", delta));

    //    if (m_bIsRuningJumpAnim)
    //    {
    //        return;
    //    }

    //    const float fConstSpeed = 0.075f;

    //    float fOSizeTry = m_3dCamera.orthographicSize;
    //    if (delta > 0)
    //    {
    //        fOSizeTry += fConstSpeed;
    //    }
    //    else
    //    {
    //        fOSizeTry -= fConstSpeed;
    //    }
    //    fOSizeTry = Mathf.Clamp(
    //        fOSizeTry,
    //        LogicWorldEarthMapHelper.Instance.GetOrthographicSize(LogicWorldEarthMapHelper.EM_OrthographicSize.Min),
    //        LogicWorldEarthMapHelper.Instance.GetOrthographicSize(LogicWorldEarthMapHelper.EM_OrthographicSize.Max)
    //        );
    //    //m_3dCamera.orthographicSize = fOSizeTry;
    //    //PrintDebugLog(string.Format("Delta: {0} \n Orth: {1}  ", delta, m_3dCamera.orthographicSize));

    //    //Debug.Log("ScrollWheel: " + m_3dCamera.transform.localPosition.x + " , " + m_3dCamera.transform.localPosition.x);
    //    //TryMoveCameraCenterTo(m_3dCamera.transform.localPosition.x, m_3dCamera.transform.localPosition.y);

    //    if (fOSizeTry == m_3dCamera.orthographicSize)
    //    {
    //        return;
    //    }

    //    float fOSizeFrom = m_3dCamera.orthographicSize;
    //    float fOSizeTo = LogicWorldEarthMapHelper.Instance.CaclOrthographicSizeLev(fOSizeFrom, fOSizeTry);

    //    JumpAnimEarthTileTo(
    //        false,
    //        m_emMapCurChunkId,
    //        GetMapCurPosRatio(),
    //        fOSizeFrom,
    //        fOSizeTo
    //        );
    //}

    void ScrollWheel(float delta)
    {
        //Debug.Log(delta);
        //PrintDebugLog(string.Format("ScrollWheel: {0}", delta));

        if (m_bIsRuningJumpAnim)
        {
            return;
        }

        float fConstSpeed = 0.0058f * (float)Math.Pow(m_3dCamera.orthographicSize, 2) + 0.0153f * m_3dCamera.orthographicSize + 0.0062f;

        float fOSize = m_3dCamera.orthographicSize;
        if (delta > 0)
        {
            fOSize += fConstSpeed;
        }
        else
        {
            fOSize -= fConstSpeed;
        }
        fOSize = Mathf.Clamp(
            fOSize,
            LogicWorldEarthMapHelper.Instance.GetOrthographicSize(LogicWorldEarthMapHelper.EM_OrthographicSize.Min),
            LogicWorldEarthMapHelper.Instance.GetOrthographicSize(LogicWorldEarthMapHelper.EM_OrthographicSize.Max)
            );
        m_3dCamera.orthographicSize = fOSize;
        //PrintDebugLog(string.Format("Delta: {0} \n Orth: {1}  ", delta, m_3dCamera.orthographicSize));

        //Debug.Log("ScrollWheel: " + m_3dCamera.transform.localPosition.x + " , " + m_3dCamera.transform.localPosition.x);
        TryMoveCameraCenterTo(m_3dCamera.transform.localPosition.x, m_3dCamera.transform.localPosition.y);
    }

    void OnMoveAbsolute(UIDragSpringOriginalFrame.EM_MessageEvent emEvent, Vector3 absolute, Vector2 v2Delta)
    {
        //Debug.Log(GameCommon.PrintVector3(absolute) + "  ,  " + GameCommon.PrintVector3(v2Delta));
        //Debug.Log(emEvent.ToString());

        if (m_bIsRuningJumpAnim)
        {
            return;
        }

        Vector3 v3Before = m_3dCamera.transform.localPosition;
        Vector3 v3After = v3Before + new Vector3(
            -1 * absolute.x,
            -1 * absolute.y,
            0
            );

        TryMoveCameraCenterTo(v3After.x, v3After.y, true);

        if (emEvent == UIDragSpringOriginalFrame.EM_MessageEvent.OnDrag)
        {
            EarthTHandler.EM_ChunkDirection emDir = EarthTHandler.EM_ChunkDirection.Invalid;
            if (IsNeedTriggerToRefresh(out emDir))
            {
                //Debug.Log("ShowTip");
            }
        }
    }

    bool OnIsWithinBounds()
    {
        Vector2 v2RangeHorizontal = GetCameraPosRangeInLocal(false);
        Vector2 v2RangeVertical = GetCameraPosRangeInLocal(true);
        if (m_3dCamera.transform.localPosition.x >= v2RangeHorizontal.x &&
            m_3dCamera.transform.localPosition.x <= v2RangeHorizontal.y &&
            m_3dCamera.transform.localPosition.y >= v2RangeVertical.x &&
            m_3dCamera.transform.localPosition.y <= v2RangeVertical.y
            )
        {
            return true;
        }
        return false;
    }

    Vector3 OnCalcNearestWithinBoundsLocalPos()
    {
        Vector2 v2RangeHorizontal = GetCameraPosRangeInLocal(false);
        Vector2 v2RangeVertical = GetCameraPosRangeInLocal(true);

        float fTargetFixPosX = m_3dCamera.transform.localPosition.x;
        if (m_3dCamera.transform.localPosition.x < v2RangeHorizontal.x)
        {
            fTargetFixPosX = v2RangeHorizontal.x;
        }
        else if (m_3dCamera.transform.localPosition.x > v2RangeHorizontal.y)
        {
            fTargetFixPosX = v2RangeHorizontal.y;
        }

        float fTargetFixPosY = m_3dCamera.transform.localPosition.y;
        if (m_3dCamera.transform.localPosition.y < v2RangeVertical.x)
        {
            fTargetFixPosY = v2RangeVertical.x;
        }
        else if (m_3dCamera.transform.localPosition.y > v2RangeVertical.y)
        {
            fTargetFixPosY = v2RangeVertical.y;
        }

        return new Vector3(fTargetFixPosX, fTargetFixPosY, m_3dCamera.transform.localPosition.z);
    }

    void OnDragFinised()
    {
        //Debug.Log("CUIWorldDragMapWindow02.OnDragFinised !");

        //EarthTHandler.EM_ChunkDirection emDir_Next = EarthTHandler.EM_ChunkDirection.Invalid;
        //if (IsNeedTriggerToRefresh(out emDir_Next))
        //{
        //    //Debug.Log("NeedTriggerRefresh !");
        //    bool bIsJumpToSucc = RefreshEarthTileTo(
        //        m_emMapCurChunkId,
        //        m_nMapCurLogicRotated90Times,
        //        emDir_Next,
        //        m_3dCamera.orthographicSize
        //        );
        //    if (bIsJumpToSucc)
        //    {
        //        m_stUIDragSpring.StopMoving();
        //        m_stUIDragSpring.DisableSpring();
        //    }
        //}
    }

    bool IsNeedTriggerToRefresh(out EarthTHandler.EM_ChunkDirection emDir_Target)
    {
        emDir_Target = EarthTHandler.EM_ChunkDirection.Invalid;

        if (OnIsWithinBounds())
        {
            return false;
        }

        Vector2 v2RangeHorizontal = GetCameraPosRangeInLocal(false);
        Vector2 v2RangeVertical = GetCameraPosRangeInLocal(true);

        float fRangeHorizontalDis = Mathf.Abs(v2RangeHorizontal.x - v2RangeHorizontal.y);
        float fRangeVerticalDis = Mathf.Abs(v2RangeVertical.x - v2RangeVertical.y);

        int nOutsideDirNum = 0;
        float fOutsideDeltaDis = 0;
        if (m_3dCamera.transform.localPosition.x < v2RangeHorizontal.x)
        {
            nOutsideDirNum++;
            emDir_Target = EarthTHandler.EM_ChunkDirection.W;
            fOutsideDeltaDis = Mathf.Abs(m_3dCamera.transform.localPosition.x - v2RangeHorizontal.x);
        }
        else if (m_3dCamera.transform.localPosition.x > v2RangeHorizontal.y)
        {
            nOutsideDirNum++;
            emDir_Target = EarthTHandler.EM_ChunkDirection.E;
            fOutsideDeltaDis = Mathf.Abs(m_3dCamera.transform.localPosition.x - v2RangeHorizontal.y);
        }

        if (m_3dCamera.transform.localPosition.y < v2RangeVertical.x)
        {
            nOutsideDirNum++;
            emDir_Target = EarthTHandler.EM_ChunkDirection.S;
            fOutsideDeltaDis = Mathf.Abs(m_3dCamera.transform.localPosition.y - v2RangeVertical.x);
        }
        else if (m_3dCamera.transform.localPosition.y > v2RangeVertical.y)
        {
            nOutsideDirNum++;
            emDir_Target = EarthTHandler.EM_ChunkDirection.N;
            fOutsideDeltaDis = Mathf.Abs(m_3dCamera.transform.localPosition.y - v2RangeVertical.y);
        }

        if (nOutsideDirNum == 1)
        {
            switch (emDir_Target)
            {
                case EarthTHandler.EM_ChunkDirection.W:
                case EarthTHandler.EM_ChunkDirection.E:
                    {
                        //Debug.Log("OuterPercent: " + (fOutsideDeltaDis / fRangeHorizontalDis));
                        if (fOutsideDeltaDis / fRangeHorizontalDis >= 0.1f)
                        {
                            return true;
                        }
                    }
                    break;
                case EarthTHandler.EM_ChunkDirection.S:
                case EarthTHandler.EM_ChunkDirection.N:
                    {
                        Debug.Log("OuterPercent: " + (fOutsideDeltaDis / fRangeVerticalDis));
                        if (fOutsideDeltaDis / fRangeVerticalDis >= 0.1f)
                        {
                            return true;
                        }
                    }
                    break;
                default: return false;
            }
            //string strDebug = string.Format("{0} | {1} | {2} | {3}",
            //    emDir.ToString(),
            //    fOutsideDeltaDis,
            //    fRangeHorizontalDis,
            //    fRangeVerticalDis
            //    );
            //Debug.Log(strDebug);
        }
        return false;
    }

    void TryMoveCameraCenterTo(float fLocalX, float fLocalY, bool bIsWithinRange = true)
    {
        float fAfterClampX = 0;
        float fAfterClampY = 0;
        if (bIsWithinRange)
        {
            Vector2 v2RangeHorizontal = GetCameraPosRangeInLocal(false);
            fAfterClampX = Mathf.Clamp(
                fLocalX,
                GameCommon.GetMinValueInVector2(v2RangeHorizontal),
                GameCommon.GetMaxValueInVector2(v2RangeHorizontal)
                );
            Vector2 v2RangeVertical = GetCameraPosRangeInLocal(true);
            fAfterClampY = Mathf.Clamp(
                fLocalY,
                GameCommon.GetMinValueInVector2(v2RangeVertical),
                GameCommon.GetMaxValueInVector2(v2RangeVertical)
                );
        }
        else
        {
            fAfterClampX = fLocalX;
            fAfterClampY = fLocalY;
        }

        SetCameraPosInLocal(fAfterClampX, fAfterClampY);
    }

    void SetCameraPosInLocal(float fLocalX, float fLocalY, bool bIsFillLogicPos = true)
    {
        //Debug.Log("SetCameraPosInLocal: "+ fLocalX + " , "+ fLocalY);
        m_3dCamera.transform.localPosition = new Vector3(
            fLocalX,
            fLocalY,
            m_3dCamera.transform.localPosition.z
            );

        Vector2 v2MapLogicPos = TransCameraLocalPosToMapLogicPos(m_3dCamera.transform.localPosition);

        //AfterRotate
        m_itCameraGoToLogicX_AfterRotate.value = v2MapLogicPos.x.ToString("0.0");
        m_itCameraGoToLogicY_AfterRotate.value = v2MapLogicPos.y.ToString("0.0");

        //BeforeRotate
        Vector2 v2MapLogicPos_BeforeRotate = EarthTileHelper.CalcCoordinateWithRotate(
            m_nMapCurLogicRotated90Times,
            v2MapLogicPos,
            m_v2MapLogicSize_BeforeRotate.x,
            m_v2MapLogicSize_BeforeRotate.y
            );
        m_itCameraGoToLogicX_BeforeRotate.value = v2MapLogicPos_BeforeRotate.x.ToString("0.0");
        m_itCameraGoToLogicY_BeforeRotate.value = v2MapLogicPos_BeforeRotate.y.ToString("0.0");

        //Ready ButtonJumpTo !!!
        Vector2 v2RangeHorizontal = GetCameraPosRangeInLocal(false);
        Vector2 v2RangeVertical = GetCameraPosRangeInLocal(true);
        if (fLocalX <= v2RangeHorizontal.x + 0.3f || fLocalX >= v2RangeHorizontal.y - 0.3f)
        {
            if (fLocalX <= v2RangeHorizontal.x + 0.3f)
            {
                bool bIsEnableDirW = IsAllowJumpTo(
                    m_emMapCurChunkId,
                    m_nMapCurLogicRotated90Times,
                    EarthTHandler.EM_ChunkDirection.W
                    );
                SetButtonJumpToHorizontal_W(true, bIsEnableDirW);
            }
            if (fLocalX >= v2RangeHorizontal.y - 0.3f)
            {
                bool bIsEnableDirE = IsAllowJumpTo(
                    m_emMapCurChunkId,
                    m_nMapCurLogicRotated90Times,
                    EarthTHandler.EM_ChunkDirection.E
                    );
                SetButtonJumpToHorizontal_E(true, bIsEnableDirE);
            }
        }
        else
        {
            SetButtonJumpToHorizontal_W(false, false);
            SetButtonJumpToHorizontal_E(false, false);
        }

        if (fLocalY <= v2RangeVertical.x + 0.3f || fLocalY >= v2RangeVertical.y - 0.3f)
        {
            if (fLocalY <= v2RangeVertical.x + 0.3f)
            {
                bool bIsEnableDirS = IsAllowJumpTo(
                    m_emMapCurChunkId,
                    m_nMapCurLogicRotated90Times,
                    EarthTHandler.EM_ChunkDirection.S
                    );
                SetButtonJumpToVertical_S(true, bIsEnableDirS);
            }
            if (fLocalY >= v2RangeVertical.y - 0.3f)
            {
                bool bIsEnableDirN = IsAllowJumpTo(
                    m_emMapCurChunkId,
                    m_nMapCurLogicRotated90Times,
                    EarthTHandler.EM_ChunkDirection.N
                    );
                SetButtonJumpToVertical_N(true, bIsEnableDirN);
            }
        }
        else
        {
            SetButtonJumpToVertical_S(false, false);
            SetButtonJumpToVertical_N(false, false);
        }
    }

    Vector2 TransMapLogicPosToCameraLocalPos(float fInMap_X, float fInMap_Y)
    {
        //Then Already AfterRotate
        Vector2 v2MapLogicCornerZero = CalcMapLogicCornerZeroInWorld();

        fInMap_X = Mathf.Clamp(fInMap_X, 0, m_v2MapLogicSize_AfterRotate.x);
        float fRatioX = fInMap_X / m_v2MapLogicSize_AfterRotate.x;
        float fInMap_X_WorldPos = v2MapLogicCornerZero.x + CalcMapLogicWidthInWorld() * fRatioX;

        fInMap_Y = Mathf.Clamp(fInMap_Y, 0, m_v2MapLogicSize_AfterRotate.y);
        float fRatioY = fInMap_Y / m_v2MapLogicSize_AfterRotate.y;
        float fInMap_Y_WorldPos = v2MapLogicCornerZero.y + CalcMapLogicHeightInWorld() * fRatioY;

        Vector2 v2InMap_LocalPos = m_3dCamera.transform.parent.InverseTransformPoint(
            new Vector2(fInMap_X_WorldPos, fInMap_Y_WorldPos)
            );

        return v2InMap_LocalPos;
    }

    Vector2 TransCameraLocalPosToMapLogicPos(Vector2 v2InMap_LocalPos)
    {
        //Then Already AfterRotate
        Vector2 v2MapLogicPosInWorld = m_3dCamera.transform.parent.TransformPoint(v2InMap_LocalPos);

        Vector2 v2MapLogicCornerZero = CalcMapLogicCornerZeroInWorld();

        float fRatioX = (v2MapLogicPosInWorld.x - v2MapLogicCornerZero.x) / CalcMapLogicWidthInWorld();
        float fInMap_X = fRatioX * m_v2MapLogicSize_AfterRotate.x;

        float fRatioY = (v2MapLogicPosInWorld.y - v2MapLogicCornerZero.y) / CalcMapLogicHeightInWorld();
        float fInMap_Y = fRatioY * m_v2MapLogicSize_AfterRotate.y;

        return new Vector2(fInMap_X, fInMap_Y);
    }

    void JumpAnimEarthTileTo(
        bool bIsOpenOpr,
        EarthTHandler.EM_ChunkIndex emTargetId,
        Vector2 v2PosRatioInChunk,
        float fOrthographicSizeFrom,
        float fOrthographicSizeTo
        )
    {
        GameCommon.ASSERT(m_stJumpAnimParam != null);
        m_stJumpAnimParam.CopyValueFrom(
            bIsOpenOpr, emTargetId, v2PosRatioInChunk,
            fOrthographicSizeFrom, fOrthographicSizeTo
            );

        if (bIsOpenOpr)
        {
            RefreshEarthTileTo(
                emTargetId,
                v2PosRatioInChunk,
                m_mMapLogicRotated90Times[emTargetId],
                fOrthographicSizeFrom
                );
        }
        else
        {
            if (m_emMapCurChunkId == emTargetId)
            {
                RefreshEarthTileTo(
                    emTargetId,
                    GetMapCurPosRatio(),
                    m_mMapLogicRotated90Times[emTargetId],
                    fOrthographicSizeFrom
                    );
            }
            else
            {
                RefreshEarthTileTo(
                    emTargetId,
                    v2PosRatioInChunk,
                    m_mMapLogicRotated90Times[emTargetId],
                    fOrthographicSizeFrom
                    );
            }
        }

        m_bIsRuningJumpAnim = true;
        m_fRuningJumpAnimCostTime = Time.time;
    }

    void RefreshEarthTileTo(
        EarthTHandler.EM_ChunkIndex emTargetId,
        Vector2 v2PosRatioInChunk,
        int nMapLogicRotatedTimes,
        float fOrthographicSize
        )
    {
        GameCommon.ASSERT(EarthTileHelper.CheckChunkIndex(emTargetId));
        GameCommon.ASSERT(nMapLogicRotatedTimes >= 0 && nMapLogicRotatedTimes <= 3);
        //Debug.Log("v2PosRatioInChunk = " + v2PosRatioInChunk);

        m_emMapCurChunkId = emTargetId;
        m_nMapCurLogicRotated90Times = nMapLogicRotatedTimes;
        m_labMapLogicRotated90Times.text = "MapLogicRotated: " + m_nMapCurLogicRotated90Times;

        m_redPlane.sharedMaterial.mainTexture = LoadDragMapTexture(m_emMapCurChunkId);
        m_redPlane.transform.localRotation = Quaternion.Euler(
            0,
            EarthTileHelper.GetRotateClampDegree(m_nMapCurLogicRotated90Times),
            0
            );
        m_v2MapLogicSize_AfterRotate = EarthTileHelper.GetCoordinateSizeWithRotate(
            m_nMapCurLogicRotated90Times,
            m_v2MapLogicSize_BeforeRotate
            );

        m_3dCamera.orthographicSize = fOrthographicSize;
        Vector2 v2CameraLocalPos = TransMapLogicPosToCameraLocalPos(
            m_v2MapLogicSize_AfterRotate.x * v2PosRatioInChunk.x,
            m_v2MapLogicSize_AfterRotate.y * v2PosRatioInChunk.y
            );
        Vector2 v2RangeHorizontal = GetCameraPosRangeInLocal(false);
        v2CameraLocalPos.x = Mathf.Clamp(
            v2CameraLocalPos.x,
            GameCommon.GetMinValueInVector2(v2RangeHorizontal),
            GameCommon.GetMaxValueInVector2(v2RangeHorizontal)
            );
        Vector2 v2RangeVertical = GetCameraPosRangeInLocal(true);
        v2CameraLocalPos.y = Mathf.Clamp(
            v2CameraLocalPos.y,
            GameCommon.GetMinValueInVector2(v2RangeVertical),
            GameCommon.GetMaxValueInVector2(v2RangeVertical)
            );
        SetCameraPosInLocal(v2CameraLocalPos.x, v2CameraLocalPos.y);
    }

    bool RefreshEarthTileTo(
        EarthTHandler.EM_ChunkIndex emMapId_Cur,
        int nMapLogicRotated90Times_Cur,
        EarthTHandler.EM_ChunkDirection emDir_Next,
        float fOrthographicSize
        )
    {
        GameCommon.ASSERT(EarthTileHelper.CheckChunkIndex(emMapId_Cur));
        GameCommon.ASSERT(nMapLogicRotated90Times_Cur >= 0 && nMapLogicRotated90Times_Cur <= 3);
        GameCommon.ASSERT(EarthTileHelper.CheckChunkDirection(emDir_Next));

        EarthTChunk stChunk = m_stEarthTileHandler.GetChunk(emMapId_Cur);
        GameCommon.ASSERT(stChunk != null);

        float fNeighborRotatedDegree = 0;
        EarthTChunk.ST_NeighborChunk stNeighborChunk = stChunk.GetNeighborChunk(
            emDir_Next,
            EarthTileHelper.GetRotateClampDegree(nMapLogicRotated90Times_Cur),
            out fNeighborRotatedDegree
            );
        GameCommon.ASSERT(stNeighborChunk != null);
        int nMapLogicRotated90Times_Neighbor = EarthTileHelper.GetRotate90DegreeTimes(fNeighborRotatedDegree);
        GameCommon.ASSERT(nMapLogicRotated90Times_Neighbor >= 0 && nMapLogicRotated90Times_Neighbor <= 3);

        if (!IsAllowJumpTo(emMapId_Cur, stNeighborChunk.GetChunk().GetId(), nMapLogicRotated90Times_Neighbor))
        {
            Debug.Log("IsAllowJumpTo False");
            return false;
        }

        Vector2 v2MapLogicPos_Cur = TransCameraLocalPosToMapLogicPos(m_3dCamera.transform.localPosition);
        Vector2 v2PosRatioInChunk_Cur = new Vector2(
            v2MapLogicPos_Cur.x / m_v2MapLogicSize_AfterRotate.x,
            v2MapLogicPos_Cur.y / m_v2MapLogicSize_AfterRotate.y
            );

        Vector2 v2PosRatioInChunk_Next = Vector2.zero;

        switch (emDir_Next)
        {
            case EarthTHandler.EM_ChunkDirection.N:
                {
                    v2PosRatioInChunk_Next.x = v2PosRatioInChunk_Cur.x;
                    v2PosRatioInChunk_Next.y = 0f;
                }
                break;
            case EarthTHandler.EM_ChunkDirection.E:
                {
                    v2PosRatioInChunk_Next.x = 0f;
                    v2PosRatioInChunk_Next.y = v2PosRatioInChunk_Cur.y;
                }
                break;
            case EarthTHandler.EM_ChunkDirection.S:
                {
                    v2PosRatioInChunk_Next.x = v2PosRatioInChunk_Cur.x;
                    v2PosRatioInChunk_Next.y = 1f;
                }
                break;
            case EarthTHandler.EM_ChunkDirection.W:
                {
                    v2PosRatioInChunk_Next.x = 1f;
                    v2PosRatioInChunk_Next.y = v2PosRatioInChunk_Cur.y;
                }
                break;
        }

        //Debug.Log("v2PosRatioInChunk_Next = " + v2PosRatioInChunk_Next);

        RefreshEarthTileTo(
            stNeighborChunk.GetChunk().GetId(),
            v2PosRatioInChunk_Next,
            nMapLogicRotated90Times_Neighbor,
            fOrthographicSize
            );

        return true;
    }

    void OnClick_BoxTexView(GameObject go)
    {
        PrintDebugLog(string.Format("OnClick_BoxTexView"));
        m_v2TouchOldPos1 = Vector2.zero;
        m_v2TouchOldPos2 = Vector2.zero;
    }

    void OnDragEnd_BoxTexView(GameObject go)
    {
        PrintDebugLog(string.Format("OnDragEnd_BoxTexView"));
        m_v2TouchOldPos1 = Vector2.zero;
        m_v2TouchOldPos2 = Vector2.zero;
    }









    void OnClick_BtnCameraGoTo_AfterRotate(GameObject go)
    {
        float fInMap_X = float.Parse(m_itCameraGoToLogicX_AfterRotate.value);
        fInMap_X = Mathf.Clamp(fInMap_X, 0, m_v2MapLogicSize_AfterRotate.x);
        //m_itCameraGoToX.value = fInMap_X.ToString("0.00");

        float fInMap_Y = float.Parse(m_itCameraGoToLogicY_AfterRotate.value);
        fInMap_Y = Mathf.Clamp(fInMap_Y, 0, m_v2MapLogicSize_AfterRotate.y);
        //m_itCameraGoToY.value = fInMap_Y.ToString("0.00");

        Vector2 v2InMap_LocalPos = TransMapLogicPosToCameraLocalPos(fInMap_X, fInMap_Y);

        TryMoveCameraCenterTo(v2InMap_LocalPos.x, v2InMap_LocalPos.y);
    }

    void OnClick_BtnCameraGoTo_BeforeRotate(GameObject go)
    {
        float fInMap_X = float.Parse(m_itCameraGoToLogicX_BeforeRotate.value);
        fInMap_X = Mathf.Clamp(fInMap_X, 0, m_v2MapLogicSize_BeforeRotate.x);
        //m_itCameraGoToX.value = fInMap_X.ToString("0.00");

        float fInMap_Y = float.Parse(m_itCameraGoToLogicY_AfterRotate.value);
        fInMap_Y = Mathf.Clamp(fInMap_Y, 0, m_v2MapLogicSize_BeforeRotate.y);
        //m_itCameraGoToY.value = fInMap_Y.ToString("0.00");

        Vector2 v2MapLogicPos_AfterRotate = EarthTileHelper.CalcCoordinateWithRotate_Opposite(
            m_nMapCurLogicRotated90Times,
            new Vector2(fInMap_X, fInMap_Y),
            m_v2MapLogicSize_BeforeRotate.x,
            m_v2MapLogicSize_BeforeRotate.y
            );

        Vector2 v2InMap_LocalPos = TransMapLogicPosToCameraLocalPos(v2MapLogicPos_AfterRotate.x, v2MapLogicPos_AfterRotate.y);

        TryMoveCameraCenterTo(v2InMap_LocalPos.x, v2InMap_LocalPos.y);
    }

    void OnClick_BtnJumpTo(GameObject go)
    {
        EarthTHandler.EM_ChunkDirection emDir_Next = (EarthTHandler.EM_ChunkDirection)UIEventListener.Get(go).parameter;
        RefreshEarthTileTo(
            m_emMapCurChunkId, 
            m_nMapCurLogicRotated90Times, 
            emDir_Next,
            m_3dCamera.orthographicSize
            );
    }


    void OnClick_BtnSelfLocation(GameObject go)
    {
        m_stUIDragSpring.StopMoving(true);
        
        EarthTHandler.EM_ChunkIndex emChunkId_Me = EarthTHandler.EM_ChunkIndex.Five;
        Vector2 v2PosRatioInChunk_Me = new Vector2(0.5f, 0.5f);

        JumpAnimEarthTileTo(
            false,
            emChunkId_Me,
            v2PosRatioInChunk_Me,
            m_3dCamera.orthographicSize,
            LogicWorldEarthMapHelper.Instance.GetOrthographicSizeLev(LogicWorldEarthMapHelper.EM_OrthographicSizeLev.Default)
            );
    }
















































    Texture LoadDragMapTexture(EarthTHandler.EM_ChunkIndex emId)
    {
        GameCommon.ASSERT(EarthTileHelper.CheckChunkIndex(emId));
        string strTexName = "WorldDragMap_Id_" + emId.ToString();
        UnityEngine.Object objTex = Resources.Load("UI/Texture/" + strTexName);
        GameCommon.ASSERT(objTex != null, "LoadDragMapTexture: " + strTexName);
        Texture tex = objTex as Texture;
        GameCommon.ASSERT(tex != null, "LoadDragMapTexture: " + strTexName);
        return tex;
    }

    UIButton GetButtonJumpTo(EarthTHandler.EM_ChunkDirection emDir)
    {
        UIButton btnRet;
        if (m_mButtonJumpTo.TryGetValue(emDir, out btnRet))
        {
            return btnRet;
        }
        return null;
    }

    void SetButtonJumpToHorizontal_W(bool bIsShowDirW, bool bIsEnableDirW)
    {
        UIButton btnDirW = GetButtonJumpTo(EarthTHandler.EM_ChunkDirection.W);
        GameCommon.ASSERT(btnDirW != null);
        btnDirW.gameObject.SetActive(bIsShowDirW);
        btnDirW.isEnabled = bIsEnableDirW;
    }

    void SetButtonJumpToHorizontal_E(bool bIsShowDirE, bool bIsEnableDirE)
    {
        UIButton btnDirE = GetButtonJumpTo(EarthTHandler.EM_ChunkDirection.E);
        GameCommon.ASSERT(btnDirE != null);
        btnDirE.gameObject.SetActive(bIsShowDirE);
        btnDirE.isEnabled = bIsEnableDirE;
    }

    void SetButtonJumpToVertical_S(bool bIsShowDirS, bool bIsEnableDirS)
    {
        UIButton btnDirS = GetButtonJumpTo(EarthTHandler.EM_ChunkDirection.S);
        GameCommon.ASSERT(btnDirS != null);
        btnDirS.gameObject.SetActive(bIsShowDirS);
        btnDirS.isEnabled = bIsEnableDirS;
    }

    void SetButtonJumpToVertical_N(bool bIsShowDirN, bool bIsEnableDirN)
    {
        UIButton btnDirN = GetButtonJumpTo(EarthTHandler.EM_ChunkDirection.N);
        GameCommon.ASSERT(btnDirN != null);
        btnDirN.gameObject.SetActive(bIsShowDirN);
        btnDirN.isEnabled = bIsEnableDirN;
    }

    bool IsAllowJumpTo(
        EarthTHandler.EM_ChunkIndex emId_From,
        EarthTHandler.EM_ChunkIndex emId_To,
        int nRotated90Times_To
        )
    {
        GameCommon.ASSERT(EarthTileHelper.CheckChunkIndex(emId_From));
        GameCommon.ASSERT(EarthTileHelper.CheckChunkIndex(emId_To));
        GameCommon.ASSERT(nRotated90Times_To >= 0 && nRotated90Times_To <= 3);

        List<ST_CustomChunkIndex> lstRet;
        if (!m_mCustomChunkTargetOnly.TryGetValue(emId_From, out lstRet))
        {
            return true;
        }

        foreach (ST_CustomChunkIndex _stId in lstRet)
        {
            if (_stId.GetId() == emId_To && _stId.GetRotated90Times() == nRotated90Times_To)
            {
                return true;
            }
        }
        return false;
    }

    bool IsAllowJumpTo(
        EarthTHandler.EM_ChunkIndex emId_From,
        int nMapLogicRotated90Times_From,
        EarthTHandler.EM_ChunkDirection emDir_To
        )
    {
        GameCommon.ASSERT(EarthTileHelper.CheckChunkIndex(emId_From));
        GameCommon.ASSERT(nMapLogicRotated90Times_From >= 0 && nMapLogicRotated90Times_From <= 3);
        GameCommon.ASSERT(EarthTileHelper.CheckChunkDirection(emDir_To));

        List<ST_CustomChunkIndex> lstRet;
        if (!m_mCustomChunkTargetOnly.TryGetValue(emId_From, out lstRet))
        {
            return true;
        }

        EarthTChunk stChunk = m_stEarthTileHandler.GetChunk(emId_From);
        GameCommon.ASSERT(stChunk != null);

        float fNeighborRotatedDegree = 0;
        EarthTChunk.ST_NeighborChunk stNeighborChunk = stChunk.GetNeighborChunk(
            emDir_To,
            EarthTileHelper.GetRotateClampDegree(nMapLogicRotated90Times_From),
            out fNeighborRotatedDegree
            );
        GameCommon.ASSERT(stNeighborChunk != null);
        int nMapLogicRotated90Times_Neighbor = EarthTileHelper.GetRotate90DegreeTimes(fNeighborRotatedDegree);
        GameCommon.ASSERT(nMapLogicRotated90Times_Neighbor >= 0 && nMapLogicRotated90Times_Neighbor <= 3);

        return IsAllowJumpTo(emId_From, stNeighborChunk.GetChunk().GetId(), nMapLogicRotated90Times_Neighbor);
    }

    Vector2 GetMapCurPosRatio()
    {
        //反向计算出当前摄像机中心点所对应的的[地图坐标比率]]
        Vector2 v2MapLogicPos = TransCameraLocalPosToMapLogicPos(m_3dCamera.transform.localPosition);
        Vector2 v2MapLogicPos_BeforeRotate = EarthTileHelper.CalcCoordinateWithRotate(
            m_nMapCurLogicRotated90Times,
            v2MapLogicPos,
            m_v2MapLogicSize_BeforeRotate.x,
            m_v2MapLogicSize_BeforeRotate.y
            );

        Vector2 v2RetRatio = Vector2.zero;
        v2RetRatio = new Vector2(
            v2MapLogicPos_BeforeRotate.x / m_v2MapLogicSize_BeforeRotate.x,
            v2MapLogicPos_BeforeRotate.y / m_v2MapLogicSize_BeforeRotate.y
            );
        v2RetRatio.x = Mathf.Clamp(v2RetRatio.x, 0f, 1f);
        v2RetRatio.y = Mathf.Clamp(v2RetRatio.y, 0f, 1f);

        return v2RetRatio;
    }

    void PrintDebugLog(string strLog)
    {
        if (m_stUIBoldTip != null)
        {
            m_stUIBoldTip.AddTip(strLog);
        }
        else
        {
            m_labDebugLog.text = strLog;
        }
    }

    #region Code Corner
    float CaclScreenWidthInWorld()
    {
        Vector3 v3Pos1 = m_3dCamera.ScreenToWorldPoint(m_aryCornerVectorInScreen[(int)(EM_CommonCornerPoint.BottomLeft)]);
        Vector3 v3Pos2 = m_3dCamera.ScreenToWorldPoint(m_aryCornerVectorInScreen[(int)(EM_CommonCornerPoint.TopRight)]);

        float fRet = Mathf.Abs(v3Pos1.x - v3Pos2.x);
        return fRet;
    }

    float CaclScreenHeightInWorld()
    {
        Vector3 v3Pos1 = m_3dCamera.ScreenToWorldPoint(m_aryCornerVectorInScreen[(int)(EM_CommonCornerPoint.BottomLeft)]);
        Vector3 v3Pos2 = m_3dCamera.ScreenToWorldPoint(m_aryCornerVectorInScreen[(int)(EM_CommonCornerPoint.TopRight)]);

        float fRet = Mathf.Abs(v3Pos1.y - v3Pos2.y);
        return fRet;
    }

    Vector2 CalcMapGlobalCornerPosRangeInWorld(bool bIsVertical)
    {
        if (!bIsVertical)
        {
            return new Vector2(
                m_aryMapGlobalCornerInWorld[(int)(EM_CommonCornerPoint.BottomLeft)].transform.position.x,
                m_aryMapGlobalCornerInWorld[(int)(EM_CommonCornerPoint.TopRight)].transform.position.x
                );
        }
        else
        {
            return new Vector2(
                m_aryMapGlobalCornerInWorld[(int)(EM_CommonCornerPoint.BottomLeft)].transform.position.y,
                m_aryMapGlobalCornerInWorld[(int)(EM_CommonCornerPoint.TopRight)].transform.position.y
                );
        }
    }

    Vector2 CalcMapLogicCornerPosRangeInWorld(bool bIsVertical)
    {
        if (!bIsVertical)
        {
            return new Vector2(
                m_aryMapLogicCornerInWorld[(int)(EM_CommonCornerPoint.BottomLeft)].transform.position.x,
                m_aryMapLogicCornerInWorld[(int)(EM_CommonCornerPoint.TopRight)].transform.position.x
                );
        }
        else
        {
            return new Vector2(
                m_aryMapLogicCornerInWorld[(int)(EM_CommonCornerPoint.BottomLeft)].transform.position.y,
                m_aryMapLogicCornerInWorld[(int)(EM_CommonCornerPoint.TopRight)].transform.position.y
                );
        }
    }

    Vector2 CalcMapGlobalCornerZeroInWorld()
    {
        return new Vector2(
            m_aryMapGlobalCornerInWorld[(int)(EM_CommonCornerPoint.BottomLeft)].transform.position.x,
            m_aryMapGlobalCornerInWorld[(int)(EM_CommonCornerPoint.BottomLeft)].transform.position.y
            );
    }

    Vector2 CalcMapLogicCornerZeroInWorld()
    {
        return new Vector2(
            m_aryMapLogicCornerInWorld[(int)(EM_CommonCornerPoint.BottomLeft)].transform.position.x,
            m_aryMapLogicCornerInWorld[(int)(EM_CommonCornerPoint.BottomLeft)].transform.position.y
            );
    }

    float CalcMapGlobalWidthInWorld()
    {
        Vector2 v2MapCornerRangeH = CalcMapGlobalCornerPosRangeInWorld(false);
        return Mathf.Abs(v2MapCornerRangeH.x - v2MapCornerRangeH.y);
    }

    float CalcMapGlobalHeightInWorld()
    {
        Vector2 v2MapCornerRangeV = CalcMapGlobalCornerPosRangeInWorld(true);
        return Mathf.Abs(v2MapCornerRangeV.x - v2MapCornerRangeV.y);
    }

    float CalcMapLogicWidthInWorld()
    {
        Vector2 v2MapCornerRangeH = CalcMapLogicCornerPosRangeInWorld(false);
        return Mathf.Abs(v2MapCornerRangeH.x - v2MapCornerRangeH.y);
    }

    float CalcMapLogicHeightInWorld()
    {
        Vector2 v2MapCornerRangeV = CalcMapLogicCornerPosRangeInWorld(true);
        return Mathf.Abs(v2MapCornerRangeV.x - v2MapCornerRangeV.y);
    }

    Vector2 GetCameraPosRangeInLocal(bool bIsVertical)
    {
        Vector2 v2LogicRangeInWorld = CalcMapLogicCornerPosRangeInWorld(bIsVertical);
        Vector2 v2GlobalRangeInWorld = CalcMapGlobalCornerPosRangeInWorld(bIsVertical);
        if (!bIsVertical)
        {
            v2GlobalRangeInWorld = new Vector2(
                v2GlobalRangeInWorld.x + CaclScreenWidthInWorld() / 2.0f,
                v2GlobalRangeInWorld.y - CaclScreenWidthInWorld() / 2.0f
                );
        }
        else
        {
            v2GlobalRangeInWorld = new Vector2(
                v2GlobalRangeInWorld.x + CaclScreenHeightInWorld() / 2.0f,
                v2GlobalRangeInWorld.y - CaclScreenHeightInWorld() / 2.0f
                );
        }

        Vector2 v2RealRangeInWorld = new Vector2(
            Mathf.Max(v2LogicRangeInWorld.x, v2GlobalRangeInWorld.x),
            Mathf.Min(v2LogicRangeInWorld.y, v2GlobalRangeInWorld.y)
            );

        Vector2 v2RealRangeInLocal = m_3dCamera.transform.parent.InverseTransformPoint(v2RealRangeInWorld);

        return v2RealRangeInLocal;
    }
    #endregion Code Corner
}
