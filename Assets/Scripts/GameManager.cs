using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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

    public bool geneticExecution = false;
    [SerializeField] private float cohesionWeightMin;
    [SerializeField] private float cohesionWeightMax;
    [SerializeField] private float separationWeightMin;
    [SerializeField] private float separationWeightMax;
    [SerializeField] private float alignmentWeightMin;
    [SerializeField] private float alignmentWeightMax;
    [SerializeField] private float leaderInfluenceMin;
    [SerializeField] private float leaderInfluenceMax;
    [SerializeField] private float wallInfluenceMin;
    [SerializeField] private float wallInfluenceMax;

    [Space]

    [SerializeField] private GameObject[] nodes;
    [SerializeField] private GameObject endNode;
    [SerializeField] private Spawn[] spawns;
    [SerializeField] private GameObject unitsParent;
    //[SerializeField] private GameObject unitAIPrefab;
    //[SerializeField] private GameObject unitBoidPrefab;
    [SerializeField] private GameObject unitPrefab;
    [SerializeField] private Tilemap[] tilemaps;
    [SerializeField] private TextMeshProUGUI unitsCount_UI;
    [SerializeField] private GameObject canva_UI;

    [SerializeField] private int unitsCount;

    public static GameManager Instance;

    private static Dictionary<GameObject, Dictionary<GameObject, float>> graph;
    private List<TileInfos> list;

    private int remainingUnits;

    //boids
    // public float cohesionWeight = 1f;
    // public float separationWeight = 1f;
    // public float alignmentWeight = 1f;
    // public float leaderInfluence = 1000f;

    // public float neighborDistance = 1f;
    // public float separationDistance = 1f;

    // TODO: update class Spawn with these two variables
    private Dictionary<Spawn, GameObject> leaderPerSpawn;
    private Dictionary<Spawn, List<GameObject>> boidsPerSpawn;

    private List<GameObject> units;

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
        if (geneticExecution)
        {
            Time.timeScale = 5f;
            Spawn s = spawns[0];
            remainingUnits = unitsCount;
            for (int i = 0; i < unitsCount; i++)
            {
                AddUnitToSpawn(s.gameObject);
            }
            BoidManager.Instance.cohesionWeight = UnityEngine.Random.Range(cohesionWeightMin, cohesionWeightMax);
            BoidManager.Instance.separationWeight = UnityEngine.Random.Range(separationWeightMin, separationWeightMax);
            BoidManager.Instance.alignmentWeight = UnityEngine.Random.Range(alignmentWeightMin, alignmentWeightMax);
            BoidManager.Instance.leaderInfluence = UnityEngine.Random.Range(leaderInfluenceMin, leaderInfluenceMax);
            BoidManager.Instance.wallInfluence = UnityEngine.Random.Range(wallInfluenceMin, wallInfluenceMax);
            remainingUnits = unitsCount;
            units = new();
            boidsPerSpawn = new();
            leaderPerSpawn = new(); 
            StartGame();
            return;
        }

        unitsCount_UI.text = unitsCount.ToString() + "/" + unitsCount.ToString();
        remainingUnits = unitsCount;

        boidsPerSpawn = new();
        leaderPerSpawn = new();
    }

    private void Update()
    {
        //foreach (var s in spawns)
        //{
        //    // TODO: not secure
        //    if (leaderPerSpawn[s].GetComponent<AI>().currentState == AI.State.Attack)
        //    {
        //        foreach (var b in boidsPerSpawn[s])
        //        {
        //            b.GetComponent<Boid>().isMoving = false;
        //        }
        //    }
        //}
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public void StartGame()
    {
        canva_UI.SetActive(false);

        list = new List<TileInfos>();

        SetGraph();

        foreach (var s in spawns)
        {
            if (s.unitsCount == 0)
            {
                continue;
            }

            var (_, parent) = Dijkstra.Compute(graph, s.gameObject);
            var pathAI = Dijkstra.GetPath(parent, endNode);
            pathAI.RemoveAt(0);

            // ---- better

            GameObject AIUnit = Instantiate(unitPrefab, s.gameObject.transform.position + (Vector3)UnityEngine.Random.insideUnitCircle * 2, Quaternion.identity);
            if (!AIUnit.TryGetComponent(out UnitManager AIUnitScript))
            {
                Debug.LogError($"{AIUnit} doesn't have a UnitManager script.");
                return;
            }
            units.Add(AIUnit);
            AIUnit.transform.SetParent(unitsParent.transform);
            BoidManager.Instance.boids.Add(AIUnitScript);

            List<GameObject> boids = new();
            for (int i = 0; i < s.unitsCount - 1; i++)
            {
                GameObject unit = Instantiate(unitPrefab, s.gameObject.transform.position + (Vector3)UnityEngine.Random.insideUnitCircle * 2, Quaternion.identity);
                if (!unit.TryGetComponent(out UnitManager unitScript))
                {
                    Debug.LogError($"{unit} doesn't have a UnitManager script.");
                    return;
                }
                units.Add(unit);
                unit.transform.SetParent(unitsParent.transform);

                unitScript.InitializeBoid(pathAI, AIUnit);
                boids.Add(unit);
                BoidManager.Instance.boids.Add(unitScript);
            }

            AIUnitScript.InitializeAI(pathAI, tilemaps, boids);

            // ---- for path finding algos

            //for (int i = 0; i < s.unitsCount; i++)
            //{
            //    GameObject unitAI = Instantiate(unitAIPrefab, s.gameObject.transform.position + (Vector3)UnityEngine.Random.insideUnitCircle * 2, Quaternion.identity);
            //    if (!unitAI.TryGetComponent(out AI unitAIScript))
            //    {
            //        Debug.LogError($"{unitAIPrefab} doesn't have a AI script.");
            //        return;
            //    }
            //    unitAI.transform.SetParent(unitsParent.transform);
            //    unitAIScript.SetTilemaps(tilemaps);
            //    unitAIScript.SetTargets(pathAI);
            //}

            // ---- for boids :

            //GameObject unit = Instantiate(unitAIPrefab, s.gameObject.transform.position + (Vector3)UnityEngine.Random.insideUnitCircle * 2, Quaternion.identity);
            //AI unitScript = unit.AddComponent<AI>();

            //unit.transform.SetParent(unitsParent.transform);

            //var path = Dijkstra.GetPath(parent, endNode);
            //path.RemoveAt(0);
            //unitScript.SetTilemaps(tilemaps);
            //unitScript.SetTargets(path);


            //List<GameObject> boids = new();

            //for (int i = 0; i < s.unitsCount - 1; i++)
            //{
            //    GameObject unitBoid = Instantiate(unitBoidPrefab, s.gameObject.transform.position + (Vector3)UnityEngine.Random.insideUnitCircle * 2, Quaternion.identity);
            //    Boid boidScript = unitBoid.AddComponent<Boid>();

            //    unitBoid.transform.SetParent(unitsParent.transform);

            //    boidScript.target = unit.transform;
            //    boidScript.velocity = UnityEngine.Random.insideUnitCircle;

            //    boids.Add(unitBoid);
            //    BoidManager.Instance.boids.Add(boidScript);
            //}

            ////TODO: not secure
            //unit.GetComponent<AI>().boids = boids;
        }
    }

    public void Restart()
    {
        int totalUnits = 0;
        foreach (var s in spawns)
        {
            var (_, parent) = Dijkstra.Compute(graph, s.gameObject);
            var pathAI = Dijkstra.GetPath(parent, endNode);
            pathAI.RemoveAt(0);

            for (int i = 0; i < s.unitsCount; i++)
            {
                units[totalUnits + i].transform.position = s.gameObject.transform.position + (Vector3)UnityEngine.Random.insideUnitCircle * 2;
                if (!units[totalUnits + i].TryGetComponent(out UnitManager unitScript)) continue;
                unitScript.Restart(pathAI);
            }
            totalUnits += s.unitsCount;
        }
           
        foreach (GameObject node in nodes)
        {
            if (!node.TryGetComponent(out Defense defenseScript)) continue;

            defenseScript.Restart();
        }
    }

    //public void AddBoid(Spawn spawner, GameObject boid)
    //{
    //    if (!boidsPerSpawn.ContainsKey(spawner))
    //    {
    //        boidsPerSpawn[spawner] = new();
    //    }

    //    boidsPerSpawn[spawner].Add(boid);
    //}

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
            // V�rifiez que parent n'est pas null
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
