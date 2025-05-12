using UnityEngine;



public class BoardManager : MonoBehaviour
{


    public static BoardManager instance;
    public static TestSpawner testSpawner;
    public bool IsInitialized => grid != null;


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

    public GameObject nodePrefab;
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
        grid = new Node[width, height]; //  

        for (int x = 0; x < width; x++)
        {
            for (int z = 0; z < height; z++)
            {
                GameObject obj = Instantiate(nodePrefab, new Vector3(x-3, 0, z-5), Quaternion.identity, transform);
                Node node = obj.GetComponent<Node>();

                // 예시: 교차 색상 배치
                NodeColorType color = ((x + z) % 2 == 0) ? NodeColorType.White : NodeColorType.Gray;
                node.Init(new Vector2Int(x, z), color);

                grid[x, z] = node;
            }
        }
    }

    public Node GetNode(Vector2Int pos)
    {
        return grid[pos.x, pos.y];
    }

    public bool IsBlocked(Vector2Int pos) // 이동 가능 여부 체크 
    {
        Node node = GetNode(pos);
        return node == null || node.IsOccupied();
    }
}
