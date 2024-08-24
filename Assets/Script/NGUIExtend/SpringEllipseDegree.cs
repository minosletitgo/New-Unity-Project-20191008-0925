using UnityEngine;


public class SpringEllipseDegree : MonoBehaviour
{
    static public SpringEllipseDegree current;

    /// <summary>
    /// Target position to spring the panel to.
    /// </summary>

    public float target = 0;

    /// <summary>
    /// Strength of the spring. The higher the value, the faster the movement.
    /// </summary>

    public float strength = 10f;

    public delegate void OnFinished();

    /// <summary>
    /// Delegate function to call when the operation finishes.
    /// </summary>

    public OnFinished onFinished;

    //UIPanel mPanel;
    Transform mTrans;
    UIScrollView_Ellipse mDrag;

    /// <summary>
    /// Cache the transform.
    /// </summary>

    void Start()
    {
        //mPanel = GetComponent<UIPanel>();
        mDrag = GetComponent<UIScrollView_Ellipse>();
        mTrans = transform;
    }

    /// <summary>
    /// Advance toward the target position.
    /// </summary>

    void Update()
    {
        AdvanceTowardsPosition();
    }

    /// <summary>
    /// Advance toward the target position.
    /// </summary>

    protected virtual void AdvanceTowardsPosition()
    {
        //PS：一定要使用[非Clamp角度]进行运算!
        float delta = RealTime.deltaTime;

        bool trigger = false;
        float before = mDrag.GetDragAmountDegreeValue();
        float after = MathEllipseHelper.SpringLerp(before, target, strength, delta);
        float fDelta = Mathf.Abs(after - target);

        if (fDelta < 0.05f)
        {
            after = target;
            enabled = false;
            trigger = true;
        }

        mDrag.SetDragAmountDegreeValue(after, false);

        //Vector3 offset = after - before;
        //Vector2 cr = mPanel.clipOffset;
        //cr.x -= offset.x;
        //cr.y -= offset.y;
        //mPanel.clipOffset = cr;

        //if (mDrag != null) mDrag.UpdateScrollbars(false);

        if (trigger && onFinished != null)
        {
            current = this;
            onFinished();
            current = null;
        }
    }

    /// <summary>
    /// Start the tweening process.
    /// </summary>

    static public SpringEllipseDegree Begin(GameObject go, float fDegreeTarget, float strength)
    {
        SpringEllipseDegree sp = go.GetComponent<SpringEllipseDegree>();
        if (sp == null) sp = go.AddComponent<SpringEllipseDegree>();
        //sp.target = MathEllipseHelper.DegreeClamp(fDegreeTarget);
        sp.target = fDegreeTarget;
        sp.strength = strength;
        sp.onFinished = null;
        sp.enabled = true;
        return sp;
    }

    /// <summary>
    /// Stop the tweening process.
    /// </summary>

    static public SpringEllipseDegree Stop(GameObject go)
    {
        SpringEllipseDegree sp = go.GetComponent<SpringEllipseDegree>();

        if (sp != null && sp.enabled)
        {
            if (sp.onFinished != null) sp.onFinished();
            sp.enabled = false;
        }
        return sp;
    }
}