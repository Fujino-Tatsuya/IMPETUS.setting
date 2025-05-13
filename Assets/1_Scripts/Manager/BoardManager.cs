using UnityEngine;



public class BoardManager : MonoBehaviour
{


    public static BoardManager instance;
    public static TestSpawner testSpawner;
    public bool IsInitialized => grid != null;

    [Header("Node Prefab")]
    [SerializeField] private GameObject nodePrefab;

    [Header("Tile Materials")]
    [Tooltip("흰색 계열 5개")]
    [SerializeField] private Material[] whiteMats = new Material[5];

    [Tooltip("회색 계열 5개")]
    [SerializeField] private Material[] grayMats = new Material[5];

    [Header("Front-Tile (고정)")]
    [SerializeField] Material frontWhiteMat;
    [SerializeField] Material frontBlackMat;   // 회색 노드용


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
        GenerateBoard();
    }


    public int width = 7;
    public int height = 7;

    private Node[,] grid;

    public int Width => width; // 값 내보내기
    public int Height => height;

    void Start()
    {
        //GenerateBoard();
        //testSpawner.SpawnRandomObstacle(3);
    }

    void GenerateBoard()
    {
        grid = new Node[Width, Height];

        for (int y = 0; y < Height; y++)
            for (int x = 0; x < Width; x++)
            {
                var world = new Vector3(x - 3, 0, y - 5);
                Node node = Instantiate(nodePrefab, world, Quaternion.identity, transform)
                                .GetComponent<Node>();


                bool isWhite = (x + y) % 2 == 0;
                Material mat = isWhite
                    ? whiteMats[Random.Range(0, whiteMats.Length)]
                    : grayMats[Random.Range(0, grayMats.Length)];

                Material frontmat = isWhite
                    ? frontWhiteMat
                    : frontBlackMat;

                node.Init(new Vector2Int(x, y),
                          isWhite ? NodeColorType.White : NodeColorType.Gray,
                          mat, frontmat);

                grid[x, y] = node;
            }
    }
    //void GenerateBoard()
    //{
    //    grid = new Node[width, height]; //  

    //    for (int x = 0; x < width; x++)
    //    {
    //        for (int z = 0; z < height; z++)
    //        {
    //            GameObject obj = Instantiate(nodePrefab, new Vector3(x-3, 0, z-5), Quaternion.identity, transform);
    //            Node node = obj.GetComponent<Node>();

    //            // 예시: 교차 색상 배치
    //            NodeColorType color = ((x + z) % 2 == 0) ? NodeColorType.White : NodeColorType.Gray;
    //            node.Init(new Vector2Int(x, z), color);

    //            grid[x, z] = node;
    //        }
    //    }
    //}

    public Node GetNode(Vector2Int pos)
    {
        return grid[pos.x, pos.y];
    }

//<<<<<<< HEAD
//    public bool IsAble(int x, int z)
//    {
//        if (grid[x,z].piece)
//            return false;
//        return true;
//=======
    public bool IsBlocked(Vector2Int pos) // 이동 가능 여부 체크 
    {
        Node node = GetNode(pos);
        return node == null || node.IsOccupied();

    }
}
