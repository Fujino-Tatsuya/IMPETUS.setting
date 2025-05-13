using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum NodeColorType
{
    None,
    White,
    Gray
}
public class Node : MonoBehaviour
{
    public Vector2Int GridPos { get; private set; }
    public NodeColorType ColorType { get; private set; }

    [SerializeField] private MeshRenderer meshRenderer;
    [SerializeField] private Material whiteMat;
    [SerializeField] private Material grayMat;
    [SerializeField] private Material nullMat;

    [Header("����")]
    public GameObject currentPiece = null;   // �ö� �⹰
    public bool isObstacle = false;   // ��ֹ� ����

    public void Init(Vector2Int pos, NodeColorType color)
    {
        GridPos = pos;
        currentPiece = null;
        isObstacle = false;
        SetColor(color);
    }

    public void SetColor(NodeColorType color)
    {
        ColorType = color;

        switch (ColorType)
        {
            case NodeColorType.White:
                meshRenderer.material = whiteMat;
                break;
            case NodeColorType.Gray:
                meshRenderer.material = grayMat;
                break;
            default:
                meshRenderer.material = nullMat;
                break;
        }
    }
    public bool IsOccupied() // �̹� �⹰�� �ְų� ��ֹ��� ������ true
    {
        return currentPiece != null || isObstacle;
    }
}
