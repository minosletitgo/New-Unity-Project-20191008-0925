using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;

public class CTBLInfo_Demo //: ScriptableObject
{
    //singleton
    //-----------------
    public static CTBLInfo_Demo s_Inst = null;//故意public
    public static CTBLInfo_Demo Inst { get { if (s_Inst == null) s_Inst = new CTBLInfo_Demo(); /*ScriptableObject.CreateInstance<CTBLInfo_Demo>();*/ return s_Inst; } }
    public static void Destroy() { s_Inst = null; }
    public CTBLInfo_Demo() { }//故意public








    public class ST_IconInfo
    {
        public int id;
        public string strAtlas;
        public string strSprite;
        public Vector2 vDimension;
    }
    Dictionary<int, ST_IconInfo> m_mIconInfo = new Dictionary<int, ST_IconInfo>();

    public bool LoadIconInfo(string strPath)
    {
        CTBLLoader_Base loader = new CTBLLoader_Base();
        loader.LoadFromFile(strPath);
        int nLine = loader.GetLineCount();
        for (int i = 0; i < nLine; i++)
        {
            loader.GoToLineByIndex(i);

            ST_IconInfo objInfo = new ST_IconInfo();
            loader.GetIntByName("ID", out objInfo.id);
            loader.GetStringByName("Atlas", out objInfo.strAtlas);
            loader.GetStringByName("Sprite", out objInfo.strSprite);

            string strDimension;
            loader.GetStringByName("Dimension", out strDimension);
            if (!string.IsNullOrEmpty(strDimension))
            {
                string[] arrayStrDimension = strDimension.Split(':');
                TCK.CHECK(arrayStrDimension.Length == 3);
                objInfo.vDimension = new Vector2(
                    float.Parse(arrayStrDimension[1]),
                    float.Parse(arrayStrDimension[2]));
            }

            m_mIconInfo.Add(objInfo.id, objInfo);
        }

        return true;
    }
}