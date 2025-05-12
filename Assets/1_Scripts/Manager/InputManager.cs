using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class InputManager : MonoBehaviour
{
    public static InputManager instance;

    private void Awake() // 씬넘겨도 유일성이 보존되는거지
                         // 보드 매니저는 
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
    }

    private Piece piece;

    // Update is called once per frame
    void Update()
    {
        if(Input.GetMouseButtonDown(0))
        {
            MouseRay();
        }
        if(Input.GetMouseButton(0))
        {
            PreView();
        }
        if (Input.GetMouseButtonUp(0))
        {
            Move();
        }
    }

    void MouseRay()
    {

        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit[] hits = Physics.RaycastAll(ray);

        int x = -1, y = -1;

        foreach (RaycastHit hit in hits)
        {
            if(hit.collider.tag == "Piece")
            {
                Piece piece = hit.collider.GetComponent<Piece>();
                if (x != -1 && y != -1)
                {
                    if (piece.y > y)
                        continue;
                    if ((piece.x <= 3 && x > piece.x) || (piece.x >= 3 && x < piece.x))
                        continue;
                }
                x = piece.x;
                y = piece.y;
                this.piece = piece;
            }
        }
        if (y == -1 && x == -1)
        {
            piece = null;
            return;
        }

        Debug.Log(x.ToString() + ' ' +  y.ToString());
        Debug.DrawRay(ray.origin, ray.direction, Color.red);
        //Input.mousePosition
    }

    void Move()
    {
        if (piece == null)
            return;

        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit[] hits = Physics.RaycastAll(ray);

        int x = -1, y = -1;

        foreach (RaycastHit hit in hits)
        {
            if (hit.collider.tag == "Node")
            {
                Node node = hit.collider.GetComponent<Node>();

                if (!BoardManager.instance.IsAble(node.GridPos.x, node.GridPos.y))
                    continue;
                if (x != -1 && y != -1)
                {
                    if (node.GridPos.y > y)
                        continue;
                    if ((node.GridPos.x <= 3 && x > node.GridPos.x) || (node.GridPos.x >= 3 && x < node.GridPos.x))
                        continue;
                }
                x = node.GridPos.x;
                y = node.GridPos.y;
                piece.x = x;
                piece.y = y;
            }
        }

        piece.RePosition();
        piece = null;

        Debug.Log(x.ToString() + ' ' + y.ToString());
        Debug.DrawRay(ray.origin, ray.direction, Color.red);
    }

    void PreView()
    {
        if (piece == null)
            return;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        if (Mathf.Abs(ray.direction.y) > 0.0001f)
        {
            float t = -ray.origin.y / ray.direction.y;
            if (t >= 0)
            {
                Vector3 groundPoint = ray.origin + ray.direction * t;
                piece.gameObject.transform.position = new Vector3(groundPoint.x, 0, groundPoint.z);
            }
        }
    }
}
