using System.Collections.Generic;
using UnityEngine;

/// <summary>보드 위 모든 기물의 이동 가능 영역을 계산·캐싱한다.</summary>
public class MovementManager : MonoBehaviour
{
    public static MovementManager instance;

    //   Piece → 이동 가능 노드 목록  (한 턴마다 갱신)
    private readonly Dictionary<Piece, List<Node>> cache = new();

    void Awake()
    {
        if (instance == null) instance = this;
        else { Destroy(gameObject); return; }
        DontDestroyOnLoad(gameObject);
    }

    /* -------------------------------------------------------------------------- */
    /* 메인 API                                                                    */
    /* -------------------------------------------------------------------------- */

    public List<Node> GetMoves(Piece piece)
    {
        if (!cache.ContainsKey(piece))
            cache[piece] = CalcMoves(piece);

        return cache[piece];
    }


    public void InvalidateAll() => cache.Clear();

    /* -------------------------------------------------------------------------- */
    /* 내부 : 이동 규칙                                                            */
    /* -------------------------------------------------------------------------- */

    private List<Node> CalcMoves(Piece p)
    {
        switch (p.pieceVariant)                     //  switch-case 핵심
        {
            case PieceVariant.Rook: return RayMoves(p, rookDirs);
            case PieceVariant.Bishop: return RayMoves(p, bishopDirs);
            case PieceVariant.Queen: return RayMoves(p, queenDirs);
            case PieceVariant.Knight: return KnightMoves(p);
            case PieceVariant.Phone: return PawnMoves(p);   // “폰” 방향은 규칙에 맞게
            default: return new();          // King 등은 필요시 추가
        }
    }

    /* ------------------------  방향·보조 루틴  --------------------------------- */

    // 직선/대각 ‘광선(Ray)’ 이동용
    static readonly Vector2Int[] rookDirs = { Vector2Int.up, Vector2Int.down,
                                                Vector2Int.left, Vector2Int.right };
    static readonly Vector2Int[] bishopDirs = { new(1, 1), new(1, -1), new(-1, 1), new(-1, -1) };
    static readonly Vector2Int[] queenDirs = { Vector2Int.up,   Vector2Int.down,
                                                Vector2Int.left, Vector2Int.right,
                                                new(1,1), new(1,-1), new(-1,1), new(-1,-1) };

    private List<Node> RayMoves(Piece p, Vector2Int[] dirs)
    {
        List<Node> list = new();
        foreach (var d in dirs)
        {
            Vector2Int cur = new Vector2Int(p.x, p.y) + d;
            while (IsFree(cur))
            {
                list.Add(BoardManager.instance.GetNode(cur));
                cur += d;                                 // 같은 방향으로 전진
            }
        }
        return list;
    }

    //  기사(Knight) 고정 오프셋
    static readonly Vector2Int[] knightOffsets =
    {
        new(1,2), new(2,1), new(-1,2), new(-2,1),
        new(1,-2), new(2,-1), new(-1,-2), new(-2,-1)
    };

    private List<Node> KnightMoves(Piece p)
    {
        List<Node> list = new();
        foreach (var off in knightOffsets)
        {
            Vector2Int pos = new Vector2Int(p.x, p.y) + off;
            if (IsFree(pos)) list.Add(BoardManager.instance.GetNode(pos));
        }
        return list;
    }

    //  폰 예시 한 칸 전진만
    private List<Node> PawnMoves(Piece p)
    {
        List<Node> list = new();
        Vector2Int front = new(p.x, p.y + 1);   //  진영이 한쪽뿐이라 가정
        if (IsFree(front)) list.Add(BoardManager.instance.GetNode(front));
        return list;
    }

    /* ------------------------  공통 체크 --------------------------------------- */

    private bool IsFree(Vector2Int pos)
    {
        int w = BoardManager.instance.Width, h = BoardManager.instance.Height;
        if (pos.x < 0 || pos.x >= w || pos.y < 0 || pos.y >= h) return false;
        return !BoardManager.instance.IsBlocked(pos);      // 장애물,아군 기물만 차단
    }
}
