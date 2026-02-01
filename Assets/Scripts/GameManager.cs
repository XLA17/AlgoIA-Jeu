using System;
using System.Collections.Generic;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Evaluation
{
    public BoidGenome genome;
    public float evaluation;

    public Evaluation(BoidGenome genome, float evaluation)
    {
        this.genome = genome;
        this.evaluation = evaluation;
    }
}

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

    [Space]

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

    private Dictionary<Spawn, GameObject> leaderPerSpawn;
    private Dictionary<Spawn, List<GameObject>> boidsPerSpawn;

    private List<GameObject> units;

    private int cycle = 0;
    private int epoch = 1;
    private List<Evaluation> evaluations;

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
            Time.timeScale = 20f;
            Spawn s = spawns[0];
            remainingUnits = unitsCount;
            for (int i = 0; i < unitsCount; i++)
            {
                AddUnitToSpawn(s.gameObject);
            }
            remainingUnits = unitsCount;
            units = new();
            boidsPerSpawn = new();
            leaderPerSpawn = new();
            evaluations = new();
            for (int i = 0; i < 100; i++)
            {
                BoidGenome g = Genetic.CreateRandomGenome();

                evaluations.Add(new(g, -1f));
            }
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
    }

    public void StartGame()
    {
        Debug.Log("test");
        canva_UI.SetActive(false);

        list = new List<TileInfos>();

        SetGraph();

        // set boid parameters
        if (geneticExecution)
        {
            BoidManager.Instance.cohesionWeight = evaluations[cycle].genome.cohesionWeight;
            BoidManager.Instance.alignmentWeight = evaluations[cycle].genome.alignmentWeight;
            BoidManager.Instance.leaderInfluence = evaluations[cycle].genome.leaderInfluence;
        }

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

            //unit.GetComponent<AI>().boids = boids;
        }
    }

    public void Restart()
    {
        Evaluate();

        cycle++;
        if (cycle == 100)
        {
            cycle = 10; // back at 10 becaause we keep the 10 best and we don't rerun them
            epoch++;
            evaluations.Sort((a, b) => a.evaluation.CompareTo(b.evaluation));
            Debug.Log("Les 10 meilleurs sont : ");
            for (int i = 0; i < 10; i++)
            {
                Debug.Log("  value : " + evaluations[i].evaluation + " ---- cohesion : " + evaluations[i].genome.cohesionWeight + " ---- alignment : " + evaluations[i].genome.alignmentWeight + " ---- leader : " + evaluations[i].genome.leaderInfluence);
            }

            for (int i = 0; i < 90; i++)
            {
                int r1 = UnityEngine.Random.Range(0, 10);
                int r2 = UnityEngine.Random.Range(0, 10);
                BoidGenome g = Genetic.Crossover(evaluations[r1].genome, evaluations[r2].genome);
                Genetic.Mutate(g);
                evaluations[cycle + i].genome = g;
                evaluations[cycle + i].evaluation = -1f;
            }
        }
        BoidManager.Instance.cohesionWeight = evaluations[cycle].genome.cohesionWeight;
        BoidManager.Instance.alignmentWeight = evaluations[cycle].genome.alignmentWeight;
        BoidManager.Instance.leaderInfluence = evaluations[cycle].genome.leaderInfluence;

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

    public void Evaluate()
    {
        float totDist = 0f;
        foreach (GameObject unit in units)
        {
            totDist += Vector3.Distance(unit.transform.position, endNode.transform.position);
        }

        Debug.Log("Epoch : " + epoch + " --- Cycle : " + cycle + " --- Evaluation : " + totDist);
        if (evaluations[cycle].genome.cohesionWeight != BoidManager.Instance.cohesionWeight || evaluations[cycle].genome.alignmentWeight != BoidManager.Instance.alignmentWeight || evaluations[cycle].genome.leaderInfluence != BoidManager.Instance.leaderInfluence) Debug.LogError("Ce ne sont pas les memes valeurs, il y a donc un pb");
        evaluations[cycle].evaluation = totDist;
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
