using System;
using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.Tilemaps;

public class GameManager : MonoBehaviour
{
    [Serializable]
    public class Spawn
    {
        public GameObject gameObject;
        public int unitsCount;
        public TextMeshProUGUI unitsCount_UI;
    }

    [SerializeField] private GameObject[] nodes;
    [SerializeField] private GameObject endNode;
    [SerializeField] private Spawn[] spawns;
    [SerializeField] private GameObject unitsParent;
    [SerializeField] private GameObject unitPrefab;
    [SerializeField] private Tilemap[] tilemaps;
    [SerializeField] private TextMeshProUGUI unitsCount_UI;
    [SerializeField] private GameObject canva_UI;

    [SerializeField] private int unitsCount;

    public static GameManager Instance;

    private static Dictionary<GameObject, Dictionary<GameObject, float>> graph;
    private List<TileInfos> list;

    private int remainingUnits;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        unitsCount_UI.text = unitsCount.ToString() + "/" + unitsCount.ToString();
        remainingUnits = unitsCount;
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public void StartGame()
    {
        canva_UI.SetActive(false);

        list = new List<TileInfos>();

        SetGraph();

        foreach (var s in spawns)
        {
            var (_, parent) = Dijkstra.Compute(graph, s.gameObject);

            for (int i = 0; i < s.unitsCount; i++)
            {
                GameObject unit = Instantiate(unitPrefab, s.gameObject.transform);
                if (!unit.TryGetComponent(out Player unitScript))
                {
                    Debug.LogError($"{unitPrefab} doesn't have a Player script.");
                    return;
                }
                unit.transform.SetParent(unitsParent.transform);
                var path = Dijkstra.GetPath(parent, endNode);
                path.RemoveAt(0);
                unitScript.SetTilemaps(tilemaps);
                unitScript.SetTargets(path);
            }
        }
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
    }

    public void ClickOnUnitsChoiceUI(GameObject o)
    {
        o.transform.Rotate(0, 0, 180f);
    }

    public Spawn FindSpawnByGameObject(GameObject targetGameObject)
    {
        foreach (Spawn spawn in spawns)
        {
            if (spawn.gameObject == targetGameObject)
            {
                return spawn;
            }
        }
        return null;
    }

    public void AddUnitToSpawn(GameObject spawn)
    {
        if (remainingUnits > 0)
        {
            Spawn s = FindSpawnByGameObject(spawn);
            s.unitsCount++;
            remainingUnits--;
            s.unitsCount_UI.text = s.unitsCount.ToString();
            unitsCount_UI.text = remainingUnits.ToString() + "/" + unitsCount.ToString();
        }
    }

    public void RemoveUnitToSpawn(GameObject spawn)
    {
        Spawn s = FindSpawnByGameObject(spawn);
        if (s.unitsCount > 0)
        {
            s.unitsCount--;
            remainingUnits++;
            s.unitsCount_UI.text = s.unitsCount.ToString();
            unitsCount_UI.text = remainingUnits.ToString() + "/" + unitsCount.ToString();
        }
    }
}
