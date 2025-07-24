using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Tilemaps;

public class MazeController : MonoBehaviour
{
    enum TemperatureLevel
    {
        None=0,
        Low = 30,
        Medium = 100,
        High = 300
    }

    public Tilemap tilemap;
    public TileBase wallTile;
    public TileBase pathTile;
    public TileBase openedWallTile30;
    public TileBase openedWallTile100;
    public TileBase openedWallTile300;
    public TileBase player;
    public TextMeshProUGUI tempretureText;
    public TextMeshProUGUI stepText;
    public TextMeshProUGUI pathText;

    private int step = 0;
    private int pathLong = 0;
    private int[,] maze;
    private Vector3Int current;

    private TemperatureLevel[] temperatureLevels = new TemperatureLevel[] {
        TemperatureLevel.None,
        TemperatureLevel.Low,
        TemperatureLevel.Medium,
        TemperatureLevel.High
    };
    private int currentTempIndex = 0; // 預設是 Low

    void Awake()
    {
        tempretureText.text = $"溫度：{(int)temperatureLevels[currentTempIndex]}";
        stepText.text = $"步數：{step}";
        pathText.text = $"路徑長度：{pathLong}";
    }

    void Start()
    {
        RestartWithTemperature((int)temperatureLevels[currentTempIndex]);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            if (currentTempIndex < temperatureLevels.Length - 1)
            {
                currentTempIndex++;
                RestartWithTemperature((int)temperatureLevels[currentTempIndex]);
            }
        }
        else if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            if (currentTempIndex > 0)
            {
                currentTempIndex--;
                RestartWithTemperature((int)temperatureLevels[currentTempIndex]);
            }
        }
    }

    void RestartWithTemperature(int newTemp)
    {
        StopAllCoroutines();
        step = 0;
        pathLong = 0;

        tempretureText.text = $"溫度：{newTemp}";
        stepText.text = $"步數：{step}";
        pathText.text = $"路徑長度：{pathLong}";

        MazeGenerator generator = new MazeGenerator(seed: 42);
        maze = generator.GenerateMaze("dfs", 41, 41);
        maze = generator.ApplyTemperatureLevels(maze, newTemp);

        if (maze[1, 1] == MazeGenerator.PATH)
        {
            current = new Vector3Int(1, -1, 30);
        }
        else
        {
            Debug.LogError("初始位置不是路径！");
        }

        maze = DrawMazeToTilemap(maze);
        tilemap.SetTile(current, player);

        Vector2Int start = new Vector2Int(1, 1);
        Vector2Int end = new Vector2Int(maze.GetLength(1) - 2, maze.GetLength(0) - 2);
        List<Vector2Int> path = generator.FindPathBFS(maze, start, end);
        StartCoroutine(MovePlayer(path));
    }

    int[,] DrawMazeToTilemap(int[,] maze)
    {
        tilemap.ClearAllTiles();
        int height = maze.GetLength(0);
        int width = maze.GetLength(1);
        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                TileBase tile = null;
                switch (maze[y, x])
                {
                    case MazeGenerator.WALL:
                        tile = wallTile;
                        break;
                    case MazeGenerator.PATH:
                        tile = pathTile;
                        break;
                    case 30:
                        tile = openedWallTile30;
                        maze[y, x] = MazeGenerator.PATH;
                        break;
                    case 100:
                        tile = openedWallTile100;
                        maze[y, x] = MazeGenerator.PATH;
                        break;
                    case 300:
                        tile = openedWallTile300;
                        maze[y, x] = MazeGenerator.PATH;
                        break;
                }
                if (tile != null)
                {
                    tilemap.SetTile(new Vector3Int(x, -y, 0), tile);
                }
            }
        }
        return maze;
    }

    IEnumerator MovePlayer(List<Vector2Int> path)
    {
        foreach (var pos in path)
        {
            Vector3Int tilePos = new Vector3Int(pos.x, -pos.y, 30);
            current = tilePos;
            tilemap.SetTile(current, player);   // 設定新位置
            step += 1;
            pathLong = path.Count;
            stepText.text = $"步數：{step}";
            pathText.text = $"路徑長度：{pathLong}";
            yield return new WaitForSeconds(0.1f);
        }
    }
}
