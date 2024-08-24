//using UnityEngine;
//using System.Collections;
//using System.Collections.Generic;

//namespace DemoProject.Profiles
//{
//    public abstract class DataListBase<T> : ScriptableObject
//    {
//        [SerializeField]
//        private string m_strDataName = "Your DataName!";

//        [SerializeField]
//        private List<T> m_Details;

//        public T this[int index]
//        {
//            get { return m_Details[index]; }
//        }

//        public int Count
//        {
//            get { return m_Details.Count; }
//        }
//    }
//}