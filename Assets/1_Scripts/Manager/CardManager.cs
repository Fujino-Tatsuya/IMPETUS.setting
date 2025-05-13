using System.Collections;
using System.Collections.Generic;
using UnityEngine;



//카드 정보 불러오는 애 m_cardLoader

public class CardManager : MonoBehaviour
{
    public static CardManager instance;
    public GameObject cardPrefab;

    public GameObject Deck;
    public GameObject Hand;
    public GameObject Grave;


    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(instance);
        }
        else
        {
            Destroy(instance);
        }

        M_InitDeck(10);

        foreach (GameObject card in m_deck)
        {
            Card content = card.GetComponent<Card>();
            print(content.m_CardIndex);
        }

    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.Space))
        {
            Draw(1);
        }
        if(Input.GetKeyDown(KeyCode.LeftShift))
        {
            Discard();
        }
    }


    void M_InitDeck(int InitCard)
    {
        int m_CardCount = InitCard;
        for (int i = 0; i < m_CardCount; i++)
        {
            GameObject gameObject = Instantiate(cardPrefab, Deck.transform);
            Card card = gameObject.GetComponent<Card>();
            card.m_CardIndex = i;
            m_deck.Add(gameObject);
        }
    }

    private List<GameObject> m_deck = new List<GameObject>();
    private List<GameObject> m_grave = new List<GameObject>();
    private List<GameObject> m_hand = new List<GameObject>();

    //로드된 정보들
    // 파싱을 해서 전체 카드 배열이 존재해야되는거 아니냐

    private static int m_deckCount
    {
        get; set;
    }
    public void Shuffle()
    {
        int i = 0;
        foreach (GameObject gameObject in m_grave)
        {
            m_deck.Add(gameObject);
            m_grave[i++].transform.position = Deck.transform.position;
        }
        m_grave.Clear();

        int n = m_deck.Count;
        while (n > 1)
        {
            n--;
            int k = Random.Range(0, n);
            GameObject card = m_deck[k];
            m_deck[k] = m_deck[n];
            m_deck[n] = card;
        }

    }

    public void Draw(int num)
    {
        if (m_deck.Count == 0)
        {
            Shuffle();
        }
        else if (m_deck.Count > 0)
        {
            for (int i = 0; i < num; i++)
            {
                m_hand.Add(m_deck[0]);
                m_deck[0].transform.position = Hand.transform.position;
                m_deck.Remove(m_deck[0]);
            }
        }
    }

    public void Discard()
    {
        foreach (GameObject card in m_hand)
        {
            card.transform.position = Grave.transform.position;
            m_grave.Add(card);
        }
        m_hand.Clear();
    }

    public void HandUse(GameObject card)
    {
        //loader가 처리
        // loader index = card.m_CardIndex;

        m_hand.Remove(card);
        m_grave.Add(card);
    }
}