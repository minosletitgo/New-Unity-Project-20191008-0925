using UnityEngine;
using UnityEngine.UI;
using UnityEngine.U2D;
using System.Collections;
using System.Collections.Generic;

public class CUGUIStart : MonoBehaviour
{
    public Image m_iage;
    public Button m_btn;
    public Button m_btn02;
    public Button m_btn03;
    public Button m_btnMyIcons;
    public Text m_lab;
    public Toggle m_tog;
    public Slider m_slder;
    public Dropdown m_dd;
    public InputField m_it;

    public CanvasGroup m_cg;

    public SpriteAtlas m_stMyIcons;






    [SerializeField]
    int m_nRepairingCurValue = 0;
    [SerializeField]
    int m_nRepairingMaxValue = 100;


    private void Start()
    {
        UGUIEventTriggerListener.Get(m_btn.gameObject).parameter = 1001;
        UGUIEventTriggerListener.Get(m_btn.gameObject).onClick = OnClick_Btn;
        UGUIEventTriggerListener.Get(m_btn02.gameObject).parameter = 1002;
        UGUIEventTriggerListener.Get(m_btn02.gameObject).onClick = OnClick_Btn02;
        UGUIEventTriggerListener.Get(m_btn03.gameObject).onClick = OnClick_Btn03;
        UGUIEventTriggerListener.Get(m_btnMyIcons.gameObject).onClick = OnClick_BtnMyIcons;
        m_tog.onValueChanged.AddListener(OnValueChanged_Tog);
        m_slder.onValueChanged.AddListener(OnValueChanged_Slder);

        m_dd.ClearOptions();
        List<string> lstString = new List<string>();
        for (int i = 0; i < 10; i++)
        {
            lstString.Add("Option " + (i + 1));
        }
        m_dd.AddOptions(lstString);
        m_dd.onValueChanged.AddListener(OnValueChanged_Dropdown);

        m_it.characterLimit = 0;

        m_cg.alpha = 0;
    }

    void OnClick_Btn(GameObject go)
    {
        int nParam = (int)UGUIEventTriggerListener.Get(go.gameObject).parameter;
        Debug.Log("OnClick_Btn : " + nParam);
        StartRepairing();
    }

    void OnClick_Btn02(GameObject go)
    {
        int nParam = (int)UGUIEventTriggerListener.Get(go.gameObject).parameter;
        Debug.Log("OnClick_Btn02 : " + nParam);
        StopRepairing();
    }

    void OnClick_Btn03(GameObject go)
    {
        StartCoroutine(GameTools.FadeCanvasGroup(m_cg, 1.2f, 1f));
    }

    void OnClick_BtnMyIcons(GameObject go)
    {
        Sprite[] arySprites = new Sprite[100];
        int nNumber = m_stMyIcons.GetSprites(arySprites);
        Debug.Log("nNumber = "+ nNumber);
    }

    void OnValueChanged_Tog(bool bValue)
    {
        Debug.Log("OnValueChanged_Tog : " + bValue);
    }

    void OnValueChanged_Slder(float fValue)
    {
        Debug.Log("OnValueChanged_Slder : " + fValue);
    }

    void OnValueChanged_Dropdown(int nValue)
    {
        Debug.Log("OnValueChanged_Dropdown : " + nValue);
    }

    void Update()
    {

    }






    void StartRepairing()
    {
        if (m_coRepairing != null)
        {
            StopCoroutine(m_coRepairing);
            m_coRepairing = null;
        }
        m_coRepairing = CoRepairing();
        StartCoroutine(m_coRepairing);
    }

    public void StopRepairing()
    {
        if (m_coRepairing != null)
        {
            StopCoroutine(m_coRepairing);
            m_coRepairing = null;
        }
    }


    IEnumerator m_coRepairing;
    IEnumerator CoRepairing()
    {
        while (m_nRepairingCurValue < m_nRepairingMaxValue)
        {
            yield return new WaitForSeconds(1.0f);
            m_nRepairingCurValue++;
        }
        m_coRepairing = null;
    }
}