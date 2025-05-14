using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SocialPlatforms.Impl;

public class Card : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    private int _m_cost;
    public int m_cost { get { return _m_cost; } set { if (0 < value && value <= 100) { _m_cost = value; } } }
    private int _m_CardIndex;
    public int m_CardIndex { get { return _m_CardIndex; } set { _m_CardIndex = value; } }
    private string _m_textBox;
    public string m_textBox { get { return _m_textBox; } set { _m_textBox = value; } }
    private int _m_ImgIndex;
    public int m_ImgIndex { get { return _m_ImgIndex; } set { _m_ImgIndex = value; } }

    public GameObject canvas;

    public void OnBeginDrag(PointerEventData eventData)
    {
        CardManager.instance.OnBeginDragEvent(gameObject);
    }

    public void OnDrag(PointerEventData eventData)
    {
        CardManager.instance.OnDragEvent(gameObject);
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        CardManager.instance.OnEndDragEvent(gameObject);
    }
}

//카드를 내면
//hand 에서 몇번째인지랑 > 
//카드 인덱스 > 