using UnityEngine;



public class BoardManager : MonoBehaviour
{


    public static BoardManager instance;
    public static TestSpawner testSpawner;
    public bool IsInitialized => grid != null;


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
        GenerateBoard();
    }

    public GameObject nodePrefab;
    public int width = 7;
    public int height = 7;

    private Node[,] grid;

    public int Width => width; // �� ��������
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

                // ����: ���� ���� ��ġ
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

    public bool IsBlocked(Vector2Int pos) // �̵� ���� ���� üũ 
    {
        Node node = GetNode(pos);
        return node == null || node.IsOccupied();
    }
}
