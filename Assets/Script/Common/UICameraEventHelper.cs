using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class UICameraEventHelper : Singleton<UICameraEventHelper>
{
    public enum EM_Event
    {
        OnHover,
        OnPress,
        OnSelect,
        OnClick,
        OnDoubleClick,
    };

    public delegate void DGOnEvent(GameObject go, EM_Event emEvent, object obj);
    public DGOnEvent m_dgOnEvent;

    public void OnEventNotify(GameObject go, string funcName, object obj)
    {
        if (!Enum.IsDefined(typeof(EM_Event), funcName))
        {
            //Don't care
            return;
        }

        EM_Event emEvent = (EM_Event)(Enum.Parse(typeof(EM_Event), funcName));
        if (m_dgOnEvent != null)
        {
            m_dgOnEvent(go, emEvent, obj);
        }
    }
}