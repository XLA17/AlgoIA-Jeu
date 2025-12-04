using NUnit.Framework;
using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UIElements;
using static UnityEngine.GraphicsBuffer;

//public class AStarBox
//{
//    private Vector2 position;
//    private int value;
//    private int weight;

//    public AStarBox(Vector2 position, int value, int weight)
//    {
//        this.position = position;
//        this.value = value;
//        this.weight = weight;
//    }
//}

//public class AStar
//{
//    private List<AStarBox> map;
//    private int widthMap;
//    private int heightMap;
//    private Vector2 startPos;
//    private Vector2 endPos;

//    public AStar(List<AStarBox> map, int widthMap, int heightMap, Vector2 startPos, Vector2 endPos)
//    {
//        if (map.Count != widthMap *  heightMap)
//        {
//            throw new Exception("map length != width * height");
//        }
//        this.map = map;
//        this.widthMap = widthMap;
//        this.heightMap = heightMap;
//        this.startPos = startPos;
//        this.endPos = endPos;
//    }

//    private int GetDistance(Vector2 position)
//    {
//        return (int)Mathf.Max(Mathf.Abs(endPos.x - position.x), Mathf.Abs(endPos.y - position.y));
//    }

//    public void Resolution()
//    {
//        List<AStarBox> boxesToCheck = new List<AStarBox>();


//    }

//    public void Recursif(Vector2 currentPos)
//    {
//        int distance = GetDistance(currentPos);


//    }
//}


public struct TileInfos
{
    public Vector2? parent;
    public int value;

    public TileInfos(Vector2? parent, int value)
    {
        this.parent = parent;
        this.value = value;
    }
}

public class AStar
{
    //[SetUp]

    private static void PrintList(List<Vector2> list)
    {
        foreach (Vector2 vector in list)
        {
            Debug.Log($"Vector2: X = {vector.x}, Y = {vector.y}");
        }
    }

    private static void PrintList(List<int> list)
    {
        foreach (int elem in list)
        {
            Debug.Log($"int: {elem}");
        }
    }

    public static void PrintList(Dictionary<Vector2, TileInfos> dictionary)
    {
        foreach (KeyValuePair<Vector2, TileInfos> entry in dictionary)
        {
            Debug.Log($"Position: {entry.Key}, Parent: {entry.Value.parent}, Value: {entry.Value.value}");
        }
    }

    public static KeyValuePair<Vector2, TileInfos> GetMinValueEntry(Dictionary<Vector2, TileInfos> dictionary)
    {
        var minEntry = new KeyValuePair<Vector2, TileInfos>(default, new TileInfos(null, int.MaxValue));
        bool first = true;

        foreach (var kvp in dictionary)
        {
            if (first || kvp.Value.value < minEntry.Value.value)
            {
                minEntry = kvp;
                first = false;
            }
        }

        return minEntry;
    }

    public static List<TileInfos> Compute(Tilemap[] tilemaps, Vector2 startPosition, Vector2 endPosition)
    {
        var closedList = new Dictionary<Vector2, TileInfos>();
        var openList = new Dictionary<Vector2, TileInfos>();

        openList[startPosition] = new TileInfos(null, 0);

        while (openList.Count > 0)
        {
            
            var current = GetMinValueEntry(openList);
            openList.Remove(current.Key);

            if (current.Key == endPosition)
            {
                closedList[current.Key] = current.Value;
                return ReconstructPath(closedList, endPosition);
            }

            List<Vector2> neighborsPos = GetNeighbors(current.Key);

            foreach (Vector2 neighborPos in neighborsPos)
            {
                if (closedList.ContainsKey(neighborPos))
                    continue;

                if (!HasTile(tilemaps, new Vector3Int((int)neighborPos.x, (int)neighborPos.y, 0)))
                    continue;

                int newValue = (int)(CalculateDistance(startPosition, neighborPos) + CalculateDistance(neighborPos, endPosition));

                if (!openList.TryGetValue(neighborPos, out TileInfos existingTile) || newValue < existingTile.value)
                {
                    openList[neighborPos] = new TileInfos(current.Key, newValue);
                }
            }

            closedList[current.Key] = current.Value;
        }

        // Aucun chemin trouvé
        return new();
    }

    public static bool HasTile(Tilemap[] tilemaps, Vector3Int position)
    {
        if (tilemaps == null)
        {
            return false;
        }
        bool res = false;
        foreach (var tilemap in tilemaps)
        {
            TileBase tile = tilemap.GetTile(position);
            if (tile != null)
            {
                return true;
            }
        }
        return res;
    }
    private static List<TileInfos> ReconstructPath(Dictionary<Vector2, TileInfos> closedList, Vector2 endPos)
    {
        var path = new List<TileInfos>();
        TileInfos current = closedList[endPos];

        while (current.parent != null)
        {
            path.Add(current);
            current = closedList[current.parent.Value];
        }

        path.Reverse();
        return path;
    }

    private static float CalculateDistance(Vector2 pos1, Vector2 pos2)
    {
        return Mathf.Max(Mathf.Abs(pos1.x - pos2.x), Mathf.Abs(pos1.y - pos2.y));
    }

    private static List<Vector2> GetNeighbors(Vector2 position)
    {
        List<Vector2> neighbors = new();
        //int[] dx = { -1, 0, 1, -1, 1, -1, 0, 1 };
        //int[] dy = { -1, -1, -1, 0, 0, 1, 1, 1 };
        int[] dx = { 0, 0, -1, 1 };
        int[] dy = { -1, 1, 0, 0 };

        for (int i = 0; i < 4; i++)
        {
            float nx = position.x + dx[i];
            float ny = position.y + dy[i];
            neighbors.Add(new Vector2(nx, ny));
        }

        return neighbors;
    }
}