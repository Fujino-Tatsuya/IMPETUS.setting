using System.Collections;
using System.Collections.Generic;
using Unity.IO.LowLevel.Unsafe;
using UnityEngine;

public class InputManager : MonoBehaviour
{
    public static InputManager instance;
    public static EffectManager effect;
    public static BossPatternManager bosspattern;

    private void Awake() // ���Ѱܵ� ���ϼ��� �����Ǵ°���
                         // ���� �Ŵ����� 
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

    public Piece piece;
    public bool isPlace = false;

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            MouseRay();
        }
        if (Input.GetMouseButton(0))
        {
            PreView();
        }
        if (Input.GetMouseButtonUp(0))
        {
            if (isPlace)
                Place();
            else
                Move();
        }
    }

    void MouseRay()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit[] hits = Physics.RaycastAll(ray);

        if (piece != null)
        {

            var moves = BoardManager.instance.GetMoves();
            EffectManager.instance.ShowPlaceEffects(moves);


            return;
        }
        foreach (var hit in hits)
        {
            if (hit.collider.CompareTag("Piece"))
            {
                Piece selected = hit.collider.GetComponent<Piece>();

                // ���� �⹰�� �ٽ� Ŭ���� ��� �� ����
                if (selected == piece)
                    return;

                piece = selected;

                // �̵� ��� ��� �� ����Ʈ ǥ��
                if (isPlace)
                {
                    var moves = BoardManager.instance.GetMoves();
                    EffectManager.instance.ShowPlaceEffects(moves);

                }
                else
                {
                    var moves = MovementManager.instance.GetMoves(piece);
                    EffectManager.instance.ShowMoveEffects(moves);
                }
                return;
            }
        }

        // Ŭ���� ����� �⹰�� �ƴϸ� ���� ���� + ����Ʈ ����
        piece = null;
        EffectManager.instance.ClearEffects();
    }
    //void MouseRay()
    //{

    //    Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
    //    RaycastHit[] hits = Physics.RaycastAll(ray);

    //    int x = -1, y = -1;

    //    foreach (RaycastHit hit in hits)
    //    {
    //        if(hit.collider.tag == "Piece")
    //        {
    //            Piece piece = hit.collider.GetComponent<Piece>();
    //            if (x != -1 && y != -1)
    //            {
    //                if (piece.y > y)
    //                    continue;
    //                if ((piece.x <= 3 && x > piece.x) || (piece.x >= 3 && x < piece.x))
    //                    continue;
    //            }
    //            x = piece.x;
    //            y = piece.y;
    //            this.piece = piece;
    //        }
    //    }
    //    if (y == -1 && x == -1)
    //    {
    //        piece = null;
    //        return;
    //    }

    //    Debug.Log(x.ToString() + ' ' +  y.ToString());
    //    Debug.DrawRay(ray.origin, ray.direction, Color.red);
    //    //Input.mousePosition
    //}

    void Move()
    {
        if (piece == null) return;

        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit[] hits = Physics.RaycastAll(ray);

        int x = -1, y = -1;
        Node lastNode = null;
        bool blocked = false;

        foreach (RaycastHit hit in hits)
        {
            if (!hit.collider.CompareTag("Node")) continue;

            Node node = hit.collider.GetComponent<Node>();

            /* ���� ���� ���� --------------------------------------------------- */
            if (x != -1 && y != -1)
            {
                if (node.GridPos.y > y) continue;
                if ((node.GridPos.x <= 3 && x > node.GridPos.x) ||
                    (node.GridPos.x >= 3 && x < node.GridPos.x))
                    continue;
            }

            /* ��ֹ� / �Ʊ� ���� ---------------------------------------------- */
            if (BoardManager.instance.IsBlocked(node.GridPos))
            {
                blocked = true;
                x = node.GridPos.x;
                y = node.GridPos.y;
                continue; // �������� �� ���� �� ��
            }

            /* �� ĭ �߰� => �ĺ��� ���� (������ ���� piece ��ǥ�� �ǵ帮�� ����) */
            blocked = false;
            x = node.GridPos.x;
            y = node.GridPos.y;
            lastNode = node;
        }

        /* ---------------------------------------------------------------------- */
        /* �̵� ���ɼ� ���� Ȯ�� + ��ǥ/��� ��ü                                */
        /* ---------------------------------------------------------------------- */
        if (lastNode != null && !blocked)
        {
            var moves = MovementManager.instance.GetMoves(piece);
            if (moves.Contains(lastNode))          // �չ� �̵�
            {
                /* ��� ��ũ ��ü */
                if (piece.node != null)
                    piece.node.currentPiece = null;

                lastNode.currentPiece = piece.gameObject;
                piece.node = lastNode;

                /* �̶� ��ǥ�� Ȯ�������� ���� */
                piece.x = lastNode.GridPos.x;
                piece.y = lastNode.GridPos.y;



            }
        }


        /* ���� �Ǵ� ���� */
        piece.RePosition();
        piece = null;

        MovementManager.instance.InvalidateAll();
        EffectManager.instance.ClearEffects();
        Debug.Log($"last = ({x},{y})");
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
            else
                piece.gameObject.transform.position = new Vector3(0, 0, -100);
        }
    }

    void Place()
    {
        if (piece == null) return;

        //isPlace = false;

        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit[] hits = Physics.RaycastAll(ray);

        int x = -1, y = -1;
        Node lastNode = null;
        bool blocked = false;

        foreach (RaycastHit hit in hits)
        {
            if (!hit.collider.CompareTag("Node")) continue;

            Node node = hit.collider.GetComponent<Node>();

            /* ���� ���� ���� --------------------------------------------------- */
            if (x != -1 && y != -1)
            {
                if (node.GridPos.y > y) continue;
                if ((node.GridPos.x <= 3 && x > node.GridPos.x) ||
                    (node.GridPos.x >= 3 && x < node.GridPos.x))
                    continue;
            }

            /* ��ֹ� / �Ʊ� ���� ---------------------------------------------- */
            if (BoardManager.instance.IsBlocked(node.GridPos))
            {
                blocked = true;
            }
            else
                blocked = false;
            /* �� ĭ �߰� => �ĺ��� ���� (������ ���� piece ��ǥ�� �ǵ帮�� ����) */
            x = node.GridPos.x;
            y = node.GridPos.y;
            lastNode = node;
        }

        /* ---------------------------------------------------------------------- */
        /* �̵� ���ɼ� ���� Ȯ�� + ��ǥ/��� ��ü                                */
        /* ---------------------------------------------------------------------- */
        if (lastNode != null && !blocked)
        {
            List<Node> moves = BoardManager.instance.GetMoves();
            if (moves.Contains(lastNode))          // �չ� �̵�
            {
                /* ��� ��ũ ��ü */
                if (piece.node != null)
                    piece.node.currentPiece = null;

                lastNode.currentPiece = piece.gameObject;
                piece.node = lastNode;

                /* �̶� ��ǥ�� Ȯ�������� ���� */
                piece.x = lastNode.GridPos.x;
                piece.y = lastNode.GridPos.y;
            }
            else
            {
                PieceManager.instance.SetCount(piece.pieceVariant, piece.level, PieceManager.instance.GetCount(piece.pieceVariant, piece.level) + 1);
                Destroy(piece.gameObject);
            }
        }
        else if (lastNode != null && blocked)
        {
            Debug.Log("�ߵ�");
            if (piece.node != lastNode)
            {
                List<Node> moves = BoardManager.instance.GetMoves();
                if (moves.Contains(lastNode))          // �չ� �̵�
                {
                    if (piece.node == null)
                    {
                        Piece currentPieceScript = lastNode.currentPiece.GetComponent<Piece>();
                        PieceManager.instance.SetCount(currentPieceScript.pieceVariant, currentPieceScript.level, PieceManager.instance.GetCount(currentPieceScript.pieceVariant, currentPieceScript.level) + 1);
                        Destroy(currentPieceScript.gameObject);

                        if (piece.node != null)
                            piece.node.currentPiece = null;

                        lastNode.currentPiece = piece.gameObject;
                        piece.node = lastNode;

                        /* �̶� ��ǥ�� Ȯ�������� ���� */
                        piece.x = lastNode.GridPos.x;
                        piece.y = lastNode.GridPos.y;
                    }
                    else
                    {
                        Piece currentPieceScript = lastNode.currentPiece.GetComponent<Piece>();
                        var temp = piece.node.currentPiece;

                        piece.node.currentPiece = currentPieceScript.gameObject;
                        currentPieceScript.node = piece.node;

                        currentPieceScript.x = piece.x;
                        currentPieceScript.y = piece.y;
                        currentPieceScript.RePosition();

                        piece.node = lastNode;
                        lastNode.currentPiece = piece.gameObject;

                        /* �̶� ��ǥ�� Ȯ�������� ���� */


                        piece.x = lastNode.GridPos.x;
                        piece.y = lastNode.GridPos.y;
                    }
                }
                else
                {
                    PieceManager.instance.SetCount(piece.pieceVariant, piece.level, PieceManager.instance.GetCount(piece.pieceVariant, piece.level) + 1);
                    Destroy(piece.gameObject);
                }
            }
        }
        else
        {
            PieceManager.instance.SetCount(piece.pieceVariant, piece.level, PieceManager.instance.GetCount(piece.pieceVariant, piece.level) + 1);
            Destroy(piece.gameObject);
        }

        /* ���� �Ǵ� ���� */
        piece.RePosition();
        piece = null;

        MovementManager.instance.InvalidateAll();
        EffectManager.instance.ClearEffects();
        Debug.Log($"last = ({x},{y})");
        Debug.DrawRay(ray.origin, ray.direction, Color.red);

    }

    void PlaceInput()
    {

    }
}
