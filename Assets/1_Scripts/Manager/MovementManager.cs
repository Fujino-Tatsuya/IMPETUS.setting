using System.Collections.Generic;
using UnityEngine;

/// <summary>���� �� ��� �⹰�� �̵� ���� ������ ��ꡤĳ���Ѵ�.</summary>
public class MovementManager : MonoBehaviour
{
    public static MovementManager instance;

    //   Piece �� �̵� ���� ��� ���  (�� �ϸ��� ����)
    private readonly Dictionary<Piece, List<Node>> cache = new();

    void Awake()
    {
        if (instance == null) instance = this;
        else { Destroy(gameObject); return; }
        DontDestroyOnLoad(gameObject);
    }

    /* -------------------------------------------------------------------------- */
    /* ���� API                                                                    */
    /* -------------------------------------------------------------------------- */

    public List<Node> GetMoves(Piece piece)
    {
        if (!cache.ContainsKey(piece))
            cache[piece] = CalcMoves(piece);

        return cache[piece];
    }


    public void InvalidateAll() => cache.Clear();

    /* -------------------------------------------------------------------------- */
    /* ���� : �̵� ��Ģ                                                            */
    /* -------------------------------------------------------------------------- */

    private List<Node> CalcMoves(Piece p)
    {
        switch (p.pieceVariant)                     //  switch-case �ٽ�
        {
            case PieceVariant.Rook: return RayMoves(p, rookDirs);
            case PieceVariant.Bishop: return RayMoves(p, bishopDirs);
            case PieceVariant.Queen: return RayMoves(p, queenDirs);
            case PieceVariant.Knight: return KnightMoves(p);
            case PieceVariant.Phone: return PawnMoves(p);   // ������ ������ ��Ģ�� �°�
            default: return new();          // King ���� �ʿ�� �߰�
        }
    }

    /* ------------------------  ���⡤���� ��ƾ  --------------------------------- */

    // ����/�밢 ������(Ray)�� �̵���
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
                cur += d;                                 // ���� �������� ����
            }
        }
        return list;
    }

    //  ���(Knight) ���� ������
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

    //  �� ���� �� ĭ ������
    private List<Node> PawnMoves(Piece p)
    {
        List<Node> list = new();
        Vector2Int front = new(p.x, p.y + 1);   //  ������ ���ʻ��̶� ����
        if (IsFree(front)) list.Add(BoardManager.instance.GetNode(front));
        return list;
    }

    /* ------------------------  ���� üũ --------------------------------------- */

    private bool IsFree(Vector2Int pos)
    {
        int w = BoardManager.instance.Width, h = BoardManager.instance.Height;
        if (pos.x < 0 || pos.x >= w || pos.y < 0 || pos.y >= h) return false;
        return !BoardManager.instance.IsBlocked(pos);      // ��ֹ�,�Ʊ� �⹰�� ����
    }
}
