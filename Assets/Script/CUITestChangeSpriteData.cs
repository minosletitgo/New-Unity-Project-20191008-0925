using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CUITestChangeSpriteData : MonoBehaviour
{
    //Mode 1
    public UISprite m_sprIcon;
       
    //Mode 2
    public UIIcon m_icon;


    //Params
    public string m_strAtlas = "MyAtlas";
    public string m_strSprite = "icon_skill_1";
    public Vector2 m_v2DimensionsXY = new Vector2();
    public Vector2 m_v2DimensionsWH = new Vector2();




    [ContextMenu("MakeChange_Mode1")]
    void MakeChange_Mode1()
    {
        GameUISpriteAtlasProvider.Instance.SetSpriteIcon(
            ref m_sprIcon,
            m_strAtlas,
            m_strSprite
            );
        m_sprIcon.MakePixelPerfect();
    }

    [ContextMenu("MakeChange_Mode2")]
    void MakeChange_Mode2()
    {
        m_icon.SetIconBase(
            m_strAtlas,
            m_strSprite,
            m_v2DimensionsXY,
            m_v2DimensionsWH,
            0
            );
        m_icon.m_sprIcon.MakePixelPerfect();
    }

    [ContextMenu("GC")]
    void GC()
    {
        GameUISpriteAtlasProvider.Instance.ClearAtlas();
        Resources.UnloadUnusedAssets();
        System.GC.Collect();
    }
}