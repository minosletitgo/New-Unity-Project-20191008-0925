using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CUITestSomeCode : MonoBehaviour
{
    public UILabel m_labFPS;


    #region
    //更新的时间间隔
    public float UpdateInterval = 0.00f;
    //最后的时间间隔
    private float _lastInterval;
    //帧[中间变量 辅助]
    private int _frames = 0;
    //当前的帧率
    private float _fps;
    #endregion



    #region

    #endregion




    // Use this for initialization
    void Start ()
    {
        EditorLOG.logWarn(GameCommon.ColorCodeTrans_RGB2Hex(254, 200, 0));

        EditorLOG.logWarn(GameCommon.ColorCodeTrans_Hex2RGB_R("#F0FF00").ToString());
        EditorLOG.logWarn(GameCommon.ColorCodeTrans_Hex2RGB_G("#F0FF00").ToString());
        EditorLOG.logWarn(GameCommon.ColorCodeTrans_Hex2RGB_B("#F0FF00").ToString());

        m_labFPS.text = null;
    }
	





	// Update is called once per frame
	void Update ()
    {
        //float fValue = (1.0f / Time.deltaTime);
        //m_labFPS.text = fValue.ToString("0.00");

        ++_frames;
        if (Time.realtimeSinceStartup > _lastInterval + UpdateInterval)
        {
            _fps = _frames / (Time.realtimeSinceStartup - _lastInterval);
            _frames = 0;
            _lastInterval = Time.realtimeSinceStartup;

            m_labFPS.text = _fps.ToString("0.00");
        }
    }
}
