using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CRigidbodyTest_Go2 : MonoBehaviour
{



    private void OnCollisionEnter(Collision collision)
    {
        Debug.Log("CRigidbodyTest_Go2.OnCollisionEnter : " + collision.gameObject.name);
    }

    private void OnCollisionExit(Collision collision)
    {
        Debug.Log("CRigidbodyTest_Go2.OnCollisionExit : " + collision.gameObject.name);
    }
}