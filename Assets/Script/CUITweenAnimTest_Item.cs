using UnityEngine;
using System.Collections;

public class CUITweenAnimTest_Item : MonoBehaviour
{
    [SerializeField]
    TweenPosition m_twPos;


    public void Show()
    {
        gameObject.SetActive(true);

        m_twPos.ResetToBeginning();
        m_twPos.PlayForward();
    }

    public void UnShow()
    {
        m_twPos.ResetToBeginning();
        
        gameObject.SetActive(false);
    }
}