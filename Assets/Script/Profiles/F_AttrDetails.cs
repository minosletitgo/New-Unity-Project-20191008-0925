using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System;

namespace DemoProject.Profiles
{
    [CreateAssetMenu(fileName = "F_AttrDetails", menuName = "Profiles/Create F_AttrDetails", order = 1)]
    public class F_AttrDetails : ScriptableObject
    {
        [SerializeField]
        string strName;

        [SerializeField]
        int nBaseHp;

        [SerializeField]
        [Range(0, 100)]
        float fHitRate;

        [SerializeField]
        int nDamage;

        [SerializeField]
        [Range(0, 1)]
        float fDamageDelay;

        [SerializeField]
        int nIncome;

        [SerializeField]
        int nRecoverHp;

        [SerializeField]
        float fWalkSpeed;

        [SerializeField]
        float fRunSpeed;
    }





    //[CreateAssetMenu(fileName = "F_AttrDetailsList", menuName = "Profiles/Create F_AttrDetails List"/*, order = 1*/)]
    //public class F_AttrDetailsList : DataListBase<F_AttrDetails>
    //{

    //}
}

