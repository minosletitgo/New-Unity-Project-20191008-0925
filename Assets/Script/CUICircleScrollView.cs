using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CUICircleScrollView : MonoBehaviour
{
    public UIPanel mPanel;
    public UIScrollView mSView;
    Vector4 mV4ClipRegion;
    public UIGrid mGrid;
    UICenterOnChild mCenterOnChild;
    public CUICircleScrollView_Item mItemInst;
    List<CUICircleScrollView_Item> mLstViewItem = new List<CUICircleScrollView_Item>();
    public GameObject mGoBottomPoint;
    public GameObject mGoCenterPoint;

    public UIInput mItEnterNumber;
    public UIButton mBtnRebuild;
    public UIButton mBtnFix;






    private void Awake()
    {
        mPanel.onClipMove = OnMove;
        mV4ClipRegion = mPanel.baseClipRegion;
        mCenterOnChild = mGrid.GetComponent<UICenterOnChild>();
        mCenterOnChild.onCenter = OnCenterCallback;
        mItemInst.gameObject.SetActive(false);

        mItEnterNumber.validation = UIInput.Validation.Integer;
        UIEventListener.Get(mBtnRebuild.gameObject).onClick = OnClick_BtnRebuild;
        UIEventListener.Get(mBtnFix.gameObject).onClick = OnClick_BtnFix;
    }

    //private void Start()
    //{
    //    Vector3 v3A = new Vector3(0, 0, 0);
    //    Vector3 v3B = new Vector3(3, 2, 0);
    //    Vector3 v3Ret = Vector3.LerpUnclamped(v3A, v3B, 2f);
    //    Debug.Log("v3Ret = " + v3Ret);
    //}

    void OnClick_BtnRebuild(GameObject go)
    {
        string strEnterNumber = mItEnterNumber.value;
        if (string.IsNullOrEmpty(strEnterNumber))
        {
            return;
        }

        int nNumber = int.Parse(strEnterNumber);
        if (nNumber <= 0)
        {
            return;
        }

        mSView.horizontalScrollBar.value = 0;

        int indexUI = 0;
        int indexData = 0;
        while (indexData < nNumber)
        {
            CUICircleScrollView_Item goCacheItem = null;
            if (indexUI < mLstViewItem.Count)
            {
                goCacheItem = mLstViewItem[indexUI];
            }
            else
            {
                GameObject goItem = GameObject.Instantiate(mItemInst.gameObject);
                GameCommon.ASSERT(goItem != null);
                goCacheItem = goItem.GetComponent<CUICircleScrollView_Item>();
                GameCommon.ASSERT(goCacheItem != null);
                goCacheItem.name = "item-" + indexData;

                mLstViewItem.Add(goCacheItem);
            }

            goCacheItem.gameObject.SetActive(true);
            goCacheItem.transform.parent = mGrid.transform;
            goCacheItem.transform.localPosition = Vector3.zero;
            goCacheItem.transform.localScale = Vector3.one;
            goCacheItem.transform.localRotation = Quaternion.Euler(Vector3.zero);

            goCacheItem.SetFillData(indexData);

            indexUI++;
            indexData++;
        }
        for (; indexUI < mLstViewItem.Count; indexUI++)
        {
            CUICircleScrollView_Item goInstItem = mLstViewItem[indexUI];
            GameCommon.ASSERT(goInstItem != null);
            goInstItem.gameObject.SetActive(false);
        }

        mGrid.repositionNow = true;
        mCenterOnChild.Recenter();
    }

    void OnClick_BtnFix(GameObject go)
    {
        DoFixItemTransform();
    }


    Vector3 m_v3Bottom = new Vector3(0, 0, 0);
    Vector3 m_v3Center = new Vector3(0, 0, 0);
    void OnMove(UIPanel panel, Vector2 v2Delta)
    {
        DoFixItemTransform();
    }

    void DoFixItemTransform()
    {
        m_v3Bottom = UICamera.currentCamera.WorldToScreenPoint(mGoBottomPoint.transform.position);
        m_v3Center = UICamera.currentCamera.WorldToScreenPoint(mGoCenterPoint.transform.position);

        Vector3[] corners = mPanel.worldCorners;
        Vector3[] v3CornerScreen = new Vector3[corners.Length];
        for (int i = 0; i < corners.Length; i++)
        {
            v3CornerScreen[i] = UICamera.currentCamera.WorldToScreenPoint(corners[i]);
            //Debug.Log("v3CornerScreen:" + i + " -> " + v3CornerScreen[i]);
        }

        Vector3 _v3PosScreen_Last = Vector3.zero;
        foreach (CUICircleScrollView_Item _item in mLstViewItem)
        {
            if (!_item.gameObject.activeSelf)
            {
                continue;
            }

            Vector3 _v3PosScreen = UICamera.currentCamera.WorldToScreenPoint(_item.transform.position);

            //if (_v3PosScreen.x < (v3CornerScreen[0].x - mGrid.cellWidth * 10))
            //{
            //    continue;
            //}
            //if (_v3PosScreen.x > (v3CornerScreen[3].x + mGrid.cellWidth * 10))
            //{
            //    continue;
            //}
            //if (_v3PosScreen.y < (v3CornerScreen[0].y - mGrid.cellWidth * 10))
            //{
            //    continue;
            //}
            //if (_v3PosScreen.y > (v3CornerScreen[1].y + mGrid.cellWidth * 10))
            //{
            //    continue;
            //}


            //y = 0.0009x2 - 0.0333x - 13.429
            {
                //演算PosY
                float fDisDeltaToBottom = _v3PosScreen.x - m_v3Bottom.x;
                double fPosY = 0.0009 * Mathf.Pow(fDisDeltaToBottom, 2.0f) - 0.0333f * fDisDeltaToBottom - 13.429f;
                _item.transform.localPosition = new Vector3(
                    _item.transform.localPosition.x,
                    (float)fPosY,
                    _item.transform.localPosition.z
                    );
            }

            _v3PosScreen = UICamera.currentCamera.WorldToScreenPoint(_item.transform.position);

            //{
            //    //演算修正Pos（需要保持空间距离一致）
            //    if (_v3PosScreen_Last != Vector3.zero)
            //    {
            //        float fDisBetweenItem = Vector3.Distance(_v3PosScreen_Last, _v3PosScreen);
            //        //Debug.Log("fDisBetweenItem = "+ fDisBetweenItem);
            //        Vector3 v3PosFix = Vector3.LerpUnclamped(_v3PosScreen_Last, _v3PosScreen, mGrid.cellWidth / fDisBetweenItem);
            //        v3PosFix = UICamera.currentCamera.ScreenToWorldPoint(v3PosFix);
            //        _item.transform.position = v3PosFix;
            //        //_item.transform.position = new Vector3(v3PosFix.x, _item.transform.position.y, _item.transform.position.z);
            //    }
            //}

            _v3PosScreen = UICamera.currentCamera.WorldToScreenPoint(_item.transform.position);

  

            //y = -0.0017x + 1.51
            {
                //演算Scale
                float fDisDeltaToCenter = _v3PosScreen.x - m_v3Center.x;
                double fScale = 0;
                if (fDisDeltaToCenter < 0)
                {
                    //fScale = 0.8f;
                    fScale = -0.0017 * Math.Abs(fDisDeltaToCenter) + 1.51;
                }
                else
                {
                    fScale = -0.0017 * fDisDeltaToCenter + 1.51;
                }
                if (fScale < 0.5f)
                {
                    fScale = 0.5f;
                }
                _item.transform.localScale = new Vector3((float)fScale, (float)fScale, 1);
            }

            _v3PosScreen = UICamera.currentCamera.WorldToScreenPoint(_item.transform.position);

            _v3PosScreen_Last = _v3PosScreen;
        }
    }

    void OnCenterCallback(GameObject centeredObject)
    {
        if (centeredObject != null)
        {
            CUICircleScrollView_Item item = centeredObject.GetComponent<CUICircleScrollView_Item>();
            //Debug.Log("OnCenterCallback: " + item.GetFillData().ToString("00"));
        }        
    }
}