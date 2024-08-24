using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class CTestDoTween : MonoBehaviour
{
    public UIButton m_btnDoTweenAnim;
    public UILabel m_labDoTweenAnim;

    public UIButton m_btnDoTweenPath;
    public UILabel m_labDoTweenPath;

    public UIButton m_btnDoTweenNGUI;
    public UILabel m_labDoTweenNGUI;
    TweenPosition m_twDoTweenNGUI01;
    TweenAlpha m_twDoTweenNGUI02;

    // Use this for initialization
    void Awake()
    {
        UIEventListener.Get(m_btnDoTweenAnim.gameObject).onClick = OnClick_BtnDoTweenAnim;
        UIEventListener.Get(m_btnDoTweenPath.gameObject).onClick = OnClick_BtnDoTweenPath;
        UIEventListener.Get(m_btnDoTweenNGUI.gameObject).onClick = OnClick_BtnDoTweenNGUI;

        m_twDoTweenNGUI01 = m_labDoTweenNGUI.GetComponent<TweenPosition>();
        GameCommon.ASSERT(m_twDoTweenNGUI01 != null);
        m_twDoTweenNGUI01.enabled = false;

        m_twDoTweenNGUI02 = m_labDoTweenNGUI.GetComponent<TweenAlpha>();
        GameCommon.ASSERT(m_twDoTweenNGUI02 != null);
        m_twDoTweenNGUI02.enabled = false;
    }

    void OnClick_BtnDoTweenAnim(GameObject go)
    {
        DOTweenAnimation stDoTween = m_labDoTweenAnim.GetComponent<DOTweenAnimation>();
        GameCommon.ASSERT(stDoTween != null);
        stDoTween.DORestart();
    }

    void OnClick_BtnDoTweenPath(GameObject go)
    {
        m_labDoTweenPath.alpha = 1;
        DOTweenPath stDoTween = m_labDoTweenPath.GetComponent<DOTweenPath>();
        GameCommon.ASSERT(stDoTween != null);
        stDoTween.DORestart();
        TweenAlpha.Begin(m_labDoTweenPath.gameObject, 0.5f, 0f, 0.5f);
    }

    void OnClick_BtnDoTweenNGUI(GameObject go)
    {
        TweenPosition stDoTween01 = m_labDoTweenNGUI.GetComponent<TweenPosition>();
        GameCommon.ASSERT(stDoTween01 != null);
        stDoTween01.ResetToBeginning();
        stDoTween01.Play(true);

        TweenAlpha stDoTween02 = m_labDoTweenNGUI.GetComponent<TweenAlpha>();
        GameCommon.ASSERT(stDoTween02 != null);
        stDoTween02.ResetToBeginning();
        stDoTween02.Play(true);
    }
}
