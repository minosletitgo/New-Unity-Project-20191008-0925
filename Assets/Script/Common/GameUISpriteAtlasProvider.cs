using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GameUISpriteAtlasProvider : Singleton<GameUISpriteAtlasProvider>
{
    Dictionary<string, string> m_dicName2UIAtlasPath = new Dictionary<string, string>();
    Dictionary<string, UIAtlas> m_dicName2UIAtlas = new Dictionary<string, UIAtlas>();

    public GameUISpriteAtlasProvider()
    {
        //模拟读表
        m_dicName2UIAtlasPath.Add("MyAtlas", "UI/Atlas/MyAtlas");
        m_dicName2UIAtlasPath.Add("MyAtlas02", "UI/Atlas/MyAtlas02");
        m_dicName2UIAtlasPath.Add("MyAtlas03", "UI/Atlas/MyAtlas03");
        m_dicName2UIAtlasPath.Add("MySkillIconAtlas", "UI/Atlas/MySkillIconAtlas");
    }

    UIAtlas GetUIAtlas(string strName, bool bIsAutoNew)
    {
        string strPath;
        if (!m_dicName2UIAtlasPath.TryGetValue(strName, out strPath))
        {
            EditorLOG.logWarn("Error GetUIAtlas: " + strName);
            return null;
        }

        UIAtlas ret = null;
        if (m_dicName2UIAtlas.TryGetValue(strName, out ret))
        {
            return ret;
        }

        if (bIsAutoNew)
        {
            ret = Resources.Load(strPath, typeof(UIAtlas)) as UIAtlas;
            GameCommon.ASSERT(ret != null, "Error GetUIAtlas: " + strPath);
            m_dicName2UIAtlas.Add(strName, ret);
            return ret;
        }

        return null;
    }

    public void SetSpriteIcon(
        ref UISprite spr, 
        string strAtlas, 
        string strSprite
        )
    {
        spr.atlas = GetUIAtlas(strAtlas, true);
        spr.spriteName = strSprite;
    }

    public void SetSpriteIcon(
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

        UIAtlas atlas = GetUIAtlas(strAtlas, true);
        GameCommon.ASSERT(atlas != null);

        UISpriteData sprDataConst = atlas.GetSprite(strSprite);
        if (sprDataConst == null)
        {
            EditorLOG.logWarn("Error SetSprIcon: " + strAtlas + " - " + strSprite);
            return;
        }

        spr.atlas = atlas;
        spr.spriteName = strSprite;

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

    public void ClearAtlas()
    {
        var ito = m_dicName2UIAtlas.GetEnumerator();
        while (ito.MoveNext())
        {
            UIAtlas atlas = ito.Current.Value;
            atlas = null;
        }
        ito.Dispose();
        m_dicName2UIAtlas.Clear();
    }
}