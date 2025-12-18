using System;
using System.Collections.Generic;
using UnityEngine;


public class Dijkstra
{
    public static (Dictionary<GameObject, float> dist, Dictionary<GameObject, GameObject> parent)
        Compute(Dictionary<GameObject, Dictionary<GameObject, float>> graph, GameObject start, GameObject target = null)
    {
        var dist = new Dictionary<GameObject, float>();
        var parent = new Dictionary<GameObject, GameObject>();

        foreach (var node in graph.Keys)
        {
            dist[node] = float.MaxValue;
            parent[node] = null;
        }

        dist[start] = 0;

        // SortedSet to simulate tas
        var pq = new SortedSet<(float dist, GameObject node)>(Comparer<(float, GameObject)>.Create((a, b) =>
        {
            int cmp = a.Item1.CompareTo(b.Item1);
            if (cmp == 0) return a.Item2.name.CompareTo(b.Item2.name);
            return cmp;
        }));

        pq.Add((0, start));

        while (pq.Count > 0)
        {
            var first = pq.Min;
            pq.Remove(first);

            float currentDist = first.dist;
            GameObject node = first.node;

            if (target != null && node == target)
                break;

            if (currentDist > dist[node])
                continue;

            foreach (var kv in graph[node])
            {
                GameObject neighbor = kv.Key;
                float weight = kv.Value;
                float newDist = currentDist + weight;

                if (newDist < dist[neighbor])
                {
                    pq.Remove((dist[neighbor], neighbor));

                    dist[neighbor] = newDist;
                    parent[neighbor] = node;
                    pq.Add((newDist, neighbor));
                }
            }
        }

        return (dist, parent);
    }

    public static List<GameObject> GetPath(Dictionary<GameObject, GameObject> parent, GameObject target)
    {
        var path = new List<GameObject>();
        GameObject current = target;
        while (current != null)
        {
            path.Add(current);
            current = parent[current];
        }
        path.Reverse();
        return path;
    }
}