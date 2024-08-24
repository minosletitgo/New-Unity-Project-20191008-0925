using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class CSmoothRotate : MonoBehaviour
{
    public BoxCollider m_boxTrigger;

    public GameObject m_goRotateRoot;
    public GameObject m_goPoles_N;
    public GameObject m_goPoles_S;
    public GameObject m_goPoles_L;
    public GameObject m_goPoles_R;
    public GameObject m_goPoles_WallZ_N;
    public GameObject m_goPoles_WallZ_S;
    public float m_fConfigRotateSpeed = 200;

    [SerializeField]
    UIDragSpringOriginalFrame m_stUIDragSpring;



    //LerpUpdate:Quaternion.SpringLerp
    bool m_bIsInRotating_QSLerp = false;
    Quaternion m_qtInRotatingToInit_QSLerp;
    Quaternion m_qtInRotatingToTarget_QSLerp;

    //LerpUpdate:Float.SpringLerp
    bool m_bIsInRotating_FSLerp = false;
    Quaternion m_qtInRotatingToTarget_FSLerp;
    float m_fInRotatingSpringValue_FSLerp = 0;


    private void Awake()
    {
        m_stUIDragSpring.Initialized(m_boxTrigger, OnRotateAbsolute);

        m_qtInRotatingToInit_QSLerp = m_goRotateRoot.transform.rotation;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.W))
        {
            string strLog = string.Format("{0}",
                m_goRotateRoot.transform.localEulerAngles
                );
            Debug.Log(strLog);
        }

        if (Input.GetKeyDown(KeyCode.F1) && 
            !m_bIsInRotating_QSLerp &&
            !m_bIsInRotating_FSLerp
            )
        {
            m_bIsInRotating_QSLerp = true;
            m_qtInRotatingToTarget_QSLerp = m_qtInRotatingToInit_QSLerp;
        }

        if (Input.GetKeyDown(KeyCode.F2) &&
            !m_bIsInRotating_QSLerp &&
            !m_bIsInRotating_FSLerp
            )
        {
            m_bIsInRotating_QSLerp = true;

            Quaternion qtCustomEulerFrom = m_goRotateRoot.transform.rotation;
            m_goRotateRoot.transform.Rotate(
                1.0f * (m_goPoles_S.transform.position - m_goPoles_N.transform.position),
                100.0f,
                Space.World
                );
            m_qtInRotatingToTarget_QSLerp = m_goRotateRoot.transform.rotation;
            m_goRotateRoot.transform.rotation = qtCustomEulerFrom;
        }

        if (Input.GetKeyDown(KeyCode.F3) &&
            !m_bIsInRotating_QSLerp &&
            !m_bIsInRotating_FSLerp
            )
        {
            m_bIsInRotating_FSLerp = true;
            m_qtInRotatingToTarget_FSLerp = m_goRotateRoot.transform.rotation;
            m_fInRotatingSpringValue_FSLerp = 0;
        }

        if (Input.GetKeyDown(KeyCode.F4) &&
            !m_bIsInRotating_QSLerp &&
            !m_bIsInRotating_FSLerp
            )
        {
            m_goRotateRoot.transform.Rotate(
                1.0f * (m_goPoles_S.transform.position - m_goPoles_N.transform.position),
                350,
                Space.World
                );
        }
    }

    private void LateUpdate()
    {
        if (m_bIsInRotating_QSLerp)
        {
            float strength = 5f;
            float delta = RealTime.deltaTime;

            bool trigger = false;
            Quaternion before = m_goRotateRoot.transform.rotation;
            Quaternion after = NGUIMath.SpringLerp(
                m_goRotateRoot.transform.rotation,
                m_qtInRotatingToTarget_QSLerp,
                strength,
                delta
                );

            if ((after.eulerAngles - before.eulerAngles).sqrMagnitude < 0.01f)
            {
                after = m_qtInRotatingToTarget_QSLerp;
                trigger = true;
            }

            m_goRotateRoot.transform.rotation = after;

            if (trigger)
            {
                m_bIsInRotating_QSLerp = false;
                Debug.Log("~~~~~~~~~~~~~~~~~~~~~~~~ m_bIsInRotating_QSLerp Is trigger ! ~~");
            }
        }
        else if (m_bIsInRotating_FSLerp)
        {
            float strength = 4f;
            float delta = RealTime.deltaTime;

            float fSpringBefore = m_fInRotatingSpringValue_FSLerp;
            m_fInRotatingSpringValue_FSLerp = NGUIMath.SpringLerp(
                fSpringBefore,
                360, strength, delta
                );
            float fspringDelta = Mathf.Abs(fSpringBefore - m_fInRotatingSpringValue_FSLerp);
            
            bool trigger = false;
            m_goRotateRoot.transform.Rotate(
                1.0f * (m_goPoles_S.transform.position - m_goPoles_N.transform.position),
                fspringDelta,
                Space.World
                );

            Quaternion after = m_goRotateRoot.transform.rotation;

            if ((after.eulerAngles - m_qtInRotatingToTarget_FSLerp.eulerAngles).sqrMagnitude < 0.1f)
            {
                Debug.Log("~~~~~~~~~~~~~~~~~~~~~~~~ m_bIsInRotating_QSLerp Is trigger [sqrMagnitude] ! ~~");
                after = m_qtInRotatingToTarget_FSLerp;
                trigger = true;
            }
            else if (Mathf.Abs(m_fInRotatingSpringValue_FSLerp - 360.0f) < 0.01f)
            {
                Debug.Log("~~~~~~~~~~~~~~~~~~~~~~~~ m_bIsInRotating_QSLerp Is trigger [SpringValue] ! ~~");
                after = m_qtInRotatingToTarget_FSLerp;
                trigger = true;
            }

            m_goRotateRoot.transform.rotation = after;

            if (trigger)
            {
                m_bIsInRotating_FSLerp = false;                
            }
        }

    }



    public void OnRotateAbsolute(UIDragSpringOriginalFrame.EM_MessageEvent emEvent, Vector3 absolute, Vector2 v2Delta)
    {
        if (m_bIsInRotating_QSLerp || m_bIsInRotating_FSLerp)
        {
            return;
        }


        Vector3 v3LocalEulerAngles = m_goRotateRoot.transform.localEulerAngles;

        //Debug.Log(GameCommon.PrintVector3(absolute));

        if (Mathf.Abs(absolute.x) > Mathf.Abs(absolute.y))
        {
            //horizontal
            float _fDir = (absolute.x >= 0) ? 1 : -1;
            m_goRotateRoot.transform.Rotate(
                _fDir * (m_goPoles_S.transform.position - m_goPoles_N.transform.position),
                Time.deltaTime * m_fConfigRotateSpeed,
                Space.World
                );
        }
        else
        {
            //vertical
            //m_goRotateRoot.transform.Rotate(
            //    new Vector3(v2Delta.y, -v2Delta.x),
            //    Time.deltaTime * m_fConfigRotateSpeed,
            //    Space.World
            //    );

            float _fDir = (absolute.y <= 0) ? 1 : -1;
            m_goRotateRoot.transform.Rotate(
                _fDir * (m_goPoles_L.transform.position - m_goPoles_R.transform.position),
                Time.deltaTime * m_fConfigRotateSpeed,
                Space.World
                );

            if (m_goPoles_N.transform.position.z < m_goPoles_WallZ_N.transform.position.z ||
                m_goPoles_S.transform.position.z < m_goPoles_WallZ_S.transform.position.z
                )
            {
                m_goRotateRoot.transform.localEulerAngles = v3LocalEulerAngles;
            }
        }


        //{
        //    //测试Log
        //    string strLog = string.Format("{0} , {1} <---> {2}",
        //        m_goPoles_N.transform.position.z,
        //        m_goPoles_S.transform.position.z,
        //        m_goPoles_NS_Z.transform.position.z
        //        );
        //    Debug.Log(strLog);
        //}
    }
}