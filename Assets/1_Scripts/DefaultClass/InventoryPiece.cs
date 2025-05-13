using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine.UI;
using UnityEngine;
using UnityEngine.EventSystems;
using static UnityEngine.Rendering.DebugUI;

public class InventoryPiece : MonoBehaviour, IPointerDownHandler, IPointerClickHandler, IPointerUpHandler
{
    public PieceVariant pieceVariant;
    public bool upgrade;
    public GameObject piecePrefab;

    public GameObject phonePrefab;

    public GameObject numberObject;
    Image numberImage;

    Image myImage;

    bool selected = false;

    private Sprite[] sprites;

    //public GameObject
    // Start is called before the first frame update
    void Start()
    {
        numberObject = transform.GetChild(0).gameObject;

        myImage = GetComponent<Image>();
        numberImage = numberObject.GetComponent<Image>();
        sprites = new Sprite[10];
        for(int i = 0; i < 10; i++)
        {
            sprites[i] = Resources.Load<Sprite>("TestImage/TestNumber/" + i.ToString());

        }
        SpriteCheck();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        GameObject clickedObject = eventData.pointerCurrentRaycast.gameObject;

        if (clickedObject == gameObject)
        {
            if (PieceManager.instance.GetCount(pieceVariant, upgrade) == 0)
                return;
            piecePrefab = Instantiate(phonePrefab, new Vector3(0, 0, 0), transform.rotation);
            InputManager.instance.piece = piecePrefab.GetComponent<Piece>();

            InputManager.instance.isPlace = true;

            myImage.color = new Color(0.7f, 0.7f, 0.7f, 1);
            selected = true;
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        //GameObject clickedObject = eventData.pointerCurrentRaycast.gameObject;

        //if (clickedObject == gameObject)
        //{
        //    Debug.Log("자식이 클릭됨: " + clickedObject.name);
        //}
        //else
        //{
        //    Debug.Log("자식 또는 다른 오브젝트 클릭됨: " + clickedObject.name);
        //}
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (!selected)
            return;
        selected = false;
        //InputManager.instance.isPlace = false;
        myImage.color = new Color(1, 1, 1, 1);
        PieceManager.instance.SetCount(pieceVariant, upgrade, PieceManager.instance.GetCount(pieceVariant, upgrade) - 1);
        Invoke("SpriteCheck", 0.05f);
    }

    void SpriteCheck()
    {
        numberImage.sprite = sprites[PieceManager.instance.GetCount(pieceVariant, upgrade)];
        if(PieceManager.instance.GetCount(pieceVariant, upgrade) == 0)
            myImage.color = new Color(0f, 0f, 0f, 0.5f);
    }
}
