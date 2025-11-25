using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] private List<GameObject> nodes;
    [SerializeField] private GameObject startNode;
    [SerializeField] private GameObject endNode;

    public static GameManager Instance;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        var graph = new Dictionary<GameObject, Dictionary<GameObject, float>>();

        foreach (GameObject n in nodes)
        {
            if (!n.TryGetComponent(out Node nodeScript)) return;

            var nextNodes = new Dictionary<GameObject, float>();
            
            foreach (GameObject nextTower in nodeScript.GetNextTowers())
            {
                if (!nextTower.TryGetComponent(out Node nextTowerScript)) return;
                nextNodes[nextTower] = nextTowerScript.GetValue();
            }

            graph[n] = nextNodes;
        }

        var (dist, parent) = Dijkstra.Compute(graph, startNode, endNode);
        Debug.Log($"Distance de {startNode} à {endNode} = {dist[endNode]}");

        var path = Dijkstra.GetPath(parent, endNode);
        Debug.Log("Chemin : " + string.Join(" -> ", path));
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
