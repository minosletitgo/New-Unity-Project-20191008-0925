using UnityEngine;
using System.Collections.Generic;

public class UIIcon : MonoBehaviour
{
    public UISprite m_sprIcon;
    protected UISpriteData m_sprIconData;



    public void SetIcon(int idIcon)
    {
        //IconConfig info = IconConfig.Get(idIcon);
        //if (info == null) { EditorLOG.logWarn("SetIcon Null idIcon:" + idIcon); return; }
        //SetIcon(info);
    }

    //public void SetIcon(IconConfig info)
    //{
    //    if (info == null) { EditorLOG.logWarn("info == null"); return; }
    //    SetIconBase(
    //        info.atlas,
    //        info.sprite,
    //        new Vector2(info.dimension_x, info.dimension_y),
    //        new Vector2(info.dimension_width, info.dimension_height),
    //        info.depth
    //        );
    //}

    public void SetIconBase(
        string strAtlas,
        string strSprite,
        Vector2 vDimensionsXY,
        Vector2 vDimensionsWH,
        int nDepth
        )
    {
        UISprite spr = GetArraySprIcon();
        GameCommon.ASSERT(spr != null);

        UISpriteData sprData = GetArraySprIconData();
        GameCommon.ASSERT(sprData != null);

        m_sprIcon.gameObject.SetActive(true);

        //CUISpriteAtlasProvider stProvider = GameObject.Find("UISpriteAtlasProvider").GetComponent<CUISpriteAtlasProvider>();        
        //GameCommon.CHECK(stProvider != null);
        //stProvider.SetSprIcon(
        //    ref spr,
        //    ref sprData,
        //    strAtlas,
        //    strSprite,
        //    vDimensionsXY,
        //    vDimensionsWH,
        //    nDepth
        //    );

        GameUISpriteAtlasProvider.Instance.SetSpriteIcon(
            ref spr,
            ref sprData,
            strAtlas,
            strSprite,
            vDimensionsXY,
            vDimensionsWH,
            nDepth
            );
    }

    public UISprite GetArraySprIcon()
    {
        return m_sprIcon;
    }

    UISpriteData GetArraySprIconData()
    {
        if (m_sprIconData == null)
        {
            m_sprIconData = new UISpriteData();
        }

        return m_sprIconData;
    }
}