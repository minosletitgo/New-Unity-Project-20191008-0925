using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CRigidbodyTest : MonoBehaviour
{
    public GameObject m_goCube1;
    Rigidbody m_rb1;

    public GameObject m_goCube2;
    Rigidbody m_rb2;
    bool m_bIsMoving1 = false;


    private void Start()
    {
        m_rb1 = m_goCube1.GetComponent<Rigidbody>();
        GameCommon.ASSERT(m_rb1 != null);

        m_rb2 = m_goCube2.GetComponent<Rigidbody>();
        GameCommon.ASSERT(m_rb2 != null);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.A))
        {
            m_bIsMoving1 = true;
        }

        if (Input.GetKeyDown(KeyCode.BackQuote))
        {
            m_bIsMoving1 = false;
        }

        if (m_bIsMoving1)
        {
            m_goCube2.transform.localPosition += new Vector3(
                -1 * Time.deltaTime * 1,
                0,
                0
                );
        }
    }
}