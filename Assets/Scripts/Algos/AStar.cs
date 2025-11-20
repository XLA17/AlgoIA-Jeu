using NUnit.Framework;
using System;
using System.Collections.Generic;
using UnityEngine;

public class AStarBox
{
    private Vector2 position;
    private int value;
    private int weight;

    public AStarBox(Vector2 position, int value, int weight)
    {
        this.position = position;
        this.value = value;
        this.weight = weight;
    }
}

public class AStar
{
    private List<AStarBox> map;
    private int widthMap;
    private int heightMap;
    private Vector2 startPos;
    private Vector2 endPos;

    public AStar(List<AStarBox> map, int widthMap, int heightMap, Vector2 startPos, Vector2 endPos)
    {
        if (map.Count != widthMap *  heightMap)
        {
            throw new Exception("map length != width * height");
        }
        this.map = map;
        this.widthMap = widthMap;
        this.heightMap = heightMap;
        this.startPos = startPos;
        this.endPos = endPos;
    }

    private int GetDistance(Vector2 position)
    {
        return (int)Mathf.Max(Mathf.Abs(endPos.x - position.x), Mathf.Abs(endPos.y - position.y));
    }

    public void Resolution()
    {
        List<AStarBox> boxesToCheck = new List<AStarBox>();

        
    }

    public void Recursif(Vector2 currentPos)
    {
        int distance = GetDistance(currentPos);


    }
}
