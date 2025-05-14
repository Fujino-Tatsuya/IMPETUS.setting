using System.Collections;
using System.Collections.Generic;
using Unity.IO.LowLevel.Unsafe;
using UnityEngine;

public class InputManager : MonoBehaviour
{
    public static InputManager instance;
    public static EffectManager effect;
    public static BossPatternManager bosspattern;

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

                // 같은 기물을 다시 클릭한 경우 → 무시
                if (selected == piece)
                    return;

                piece = selected;

                // 이동 노드 계산 후 이펙트 표시
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

        // 클릭한 대상이 기물이 아니면 선택 해제 + 이펙트 제거
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

            /* 기존 필터 로직 --------------------------------------------------- */
            if (x != -1 && y != -1)
            {
                if (node.GridPos.y > y) continue;
                if ((node.GridPos.x <= 3 && x > node.GridPos.x) ||
                    (node.GridPos.x >= 3 && x < node.GridPos.x))
                    continue;
            }

            /* 장애물 / 아군 차단 ---------------------------------------------- */
            if (BoardManager.instance.IsBlocked(node.GridPos))
            {
                blocked = true;
                x = node.GridPos.x;
                y = node.GridPos.y;
                continue; // 막혔으면 더 진행 안 함
            }

            /* 빈 칸 발견 => 후보로 저장 (하지만 아직 piece 좌표는 건드리지 않음) */
            blocked = false;
            x = node.GridPos.x;
            y = node.GridPos.y;
            lastNode = node;
        }

        /* ---------------------------------------------------------------------- */
        /* 이동 가능성 최종 확인 + 좌표/노드 교체                                */
        /* ---------------------------------------------------------------------- */
        if (lastNode != null && !blocked)
        {
            var moves = MovementManager.instance.GetMoves(piece);
            if (moves.Contains(lastNode))          // 합법 이동
            {
                /* 노드 링크 교체 */
                if (piece.node != null)
                    piece.node.currentPiece = null;

                lastNode.currentPiece = piece.gameObject;
                piece.node = lastNode;

                /* 이때 좌표를 확정적으로 갱신 */
                piece.x = lastNode.GridPos.x;
                piece.y = lastNode.GridPos.y;



            }
        }


        /* 스냅 또는 원복 */
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

            /* 기존 필터 로직 --------------------------------------------------- */
            if (x != -1 && y != -1)
            {
                if (node.GridPos.y > y) continue;
                if ((node.GridPos.x <= 3 && x > node.GridPos.x) ||
                    (node.GridPos.x >= 3 && x < node.GridPos.x))
                    continue;
            }

            /* 장애물 / 아군 차단 ---------------------------------------------- */
            if (BoardManager.instance.IsBlocked(node.GridPos))
            {
                blocked = true;
            }
            else
                blocked = false;
            /* 빈 칸 발견 => 후보로 저장 (하지만 아직 piece 좌표는 건드리지 않음) */
            x = node.GridPos.x;
            y = node.GridPos.y;
            lastNode = node;
        }

        /* ---------------------------------------------------------------------- */
        /* 이동 가능성 최종 확인 + 좌표/노드 교체                                */
        /* ---------------------------------------------------------------------- */
        if (lastNode != null && !blocked)
        {
            List<Node> moves = BoardManager.instance.GetMoves();
            if (moves.Contains(lastNode))          // 합법 이동
            {
                /* 노드 링크 교체 */
                if (piece.node != null)
                    piece.node.currentPiece = null;

                lastNode.currentPiece = piece.gameObject;
                piece.node = lastNode;

                /* 이때 좌표를 확정적으로 갱신 */
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
            Debug.Log("잘됨");
            if (piece.node != lastNode)
            {
                List<Node> moves = BoardManager.instance.GetMoves();
                if (moves.Contains(lastNode))          // 합법 이동
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

                        /* 이때 좌표를 확정적으로 갱신 */
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

                        /* 이때 좌표를 확정적으로 갱신 */


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

        /* 스냅 또는 원복 */
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
