using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class LookRotationTest : MonoBehaviour
{
    public GameObject m_goCube;
    public GameObject m_goSphere;

    void Update()
    {
        Vector3 v3Dir = m_goSphere.transform.position - m_goCube.transform.position;
        m_goCube.transform.rotation = Quaternion.LookRotation(v3Dir);
    }
}