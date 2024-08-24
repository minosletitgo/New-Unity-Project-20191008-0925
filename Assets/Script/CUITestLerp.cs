using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CUITestLerp : MonoBehaviour
{
    public GameObject m_goPointFrom;
    public GameObject m_goPointTo;

    public Transform m_trMoving;

    private float m_fLerpRate;
    private float m_fNormalLerpRate = 5f;
    private float m_fFasterLerpRate = 27.0f;

    private bool m_bUseHistoriicalLerping = false; //是否启用平滑插值的开关，直接在 inspector 中设置
    private float m_fCloseEnough = 0.11f;

    private Vector3 m_v3SyncPos;
    private List<Vector3> m_lstSyncPos = new List<Vector3>();




    // Use this for initialization
    void Start()
    {
        m_trMoving.position = m_goPointFrom.transform.position;

        m_fLerpRate = m_fNormalLerpRate;

        SyncPostionsValues(m_goPointTo.transform.position);
    }

    // Update is called once per frame
    void Update()
    {
        if (m_bUseHistoriicalLerping) //更加平滑
        {
            HistoryLerping();
        }
        else
        {
            OrdinaryLerping();
        }
    }








    public void SyncPostionsValues(Vector3 v3LastPos)
    {
        m_v3SyncPos = v3LastPos;
        m_lstSyncPos.Add(m_v3SyncPos); //将所有服务端同步过来的 pos 全都保存在队列中
    }

    void OrdinaryLerping() //普通插值，有卡顿现象
    {
        m_trMoving.position = Vector3.Lerp(m_trMoving.position, m_v3SyncPos, Time.deltaTime * m_fLerpRate);
    }

    void HistoryLerping() //平滑插值
    {
        if (m_lstSyncPos.Count > 0)
        {
            //取出队列中的第一个设为插值的目标
            m_trMoving.position = Vector3.Lerp(m_trMoving.position, m_lstSyncPos[0], Time.deltaTime * m_fLerpRate);

            //位置足够接近，从队列中移除第一个，紧接着就是第二个
            if (Vector3.Distance(m_trMoving.position, m_lstSyncPos[0]) < m_fCloseEnough)
            {
                m_lstSyncPos.RemoveAt(0);
            }

            //如果同步队列过大，加快插值速率，使其更快到达目标点
            if (m_lstSyncPos.Count > 10)
            {
                m_fLerpRate = m_fFasterLerpRate;
            }
            else
            {
                m_fLerpRate = m_fNormalLerpRate;
            }

            Debug.LogFormat("--- syncPosList, count:{0}", m_lstSyncPos.Count);
        }
    }
}
