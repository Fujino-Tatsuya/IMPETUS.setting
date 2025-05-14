using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PieceManager : MonoBehaviour
{
    public static PieceManager instance;
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

    public int pawnCount = 0;
    public int knightCount = 0;
    public int bishopCount = 0;
    public int rookCount = 0;
    public int queenCount = 0;
    public int upgradedPawnCount = 0;
    public int upgradedKnightCount = 0;
    public int upgradedBishopCount = 0;
    public int upgradedRookCount = 0;
    public int upgradedQueenCount = 0;

    public int GetCount(PieceVariant pieceVariant, bool upgrade)
    {
        if (upgrade)
        {
            if (pieceVariant == PieceVariant.Phone)
                return upgradedPawnCount;
            if (pieceVariant == PieceVariant.Knight)
                return upgradedKnightCount;
            if (pieceVariant == PieceVariant.Bishop)
                return upgradedBishopCount;
            if (pieceVariant == PieceVariant.Rook)
                return upgradedRookCount;
            if (pieceVariant == PieceVariant.Queen)
                return upgradedQueenCount;
        }
        else
        {
            if (pieceVariant == PieceVariant.Phone)
                return pawnCount;
            if (pieceVariant == PieceVariant.Knight)
                return knightCount;
            if (pieceVariant == PieceVariant.Bishop)
                return bishopCount;
            if (pieceVariant == PieceVariant.Rook)
                return rookCount;
            if (pieceVariant == PieceVariant.Queen)
                return queenCount;
        }
        return 0;
    }

    public void SetCount(PieceVariant pieceVariant, bool upgrade, int count)
    {
        if (upgrade)
        {
            if (pieceVariant == PieceVariant.Phone)
                upgradedPawnCount = count;
            if (pieceVariant == PieceVariant.Knight)
                upgradedKnightCount = count;
            if (pieceVariant == PieceVariant.Bishop)
                upgradedBishopCount = count;
            if (pieceVariant == PieceVariant.Rook)
                upgradedRookCount = count;
            if (pieceVariant == PieceVariant.Queen)
                upgradedQueenCount = count;
        }
        else
        {
            if (pieceVariant == PieceVariant.Phone)
                pawnCount = count;
            if (pieceVariant == PieceVariant.Knight)
                knightCount = count;
            if (pieceVariant == PieceVariant.Bishop)
                bishopCount = count;
            if (pieceVariant == PieceVariant.Rook)
                rookCount = count;
            if (pieceVariant == PieceVariant.Queen)
                queenCount = count;
        }

        var inventoryPieces = GameObject.Find("Canvas").GetComponentsInChildren<InventoryPiece>();
        foreach (InventoryPiece inventoryPiece in inventoryPieces)
        {
            inventoryPiece.SpriteCheck();
        }
    }

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
}
