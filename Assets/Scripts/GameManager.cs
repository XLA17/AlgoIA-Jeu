using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;
using UnityEngine.Tilemaps;

public class GameManager : MonoBehaviour
{
    [SerializeField] private GameObject[] nodes;
    [SerializeField] private GameObject endNode;
    [SerializeField] private GameObject[] spawns;
    [SerializeField] private GameObject unitsParent;
    [SerializeField] private GameObject unitPrefab;
    [SerializeField] private Tilemap[] tilemaps;

    public static GameManager Instance;

    private static Dictionary<GameObject, Dictionary<GameObject, float>> graph;
    private List<TileInfos> list;
    private Dictionary<Vector2, TileInfos> closedList;
    private Dictionary<Vector2, TileInfos> openList;

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
        list = new List<TileInfos>();
        openList = new Dictionary<Vector2, TileInfos>();
        closedList = new Dictionary<Vector2, TileInfos>();

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


        (list, closedList, openList) = AStar.Compute(tilemaps, new Vector2(-35, -19), new Vector2(-21, -5));


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

    private void OnDrawGizmos()
    {
        if (list == null || list.Count < 2)
            return;

        Gizmos.color = Color.green;

        for (int i = 0; i < list.Count - 1; i++)
        {
            // Vérifiez que parent n'est pas null
            if (!list[i].parent.HasValue || !list[i + 1].parent.HasValue)
                continue;

            Vector3 start = new Vector3(list[i].parent.Value.x + 0.5f, list[i].parent.Value.y + 0.5f, 0);
            Vector3 end = new Vector3(list[i + 1].parent.Value.x + 0.5f, list[i + 1].parent.Value.y + 0.5f, 0);

            // Dessiner la ligne
            Gizmos.DrawLine(start, end);

            // Calculer le point milieu
            Vector3 midPoint = (start + end) / 2;

            // Afficher le texte au milieu
            Handles.Label(midPoint, list[i].value.ToString());
        }

        if (closedList == null)
            return;

        //Gizmos.color = Color.red; // Couleur des carrés

        //foreach (KeyValuePair<Vector2, TileInfos> entry in closedList)
        //{
        //    // Convertir Vector2 en Vector3 pour Gizmos.DrawCube
        //    Vector3 position = new Vector3(entry.Key.x + 0.5f, entry.Key.y + 0.5f, 0);

        //    // Dessiner un cube (carré en 2D) de taille 1.1x1.1
        //    Gizmos.DrawCube(position, new Vector3(1.1f, 1.1f, 0.1f));
        //}

        //if (openList == null)
        //    return;

        //Gizmos.color = Color.blue; // Couleur des carrés

        //foreach (KeyValuePair<Vector2, TileInfos> entry in openList)
        //{
        //    // Convertir Vector2 en Vector3 pour Gizmos.DrawCube
        //    Vector3 position = new Vector3(entry.Key.x + 0.5f, entry.Key.y + 0.5f, 0);

        //    // Dessiner un cube (carré en 2D) de taille 1.1x1.1
        //    Gizmos.DrawCube(position, new Vector3(1.1f, 1.1f, 0.1f));
        //}
    }
}
