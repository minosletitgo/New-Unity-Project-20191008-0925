using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIScrollView_Ellipse : MonoBehaviour
{
    public enum Movement
    {
        Normal,
        Custom,
    }

    public GameObject m_goCenter;
    public float m_fAxisA;
    public float m_fAxisB;
    public float m_fDegreePointPosOffset;
    public float m_fDegreeRotateOffsetX = 0;
    public float m_fDegreeRotateOffsetY = 0;
    public float m_fDegreeRotateOffsetZ = 0;

    public Transform m_trRoot;
    public UIScrollBar m_sbValue;

    public bool hideInactive = false;

    List<Transform> m_lstChild = new List<Transform>();    
    float m_fScrollDegreeValue = 0;

    public delegate void OnSetAmountNotification(float fDegree);
    public OnSetAmountNotification onSetAmount;

    public delegate void OnDragNotification();
    public OnDragNotification onDragFinished;

    public Movement movement = Movement.Normal;
    public Vector2 customMovement = new Vector2(1f, 0f);
    public float momentumAmount = 35f;
    public float dampenStrength = 9f;

    protected Transform mTrans;
    protected Plane mPlane;
    protected Vector3 mLastPos;
    protected bool mPressed = false;
    protected Vector3 mMomentum = Vector3.zero;
    protected float mScroll = 0f;
    protected int mDragID = -10;
    protected bool mDragStarted = false;
       

    public bool isDragging { get { return mPressed && mDragStarted; } }

    public UIScrollView_Ellipse_CrossBox mBoxCross;


    public enum EM_RecenterEvent
    {
        OnSViewStart,
        OnSOChildStart,
        OnSOChildEnable,
        OnSViewPressFinished,
        OnSViewDragFinished,
        OnSViewLateUpdate,
        OnCustomClick,
        OnCustomRefresh,
    };

    public enum EM_MoveAbsoluteEvent
    {
        OnSViewDrag,
        OnSViewLateUpdate,
    };

    public delegate void OnMoveAbsoluteNotification(
        Vector3 _v3Absolute,
        float _fAngleDegreeFrom, 
        float _fAngleDegreeTo, 
        UIScrollView_Ellipse.EM_MoveAbsoluteEvent emMAEvent
        );
    public OnMoveAbsoluteNotification onMoveAbsoluteNotification;
    public OnDragNotification onDragStart;


    private void Awake()
    {
        mTrans = transform;
        mBoxCross.Initialized(this);
    }

    private void Start()
    {
        ResetPosition();
        Recenter(UIScrollView_Ellipse.EM_RecenterEvent.OnSViewStart);
    }

    public void Recenter(UIScrollView_Ellipse.EM_RecenterEvent emREvent)
    {
        if (centerOnChild != null)
        {
            centerOnChild.Recenter(emREvent);
        }
    }

    public void SynChildList()
    {
        Transform myTrans = m_trRoot;
        m_lstChild.Clear();

        for (int i = 0; i < myTrans.childCount; ++i)
        {
            Transform t = myTrans.GetChild(i);

            if (!hideInactive || (t && t.gameObject.activeSelf))
            {
                /*if (!UIDragDropItem.IsDragged(t.gameObject)) */
                m_lstChild.Add(t);
            }
        }

        if (m_sbValue != null)
        {
            //随便写一个barSize
            m_sbValue.barSize = myTrans.childCount / 360.0f;
        }
    }

    public IEnumerable EnumChildList()
    {
        foreach (Transform tr in m_lstChild)
        {
            if (!hideInactive || (tr && tr.gameObject.activeSelf))
            {
                yield return tr;
            }
        }
    }

    public int GetChildCount()
    {
        int nCount = 0;
        foreach (Transform tr in m_lstChild)
        {
            if (!hideInactive || (tr && tr.gameObject.activeSelf))
            {
                nCount++;
            }
        }
        return nCount;
    }

    public void DisableSpring()
    {
        SpringEllipseDegree sp = GetComponent<SpringEllipseDegree>();
        if (sp != null) sp.enabled = false;
        //Debug.Log("UIScrollView_Ellipse.DisableSpring");
    }

    public void SetDragAmountDegreeValue(float fDegreeValue, bool bIsDisableSpring)
    {
        if (bIsDisableSpring)
        {
            DisableSpring();
        }        

        //fDegreeValue 即 0~360
        SynChildList();

        if (m_lstChild.Count <= 0)
        {
            return;
        }

        float fDegreeValueClamp = MathEllipseHelper.DegreeClamp(fDegreeValue);

        //Vector3 v3Center = UICamera.mainCamera.WorldToScreenPoint(m_goCenter.transform.position);

        float fDegreeStep = 360.0f / (float)(m_lstChild.Count);
        for (int i = 0; i < m_lstChild.Count; i++)
        {
            Transform tran = m_lstChild[i];

            float _fDegree = 0 + i * fDegreeStep + m_fDegreePointPosOffset + fDegreeValueClamp;
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

        if (m_sbValue != null)
        {
            m_sbValue.value = fDegreeValueClamp / 360.0f;
        }

        //PS：一定要存储[非Clamp角度]进行运算!
        m_fScrollDegreeValue = fDegreeValue;

        if (onSetAmount != null)
        {
            onSetAmount(fDegreeValue);
        }
    }

    public float GetDragAmountDegreeValue(bool bIsClamp = false)
    {
        if (!bIsClamp)
        {
            return m_fScrollDegreeValue;
        }
        return MathEllipseHelper.DegreeClamp(m_fScrollDegreeValue);
    }

    [ContextMenu("Reset Position")]
    void ResetPosition()
    {
        if (NGUITools.GetActive(this))
        {
            SetDragAmountDegreeValue(0, true);
        }
    }

    void MoveAbsolute(Vector3 absolute, EM_MoveAbsoluteEvent emMAEvent)
    {
        //EditorLOG.log("MoveAbsolute = " + absolute.x);
        float fDeltaScrollValue = absolute.x * 360.0f;

        if (onMoveAbsoluteNotification != null)
        {
            onMoveAbsoluteNotification(
                absolute,
                GetDragAmountDegreeValue(),
                GetDragAmountDegreeValue() + fDeltaScrollValue,
                emMAEvent
                );
        }

        SetDragAmountDegreeValue(
            GetDragAmountDegreeValue() + fDeltaScrollValue,
            true
            );
    }

    public void Press(bool pressed)
    {
        if (mPressed == pressed || UICamera.currentScheme == UICamera.ControlScheme.Controller) return;

        //if (smoothDragStart && pressed)
        //{
        //    mDragStarted = false;
        //    mDragStartOffset = Vector2.zero;
        //}

        if (enabled && NGUITools.GetActive(gameObject))
        {
            if (!pressed && mDragID == UICamera.currentTouchID) mDragID = -10;

            mPressed = pressed;

            if (pressed)
            {
                mMomentum = Vector3.zero;
                mScroll = 0f;

                DisableSpring();

                mLastPos = UICamera.lastWorldPosition;

                mPlane = new Plane(mTrans.rotation * Vector3.back, mLastPos);

                mDragStarted = true;
            }
            else if (centerOnChild)
            {
                //if (mDragStarted && onDragFinished != null) onDragFinished();
                centerOnChild.Recenter(UIScrollView_Ellipse.EM_RecenterEvent.OnSViewPressFinished);
            }
            else
            {
                if (mDragStarted && onDragFinished != null) onDragFinished();
            }
        }
    }

    public void Drag()
    {
        if (!mPressed || UICamera.currentScheme == UICamera.ControlScheme.Controller) return;

        if (enabled && NGUITools.GetActive(gameObject) /*&& mShouldMove*/)
        {
            if (mDragID == -10) mDragID = UICamera.currentTouchID;
            UICamera.currentTouch.clickNotification = UICamera.ClickNotification.BasedOnDelta;

            Ray ray = UICamera.currentCamera.ScreenPointToRay(UICamera.currentTouch.pos);

            float dist = 0f;

            if (mPlane.Raycast(ray, out dist))
            {
                Vector3 currentPos = ray.GetPoint(dist);
                Vector3 offset = currentPos - mLastPos;
                mLastPos = currentPos;

                if (offset.x != 0f || offset.y != 0f || offset.z != 0f)
                {
                    offset = mTrans.InverseTransformDirection(offset);

                    if (movement != Movement.Custom)
                    {
                        offset.y = 0f;
                        offset.z = 0f;
                    }
                    else
                    {
                        offset.Scale((Vector3)customMovement);
                    }

                    offset = mTrans.TransformDirection(offset);
                }

                mMomentum = Vector3.Lerp(mMomentum, mMomentum + offset * (0.01f * momentumAmount), 0.67f);
                //Debug.Log("Drag:mMomentum.x = " + mMomentum.x + " offset.x = " + offset.x);

                //PS:开始模拟椭圆拖拽位移(无脑等价于[dragEffect != DragEffect.MomentumAndSpring])
                MoveAbsolute(offset, EM_MoveAbsoluteEvent.OnSViewDrag);
            }
        }
    }

    public void DragStart()
    {
        if (onDragStart != null) onDragStart();
    }

    public void DragEnd()
    {
        if (mDragStarted && onDragFinished != null) onDragFinished();

        mDragStarted = false;
    }

    [HideInInspector]
    public UICenterOnChild_Ellipse centerOnChild = null;

    void LateUpdate()
    {
        if (!Application.isPlaying) return;
        float delta = RealTime.deltaTime;


        if (!mPressed)
        {
            if (mMomentum.magnitude > 0.0001f || Mathf.Abs(mScroll) > 0.0001f)
            {
                if (movement != Movement.Custom)
                {
                    mMomentum -= mTrans.TransformDirection(new Vector3(mScroll * 0.05f, 0f, 0f));
                    //Debug.Log("LateUpdate.01:mMomentum.x = " + mMomentum.x);
                }
                else
                {
                    mMomentum -= mTrans.TransformDirection(new Vector3(
                        mScroll * customMovement.x * 0.05f,
                        mScroll * customMovement.y * 0.05f, 0f));
                }

                mScroll = NGUIMath.SpringLerp(mScroll, 0f, 20f, delta);

                Vector3 offset = NGUIMath.SpringDampen(ref mMomentum, dampenStrength, delta);
                //Debug.Log("LateUpdate.02:mMomentum.x = " + mMomentum.x + " offset.x = " + offset.x);

                //开始模拟椭圆拖拽位移...
                MoveAbsolute(offset, EM_MoveAbsoluteEvent.OnSViewLateUpdate);

                if (NGUITools.GetActive(centerOnChild))
                {
                    centerOnChild.Recenter(UIScrollView_Ellipse.EM_RecenterEvent.OnSViewLateUpdate);
                }
            }
            else
            {
                mScroll = 0f;
                mMomentum = Vector3.zero;
            }
        }
        else
        {
            // Dampen the momentum
            mScroll = 0f;
            NGUIMath.SpringDampen(ref mMomentum, 9f, delta);
        }     
    }
}