using UnityEngine;
using System.Collections;

public class CCameraShakeTest : MonoBehaviour
{
    public Transform m_trCamera;

    private Vector3 m_v3PositionBefore;       //记录抖动前的位置
    public float m_fShakeCD = 0.002f;        //抖动的频率
    public int m_nShakeMaxCount = 100;
    private int m_nShakeCount = -1;           //设置抖动次数
    private float m_fShakeTime;

    void Start()
    {
        if (m_trCamera == null) m_trCamera = transform;

        m_v3PositionBefore = m_trCamera.position;    //记录抖动前的位置
        m_nShakeCount = m_nShakeMaxCount;
    }

    void Update()
    {
        if (m_fShakeTime + m_fShakeCD < Time.time && m_nShakeCount > 0)
        {
            m_nShakeCount--;
            float fRadio = Random.Range(-0.01f, 0.01f);

            if (m_nShakeCount == 1)   //抖动最后一次时设置为都动前记录的位置
                fRadio = 0;

            m_fShakeTime = Time.time;
            m_trCamera.position = m_v3PositionBefore + Vector3.one * fRadio;
        }
    }
}
