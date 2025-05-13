using System.Collections;
using System.Collections.Generic;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine;



//카드 정보 불러오는 애 m_cardLoader

public class CardManager : MonoBehaviour
{
    public static CardManager instance;
    public GameObject cardPrefab;
<<<<<<< Updated upstream

    public GameObject Deck;
    public GameObject Hand;
    public GameObject Grave;


=======
>>>>>>> Stashed changes
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

<<<<<<< Updated upstream
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


=======
>>>>>>> Stashed changes
    void M_InitDeck(int InitCard)
    {
        int m_CardCount = InitCard;
        for (int i = 0; i < m_CardCount; i++)
        {
<<<<<<< Updated upstream
            GameObject gameObject = Instantiate(cardPrefab, Deck.transform);
            Card card = gameObject.GetComponent<Card>();
=======
>>>>>>> Stashed changes
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
<<<<<<< Updated upstream
        int i = 0;
        foreach (GameObject gameObject in m_grave)
        {
            m_deck.Add(gameObject);
            m_grave[i++].transform.position = Deck.transform.position;
=======
>>>>>>> Stashed changes
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
<<<<<<< Updated upstream
        foreach (GameObject card in m_hand)
=======
>>>>>>> Stashed changes
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
<<<<<<< Updated upstream
}
=======

    //정렬 - 사용이나 드로우 시
    //public List<Transform> cards = new List<Transform>();

    //[Header("Hand Layout Settings")]
    //public float radius = 300f;             // 아치의 반지름
    //public float maxAngle = 30f;            // 전체 카드가 차지할 최대 각도 (도)
    //public Vector3 handCenter = new Vector3(0, -200f, 0);  // 중심 위치 (월드 좌표)

    //[Header("Animation Settings")]
    //public float lerpSpeed = 10f;           // 부드러운 이동 속도

    //void Update()
    //{
    //    ArrangeHand();
    //}

    //void ArrangeHand()
    //{
    //    int count = cards.Count;
    //    if (count == 0) return;

    //    for (int i = 0; i < count; i++)
    //    {
    //        float t = (count == 1) ? 0.5f : (float)i / (count - 1); // 카드 1장일 때는 중앙
    //        float angle = Mathf.Lerp(-maxAngle / 2f, maxAngle / 2f, t);
    //        float rad = angle * Mathf.Deg2Rad;

    //        Vector3 targetPos = handCenter + new Vector3(Mathf.Sin(rad), Mathf.Cos(rad), 0) * radius;
    //        float targetRot = angle;

    //        // 부드러운 이동 & 회전
    //        cards[i].position = Vector3.Lerp(cards[i].position, targetPos, Time.deltaTime * lerpSpeed);
    //        Quaternion rot = Quaternion.Euler(0, 0, targetRot);
    //        cards[i].rotation = Quaternion.Slerp(cards[i].rotation, rot, Time.deltaTime * lerpSpeed);

    //        // 선택 시를 대비해 Z 정렬 (선택 카드만 앞으로)
    //        cards[i].SetSiblingIndex(i);
    //    }
    //}

    //트리거 마우스 레이캐스트 카드랑 될때 올리기
    //나가면 내리기
    //들고 드래그 시 사용
    //위에 오버 시 확대 카드 중심 - 마우스 위치
}
>>>>>>> Stashed changes
