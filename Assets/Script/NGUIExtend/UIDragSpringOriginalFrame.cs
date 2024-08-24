using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIDragSpringOriginalFrame : MonoBehaviour
{
    public enum Movement
    {
        Normal,
        Custom,
    }

    public delegate void OnDragNotification();
    public OnDragNotification onDragFinished;
    public OnDragNotification onStoppedMoving;

    public Movement movement = Movement.Normal;
    public Vector2 customMovement = new Vector2(1f, 1f);
    public float momentumAmount = 35f;
    public float dampenStrength = 9f;

    protected Plane mPlane;
    protected Vector3 mLastPos;
    protected bool mPressed = false;
    protected Vector3 mMomentum = Vector3.zero;
    protected float mScroll = 0f;
    protected bool mShouldMove = false;
    protected int mDragID = -10;
    protected bool mDragStarted = false;

    public bool isDragging { get { return mPressed && mDragStarted; } }




    public enum EM_MessageEvent
    {
        OnPressDown,
        OnPressUp,
        OnDrag,
        OnLateUpdatePressDown,
        OnLateUpdatePressUp,
    };

    BoxCollider m_boxTrigger;
    Vector2 m_v2DragDelta;

    public delegate void OnMoveAbsolute(EM_MessageEvent emEvent, Vector3 absolute, Vector2 v2Delta);
    OnMoveAbsolute m_dgOnMoveAbsolute;

    Transform m_trRestrict;
    public delegate bool OnIsWithinBounds();
    OnIsWithinBounds m_dgOnIsWithinBounds;
    public delegate Vector3 OnCalcNearestWithinBoundsPos();
    OnCalcNearestWithinBoundsPos m_dgOnCalcNearestWithinBoundsPos;

    public delegate void OnScrollMove(float delta);
    public OnScrollMove m_dgOnScrollMove;

    bool m_bIsMultiTouchDisabled = true;



    public void Initialized(BoxCollider boxTrigger, OnMoveAbsolute dgOnMoveAbsolute)
    {
        GameCommon.ASSERT(boxTrigger != null);
        GameCommon.ASSERT(dgOnMoveAbsolute != null);

        UIEventListener.Get(boxTrigger.gameObject).onPress += OnPress_Trigger;
        UIEventListener.Get(boxTrigger.gameObject).onDrag += OnDrag_Trigger;
        UIEventListener.Get(boxTrigger.gameObject).onScroll += OnScroll_Trigger;

        m_boxTrigger = boxTrigger;
        m_dgOnMoveAbsolute = dgOnMoveAbsolute;
    }

    public void InitRestrictWithinBounds(
        Transform trRestrict,
        OnIsWithinBounds dgOnIsWithinBounds,
        OnCalcNearestWithinBoundsPos dgOnCalcNearestWithinBoundsPos
        )
    {
        GameCommon.ASSERT(trRestrict != null);
        GameCommon.ASSERT(dgOnIsWithinBounds != null);
        GameCommon.ASSERT(dgOnCalcNearestWithinBoundsPos != null);

        m_trRestrict = trRestrict;
        m_dgOnIsWithinBounds = dgOnIsWithinBounds;
        m_dgOnCalcNearestWithinBoundsPos = dgOnCalcNearestWithinBoundsPos;
    }

    /// <summary>
    /// Disable the spring movement.
    /// </summary>

    public void DisableSpring()
    {
        if (m_trRestrict != null)
        {
            SpringTransform sp = m_trRestrict.GetComponent<SpringTransform>();
            if (sp != null) sp.enabled = false;
        }
    }

    private void Start()
    {
        if (m_boxTrigger == null)
        {
            Debug.LogError("UIDragSpringOriginalFrame.Initialized Failed : m_boxTrigger");
        }

        if (m_dgOnMoveAbsolute == null)
        {
            Debug.LogError("UIDragSpringOriginalFrame.Initialized Failed : m_dgOnMoveAbsolute");
        }
    }

    void RestrictWithinBounds()
    {
        if (m_trRestrict != null)
        {
            if (m_dgOnIsWithinBounds())
            {
                return;
            }

            Vector3 v3Nearest = m_dgOnCalcNearestWithinBoundsPos();

            SpringTransform.Begin(m_trRestrict.gameObject, v3Nearest, 8f);
        }
    }

    void OnPress_Trigger(GameObject go, bool pressed)
    {
        //Debug.Log("OnPress_Trigger: " + pressed);
        if (mPressed == pressed || UICamera.currentScheme == UICamera.ControlScheme.Controller) return;

        if (IsMultiTouchDisabled()) return;

        if (!pressed && mDragID == UICamera.currentTouchID) mDragID = -10;
        mShouldMove = true;
        mPressed = pressed;

        if (pressed)
        {
            mMomentum = Vector3.zero;
            mScroll = 0f;

            // Disable the spring movement
            DisableSpring();

            mLastPos = UICamera.lastWorldPosition;

            mPlane = new Plane(m_boxTrigger.transform.rotation * Vector3.back, mLastPos);

            mDragStarted = true;
        }
        else
        {
            if (mDragStarted) RestrictWithinBounds();
            if (mDragStarted && onDragFinished != null) onDragFinished();
            if (!mShouldMove && onStoppedMoving != null) onStoppedMoving();
        }
    }

    void OnDrag_Trigger(GameObject go, Vector2 delta)
    {
        if (!mPressed || UICamera.currentScheme == UICamera.ControlScheme.Controller) return;

        if (IsMultiTouchDisabled()) return;

        if (mShouldMove)
        {
            if (mDragID == -10) mDragID = UICamera.currentTouchID;
            UICamera.currentTouch.clickNotification = UICamera.ClickNotification.BasedOnDelta;

            if (!mDragStarted)
            {
                mDragStarted = true;
            }

            Ray ray = UICamera.currentCamera.ScreenPointToRay(UICamera.currentTouch.pos);

            float dist = 0f;

            if (mPlane.Raycast(ray, out dist))
            {
                Vector3 currentPos = ray.GetPoint(dist);
                Vector3 offset = currentPos - mLastPos;
                mLastPos = currentPos;

                if (offset.x != 0f || offset.y != 0f || offset.z != 0f)
                {
                    offset = m_boxTrigger.transform.InverseTransformDirection(offset);

                    if (movement != Movement.Custom)
                    {
                        //offset.y = 0f;
                        offset.z = 0f;
                    }
                    else
                    {
                        offset.Scale((Vector3)customMovement);
                    }

                    offset = m_boxTrigger.transform.TransformDirection(offset);
                }

                mMomentum = Vector3.Lerp(mMomentum, mMomentum + offset * (0.01f * momentumAmount), 0.67f);
                //Debug.Log("Drag:mMomentum.x = " + mMomentum.x + " offset.x = " + offset.x);

                //开始模拟拖拽位移...(无脑等价于[dragEffect != DragEffect.MomentumAndSpring])
                if (m_dgOnMoveAbsolute != null)
                {
                    m_dgOnMoveAbsolute(EM_MessageEvent.OnDrag, offset, delta);
                }
                //Debug.Log("MoveAbsolute:OnSViewDrag: " + offset);
            }

            m_v2DragDelta = delta;
        }
    }

    void LateUpdate()
    {
        if (!Application.isPlaying) return;
        float delta = RealTime.deltaTime;

        if (IsMultiTouchDisabled()) return;

        if (!mShouldMove) return;

        if (!mPressed)
        {
            if (mMomentum.magnitude > 0.0001f || Mathf.Abs(mScroll) > 0.0001f)
            {
                if (movement != Movement.Custom)
                {
                    mMomentum -= m_boxTrigger.transform.TransformDirection(new Vector3(mScroll * 0.05f, 0f, 0f));
                    //Debug.Log("LateUpdate.01:mMomentum.x = " + mMomentum.x);
                }
                else
                {
                    mMomentum -= m_boxTrigger.transform.TransformDirection(new Vector3(
                        mScroll * customMovement.x * 0.05f,
                        mScroll * customMovement.y * 0.05f, 0f));
                }

                mScroll = NGUIMath.SpringLerp(mScroll, 0f, 20f, delta);

                Vector3 offset = NGUIMath.SpringDampen(ref mMomentum, dampenStrength, delta);
                //Debug.Log("LateUpdate.02:mMomentum.x = " + mMomentum.x + " offset.x = " + offset.x);

                //开始模拟拖拽位移...
                if (m_dgOnMoveAbsolute != null)
                {
                    m_dgOnMoveAbsolute(EM_MessageEvent.OnLateUpdatePressUp, offset, m_v2DragDelta);
                }
                //Debug.Log("MoveAbsolute:OnSViewLateUpdate: " + offset);

                if (mDragStarted) RestrictWithinBounds();
            }
            else
            {
                mScroll = 0f;
                mMomentum = Vector3.zero;

                mShouldMove = false;
                if (onStoppedMoving != null) onStoppedMoving();
            }
        }
        else
        {
            // Dampen the momentum
            mScroll = 0f;
            NGUIMath.SpringDampen(ref mMomentum, 9f, delta);
        }
    }

    void OnScroll_Trigger(GameObject go, float delta)
    {
        DisableSpring();
        if (m_dgOnScrollMove != null) m_dgOnScrollMove(delta);
    }

    public void StopMoving(bool bDisableSpring = false)
    {
        mShouldMove = false;
        mMomentum = Vector3.zero;

        if (bDisableSpring)
        {
            DisableSpring();
        }
    }

    public void SetMultiTouchDisabled(bool bIsDisabled)
    {
        m_bIsMultiTouchDisabled = bIsDisabled;
    }

    bool IsMultiTouchDisabled()
    {
        if (Input.touchCount >= 2 && m_bIsMultiTouchDisabled)
        {
            if (Input.GetTouch(0).phase == TouchPhase.Moved ||
                Input.GetTouch(1).phase == TouchPhase.Moved
                )
            {
                return true;
            }
        }
        return false;
    }
}
