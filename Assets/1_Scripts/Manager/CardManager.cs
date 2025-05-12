using System.Collections;
using System.Collections.Generic;
using UnityEngine;



//ī�� ���� �ҷ����� �� m_cardLoader

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

    //�ε�� ������
    // �Ľ��� �ؼ� ��ü ī�� �迭�� �����ؾߵǴ°� �ƴϳ�

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
        //loader�� ó��
        // loader index = card.m_CardIndex;
        m_hand.Remove(card);
        m_grave.Add(card);
    }
}
