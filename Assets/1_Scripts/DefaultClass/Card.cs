using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;

public class Card : MonoBehaviour
{

    [SerializeField]
    public int m_cost;
    //{
    //    get { return m_cost; }
    //    set
    //    {
    //        if (0 < value && value <= 100)
    //        {
    //            this.m_cost = value;
    //        }
    //    }
    //}
    [SerializeField] public int m_CardIndex { get { return m_CardIndex; } set { } }
    [SerializeField] public string m_textBox { get { return m_textBox; } set { } }
    [SerializeField] public int m_ImgIndex { get { return m_ImgIndex; } set { } }

    public void Use()
    {
        CardManager.instance.HandUse(this);
    }
}

//카드를 내면
//hand 에서 몇번째인지랑 > 
//카드 인덱스 > 