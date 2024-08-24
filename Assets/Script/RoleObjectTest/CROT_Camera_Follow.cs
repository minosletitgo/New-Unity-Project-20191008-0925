using UnityEngine;

public class CROT_Camera_Follow : MonoBehaviour
{
    public Transform m_trPlayer; // 主角位置
    public float m_fSpeed = 5f; // 相机速度
    Vector3 m_v3Distance; // 主角和摄像机之间的距离


    void Start()
    {
        // 计算人物与摄像机之间的向量
        // 用当前摄像机的坐标 - 玩家的坐标（可以画一张图来算一算）
        m_v3Distance = transform.position - m_trPlayer.position;
    }


    void FixedUpdate()
    {
        // 摄像机应该在的位置
        // 不直接赋值给当前摄像机的原因是，需要这个参数来实现一个延迟功能
        Vector3 v3TargetCamPos = m_trPlayer.position + m_v3Distance;

        // 给摄像机移动到应该在的位置的过程中加上延迟效果
        transform.position = Vector3.Lerp(transform.position, v3TargetCamPos, m_fSpeed * Time.deltaTime);
    }
}
