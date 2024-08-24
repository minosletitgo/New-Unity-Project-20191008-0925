using UnityEngine;
using System;
using System.Collections.Generic;

/// <summary>
/// 设计顺序:
/// 01.初版只编写拖拽方向为[Vertical]，元素坐标延伸方向为[递减]
/// 02.核心函数:TestResetChildPosition
/// 03.核心函数:ClearVirtualChild | AddVirtualChild
/// 04.核心函数:ReCalcScrollBarSize
/// 05.核心函数:SetDragAmountValue(请保持所有元素的纵轴对齐方式为"上")
/// 06.核心函数:WrapContent
/// </summary>

[AddComponentMenu("NGUI/Interaction/Wrap ContentEx")]
public class UIWrapContentEx : MonoBehaviour
{
    public bool mIsDirectionAdd = false;
    public float mIntervalPixel = 0;

    [SerializeField]
    UIScrollBar mHorizontalScrollBar;
    [SerializeField]
    UIScrollBar mVerticalScrollBar;

    public delegate void OnFillingItem(GameObject go, int realIndex);
    public OnFillingItem mDGOnFillingItem;

    public delegate Bounds OnCalcItemCellSize(GameObject go, int realIndex);
    public OnCalcItemCellSize mDGOnCalcItemCellSize;


    protected Transform mTrans;
    protected UIPanel mPanel;
    protected UIScrollView mScroll;
    protected bool mHorizontal = false;

    protected List<Transform> mChildren = new List<Transform>();
    Dictionary<int, int> mDicChildToVitualIndex = new Dictionary<int, int>();
    GameObject mChildCommonCalcSize;

    protected int mVirtualChildCount = 0;
    protected float mTotalCellSize = 0;

    Dictionary<int, Bounds> mDicVirtualIndexToBounds = new Dictionary<int, Bounds>();





    protected virtual void Start()
    {
        StartChecking();
    }

    void StartChecking()
    {
        if (!CacheScrollView())
        {
            Debug.LogError("CacheScrollView.Failed!");
            return;
        }

        if (mPanel.clipOffset != Vector2.zero)
        {
            Debug.LogError("mPanel.clipOffset != Vector2.zero");
            return;
        }

        if (mPanel.baseClipRegion.x != 0.0f)
        {
            Debug.LogError("mPanel.baseClipRegion.x != 0.0f");
            return;
        }

        if (mPanel.baseClipRegion.y != 0.0f)
        {
            Debug.LogError("mPanel.baseClipRegion.y != 0.0f");
            return;
        }

        if (transform.childCount <= 0)
        {
            Debug.LogError("transform.childCount Failed");
            return;
        }

        if (Application.isPlaying)
        {
            if (mDGOnFillingItem == null)
            {
                Debug.LogError("mDGOnFillingItem Failed");
                return;
            }

            if (mDGOnCalcItemCellSize == null)
            {
                Debug.LogError("mDGOnCaclItemCellSize Failed");
                return;
            }
        }

        mChildren.Clear();
        for (int i = 0; i < mTrans.childCount; ++i)
        {
            Transform t = mTrans.GetChild(i);
            if (!t.gameObject.activeInHierarchy) continue;
            t.name = "Item_" + i;
            mChildren.Add(t);
        }
        ClearChildIndexToVirtual();
        ClearVirtualIndexToBounds();

        if (mChildCommonCalcSize == null)
        {
            Transform trPanelParent = mTrans.parent.parent;
            GameCommon.ASSERT(trPanelParent != null);
            GameObject go = Instantiate(mChildren[0].gameObject) as GameObject;
            go.name = "ItemAnalyse";
            go.gameObject.SetActive(true);
            go.transform.parent = trPanelParent;
            go.transform.localPosition = new Vector3(5000, 5000, 0);
            go.transform.localRotation = Quaternion.Euler(Vector3.zero);
            go.transform.localScale = Vector3.one;
            mChildCommonCalcSize = go;
        }
    }

    protected bool CacheScrollView()
    {
        mTrans = transform;
        mPanel = NGUITools.FindInParents<UIPanel>(gameObject);
        mPanel.onClipMove = OnMove;
        mScroll = mPanel.GetComponent<UIScrollView>();
        if (mScroll == null) return false;
        if (mScroll.movement == UIScrollView.Movement.Horizontal)
        {
            if (mHorizontalScrollBar == null)
            {
                return false;
            }
            mHorizontal = true;
        }
        else if (mScroll.movement == UIScrollView.Movement.Vertical)
        {
            if (mVerticalScrollBar == null)
            {
                return false;
            }
            mHorizontal = false;
        }
        else
        {
            return false;
        }
        return true;
    }

    [ContextMenu("Sort Based on Scroll Movement")]
    void SortBasedOnScrollMovement()
    {
        if (Application.isPlaying)
        {
            Debug.LogError("SortBasedOnScrollMovement : Application.isPlaying");
            return;
        }

        StartChecking();
        TestResetChildPosition();
    }

    protected virtual void OnMove(UIPanel panel,Vector2 v2Delta) { WrapContent(v2Delta); }

    void TestResetChildPosition()
    {
        Bounds bdLast = new Bounds();
        float fLocalPosLast = 0;
        for (int indexChild = 0;
            indexChild < mChildren.Count;
            indexChild++
            )
        {
            if (mHorizontal)
            {

            }
            else
            {
                if (mIsDirectionAdd)
                {

                }
                else
                {
                    Transform trChild = mChildren[indexChild];
                    trChild.gameObject.SetActive(true);

                    Bounds bd = NGUIMath.CalculateRelativeWidgetBounds(trChild, false);

                    if (indexChild == 0)
                    {
                        trChild.localPosition = new Vector3(0, 0, 0);
                    }
                    else
                    {
                        float fPos = fLocalPosLast;
                        fPos += bdLast.center.y;
                        fPos -= bdLast.size.y / 2.0f;
                        fPos -= bd.center.y;
                        fPos -= bd.size.y / 2.0f;
                        fPos -= mIntervalPixel;

                        trChild.localPosition = new Vector3(0, fPos, 0);
                    }

                    fLocalPosLast = trChild.localPosition.y;
                    bdLast = bd;
                }
            }
        }
    }

    public void ClearVirtualChild()
    {
        mVirtualChildCount = 0;

        ReCalcScrollBarSize(0, 0);
        SetDragAmountValue(0.0f);

        ClearVirtualIndexToBounds();
    }

    public void AddVirtualChild(int nCount)
    {
        GameCommon.ASSERT(nCount > 0);

        int nOldVirtualChildCount = mVirtualChildCount;
        mVirtualChildCount += nCount;
        ReCalcScrollBarSize(nOldVirtualChildCount, mVirtualChildCount - 1);
    }

    void ReCalcScrollBarSize(int indexBegin, int indexEnd)
    {
        if (mVirtualChildCount == 0)
        {
            mTotalCellSize = 0;

            mVerticalScrollBar.barSize = 1.0f;
        }
        else
        {
            GameCommon.ASSERT(indexBegin >= 0 && indexBegin < mVirtualChildCount);
            GameCommon.ASSERT(indexEnd >= 0 && indexEnd < mVirtualChildCount);
            GameCommon.ASSERT(indexBegin <= indexEnd);

            if (indexBegin == 0)
            {
                mTotalCellSize = 0;
            }

            for (int i = indexBegin; i <= indexEnd; i++)
            {
                Bounds bd = mDGOnCalcItemCellSize(mChildCommonCalcSize, i);
                AddVirtualIndexToBounds(i, bd);
                mTotalCellSize += bd.size.y;
            }

            if (mHorizontal)
            {

            }
            else
            {
                mVerticalScrollBar.barSize = Mathf.Clamp01(mPanel.baseClipRegion.w / mTotalCellSize);
            }
        }
    }

    void ReCalcScrollBarVaue()
    {
        int indexChild_Min = -1;
        int indexVirtual_Min = FindVirtualMinChild(out indexChild_Min);
        //Debug.Log(indexVirtual_Min + " | " + indexChild_Min);

        GameCommon.ASSERT(indexVirtual_Min >= 0 && indexVirtual_Min < mVirtualChildCount);
        GameCommon.ASSERT(indexChild_Min >= 0 && indexChild_Min < mChildren.Count);

        Transform trChild_Min = mChildren[indexChild_Min];
        GameCommon.ASSERT(trChild_Min != null);
        GameCommon.ASSERT(trChild_Min.gameObject.activeSelf);

        Bounds bd_Min;
        GameCommon.ASSERT(GetVirtualIndexToBounds(indexVirtual_Min, out bd_Min));
        Vector3 v3Pos_Min = UICamera.currentCamera.WorldToScreenPoint(trChild_Min.position);


        //反向计算[Clip裁剪区-上底边线]上的尺寸，进而直接计算“滑动条写入数值”
        Vector3[] corners = mPanel.worldCorners;
        for (int i = 0; i < corners.Length; ++i)
        {
            Vector3 v = corners[i];
            //v = mTrans.InverseTransformPoint(v);
            v = UICamera.currentCamera.WorldToScreenPoint(v);
            corners[i] = v;
        }









        if (mHorizontal)
        {

        }
        else
        {
            if (mIsDirectionAdd)
            {

            }
            else
            {
                //修正计算具体的Min                
                float fPosUp_Min = v3Pos_Min.y + bd_Min.center.y + bd_Min.size.y / 2.0f;
                float fPosBottom_Min = v3Pos_Min.y + bd_Min.center.y - bd_Min.size.y / 2.0f;
                //Debug.Log(indexVirtual_Min + " -> " + fPosUp_Min + " , " + fPosBottom_Min);
                //return;
                bool bIsFindCross = false;
                int indexVirtual_Cross = -1;
                float fSizeUpToCross = 0;

                if (fPosUp_Min < corners[1].y)
                {
                    float fPosUp_Last = 0;
                    for (int indexVirtual = indexVirtual_Min; indexVirtual >= 0; indexVirtual--)
                    {
                        Bounds bd;
                        GameCommon.ASSERT(GetVirtualIndexToBounds(indexVirtual, out bd));

                        float fPosUp = 0;
                        float fPosBottom = 0;
                        if (indexVirtual == indexVirtual_Min)
                        {
                            fPosUp = v3Pos_Min.y + bd.center.y + bd.size.y / 2.0f;
                            fPosBottom = v3Pos_Min.y + bd.center.y - bd.size.y / 2.0f;
                        }
                        else
                        {
                            fPosUp = fPosUp_Last + bd.size.y;
                            fPosBottom = fPosUp_Last;
                        }

                        if (fPosUp >= corners[1].y && fPosBottom <= corners[1].y)
                        {
                            bIsFindCross = true;
                            indexVirtual_Cross = indexVirtual;
                            fSizeUpToCross = fPosUp - corners[1].y;
                            break;
                        }

                        if (fPosBottom > corners[1].y)
                        {
                            bIsFindCross = true;
                            indexVirtual_Cross = indexVirtual;
                            fSizeUpToCross = fPosUp - corners[1].y;
                            break;
                        }

                        fPosUp_Last = fPosUp;
                        fPosUp_Last += mIntervalPixel;
                    }
                }
                else if(fPosUp_Min == corners[1].y)
                {
                    bIsFindCross = true;
                    indexVirtual_Cross = indexVirtual_Min;
                    fSizeUpToCross = 0;
                }
                else if (fPosUp_Min > corners[1].y && fPosBottom_Min < corners[1].y)
                {
                    bIsFindCross = true;
                    indexVirtual_Cross = indexVirtual_Min;
                    fSizeUpToCross = fPosUp_Min - corners[1].y;
                }
                else if (fPosBottom_Min > corners[1].y)
                {
                    for (int indexVirtual = indexVirtual_Min; indexVirtual < mVirtualChildCount; indexVirtual++)
                    {
                        int indexChild = -1;
                        GameCommon.ASSERT(FindIndexByVirtual(indexVirtual, out indexChild));
                        GameCommon.ASSERT(indexChild >= 0 && indexChild < mChildren.Count);

                        Transform trChild = mChildren[indexChild];
                        GameCommon.ASSERT(trChild != null);
                        GameCommon.ASSERT(trChild.gameObject.activeSelf);

                        Bounds bd;
                        GameCommon.ASSERT(GetVirtualIndexToBounds(indexVirtual, out bd));

                        Vector3 v3Pos = UICamera.currentCamera.WorldToScreenPoint(trChild.position);
                        float fPosUp = v3Pos.y + bd.center.y + bd.size.y / 2.0f;
                        float fPosBottom = v3Pos.y + bd.center.y - bd.size.y / 2.0f;                        
                        if (fPosUp >= corners[1].y && fPosBottom <= corners[1].y)
                        {
                            bIsFindCross = true;
                            indexVirtual_Cross = indexVirtual;
                            fSizeUpToCross = fPosUp - corners[1].y;
                            break;
                        }

                        if (fPosUp < corners[1].y)
                        {
                            bIsFindCross = true;
                            indexVirtual_Cross = indexVirtual;
                            fSizeUpToCross = fPosUp - corners[1].y;
                            break;
                        }
                    }
                }
                else if (fPosBottom_Min == corners[1].y)
                {
                    bIsFindCross = true;
                    indexVirtual_Cross = indexVirtual_Min + 1;
                    fSizeUpToCross = 0;
                }



                if (bIsFindCross)
                {
                    GameCommon.ASSERT(indexVirtual_Cross >= 0 && indexVirtual_Cross < mVirtualChildCount);
                    //GameCommon.CHECK(fSizeUpToCross >= 0);
                    fSizeUpToCross = Math.Max(0, fSizeUpToCross);

                    if (indexVirtual_Cross > 0)
                    {
                        for (int indexVirtual = indexVirtual_Cross - 1; indexVirtual >= 0; indexVirtual--)
                        {
                            Bounds bd;
                            GameCommon.ASSERT(GetVirtualIndexToBounds(indexVirtual, out bd));

                            fSizeUpToCross += bd.size.y;
                            fSizeUpToCross += mIntervalPixel;
                        }
                    }                  

                    float fValue = fSizeUpToCross / (mTotalCellSize - mPanel.baseClipRegion.w);
                    mVerticalScrollBar.value = fValue;
                }
            }
        }        
    }

    public void SetDragAmountValue(float fValue)
    {
        foreach (Transform trChild in mChildren)
        {
            trChild.gameObject.SetActive(false);
        }
        ClearChildIndexToVirtual();

        if (mHorizontal)
        {

        }
        else
        {
            if (mVirtualChildCount == 0)
            {
                mPanel.transform.localPosition = Vector3.zero;
                mPanel.clipOffset = Vector3.zero;

                mVerticalScrollBar.value = 0;
            }
            else
            {
                //由AmountValue反向演算:
                //01.[VirtualChild矩形]头部超出[Clip裁剪区域]的尺寸
                //02.VirtualChild的具体坐标
                //03.Panel的具体坐标
                //04.ScrollBar.value值写入

                if (mTotalCellSize <= mPanel.baseClipRegion.w)
                {
                    if (mIsDirectionAdd)
                    {

                    }
                    else
                    {
                        //02.Part
                        //演算出第一个应该显示的Child，当前就是第一个Child
                        Bounds bdLast = new Bounds();
                        float fLocalPosLast = 0f;
                        //从第一个显示的Child开始，向后演算，直到最后一个应该显示的Child
                        for (int indexVirtual = 0, indexChild = 0;
                            indexVirtual < mVirtualChildCount && indexChild < mChildren.Count;
                            indexVirtual++, indexChild++
                            )
                        {
                            Transform trChild = mChildren[indexChild];
                            trChild.gameObject.SetActive(true);

                            mDGOnFillingItem(trChild.gameObject, indexVirtual);

                            Bounds bd;
                            GameCommon.ASSERT(GetVirtualIndexToBounds(indexVirtual, out bd));

                            if (indexChild == 0)
                            {
                                trChild.localPosition = new Vector3(0, 0, 0);
                            }
                            else
                            {
                                float fPos = fLocalPosLast;
                                fPos += bdLast.center.y;
                                fPos -= bdLast.size.y / 2.0f;
                                fPos -= bd.center.y;
                                fPos -= bd.size.y / 2.0f;
                                fPos -= mIntervalPixel;

                                trChild.localPosition = new Vector3(0, fPos, 0);
                            }

                            fLocalPosLast = trChild.localPosition.y;
                            bdLast = bd;

                            AddChildIndexToVirtual(indexChild, indexVirtual);
                        }

                        //03.Part
                        mPanel.transform.localPosition = Vector3.zero;
                        mPanel.clipOffset = Vector3.zero;

                        //04.Part
                        mVerticalScrollBar.value = 0;
                    }
                }
                else
                {
                    if (mIsDirectionAdd)
                    {

                    }
                    else
                    {
                        //01.Part
                        float fDisAmountValue = (mTotalCellSize - mPanel.baseClipRegion.w) * fValue;
                        GameCommon.ASSERT(fDisAmountValue >= 0);

                        //02.Part
                        //演算出第一个应该显示的Child，当前需要计算                        
                        int indexVirtualFirstShow = -1;
                        float fTotalCellSize = 0;
                        float fDeltaCellSize = 0;
                        for (int indexVirtual = 0;
                            indexVirtual < mVirtualChildCount;
                            indexVirtual++
                            )
                        {
                            Bounds bd = mDGOnCalcItemCellSize(mChildCommonCalcSize, indexVirtual);
                            fTotalCellSize += bd.size.y;

                            if (fTotalCellSize >= fDisAmountValue)
                            {
                                indexVirtualFirstShow = indexVirtual;
                                fDeltaCellSize = bd.size.y - (fTotalCellSize - fDisAmountValue);
                                break;
                            }
                        }

                        GameCommon.ASSERT(indexVirtualFirstShow >= 0);
                        GameCommon.ASSERT(fDeltaCellSize >= 0);//预留给mPanel

                        fTotalCellSize = 0;

                        Bounds bdLast = new Bounds();
                        float fLocalPosLast = 0f;
                        //从第一个显示的Child开始，向后演算，直到最后一个应该显示的Child
                        for (int indexVirtual = indexVirtualFirstShow, indexChild = 0;
                            indexVirtual < mVirtualChildCount && indexChild < mChildren.Count;
                            indexVirtual++, indexChild++
                            )
                        {
                            Transform trChild = mChildren[indexChild];
                            trChild.gameObject.SetActive(true);

                            mDGOnFillingItem(trChild.gameObject, indexVirtual);

                            Bounds bd;
                            GameCommon.ASSERT(GetVirtualIndexToBounds(indexVirtual, out bd));

                            if (indexChild == 0)
                            {
                                trChild.localPosition = new Vector3(0, 0, 0);

                                fTotalCellSize = bd.size.y - fDeltaCellSize;
                            }
                            else
                            {
                                float fPos = fLocalPosLast;
                                fPos += bdLast.center.y;
                                fPos -= bdLast.size.y / 2.0f;
                                fPos -= bd.center.y;
                                fPos -= bd.size.y / 2.0f;
                                fPos -= mIntervalPixel;

                                trChild.localPosition = new Vector3(0, fPos, 0);

                                fTotalCellSize += bd.size.y;
                            }

                            fLocalPosLast = trChild.localPosition.y;
                            bdLast = bd;

                            AddChildIndexToVirtual(indexChild, indexVirtual);

                            if (fTotalCellSize > mPanel.baseClipRegion.w)
                            {
                                break;
                            }
                        }

                        //03.Part
                        mPanel.transform.localPosition = new Vector3(0, fDeltaCellSize, 0);
                        mPanel.clipOffset = new Vector3(0, -fDeltaCellSize, 0);

                        //04.Part
                        mVerticalScrollBar.value = fValue;
                    }
                }
            }
        }
    }

    public float GetDragAmountValue()
    {
        if (mHorizontal)
        {

        }
        else
        {
            return mVerticalScrollBar.value;
        }

        return 0f;
    }

    bool AddChildIndexToVirtual(int indexChild, int indexVirtual)
    {
        GameCommon.ASSERT(indexChild >= 0 && indexChild < mChildren.Count);
        GameCommon.ASSERT(indexVirtual >= 0 && indexVirtual < mVirtualChildCount);

        mDicChildToVitualIndex.Add(indexChild, indexVirtual);

        return true;
    }

    bool ReduceChildIndexToVirtual(int indexChild)
    {
        GameCommon.ASSERT(indexChild >= 0 && indexChild < mChildren.Count);

        return mDicChildToVitualIndex.Remove(indexChild);
    }

    void ClearChildIndexToVirtual()
    {
        mDicChildToVitualIndex.Clear();
    }

    bool FindIndexByVirtual(int _indexVirtual, out int _indexChild)
    {
        _indexChild = -1;

        foreach (KeyValuePair<int, int> pPair in mDicChildToVitualIndex)
        {
            int indexChild = pPair.Key;
            int indexVirtual = pPair.Value;

            if (_indexVirtual == indexVirtual)
            {
                _indexChild = indexChild;
                return true;
            }
        }
        return false;
    }

    bool IsExistByVirtual(int _indexVirtual)
    {
        foreach (KeyValuePair<int, int> pPair in mDicChildToVitualIndex)
        {
            int indexChild = pPair.Key;
            int indexVirtual = pPair.Value;

            if (_indexVirtual == indexVirtual)
            {
                return true;
            }
        }
        return false;
    }

    bool FindIndexByChild(int _indexChild, out int _indexVirtual)
    {
        _indexVirtual = -1;

        if (mDicChildToVitualIndex.TryGetValue(_indexChild, out _indexVirtual))
        {
            return true;
        }
        return false;
    }

    Transform FindIdleChild(out int _indexChild)
    {
        Transform trChildReturn = null;
        _indexChild = 0;
        int indexChild = 0;
        foreach (Transform trChild in mChildren)
        {
            if (mDicChildToVitualIndex.ContainsKey(indexChild))
            {
                indexChild++;
                continue;
            }

            trChildReturn = trChild;
            _indexChild = indexChild;
            return trChildReturn;
        }
        
        return trChildReturn;
    }

    int FindVirtualMinChild(out int _indexChild)
    {
        int _indexVirtual = int.MaxValue;
        _indexChild = -1;

        foreach (KeyValuePair<int, int> pPair in mDicChildToVitualIndex)
        {
            int indexChild = pPair.Key;
            int indexVirtual = pPair.Value;

            if (indexVirtual <= _indexVirtual)
            {
                _indexVirtual = indexVirtual;
                _indexChild = indexChild;
            }
        }

        return _indexVirtual;
    }

    public string PrintDicChildToVitualIndex()
    {
        string strDebug = "PrintDicChildToVitualIndex: \n\n";
        int nLoopTimes = 0;
        foreach (KeyValuePair<int, int> pPair in mDicChildToVitualIndex)
        {
            int indexChild = pPair.Key;
            int indexVirtual = pPair.Value;

            strDebug += string.Format("{0} -> [FFFF00]{1}[-]", indexChild, indexVirtual);

            if (nLoopTimes % 2 != 0)
            {
                strDebug += "\n";
            }
            else
            {
                strDebug += "    ";
            }            
        }
        return strDebug;
    }

    void AddVirtualIndexToBounds(int indexVirtual, Bounds bd)
    {
        mDicVirtualIndexToBounds.Add(indexVirtual, bd);
    }

    bool GetVirtualIndexToBounds(int indexVirtual, out Bounds bd)
    {
        if (mDicVirtualIndexToBounds.TryGetValue(indexVirtual, out bd))
        {
            return true;
        }
        return false;
    }

    void ClearVirtualIndexToBounds()
    {
        mDicVirtualIndexToBounds.Clear();
    }

    protected virtual void WrapContent(Vector2 v2Delta)
    {
        if (v2Delta == Vector2.one)
        {
            return;
        }
        
        Vector3[] corners = mPanel.worldCorners;
        for (int i = 0; i < corners.Length; ++i)
        {
            Vector3 v = corners[i];
            //v = mTrans.InverseTransformPoint(v);
            v = UICamera.currentCamera.WorldToScreenPoint(v);
            corners[i] = v;
        }

        if (mHorizontal)
        {

        }
        else
        {
            if (mIsDirectionAdd)
            {

            }
            else
            {
                if (v2Delta.y < 0)
                {
                    //把mPanel向上拖拽

                    int indexVirtualReadyAdd = -1;//准备收集理应[添加]的Child
                    bool bIsDragToLimit = false;
                    foreach (KeyValuePair<int, int> pPair in mDicChildToVitualIndex)
                    {
                        int indexChild = pPair.Key;
                        int indexVirtual = pPair.Value;

                        Transform trChild = mChildren[indexChild];
                        GameCommon.ASSERT(trChild.gameObject.activeSelf);

                        mDGOnFillingItem(trChild.gameObject, indexVirtual);

                        Bounds bd;
                        GameCommon.ASSERT(GetVirtualIndexToBounds(indexVirtual, out bd));

                        Vector3 v3Pos = UICamera.currentCamera.WorldToScreenPoint(trChild.position);                     
                        float fPosUp = v3Pos.y + bd.center.y + bd.size.y / 2.0f;
                        float fPosBottom = v3Pos.y + bd.center.y - bd.size.y / 2.0f;

                        if (fPosBottom > corners[0].y &&
                            indexVirtual == mVirtualChildCount - 1
                            )
                        {
                            //已经达到拖拽极限
                            bIsDragToLimit = true;
                        }

                        bool bIsBingo = false;
                        if (fPosUp > corners[0].y &&
                            fPosBottom < corners[0].y &&
                            indexVirtual < mVirtualChildCount - 1 &&
                            !IsExistByVirtual(indexVirtual + 1)
                            )
                        {
                            //逐个对比Child的上下面积区域，选取刚好压住[Clip裁剪区-下底边线]
                            bIsBingo = true;
                            indexVirtualReadyAdd = indexVirtual + 1;
                        }

                        if (fPosBottom > corners[0].y &&
                            indexVirtual < mVirtualChildCount - 1 &&
                            !IsExistByVirtual(indexVirtual + 1)
                            )
                        {
                            //逐个对比Child的上下面积区域，选取完全高于[Clip裁剪区-下底边线]
                            //针对两帧之间的拖拽跨度过大
                            bIsBingo = true;
                            indexVirtualReadyAdd = indexVirtual + 1;
                        }

                        if (bIsBingo)
                        {
                            break;
                        }
                    }

                    if (indexVirtualReadyAdd > 0)
                    {
                        GameCommon.ASSERT(!IsExistByVirtual(indexVirtualReadyAdd));

                        //查找上一个
                        int indexVirtual_Last = indexVirtualReadyAdd - 1;
                        int indexChild_Last = -1;
                        GameCommon.ASSERT(FindIndexByVirtual(indexVirtual_Last, out indexChild_Last));
                        GameCommon.ASSERT(indexChild_Last >= 0 && indexChild_Last < mChildren.Count);
                        Transform trChild_Last = mChildren[indexChild_Last];
                        GameCommon.ASSERT(trChild_Last != null);

                        Bounds bd_Last;//只是单纯拿Bounds，可以考虑[Bounds缓存池]来优化 
                        GameCommon.ASSERT(GetVirtualIndexToBounds(indexVirtual_Last, out bd_Last));

                        int indexVirtual = indexVirtualReadyAdd;
                        int indexChild = -1;

                        int nWhileTimes = 0;
                        while (true)
                        {
                            if (nWhileTimes >= 100)
                            {
                                //异常
                                break;
                            }
                                                        
                            GameCommon.ASSERT(FindIdleChild(out indexChild));
                            GameCommon.ASSERT(indexChild >= 0 && indexChild < mChildren.Count);
                            Transform trChild = mChildren[indexChild];
                            GameCommon.ASSERT(trChild != null);
                            trChild.gameObject.SetActive(true);
                            AddChildIndexToVirtual(indexChild, indexVirtual);

                            mDGOnFillingItem(trChild.gameObject, indexVirtual);

                            Bounds bd;
                            GameCommon.ASSERT(GetVirtualIndexToBounds(indexVirtual, out bd));

                            float fPosTarget = trChild_Last.localPosition.y;
                            fPosTarget += bd_Last.center.y;
                            fPosTarget -= bd_Last.size.y / 2.0f;
                            fPosTarget -= bd.center.y;
                            fPosTarget -= bd.size.y / 2.0f;
                            fPosTarget -= mIntervalPixel;

                            trChild.localPosition = new Vector3(0, fPosTarget, 0);

                            Vector3 v3Pos = UICamera.currentCamera.WorldToScreenPoint(trChild.position);
                            float fPosBottom = v3Pos.y + bd.center.y - bd.size.y / 2.0f;

                            if (fPosBottom < corners[0].y || indexVirtual == mVirtualChildCount - 1)
                            {
                                break;
                            }

                            //自动移到下一个
                            indexVirtual++;
                            trChild_Last = trChild;
                            bd_Last = bd;

                            nWhileTimes++;
                        }
                    }
    
                    if (!bIsDragToLimit)
                    {
                        List<int> lstIndexReadyRemove = new List<int>();
                        foreach (KeyValuePair<int, int> pPair in mDicChildToVitualIndex)
                        {
                            int indexChild = pPair.Key;
                            int indexVirtual = pPair.Value;

                            Transform trChild = mChildren[indexChild];
                            GameCommon.ASSERT(trChild.gameObject.activeSelf);

                            Bounds bd;//只是单纯拿Bounds，可以考虑[Bounds缓存池]来优化
                            GameCommon.ASSERT(GetVirtualIndexToBounds(indexVirtual, out bd));

                            Vector3 v3Pos = UICamera.currentCamera.WorldToScreenPoint(trChild.position);
                            float fPosBottom = v3Pos.y + bd.center.y - bd.size.y / 2.0f;
                            if (fPosBottom > corners[1].y)
                            {
                                lstIndexReadyRemove.Add(indexChild);
                            }
                        }

                        if (lstIndexReadyRemove.Count > 0)
                        {
                            foreach (int indexRemove in lstIndexReadyRemove)
                            {
                                GameCommon.ASSERT(indexRemove >= 0 && indexRemove < mChildren.Count);

                                Transform trChildRemove = mChildren[indexRemove];
                                GameCommon.ASSERT(trChildRemove != null);
                                trChildRemove.gameObject.SetActive(false);
                                ReduceChildIndexToVirtual(indexRemove);
                            }
                        }
                    }
                }
                else
                {
                    //把mPanel向下拖拽

                    int indexVirtualReadyAdd = -1;//准备收集理应[添加]的Child
                    bool bIsDragToLimit = false;
                    foreach (KeyValuePair<int, int> pPair in mDicChildToVitualIndex)
                    {
                        int indexChild = pPair.Key;
                        int indexVirtual = pPair.Value;

                        Transform trChild = mChildren[indexChild];
                        GameCommon.ASSERT(trChild.gameObject.activeSelf);

                        Bounds bd;//只是单纯拿Bounds，可以考虑[Bounds缓存池]来优化
                        GameCommon.ASSERT(GetVirtualIndexToBounds(indexVirtual, out bd));

                        Vector3 v3Pos = UICamera.currentCamera.WorldToScreenPoint(trChild.position);
                        float fPosUp = v3Pos.y + bd.center.y + bd.size.y / 2.0f;
                        float fPosBottom = v3Pos.y + bd.center.y - bd.size.y / 2.0f;

                        if (fPosUp < corners[1].y &&
                            indexVirtual == 0
                            )
                        {
                            //已经达到拖拽极限
                            bIsDragToLimit = true;
                        }

                        bool bIsBingo = false;
                        if (fPosUp > corners[1].y &&
                            fPosBottom < corners[1].y &&
                            indexVirtual > 0 &&
                            !IsExistByVirtual(indexVirtual - 1)
                            )
                        {
                            //逐个对比Child的上下面积区域，选取刚好压住[Clip裁剪区-上底边线]
                            bIsBingo = true;
                            indexVirtualReadyAdd = indexVirtual - 1;
                        }

                        if (fPosUp < corners[1].y &&
                            indexVirtual > 0 &&
                            !IsExistByVirtual(indexVirtual - 1)
                            )
                        {
                            //逐个对比Child的上下面积区域，选取完全低于[Clip裁剪区-上底边线]
                            //针对两帧之间的拖拽跨度过大
                            bIsBingo = true;
                            indexVirtualReadyAdd = indexVirtual - 1;
                        }
                                               
                        if (bIsBingo)
                        {
                            break;
                        }
                    }

                    if (indexVirtualReadyAdd >= 0)
                    {
                        GameCommon.ASSERT(!IsExistByVirtual(indexVirtualReadyAdd));

                        //查找上一个
                        int indexVirtual_Last = indexVirtualReadyAdd + 1;
                        int indexChild_Last = -1;
                        GameCommon.ASSERT(FindIndexByVirtual(indexVirtual_Last, out indexChild_Last));
                        GameCommon.ASSERT(indexChild_Last >= 0 && indexChild_Last < mChildren.Count);
                        Transform trChild_Last = mChildren[indexChild_Last];
                        GameCommon.ASSERT(trChild_Last != null);

                        Bounds bd_Last;//只是单纯拿Bounds，可以考虑[Bounds缓存池]来优化 
                        GameCommon.ASSERT(GetVirtualIndexToBounds(indexVirtual_Last, out bd_Last));

                        int indexVirtual = indexVirtualReadyAdd;
                        int indexChild = -1;

                        int nWhileTimes = 0;
                        while (true)
                        {
                            if (nWhileTimes >= 100)
                            {
                                //异常
                                break;
                            }

                            GameCommon.ASSERT(FindIdleChild(out indexChild));
                            GameCommon.ASSERT(indexChild >= 0 && indexChild < mChildren.Count);
                            Transform trChild = mChildren[indexChild];
                            GameCommon.ASSERT(trChild != null);
                            trChild.gameObject.SetActive(true);
                            AddChildIndexToVirtual(indexChild, indexVirtual);

                            mDGOnFillingItem(trChild.gameObject, indexVirtual);

                            Bounds bd;
                            GameCommon.ASSERT(GetVirtualIndexToBounds(indexVirtual, out bd));

                            float fPosTarget = trChild_Last.localPosition.y;
                            fPosTarget += bd_Last.center.y;
                            fPosTarget += bd_Last.size.y / 2.0f;
                            fPosTarget -= bd.center.y;
                            fPosTarget += bd.size.y / 2.0f;
                            fPosTarget += mIntervalPixel;

                            trChild.localPosition = new Vector3(0, fPosTarget, 0);

                            Vector3 v3Pos = UICamera.currentCamera.WorldToScreenPoint(trChild.position);
                            float fPosUp = v3Pos.y + bd.center.y + bd.size.y / 2.0f;

                            if (fPosUp > corners[1].y || indexVirtual == 0)
                            {
                                break;
                            }

                            //自动移到下一个
                            indexVirtual--;
                            trChild_Last = trChild;
                            bd_Last = bd;

                            nWhileTimes++;
                        }
                    }

                    if(!bIsDragToLimit)
                    {
                        List<int> lstIndexReadyRemove = new List<int>();
                        foreach (KeyValuePair<int, int> pPair in mDicChildToVitualIndex)
                        {
                            int indexChild = pPair.Key;
                            int indexVirtual = pPair.Value;

                            Transform trChild = mChildren[indexChild];
                            GameCommon.ASSERT(trChild.gameObject.activeSelf);

                            Bounds bd;//只是单纯拿Bounds，可以考虑[Bounds缓存池]来优化 
                            GameCommon.ASSERT(GetVirtualIndexToBounds(indexVirtual, out bd));

                            //逐个对比Child的上下面积区域，选取刚好低于[Clip裁剪区-下底边线]
                            Vector3 v3Pos = UICamera.currentCamera.WorldToScreenPoint(trChild.position);
                            float fPosUp = v3Pos.y + bd.center.y + bd.size.y / 2.0f;
                            if (fPosUp < corners[0].y)
                            {
                                lstIndexReadyRemove.Add(indexChild);
                            }
                        }

                        if (lstIndexReadyRemove.Count > 0)
                        {
                            foreach (int indexRemove in lstIndexReadyRemove)
                            {
                                GameCommon.ASSERT(indexRemove >= 0 && indexRemove < mChildren.Count);

                                Transform trChildRemove = mChildren[indexRemove];
                                GameCommon.ASSERT(trChildRemove != null);
                                trChildRemove.gameObject.SetActive(false);
                                ReduceChildIndexToVirtual(indexRemove);
                            }
                        }
                    }                    
                }

                //还原计算ScrollBar.Value
                ReCalcScrollBarVaue();                
            }
        }
    }
}
