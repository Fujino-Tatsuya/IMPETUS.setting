using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

//카드 정보 불러오는 애 m_cardLoader

public class CardManager : MonoBehaviour
{
    #region
    [Header("Hand Layout Settings")]
    public float radius = 0;             // 아치의 반지름
    public float maxAngle = 0;            // 전체 카드가 차지할 최대 각도 (도)
    public float minAngle = 0; // 카드가 적을 때 최소 각도
    public Vector3 handCenter;  // 중심 위치 (월드 좌표)
    public int idealCardCount = 0;

    [Header("Animation Settings")]
    public float lerpSpeed = 0;           // 부드러운 이동 속도


    public static CardManager instance;
    public GameObject cardPrefab;
    public GameObject cardCanvas;

    public GameObject Deck;
    public GameObject Hand;
    public GameObject Grave;

    private GameObject currentCard;
    private int currentCardIndex;

    public Sprite[] sprites;

    public RectTransform ActiveHandRect;

    private int deckCount = 0;
    private int graveCount = 0;
    private int currentHoverIndex = 0;
    private int lastHoverIndex = 0;
    private int prevSiblingIndex = 0;

    [SerializeField] GraphicRaycaster raycaster;
    PointerEventData pointerData = new PointerEventData(EventSystem.current);
    List<RaycastResult> results = new List<RaycastResult>();
    public GameObject canvas;

    [Header("Discard Animation Curve")]
    public AnimationCurve discardCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);
    public float elapsed = 0f;
    public float duration = 1f;

    bool isDrag = false;
    #endregion



    private void Awake()
    {
        #region singleton
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(instance);
        }
        else
        {
            Destroy(instance);
        }
        #endregion
        M_InitDeck(10);
        radius = 4000f;
        maxAngle = 15f;
        minAngle = 0.5f;

        handCenter = new Vector3(0, -4640f, 0);
        idealCardCount = 5;

        ActiveHandRect = GameObject.FindWithTag("ActiveHand").GetComponent<RectTransform>();

        pointerData.position = Input.mousePosition;
        EventSystem.current.RaycastAll(pointerData, results);

        canvas = GameObject.Find("Canvas");
        raycaster = canvas.GetComponent<GraphicRaycaster>();
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Draw(1);
        }
        if (Input.GetKeyDown(KeyCode.LeftShift))
        {
            Discard();
        }

        if (!isDrag)
        {
            ViewMaximum(m_hand);
            ArrangeHand();
        }
        UpdateCount();
    }

    private List<GameObject> m_deck = new List<GameObject>();
    private List<GameObject> m_grave = new List<GameObject>();
    private List<GameObject> m_hand = new List<GameObject>();

    //로드된 정보들
    // 파싱을 해서 전체 카드 배열이 존재해야되는거 아니냐
    void M_InitDeck(int InitCard)
    {
        int m_CardCount = InitCard;
        for (int i = 0; i < m_CardCount; i++)
        {
            GameObject gameObject = Instantiate(cardPrefab, cardCanvas.transform);
            gameObject.GetComponent<RectTransform>().anchoredPosition = Deck.GetComponent<RectTransform>().anchoredPosition;
            Card card = gameObject.GetComponent<Card>();
            Image image = card.GetComponent<Image>();
            image.sprite = sprites[i];
            card.m_CardIndex = i;
            m_deck.Add(gameObject);
            deckCount++;
            gameObject.SetActive(false);
        }
    }
    void ArrangeHand()
    {
        int count = m_hand.Count;
        if (count == 0) return;

        float dynamicAngle = (count <= 1) ? 0f : Mathf.Lerp(minAngle, maxAngle, Mathf.Clamp01((float)(count - 1) / (idealCardCount - 1)));

        for (int i = 0; i < count; i++)
        {
            float t = (count == 1) ? 0.5f : (float)i / (count - 1); // 카드 1장일 때는 중앙
            float angle = Mathf.Lerp(-dynamicAngle / 2f, dynamicAngle / 2f, t);
            float rad = angle * Mathf.Deg2Rad;

            Vector3 targetPos = handCenter + new Vector3(Mathf.Sin(rad), Mathf.Cos(rad), 0) * radius;

            float targetRot = angle;

            RectTransform rect = m_hand[i].GetComponent<RectTransform>();

            // 부드러운 이동 & 회전
            rect.anchoredPosition = Vector2.Lerp(rect.anchoredPosition, targetPos, Time.deltaTime * lerpSpeed);
            Quaternion rot = Quaternion.Euler(0, 0, -targetRot);
            rect.localRotation = Quaternion.Slerp(rect.localRotation, rot, Time.deltaTime * lerpSpeed);

            if (i == currentHoverIndex)
            {
                continue;
            }

            // UI 기준 Z정렬 (앞으로)
            m_hand[i].transform.SetSiblingIndex(i);
        }
    }

    void StartDiscardAnim(GameObject card)
    {
        
    }

    IEnumerator DiscardAnim(GameObject card)
    {
        elapsed = 0;

        RectTransform rect = card.GetComponent<RectTransform>();
        Vector2 startPos = rect.anchoredPosition;
        Vector2 targetPos = Grave.GetComponent<RectTransform>().anchoredPosition;
        

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / duration);
            float curveT = discardCurve.Evaluate(t);
            rect.anchoredPosition = Vector2.Lerp(startPos, targetPos, curveT);

            yield return null;
        }

        rect.anchoredPosition = targetPos;
    }

    public void Shuffle()
    {
        int i = 0;
        foreach (GameObject gameObject in m_grave)
        {
            graveCount--;
            deckCount++;
            m_deck.Add(gameObject);
            RectTransform rect = m_grave[i++].GetComponent<RectTransform>();
            RectTransform deck = Deck.GetComponent<RectTransform>();
            rect.anchoredPosition = deck.anchoredPosition;
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
        if (m_deck.Count == 0 && m_grave.Count > 0)
        {
            Shuffle();
            Draw(num);
        }
        else if (m_deck.Count > 0)
        {
            for (int i = 0; i < num; i++)
            {
                m_hand.Add(m_deck[0]);
                m_deck[0].SetActive(true);
                RectTransform rect = m_deck[0].GetComponent<RectTransform>();
                RectTransform hand = Hand.GetComponent<RectTransform>();
                rect.anchoredPosition = hand.anchoredPosition;
                deckCount--;
                m_deck.Remove(m_deck[0]);
            }
        }
    }
    public void Discard()
    {

        foreach (GameObject card in m_hand)
        { 
            m_grave.Add(card);
            graveCount++;
            StartWaitAnim(card);
        }
        m_hand.Clear();
    }
    public void HandUse(GameObject card)
    {
        // loader가 처리
        // loader index = card.m_CardIndex;
        m_hand.Remove(card);
        m_grave.Add(card);
        graveCount++;
    }
    void UpdateCount()
    {
        TextMeshProUGUI deckText = Deck.GetComponentInChildren<TextMeshProUGUI>();
        if (deckText != null)
        {
            deckText.text = deckCount.ToString();
        }

        TextMeshProUGUI graveText = Grave.GetComponentInChildren<TextMeshProUGUI>();
        if (graveText != null)
        {
            graveText.text = graveCount.ToString();
        }
    }
    void ViewMaximum(List<GameObject> cards)
    {
        // 1.포인터 이벤트 설정
        pointerData = new PointerEventData(EventSystem.current);
        pointerData.position = Input.mousePosition;

        // 2. 결과 리스트 초기화
        results.Clear();

        // 3. 레이캐스트 실행
        if (raycaster != null)
        {
            raycaster.Raycast(pointerData, results);
            bool found = false;

            // 4. 감지 처리
            foreach (RaycastResult result in results)
            {
                if (result.gameObject.CompareTag("Card"))
                {
                    int i = 0;
                    foreach (GameObject card in m_hand)
                    {
                        if (card.GetComponent<Card>().m_CardIndex == result.gameObject.GetComponent<Card>().m_CardIndex)
                        {
                            currentHoverIndex = i;
                            found = true;
                            break;
                        }
                        i++;
                    }
                    if (found)
                    {
                        break;
                    }
                }
            }

            handCenter = found ? new Vector3(0, -4350f, 0) : new Vector3(0, -4590f, 0);

            if (!found)
            {
                currentHoverIndex = -1;
            }

            if (currentHoverIndex != lastHoverIndex)
            {
                if (lastHoverIndex != -1 && lastHoverIndex < m_hand.Count)
                {
                    m_hand[lastHoverIndex].transform.localScale = Vector3.one;
                    m_hand[lastHoverIndex].GetComponent<RectTransform>().anchoredPosition *= new Vector2(1, 1.5f);
                    m_hand[lastHoverIndex].transform.SetSiblingIndex(prevSiblingIndex);
                }

                if (currentHoverIndex != -1 && currentHoverIndex < m_hand.Count)
                {
                    m_hand[currentHoverIndex].transform.localScale = new Vector3(1.5f, 1.5f, 1.5f);
                    m_hand[currentHoverIndex].GetComponent<RectTransform>().anchoredPosition *= new Vector2(1, 0.66f);
                    prevSiblingIndex = m_hand[currentHoverIndex].transform.GetSiblingIndex();
                    m_hand[currentHoverIndex].transform.SetAsLastSibling();
                }

                lastHoverIndex = currentHoverIndex;
            }
        }
    }

    public void OnBeginDragEvent(GameObject card)
    {
        isDrag = true;
        card.GetComponent<CanvasGroup>().blocksRaycasts = false; // 드래그 중엔 다른 UI가 Raycast를 받을 수 있도록 비활성화
    }

    public void OnDragEvent(GameObject card)
    {
        // 드래그 중 위치 갱신
        Vector2 mousePos = Input.mousePosition;
        mousePos.x -= 960f;
        mousePos.y -= 540f;
        card.GetComponent<RectTransform>().anchoredPosition = mousePos;
    }

    public void OnEndDragEvent(GameObject card)
    {
        isDrag = false;
        if (lastHoverIndex != -1 && lastHoverIndex < m_hand.Count)
        {
            m_hand[lastHoverIndex].transform.localScale = Vector3.one;
            m_hand[lastHoverIndex].GetComponent<RectTransform>().anchoredPosition *= new Vector2(1, 1.5f);
            m_hand[lastHoverIndex].transform.SetSiblingIndex(prevSiblingIndex);
        }
        card.GetComponent<CanvasGroup>().blocksRaycasts = true;
        RectTransform rectTransform = ActiveHandRect.GetComponent<RectTransform>();
        Vector2 mousePos = Input.mousePosition;
        if (RectTransformUtility.RectangleContainsScreenPoint(rectTransform, mousePos))
        {
            HandUse(card);
            StartWaitAnim(card);
        }
    }

    IEnumerator CoWaitAnim(GameObject card)
    {
        yield return StartCoroutine(DiscardAnim(card));
        card.SetActive(false);
    }

    private void StartWaitAnim(GameObject card)
    {
        StartCoroutine(CoWaitAnim(card));
    }
}

//들고 드래그 시 사용