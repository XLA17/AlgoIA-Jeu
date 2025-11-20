using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;
using UnityEditor.Experimental.GraphView;
using UnityEngine;


public class DijkstraPath
{
    public List<DijkstraNode> nodes;
    public int value;

    public DijkstraPath(List<DijkstraNode> nodes, int value)
    {
        this.nodes = nodes;
        this.value = value;
    }
}

public class DijkstraNode
{
    private GameObject gameObject;
    private DijkstraLink[] links;

    public DijkstraNode(GameObject gameObject, DijkstraLink[] links)
    {
        this.gameObject = gameObject;
        this.links = links;
    }

    public DijkstraLink[] GetNearestLinksOrder(int value = 0)
    {
        return links.OrderBy(l => l.value).ToArray();
    }
}

public class DijkstraLink
{
    public DijkstraNode endNode;
    public int value;

    public DijkstraLink(DijkstraNode endNode, int value)
    {
        this.endNode = endNode;
        this.value = value;
    }
}

public class Dijkstra
{
    private DijkstraNode[] nodes;
    private DijkstraNode startNode;
    private DijkstraNode endNode;

    public Dijkstra(DijkstraNode[] nodes, DijkstraNode startNode, DijkstraNode endNode)
    {
        this.nodes = nodes;
        this.startNode = startNode;
        this.endNode = endNode;
    }

    public void Resolution()
    {
        int totalValue = int.MaxValue;
        DijkstraPath bestPath = new DijkstraPath(new List<DijkstraNode>(), 0);
        bestPath.nodes.Add(startNode);

        List<DijkstraPath> paths = new List<DijkstraPath>();
        paths.Add(bestPath);


        //recursif(paths, startNode);

        List<DijkstraLink> links = startNode.GetNearestLinksOrder().ToList();

        DijkstraLink link = links[0];

        totalValue += links[0].value;

        DijkstraLink[] newLinks = link.endNode.GetNearestLinksOrder().ToArray();
        AddValueToLinks(newLinks, totalValue);
        links.AddRange(newLinks);
    }

    private void recursif(List<DijkstraPath> paths)
    {
        for (int i = 0; i < paths.Count; i++)
        {
            if (paths[i].nodes[^1] == endNode)
            {
                return;
            }
        }





        //if (node == endNode)
        //{
        //    return;
        //}

        //DijkstraLink[] links = node.GetNearestLinksOrder();

        //for (int i = 0; i < links.Length; i++)
        //{
        //    if (links[i])
        //    {
        //    }

        //    if (link.value) { }
        //}
    }
    public void AddValueToLinks(DijkstraLink[] links, int value)
    {
        foreach (var l in links)
        {
            l.value += value;
        }
    }
}
