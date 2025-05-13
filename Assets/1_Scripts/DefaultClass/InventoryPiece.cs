using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using static UnityEngine.Rendering.DebugUI;

public class InventoryPiece : MonoBehaviour, IPointerDownHandler, IPointerClickHandler, IPointerUpHandler
{
    public PieceVariant pieceVariant;
    public GameObject piecePrefab;

    public GameObject phonePrefab;
    //public GameObject
    // Start is called before the first frame update
    void Start()
    {
        
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
            piecePrefab = Instantiate(phonePrefab, new Vector3(0, 0, 0), transform.rotation);
            InputManager.instance.piece = piecePrefab.GetComponent<Piece>();

            InputManager.instance.isPlace = true;
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        GameObject clickedObject = eventData.pointerCurrentRaycast.gameObject;

        if (clickedObject == gameObject)
        {
            Debug.Log("자식이 클릭됨: " + clickedObject.name);
        }
        else
        {
            Debug.Log("자식 또는 다른 오브젝트 클릭됨: " + clickedObject.name);
        }
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        //InputManager.instance.isPlace = false;
    }
}
