using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] private GameObject[] nodes;
    [SerializeField] private GameObject endNode;
    [SerializeField] private GameObject[] spawns;
    [SerializeField] private GameObject unitsParent;
    [SerializeField] private GameObject unitPrefab;

    public static GameManager Instance;

    private static Dictionary<GameObject, Dictionary<GameObject, float>> graph;

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
        GameObject unit = Instantiate(unitPrefab, spawns[0].transform);
        if (!unit.TryGetComponent(out Player unitScript))
        {
            Debug.LogError($"{unitPrefab} doesn't have a Player script.");
            return;
        }
        unit.transform.SetParent(unitsParent.transform);
        SetGraph();
        //TestDijkstra();
        var (dist, parent) = Dijkstra.Compute(graph, spawns[0], endNode);
        var path = Dijkstra.GetPath(parent, endNode);
        path.RemoveAt(0);
        unitScript.SetTargets(path);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void SetGraph()
    {
        graph = new Dictionary<GameObject, Dictionary<GameObject, float>>();

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
    }
}
