using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum NodeColorType
{
    None,
    White,
    Gray
}

public enum NodeStyleType 
{
    first,
    second,
    third,
    fourth,
    fifth

}

public class Node : MonoBehaviour
{
    public Vector2Int GridPos { get; private set; }
    public NodeColorType ColorType { get; private set; }

    [SerializeField] MeshRenderer meshRenderer;
    [SerializeField] MeshRenderer frontRenderer;  

    [Header("상태")]
    public GameObject currentPiece;
    public bool isObstacle;

    public void Init(Vector2Int pos, NodeColorType color, Material mat, Material frontMat)
    {
        GridPos = pos;
        ColorType = color;
        currentPiece = null;
        isObstacle = false;
        meshRenderer.material = mat;   // 전달받은 머티리얼 적용
        frontRenderer.material = frontMat;  // 정면 고정
    }

    public bool IsOccupied() => currentPiece != null || isObstacle;
}

//public class Node : MonoBehaviour
//{
//    public Vector2Int GridPos { get; private set; }
//    public NodeColorType ColorType { get; private set; }

//    [SerializeField] private MeshRenderer meshRenderer;
//    [SerializeField] private Material whiteMat;
//    [SerializeField] private Material grayMat;
//    [SerializeField] private Material nullMat;

//    [Header("상태")]
//    public GameObject currentPiece = null;   // 올라간 기물
//    public bool isObstacle = false;   // 장애물 여부

//    public void Init(Vector2Int pos, NodeColorType color)
//    {
//        GridPos = pos;
//        currentPiece = null;
//        isObstacle = false;
//        SetColor(color);
//    }

//    public void SetColor(NodeColorType color)
//    {
//        ColorType = color;

//        switch (ColorType)
//        {
//            case NodeColorType.White:
//                meshRenderer.material = whiteMat;
//                break;
//            case NodeColorType.Gray:
//                meshRenderer.material = grayMat;
//                break;
//            default:
//                meshRenderer.material = nullMat;
//                break;
//        }
//    }
//    public bool IsOccupied() // 이미 기물이 있거나 장애물이 있으면 true
//    {
//        return currentPiece != null || isObstacle;
//    }
//}
