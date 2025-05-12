using System.Collections;
using System.Collections.Generic;
using UnityEngine;



//카드 정보 불러오는 애 m_cardLoader

public class CardManager : MonoBehaviour
{
    public static CardManager instance;

    private void Awake()
    {
        if(instance == null)
        {
            instance = this;
            DontDestroyOnLoad(instance);
        }
        else
        {
            Destroy(instance);
        }
    }

    private List<Card> m_deck = new List<Card>();
    private List<Card> m_grave = new List<Card>();
    private List<Card> m_hand = new List<Card>();

    //로드된 정보들
    // 파싱을 해서 전체 카드 배열이 존재해야되는거 아니냐

    private static int m_deckCount
    {
        get; set;
    }
    public void Shuffle()
    {
        foreach(Card card in m_grave)
        {
            m_deck.Add(card);
            m_grave.Remove(card);
        }

        int n = m_deck.Count;
        while (n > 1)
        {
            n--;
            int k = Random.Range(0, n);
            Card card = m_deck[k];
            m_deck[k] = m_deck[n];
            m_deck[n] = card;
        }

    }

    public void Draw(int num)
    {
        for (int i = 0; i < num; i++)
        {
            m_hand.Add(m_deck[0]);
            m_deck.Remove(m_deck[0]);
        }
    }

    public void Discard()
    {
        foreach(Card card in m_hand)
        {
            m_hand.Remove(card);
            m_grave.Add(card);
        }
    }

    public void HandUse(Card card)
    {
        //loader가 처리
        // loader index = card.m_CardIndex;
        m_hand.Remove(card);
        m_grave.Add(card);
    }
}
