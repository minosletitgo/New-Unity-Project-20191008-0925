using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIGrid_Ellipse : MonoBehaviour
{
    public GameObject m_goCenter;
    public float m_fAxisA;
    public float m_fAxisB;
    public float m_fDegreePointPosOffset;
    public float m_fDegreeRotateOffsetX = 0;
    public float m_fDegreeRotateOffsetY = 0;
    public float m_fDegreeRotateOffsetZ = 0;

    public Transform m_trRoot;

    public bool hideInactive = false;

    protected bool mReposition = false;
    protected bool mInitDone = false;

    /// <summary>
    /// Reposition the children on the next Update().
    /// </summary>

    public bool repositionNow { set { if (value) { mReposition = true; enabled = true; } } }

    /// <summary>
    /// Get the current list of the grid's children.
    /// </summary>

    public List<Transform> GetChildList()
    {
        Transform myTrans = m_trRoot;
        List<Transform> list = new List<Transform>();

        for (int i = 0; i < myTrans.childCount; ++i)
        {
            Transform t = myTrans.GetChild(i);

            if (!hideInactive || (t && t.gameObject.activeSelf))
            {
                /*if (!UIDragDropItem.IsDragged(t.gameObject)) */list.Add(t);
            }
        }

        //// Sort the list using the desired sorting logic
        //if (sorting != Sorting.None && arrangement != Arrangement.CellSnap)
        //{
        //    if (sorting == Sorting.Alphabetic) list.Sort(SortByName);
        //    else if (sorting == Sorting.Horizontal) list.Sort(SortHorizontal);
        //    else if (sorting == Sorting.Vertical) list.Sort(SortVertical);
        //    else if (onCustomSort != null) list.Sort(onCustomSort);
        //    else Sort(list);
        //}
        return list;
    }

    /// <summary>
    /// Convenience method: get the child at the specified index.
    /// Note that if you plan on calling this function more than once, it's faster to get the entire list using GetChildList() instead.
    /// </summary>

    public Transform GetChild(int index)
    {
        List<Transform> list = GetChildList();
        return (index < list.Count) ? list[index] : null;
    }

    /// <summary>
    /// Get the index of the specified item.
    /// </summary>

    public int GetIndex(Transform trans) { return GetChildList().IndexOf(trans); }




    protected virtual void Init()
    {
        mInitDone = true;
        //mPanel = NGUITools.FindInParents<UIPanel>(gameObject);
    }

    protected virtual void Start()
    {
        if (!mInitDone) Init();
        //bool smooth = animateSmoothly;
        //animateSmoothly = false;
        Reposition();
        //animateSmoothly = smooth;
        enabled = false;
    }

    protected virtual void Update()
    {
        Reposition();
        enabled = false;
    }

    //void OnValidate() { if (!Application.isPlaying && NGUITools.GetActive(this)) Reposition(); }

    [ContextMenu("Execute")]
    public virtual void Reposition()
    {
        if (Application.isPlaying && !mInitDone && NGUITools.GetActive(gameObject)) Init();

        //// Legacy functionality
        //if (sorted)
        //{
        //    sorted = false;
        //    if (sorting == Sorting.None)
        //        sorting = Sorting.Alphabetic;
        //    NGUITools.SetDirty(this);
        //}

        // Get the list of children in their current order
        List<Transform> list = GetChildList();

        // Reset the position and order of all objects in the list
        ResetPosition(list);

        //// Constrain everything to be within the panel's bounds
        //if (keepWithinPanel) ConstrainWithinPanel();

        //// Notify the listener
        //if (onReposition != null)
        //    onReposition();
    }

    /// <summary>
    /// Reset the position of all child objects based on the order of items in the list.
    /// </summary>

    protected virtual void ResetPosition(List<Transform> list)
    {
        mReposition = false;

        if (list.Count <= 0)
        {
            return;
        }

        //Vector3 v3Center = UICamera.mainCamera.WorldToScreenPoint(m_goCenter.transform.position);

        float fDegreeStep = 360.0f / (float)(list.Count);

        for (int i = 0, imax = list.Count; i < imax; ++i)
        {
            Transform tran = list[i];

            float _fDegree = 0 + i * fDegreeStep + m_fDegreePointPosOffset;
            _fDegree = MathEllipseHelper.DegreeClamp(_fDegree);

            Vector3 _v3Pos = MathEllipseHelper.GetScreenPostion(
                m_goCenter.transform,
                m_fAxisA,
                m_fAxisB,
                _fDegree,
                m_fDegreeRotateOffsetX,
                m_fDegreeRotateOffsetY,
                m_fDegreeRotateOffsetZ
                );

            tran.position = UICamera.mainCamera.ScreenToWorldPoint(_v3Pos);
        }
    }
}
