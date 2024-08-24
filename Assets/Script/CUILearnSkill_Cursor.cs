using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CUILearnSkill_Cursor : MonoBehaviour
{
    public GameObject mRoot;

    CUILearnSkill_ItemMix mCacheItem;
    BoxCollider mBoxItemSize;

    public BoxCollider mBoxMoving;
    float mF_X_Min;
    float mF_X_Max;
    float mF_Y_Min;
    float mF_Y_Max;

    public void InitRootItem(CUILearnSkill_ItemMix itemInst)
    {
        GameObject go = Instantiate(itemInst.gameObject) as GameObject;
        go.name = "item(Cache)";
        go.transform.parent = mRoot.transform;
        go.transform.localPosition = Vector3.zero;
        go.transform.localRotation = Quaternion.Euler(Vector3.zero);
        go.transform.localScale = Vector3.one;
        mCacheItem = go.GetComponent<CUILearnSkill_ItemMix>();

        mBoxItemSize = mCacheItem.GetComponent<BoxCollider>();
        mBoxItemSize.enabled = false;
        
        mCacheItem.gameObject.SetActive(false);
    }

    bool mIsInitBoxMoving = false;
    void InitBoxMoving()
    {
        if (mIsInitBoxMoving)
        {
            return;
        }

        Vector3 v3RenderSizeCenter = UICamera.currentCamera.WorldToScreenPoint(mBoxMoving.transform.position);
        {
            float _fTmp = v3RenderSizeCenter.x - mBoxMoving.size.x / 2 + mBoxItemSize.size.x / 2;
            Vector3 v3Tmp = new Vector3(_fTmp, v3RenderSizeCenter.y, v3RenderSizeCenter.z);
            Vector3 v3_X_Min = UICamera.currentCamera.ScreenToWorldPoint(v3Tmp);
            mF_X_Min = v3_X_Min.x;
        }
        {
            float _fTmp = v3RenderSizeCenter.x + mBoxMoving.size.x / 2 - mBoxItemSize.size.x / 2;
            Vector3 v3Tmp = new Vector3(_fTmp, v3RenderSizeCenter.y, v3RenderSizeCenter.z);
            Vector3 v3_X_Max = UICamera.currentCamera.ScreenToWorldPoint(v3Tmp);
            mF_X_Max = v3_X_Max.x;
        }
        {
            float _fTmp = v3RenderSizeCenter.y - mBoxMoving.size.y / 2 + mBoxItemSize.size.y / 2;
            Vector3 v3Tmp = new Vector3(v3RenderSizeCenter.x, _fTmp, v3RenderSizeCenter.z);
            Vector3 v3_Y_Min = UICamera.currentCamera.ScreenToWorldPoint(v3Tmp);
            mF_Y_Min = v3_Y_Min.y;
        }
        {
            float _fTmp = v3RenderSizeCenter.y + mBoxMoving.size.y / 2 - mBoxItemSize.size.y / 2;
            Vector3 v3Tmp = new Vector3(v3RenderSizeCenter.x, _fTmp, v3RenderSizeCenter.z);
            Vector3 v3_Y_Max = UICamera.currentCamera.ScreenToWorldPoint(v3Tmp);
            mF_Y_Max = v3_Y_Max.y;
        }

        mIsInitBoxMoving = true;
    }

    Vector3 CalcPostionInBoxMoving()
    {
        Vector3 v3Mouse = UICamera.currentCamera.ScreenToWorldPoint(Input.mousePosition);

        float fX = v3Mouse.x;
        if (v3Mouse.x < mF_X_Min)
        {
            fX = mF_X_Min;
        }
        else if (v3Mouse.x > mF_X_Max)
        {
            fX = mF_X_Max;
        }

        float fY = v3Mouse.y;
        if (v3Mouse.y < mF_Y_Min)
        {
            fY = mF_Y_Min;
        }
        else if (v3Mouse.y > mF_Y_Max)
        {
            fY = mF_Y_Max;
        }

        return new Vector3(fX, fY, v3Mouse.z);
    }

    public CUILearnSkill_ItemMix GetCacheItem()
    {
        return mCacheItem;
    }

    bool mIsRuning = false;
    public void BeginRunning(CUILearnSkill.ST_ItemData stData)
    {
        InitBoxMoving();

        mCacheItem.gameObject.SetActive(true);
        mCacheItem.SetFillData(stData, true);

        mIsRuning = true;
    }

    public void EndRunning()
    {
        mCacheItem.gameObject.SetActive(false);
        mCacheItem.ClearFillData();
        mIsRuning = false;
    }

    void Update()
    {
        if (mIsRuning)
        {
            mRoot.transform.position = CalcPostionInBoxMoving();
            //Debug.Log("mRoot.transform.position = "+ mRoot.transform.position);
        }
    }
}