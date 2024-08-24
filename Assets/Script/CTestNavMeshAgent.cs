using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class CTestNavMeshAgent : MonoBehaviour
{
    [SerializeField]
    NavMeshAgent m_stAgent;

    private void Awake()
    {
        //m_stAgent = gameObject.GetComponent<NavMeshAgent>();
    }


    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit))
            {
                // target.transform.position = hit.point;
                //m_stAgent.destination = hit.point;
                m_stAgent.SetDestination(hit.point);
            }
        }
    }

}