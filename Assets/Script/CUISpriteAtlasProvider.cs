using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CUISpriteAtlasProvider : MonoBehaviour
{
    public GameObject m_goSprIconRoot;
    UISprite[] m_arySprIcon;
    Dictionary<string, UISprite> m_mAtlas2SprIcon = new Dictionary<string, UISprite>();




    void Awake()
    {
        m_arySprIcon = m_goSprIconRoot.gameObject.GetComponentsInChildren<UISprite>(true);
        GameCommon.ASSERT(m_arySprIcon != null);
        GameCommon.ASSERT(m_arySprIcon.Length > 0, "CUISpriteAtlasProvider m_arySprIcon.Length > 0");

        m_mAtlas2SprIcon.Clear();
        foreach (UISprite spr in m_arySprIcon)
        {
            GameCommon.ASSERT(spr != null, "CUISpriteAtlasProvider m_arySprIcon.spr == null");
            GameCommon.ASSERT(spr.GetAtlasSprite() != null, "CUISpriteAtlasProvider m_arySprIcon.spr GetAtlasSprite 未填充");
            m_mAtlas2SprIcon.Add(spr.atlas.name, spr);
        }
    }

    public void SetSprIcon(
        ref UISprite spr,
        ref UISpriteData sprData,
        int idIcon
        )
    {
        //CTBLInfo.CIconInfo info = CTBLInfo.Inst.GetIconInfo(idIcon);
        //if (info == null) { EditorLOG.logWarn("SetSprIcon idIcon:" + idIcon); return; }
        //SetSprIcon(
        //    ref spr,
        //    ref sprData,
        //    info.strAtlas,
        //    info.strSprite,
        //    info.vDimensionXY,
        //    info.vDimensionWH,
        //    info.nDepth);
    }

    //public void SetSprIcon(
    //    ref UISprite spr,
    //    ref UISpriteData sprData,
    //    CTBLInfo.CIconInfo info
    //    )
    //{
    //    if (info == null) { EditorLOG.logWarn("SetSprIcon info == null"); return; }
    //    SetSprIcon(
    //        ref spr,
    //        ref sprData,
    //        info.strAtlas,
    //        info.strSprite,
    //        info.vDimensionXY,
    //        info.vDimensionWH,
    //        info.nDepth);
    //}

    public void SetSprIcon(
        ref UISprite spr,
        ref UISpriteData sprData,
        string strAtlas,
        string strSprite,
        Vector2 vDimensionsXY,
        Vector2 vDimensionsWH,
        int nDepth
        )
    {
        GameCommon.ASSERT(spr != null);
        GameCommon.ASSERT(sprData != null);

        if (!IsContainsAtlas(strAtlas))
        {
            EditorLOG.logWarn("CUISpriteAtlasProvider.SetSprIcon 不支持的strAtlas " + strAtlas);
            return;
        }

        UISpriteData sprDataConst = GetConstSprIconData(strAtlas, strSprite);
        //GameCommon.CHECK(sprDataConst != null);
        if (sprDataConst == null)
        {
            EditorLOG.logWarn("CUISpriteAtlasProvider 目标strAtlas - strSprite不存在 " + strAtlas + " - " + strSprite);
            return;
        }

        spr.atlas = GetConstSprAtlas(strAtlas);
        spr.spriteName = strSprite;

        ////UISpriteData sprDataCalculate = spr.GetAtlasSprite();
        //UISpriteData sprDataCalculate = new UISpriteData();
        //if (sprDataCalculate == null)
        //{
        //    EditorLOG.logWarn("CUISpriteAtlasProvider.SetSprIcon 目标spr预置状态缺失有效UISpriteData " + spr.name);
        //    sprDataCalculate = new UISpriteData();
        //}
        UISpriteData sprDataCalculate = sprData;
        GameCommon.ASSERT(sprDataCalculate != null);
        sprDataCalculate.CopyFrom(sprDataConst);
        sprDataCalculate.x += (int)vDimensionsXY.x;
        sprDataCalculate.y += (int)vDimensionsXY.y;
        sprDataCalculate.name = strSprite;
        if (vDimensionsWH.x != 0 && vDimensionsWH.y != 0)
        {
            sprDataCalculate.width = /*spr.width*/(int)vDimensionsWH.x;
            sprDataCalculate.height = /*spr.height*/(int)vDimensionsWH.y;
        }
        spr.SetAtlasSprite(sprDataCalculate, true);

        if (nDepth > 0) { spr.depth = nDepth; }
    }

    public bool IsContainsAtlas(string strAtlas)
    {
        return m_mAtlas2SprIcon.ContainsKey(strAtlas);
    }

    public UISprite GetConstSpr(string strAtlas)
    {
        UISprite ret = null;
        m_mAtlas2SprIcon.TryGetValue(strAtlas, out ret);
        GameCommon.ASSERT(ret != null, "CUISpriteAtlasProvider.不支持的strAtlas " + strAtlas);
        return ret;
    }

    public UIAtlas GetConstSprAtlas(string strAtlas)
    {
        return GetConstSpr(strAtlas).atlas;
    }

    public UISpriteData GetConstSprIconData(string strAtlas, string strSprite)
    {
        return GetConstSprAtlas(strAtlas).GetSprite(strSprite);
    }
}