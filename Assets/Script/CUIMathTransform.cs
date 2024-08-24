using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CUIMathTransform : MonoBehaviour
{
    public GameObject mEarth;
    Vector3 mPostionEarthStart;
    public float mTransSpeed = 10.0f;

    public GameObject mEarth02;

    

    // Use this for initialization
    void Start ()
    {
        Test();
        Test02();

        mPostionEarthStart = mEarth.transform.position;
    }
	
	// Update is called once per frame
	void Update ()
    {
		
	}

    void Test()
    {
        /*
         * 向量的点积:
         * a.b = |a| |b| cos(θ)         
        */

        Debug.Log("Test.........Begin.............");

        Vector2 v2 = new Vector2(Mathf.Sqrt(3.0f), 1);
        Debug.Log("v2.normalized" + v2.normalized.ToString());
        //Vector2.Dot();

        Vector2 v2_A = new Vector2(Mathf.Sqrt(3.0f), 1);
        Vector2 v2_B = new Vector2(Mathf.Sqrt(3.0f), 0);

        float fDot = Vector2.Dot(v2_A, v2_B);
        Debug.Log("fDot = " + fDot);

        float fDot02 = Vector2.Dot(v2_A.normalized, v2_B.normalized);
        Debug.Log("fDot02 = " + fDot02);

        float fAngle_Dot = Mathf.Acos(Vector2.Dot(v2_A.normalized, v2_B.normalized)) * Mathf.Rad2Deg;
        Debug.Log("fAngle_Dot = " + fAngle_Dot);

        float fAngle_Dot02 = Mathf.Acos(Vector2.Dot(v2_A, v2_B) / (v2_A.magnitude * v2_B.magnitude)) * Mathf.Rad2Deg;
        Debug.Log("fAngle02 = " + fAngle_Dot02);

        float fAngle_Base = Vector3.Angle(v2_A,v2_B);
        Debug.Log("fAngle_Base = " + fAngle_Base);

        Debug.Log("Test.........End.............");
    }

    void Test02()
    {
        /*
         * 向量的叉积:
         * |axb|=|a| |b| sinθ     
        */
        Debug.Log("Test02.........Begin.............");

        Vector3 v3_A = new Vector3(10, 50, 1);
        Vector3 v3_B = new Vector3(1, 5, 10);

        Vector3 vCross = Vector3.Cross(v3_A, v3_B);
        Debug.Log("vCross = " + vCross);

        float fAngle_Cross = Mathf.Asin(Vector3.Distance(Vector3.zero, Vector3.Cross(v3_A.normalized, v3_B.normalized))) * Mathf.Rad2Deg;
        Debug.Log("fAngle_Cross = " + fAngle_Cross);

        float fAngle_Cross02 = Mathf.Asin(Vector3.Distance(Vector3.zero, Vector3.Cross(v3_A, v3_B) / (v3_A.magnitude * v3_B.magnitude))) * Mathf.Rad2Deg;
        Debug.Log("fAngle_Cross02 = " + fAngle_Cross02);

        float fAngle_Base = Vector3.Angle(v3_A, v3_B);
        Debug.Log("fAngle_Base = " + fAngle_Base);

        Debug.Log("Test02.........End.............");
    }

    private void LateUpdate()
    {
        if (Input.GetKey(KeyCode.W))
        {
            mEarth.transform.RotateAround(
                mPostionEarthStart,
                1 * Vector3.right,
                Time.deltaTime * mTransSpeed
                );
        }

        if (Input.GetKey(KeyCode.S))
        {
            mEarth.transform.RotateAround(
                mPostionEarthStart,
                -1 * Vector3.right,
                Time.deltaTime * mTransSpeed
                );
        }

        if (Input.GetKey(KeyCode.A))
        {
            mEarth.transform.RotateAround(
                mPostionEarthStart,
                1 * Vector3.up,
                Time.deltaTime * mTransSpeed
                );
        }

        if (Input.GetKey(KeyCode.D))
        {
            mEarth.transform.RotateAround(
                mPostionEarthStart,
                -1 * Vector3.up,
                Time.deltaTime * mTransSpeed
                );
        }





        if (Input.GetKey(KeyCode.LeftBracket))
        {
            mEarth.transform.LookAt(mEarth02.transform);
        }
        if (Input.GetKey(KeyCode.RightBracket))
        {
            mEarth.transform.rotation = Quaternion.LookRotation(mEarth02.transform.forward);
        }
        if (Input.GetKey(KeyCode.Equals))
        {
            Quaternion qua = Quaternion.AngleAxis(5, Vector3.up);
            mEarth.transform.rotation = mEarth.transform.rotation * qua;
        }
        if (Input.GetKey(KeyCode.Minus))
        {
            Quaternion qua = Quaternion.AngleAxis(5, Vector3.up);
            mEarth.transform.rotation = mEarth.transform.rotation * Quaternion.Inverse(qua);
        }
    }
}
