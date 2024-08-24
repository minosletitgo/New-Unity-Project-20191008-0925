using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CSimpleUIManage : Singleton<CSimpleUIManage>
{
    public enum EM_UIName
    {
        Invalid = -1,

        DebugConsoleWindow,
        WorldDragMapWindow02,

        Max,
    }

    Dictionary<EM_UIName, string> m_mapContainer_Path = new Dictionary<EM_UIName, string>();
    Dictionary<EM_UIName, GameObject> m_mapContainer_GO = new Dictionary<EM_UIName, GameObject>();
    UIRoot m_stRoot;

    public CSimpleUIManage()
    {
        m_mapContainer_Path.Add(EM_UIName.DebugConsoleWindow, "UI/Prefab/UIDebugConsoleWindow");
        m_mapContainer_Path.Add(EM_UIName.WorldDragMapWindow02, "UI/Prefab/WorldDragMapWindow");

        GameObject goRoot = GameObject.Find("UI Root");
        GameCommon.ASSERT(goRoot != null, "CSimpleUIManage Failed UIRoot !");
        m_stRoot = goRoot.GetComponent<UIRoot>();
        GameCommon.ASSERT(m_stRoot != null, "CSimpleUIManage Failed UIRoot !");
    }




    public T GetUI<T>(EM_UIName emUIName, out bool bIsNew)
    {
        bIsNew = false;

        string strUIPath;
        if (!m_mapContainer_Path.TryGetValue(emUIName, out strUIPath))
        {
            Debug.LogError("CSimpleUIManage.GetUI Failed m_mapContainer: " + emUIName.ToString());
            return default(T);
        }

        GameObject goRet = null;
        if (m_mapContainer_GO.TryGetValue(emUIName, out goRet))
        {
            return goRet.GetComponent<T>();
        }

        UnityEngine.Object objRet = (UnityEngine.Object.FindObjectOfType(typeof(T)));
        if (objRet != null)
        {
            return (T)((object)objRet);
        }

        switch (emUIName)
        {
            case EM_UIName.DebugConsoleWindow:
                {
                    bIsNew = true;

                    UnityEngine.Object objSrc = Resources.Load(strUIPath);
                    GameCommon.ASSERT(objSrc != null, "CSimpleUIManage.GetUI Failed Src: " + strUIPath);

                    goRet = GameObject.Instantiate(objSrc) as GameObject;
                    GameCommon.ASSERT(goRet != null);
                    goRet.SetActive(true);
                    goRet.transform.SetParent(m_stRoot.transform);
                    goRet.transform.localPosition = Vector3.zero;
                    goRet.transform.localRotation = Quaternion.Euler(Vector3.zero);
                    goRet.transform.localScale = Vector3.one;

                    m_mapContainer_GO.Add(emUIName, goRet);

                    return goRet.GetComponent<T>();
                }
            //break;
            default:
                {
                    Debug.LogError("CSimpleUIManage.GetUI Failed emUIName: " + emUIName.ToString());
                    return default(T);
                }
                //break;
        }
    }



}