using System;
using System.Collections.Generic;
using UnityEngine;

public class MazeGenerator
{
    public const int WALL = 1;
    public const int PATH = 0;

    private System.Random random;

    public MazeGenerator(int? seed = null)
    {
        if (seed.HasValue)
            random = new System.Random(seed.Value);
        else
            random = new System.Random();
    }

    // 產生迷宮主程式 (可改用"prim"或"dfs")
    public int[,] GenerateMaze(string method, int width, int height)
    {
        // 寬高強制為奇數
        if (width % 2 == 0) width--;
        if (height % 2 == 0) height--;

        if (method == "prim")
            return GenerateMazeWithPrim(width, height);
        else if (method == "dfs")
            return GenerateMazeWithDFS(width, height);
        else
            throw new Exception("Unknown method: " + method);
    }

    // Prim 算法
    private int[,] GenerateMazeWithPrim(int width, int height)
    {
        int[,] maze = new int[height, width];
        // 初始全部為牆壁
        for (int y = 0; y < height; y++)
            for (int x = 0; x < width; x++)
                maze[y, x] = WALL;

        // 隨機起點
        int startX = RandomOdd(width);
        int startY = RandomOdd(height);
        maze[startY, startX] = PATH;

        List<(int x, int y, int fromX, int fromY)> frontier = new List<(int, int, int, int)>();

        void AddFrontiers(int x, int y)
        {
            foreach (var dir in new (int, int)[] { (-2, 0), (2, 0), (0, -2), (0, 2) })
            {
                int nx = x + dir.Item1;
                int ny = y + dir.Item2;
                if (nx > 0 && nx < width && ny > 0 && ny < height && maze[ny, nx] == WALL)
                    frontier.Add((nx, ny, x, y));
            }
        }

        AddFrontiers(startX, startY);

        while (frontier.Count > 0)
        {
            int idx = random.Next(frontier.Count);
            var (x, y, fromX, fromY) = frontier[idx];
            frontier.RemoveAt(idx);

            if (maze[y, x] == WALL)
            {
                maze[y, x] = PATH;
                maze[(y + fromY) / 2, (x + fromX) / 2] = PATH;
                AddFrontiers(x, y);
            }
        }

        return maze;
    }

    // DFS 算法
    private int[,] GenerateMazeWithDFS(int width, int height)
    {
        int[,] maze = new int[height, width];
        for (int y = 0; y < height; y++)
            for (int x = 0; x < width; x++)
                maze[y, x] = WALL;

        int startX = RandomOdd(width);
        int startY = RandomOdd(height);
        maze[startY, startX] = PATH;

        void Carve(int x, int y)
        {
            var dirs = new List<(int, int)> { (2, 0), (-2, 0), (0, 2), (0, -2) };
            Shuffle(dirs);
            foreach (var (dx, dy) in dirs)
            {
                int nx = x + dx;
                int ny = y + dy;
                if (nx > 0 && nx < width - 1 && ny > 0 && ny < height - 1 && maze[ny, nx] == WALL)
                {
                    maze[ny, nx] = PATH;
                    maze[y + dy / 2, x + dx / 2] = PATH;
                    Carve(nx, ny);
                }
            }
        }

        Carve(startX, startY);

        return maze;
    }

    // 根據溫度調整打開牆壁 (改變maze內牆壁為指定溫度值)
    // 30 => 4% 牆開啟, 100 => 8%, 300 => 16%
    public int[,] ApplyTemperatureLevels(int[,] maze, int temperature)
    {
        int height = maze.GetLength(0);
        int width = maze.GetLength(1);

        double ratio = 0;
        int tag = 0;

        if (temperature == 30)
        {
            ratio = 0.04;
            tag = 30;
        }
        else if (temperature == 100)
        {
            ratio = 0.08;
            tag = 100;
        }
        else if (temperature == 300)
        {
            ratio = 0.16;
            tag = 300;
        }
        else
        {
            Debug.LogWarning($"Unsupported temperature: {temperature}");
            return maze;
        }

        List<(int x, int y)> candidates = new List<(int, int)>();
        for (int y = 1; y < height - 1; y++)
        {
            for (int x = 1; x < width - 1; x++)
            {
                if (maze[y, x] == WALL)
                    candidates.Add((x, y));
            }
        }

        int numToOpen = (int)(candidates.Count * ratio);
        var chosen = new HashSet<(int, int)>();
        while (chosen.Count < numToOpen && candidates.Count > 0)
        {
            int idx = random.Next(candidates.Count);
            var pos = candidates[idx];
            if (!chosen.Contains(pos))
            {
                chosen.Add(pos);
                maze[pos.y, pos.x] = tag;
            }
        }

        Debug.Log($"Temperature {temperature}°C: Opened {numToOpen} walls (marked as {tag})");

        return maze;
    }

    // 工具函數 - 取得一個奇數小於limit
    private int RandomOdd(int limit)
    {
        int r = random.Next(1, limit);
        if (r % 2 == 0)
            r = (r == limit - 1) ? r - 1 : r + 1;
        return r;
    }

    // 洗牌
    private void Shuffle<T>(IList<T> list)
    {
        int n = list.Count;
        while (n > 1)
        {
            n--;
            int k = random.Next(n + 1);
            T temp = list[k];
            list[k] = list[n];
            list[n] = temp;
        }
    }

    public List<Vector2Int> FindPathBFS(int[,] maze, Vector2Int start, Vector2Int end)
    {
        int height = maze.GetLength(0);
        int width = maze.GetLength(1);
        bool[,] visited = new bool[height, width];
        Dictionary<Vector2Int, Vector2Int> parent = new Dictionary<Vector2Int, Vector2Int>();

        Queue<Vector2Int> queue = new Queue<Vector2Int>();
        queue.Enqueue(start);
        visited[start.y, start.x] = true;

        Vector2Int[] directions = new Vector2Int[]
        {
        new Vector2Int(0, 1), new Vector2Int(0, -1),
        new Vector2Int(1, 0), new Vector2Int(-1, 0)
        };

        while (queue.Count > 0)
        {
            Vector2Int current = queue.Dequeue();
            if (current == end) break;

            foreach (var dir in directions)
            {
                Vector2Int next = current + dir;
                if (next.x >= 0 && next.x < width && next.y >= 0 && next.y < height &&
                    maze[next.y, next.x] == MazeGenerator.PATH && !visited[next.y, next.x])
                {
                    queue.Enqueue(next);
                    visited[next.y, next.x] = true;
                    parent[next] = current;
                }
            }
        }

        // 回溯找路徑
        List<Vector2Int> path = new List<Vector2Int>();
        Vector2Int cur = end;
        while (cur != start)
        {
            path.Add(cur);
            if (!parent.ContainsKey(cur))
                return new List<Vector2Int>(); // 找不到
            cur = parent[cur];
        }
        path.Add(start);
        path.Reverse();
        return path;
    }
}