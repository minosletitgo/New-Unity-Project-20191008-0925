using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using Jacovone;
//using UnityEngine.Collections;

public class PMT_Main : MonoBehaviour
{
    public PathMagic m_pPathMagic;
    public GameObject m_goPreTarget_Mirror;
    public List<GameObject> m_lstPreTarget_Monster;
    

    [ReadOnly]
    [SerializeField]
    Vector3 m_v3ZeroPosInPath;
    [ReadOnly]
    [SerializeField]
    float m_fCurrentPosRatio;
    [SerializeField]
    [ReadOnly]
    float m_fPreDeltaPosRatio;
    [SerializeField]
    [ReadOnly]
    float m_fTotalDistance;

    public float m_fPreDeltaDistance = 10.0f;
    [SerializeField]
    [ReadOnly]
    float m_fPreDeltaDistanceRatio = 0.0f;



    //[SerializeField]
    //[ReadOnly]
    //float m_fAngle = 0.0f;
    //[SerializeField]
    //[ReadOnly]
    //float m_fDisToRoad = 0.0f;

    //[SerializeField]
    //[ReadOnly]
    //Vector3 m_v3OrginOffset;

    List<float> m_lstMonsterToZero_AngleToRoad = new List<float>();    
    List<float> m_lstMonsterToZero_DisToRoad = new List<float>();
    List<bool> m_lstMonsterToZero_IsRight = new List<bool>();

    //////////////////////////////////////////////////////

    void Awake()
    {
        Quaternion rotation;
        float vel;
        int wp;
        m_pPathMagic.sampledPositionAndRotationAndVelocityAndWaypointAtPos(0, out m_v3ZeroPosInPath, out rotation, out vel, out wp);
        
        m_fTotalDistance = m_pPathMagic.TotalDistance;
        m_fPreDeltaDistanceRatio = m_fPreDeltaDistance / m_fTotalDistance;

        m_lstMonsterToZero_AngleToRoad.Clear();
        m_lstMonsterToZero_DisToRoad.Clear();
        m_lstMonsterToZero_IsRight.Clear();
        for (int i = 0; i < m_lstPreTarget_Monster.Count; i++)
        {
            float fAngleToRoad = Vector3.Angle(m_lstPreTarget_Monster[i].transform.position - m_v3ZeroPosInPath, m_lstPreTarget_Monster[i].transform.forward * -1);
            m_lstMonsterToZero_AngleToRoad.Add(fAngleToRoad);
            
            float fDisToRoad = Vector3.Distance(m_lstPreTarget_Monster[i].transform.position, m_v3ZeroPosInPath) * (float)Mathf.Sin(fAngleToRoad * UnityEngine.Mathf.Deg2Rad);
            m_lstMonsterToZero_DisToRoad.Add(fDisToRoad);

            float fAngleToRight = Vector3.Angle(m_lstPreTarget_Monster[i].transform.position - m_v3ZeroPosInPath, m_lstPreTarget_Monster[i].transform.right * +1);
            //Debug.Log("fAngleToRight "+ fAngleToRight);
            m_lstMonsterToZero_IsRight.Add(fAngleToRight <= 90f);
        }        

        //m_v3OrginOffset = m_v3ZeroPosInPath - m_goPreTarget.transform.position;
    }

    void Update()
    {
        if (m_pPathMagic.IsPlaying)
        {
            m_fCurrentPosRatio = m_pPathMagic.CurrentPos;

            m_fPreDeltaPosRatio = m_pPathMagic.CurrentPos - m_fPreDeltaDistanceRatio;
            if (m_fPreDeltaPosRatio > 0)
            {
                if (m_pPathMagic.PresampledPath)
                {
                    Vector3 position;
                    Quaternion rotation;
                    float vel;
                    int wp;
                    m_pPathMagic.sampledPositionAndRotationAndVelocityAndWaypointAtPos(m_fPreDeltaPosRatio, out position, out rotation, out vel, out wp);
                    
                    Quaternion rotationFace = m_pPathMagic.GetFaceForwardForPos(m_fPreDeltaPosRatio);

                    m_goPreTarget_Mirror.transform.rotation = rotation;
                    m_goPreTarget_Mirror.transform.position = position;


                    for (int iMonster = 0; iMonster < m_lstPreTarget_Monster.Count; iMonster++)
                    {
                        m_lstPreTarget_Monster[iMonster].transform.rotation = rotation;

                        Vector3 v3Dir = m_goPreTarget_Mirror.transform.right;
                        if (!m_lstMonsterToZero_IsRight[iMonster])
                        {
                            v3Dir = v3Dir * -1;
                        }
                        m_lstPreTarget_Monster[iMonster].transform.position = position + v3Dir * m_lstMonsterToZero_DisToRoad[iMonster];
                    }
                }
                else
                {
                    //m_goPreTarget_Monster.transform.position = m_pPathMagic.computePositionAtPos(m_fPreDeltaPosRatio);
                }                
            }            
        }
    }
}