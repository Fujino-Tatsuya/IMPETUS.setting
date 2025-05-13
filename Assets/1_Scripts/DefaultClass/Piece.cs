using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum PieceVariant { Phone, Rook, Knight, Bishop, Queen, King }

public class Piece : Stat
{
    public PieceVariant pieceVariant;

    public int x;
    public int y;

    public Node node = null;

    public bool level = false; //0에서부터 아님 1에서 부터?
    [SerializeField] private int moveRadius = 10; //아직은 제한이 없을려나

    public void SetPosition(int x, int y)
    {
        this.x = x;
        this.y = y;
    }

    public void Upgrade()
    {
        level = true;
    }

    public void SetMoveRadius(int radius)
    {
        moveRadius = radius;
    }

    public void RePosition()
    {
        transform.position = new Vector3(x - 3, 0, y - 5);
    }


}

