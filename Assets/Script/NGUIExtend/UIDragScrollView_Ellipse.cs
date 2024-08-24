using UnityEngine;
using System.Collections;

public class UIDragScrollView_Ellipse : MonoBehaviour
{
    public UIScrollView_Ellipse scrollView;

    [HideInInspector] [SerializeField] UIScrollView_Ellipse draggablePanel;

    Transform mTrans;
    UIScrollView_Ellipse mScroll;
    bool mAutoFind = false;
    bool mStarted = false;
    
    void OnEnable()
    {
        mTrans = transform;

        // Auto-upgrade
        if (scrollView == null && draggablePanel != null)
        {
            scrollView = draggablePanel;
            draggablePanel = null;
        }

        if (mStarted && (mAutoFind || mScroll == null))
            FindScrollView();
    }

    void Start()
    {
        mStarted = true;
        FindScrollView();
    }

    void FindScrollView()
    {
        // If the scroll view is on a parent, don't try to remember it (as we want it to be dynamic in case of re-parenting)
        UIScrollView_Ellipse sv = NGUITools.FindInParents<UIScrollView_Ellipse>(mTrans);

        if (scrollView == null || (mAutoFind && sv != scrollView))
        {
            scrollView = sv;
            mAutoFind = true;
        }
        else if (scrollView == sv)
        {
            mAutoFind = true;
        }
        mScroll = scrollView;
    }

    [System.NonSerialized] bool mPressed = false;

    void OnDisable()
    {
        if (mPressed && mScroll != null/* && mScroll.GetComponentInChildren<UIWrapContent>() == null*/)
        {
            mScroll.Press(false);
            mScroll = null;
        }
    }

    void OnPress(bool pressed)
    {
        mPressed = pressed;

        // If the scroll view has been set manually, don't try to find it again
        if (mAutoFind && mScroll != scrollView)
        {
            mScroll = scrollView;
            mAutoFind = false;
        }

        if (scrollView && enabled && NGUITools.GetActive(gameObject))
        {
            scrollView.Press(pressed);

            if (!pressed && mAutoFind)
            {
                scrollView = NGUITools.FindInParents<UIScrollView_Ellipse>(mTrans);
                mScroll = scrollView;
            }
        }
    }

    void OnDrag(Vector2 delta)
    {
        if (scrollView && NGUITools.GetActive(this))
            scrollView.Drag();
    }

    void OnDragStart()
    {
        if (scrollView && NGUITools.GetActive(this))
            scrollView.DragStart();
    }

    void OnDragEnd()
    {
        if (scrollView && NGUITools.GetActive(this))
            scrollView.DragEnd();
    }
}