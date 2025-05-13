using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using static UnityEngine.Rendering.DebugUI;

public class ImageClickHandler : MonoBehaviour, IPointerDownHandler, IPointerClickHandler, IPointerUpHandler
{
    public GameObject childGameObject;

    RectTransform rectTransform;
    Image myImage;
    Image childImage;

    float left;
    float right;

    public Vector3 targetPosition = new Vector3(-450, 0, 0); // ��ǥ ��ġ
    private float moveSpeed = 200.0f;       // �̵� �ӵ�
    private float bounceHeight = 3f;    // Ƣ�� ����
    private float bounceDamping = 0;   // Ƣ�� ���� ���

    private bool reachedTarget = true;
    private float bounceVelocity = 0.0f;
    private float currentHeight = 0.0f;

    private void Start()
    {
        rectTransform = GetComponent<RectTransform>();
        myImage = GetComponent<Image>();
        childImage = childGameObject.GetComponent<Image>();
        right = -myImage.rectTransform.rect.width/2;
        left = right - childImage.rectTransform.rect.width;
        //bounceVelocity = bounceHeight / 3;
    }

    void Update()
    {
        if (!reachedTarget)
        {
            // �̵� ��
            moveSpeed += Time.deltaTime*1500;
            rectTransform.anchoredPosition = Vector3.MoveTowards(rectTransform.anchoredPosition, targetPosition, (moveSpeed * Time.deltaTime));

            // ���� ���� Ȯ��
            if (targetPosition.x == left)
            {
                if (rectTransform.anchoredPosition.x <= targetPosition.x)
                {
                    reachedTarget = true;
                    moveSpeed = moveSpeed/700;
                    bounceDamping = Mathf.Floor(moveSpeed * 100) / 100f;
                    bounceHeight = bounceDamping;
                }
            }
            else
            {
                if (rectTransform.anchoredPosition.x >= targetPosition.x)
                {
                    reachedTarget = true;
                    moveSpeed = moveSpeed / 700;
                    bounceDamping = Mathf.Floor(moveSpeed * 100) / 100f;
                    bounceHeight = bounceDamping;
                }
            }
        }
        else if (bounceDamping != 0)
        {
            // ���� Ƣ�� �� (Y ���⸸)
            currentHeight += bounceVelocity;
            bounceVelocity -= 9f * Time.deltaTime; // �߷� ȿ��

            if (currentHeight < 0)
            {
                currentHeight = 0;
                bounceDamping -= bounceHeight/3 + 0.05f;
                bounceVelocity = bounceDamping; // Ʀ + ����
                Debug.Log(bounceVelocity);
            }

            // ����
            Vector3 bouncePos;
            if(targetPosition.x == left)
                bouncePos = targetPosition + Vector3.right * currentHeight;
            else
                bouncePos = targetPosition + Vector3.left * currentHeight;
            rectTransform.anchoredPosition = bouncePos;

            if (bounceDamping <= 0)
                bounceDamping = 0;
        }
    }
    public void OnPointerDown(PointerEventData eventData)
    {
        GameObject clickedObject = eventData.pointerCurrentRaycast.gameObject;

        if (clickedObject == gameObject)
        {
            Debug.Log("Pointer Down!");
            // ���⿡ ������ �� ���� �߰�
            myImage.color = new Color(0.7f, 0.7f, 0.7f, 1);
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        GameObject clickedObject = eventData.pointerCurrentRaycast.gameObject;

        if (clickedObject == gameObject)
        {
            Debug.Log("�θ� �ڽ��� Ŭ����");

            Debug.Log("Pointer Click!");
            // ���⿡ Ŭ������ �� ���� �߰�
            reachedTarget = false;
            if (targetPosition.x == left)
                targetPosition.x = right;
            else
                targetPosition.x = left;
            moveSpeed = 200;
        }
        else
        {
            Debug.Log("�ڽ� �Ǵ� �ٸ� ������Ʈ Ŭ����: " + clickedObject.name);
        }
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        Debug.Log("Pointer Up!");
        // ���⿡ ���� ���� �� ���� �߰�
        myImage.color = new Color(1, 1, 1, 1);
        

    }
}
