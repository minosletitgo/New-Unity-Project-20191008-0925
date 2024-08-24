using UnityEngine;

public class SpringTransform : MonoBehaviour
{
    static public SpringTransform current;

    /// <summary>
    /// Target position to spring the panel to.
    /// </summary>

    public Vector3 target = Vector3.zero;

    /// <summary>
    /// Strength of the spring. The higher the value, the faster the movement.
    /// </summary>

    public float strength = 10f;

    public delegate void OnFinished();

    /// <summary>
    /// Delegate function to call when the operation finishes.
    /// </summary>

    public OnFinished onFinished;


    Transform mTrans;

    /// <summary>
    /// Cache the transform.
    /// </summary>

    void Start()
    {
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
        float delta = RealTime.deltaTime;

        bool trigger = false;
        Vector3 before = mTrans.localPosition;
        Vector3 after = NGUIMath.SpringLerp(mTrans.localPosition, target, strength, delta);

        if ((after - target).sqrMagnitude < 0.0001f)
        {
            after = target;
            enabled = false;
            trigger = true;
        }
        mTrans.localPosition = after;
        //Debug.Log("AdvanceTowardsPosition");
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

    static public SpringTransform Begin(GameObject go, Vector3 pos, float strength)
    {
        SpringTransform sp = go.GetComponent<SpringTransform>();
        if (sp == null) sp = go.AddComponent<SpringTransform>();
        sp.target = pos;
        sp.strength = strength;
        sp.onFinished = null;
        sp.enabled = true;
        return sp;
    }

    /// <summary>
    /// Stop the tweening process.
    /// </summary>

    static public SpringTransform Stop(GameObject go)
    {
        SpringTransform sp = go.GetComponent<SpringTransform>();

        if (sp != null && sp.enabled)
        {
            if (sp.onFinished != null) sp.onFinished();
            sp.enabled = false;
        }
        return sp;
    }

}