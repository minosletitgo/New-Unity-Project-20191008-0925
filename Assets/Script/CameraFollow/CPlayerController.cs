using UnityEngine;
using System.Collections;

public class CPlayerController : MonoBehaviour
{
    GameObject m_goPlayer;

    private void Start()
    {
        m_goPlayer = GameObject.FindGameObjectWithTag("Player");
        GameCommon.ASSERT(m_goPlayer != null);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.W))
        {
            m_goPlayer.transform.localPosition += new Vector3(0, 1, 0);
        }
        if (Input.GetKeyDown(KeyCode.S))
        {
            m_goPlayer.transform.localPosition += new Vector3(0, -1, 0);
        }
    }
}