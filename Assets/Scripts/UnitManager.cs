using System.Collections.Generic;
using System.Linq;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UIElements;

public class UnitManager : MonoBehaviour
{
    [SerializeField] private UnitScriptableObject unitScriptableObject;

    private Tilemap[] m_tilemaps;
    private List<GameObject> m_towerTargets;
    private List<TileInfos> m_tilesToCross;

    private State m_currentState = State.Idle;
    private bool m_isBoid;
    private GameObject m_currentTarget;
    private bool m_canAttack;
    public Vector3 boidVelocity;

    private Animator m_animator;
    private LancerAnimationHandler animationHandler; // TODO: remove

    private List<GameObject> m_boids;

    private enum State
    {
        Idle,
        Move,
        Attack
    }

    void Awake()
    {
        m_animator = transform.GetChild(0).GetComponent<Animator>();
        animationHandler = transform.GetChild(0).GetComponent<LancerAnimationHandler>();
    }

    void Start()
    {
        // TODO: to improve
        // m_animator.SetBool("isMoving", true);
        m_tilesToCross = m_tilesToCross ?? new();
    }

    void Update()
    {
        m_canAttack = !animationHandler.attackAnimationIsPlaying;

        switch (m_currentState)
        {
            case State.Idle:
                // there is no tower left to destroy -> the game is finished
                return;

            case State.Move:
                if (m_isBoid)
                {
                    MoveBoid();
                }
                else
                {
                    if (m_tilesToCross == null || m_tilesToCross.Count == 0) return;

                    MoveAI();
                }
                return;

            case State.Attack:
                if (m_currentTarget == null)
                {
                    m_currentState = State.Idle;
                    m_animator.SetBool("isMoving", false);
                    return;
                }

                Attack(m_currentTarget);

                return;

            default:
                return;
        }
    }

    public void InitializeAI(List<GameObject> towerTargets, Tilemap[] tilemaps, List<GameObject> boids)
    {
        Debug.Log("AI initialization");
        m_isBoid = false;
        m_towerTargets = new List<GameObject>(towerTargets);
        m_tilemaps = tilemaps;
        SetCurrentTarget();
        m_boids = new(boids);
    }

    public void InitializeBoid(List<GameObject> towerTargets, GameObject target)
    {
        Debug.Log("Boid initialization");
        m_isBoid = true;
        m_towerTargets = new List<GameObject>(towerTargets);
        m_currentState = State.Move;
        m_animator.SetBool("isMoving", true);
        m_currentTarget = target;
        // SetCurrentTarget();
        m_boids = new();
    }

    private void MoveAI()
    {
        // Debug.Log("Move AI-----");
        Vector2 centerPos = m_tilesToCross[0].parent.Value + new Vector2(0.5f, 0.5f);
        if (Vector2.Distance(transform.position, centerPos) >= 0.05f)
        {
            Vector3 dir = (centerPos - (Vector2)transform.position).normalized;
            transform.position += dir * Time.deltaTime * unitScriptableObject.speed;
        }
        else
        {
            m_tilesToCross.RemoveAt(0);
        }
        return;
    }

    private void MoveBoid()
    {
        Vector3 accel = Vector3.zero;

        accel += BoidManager.Instance.Cohesion(this) * BoidManager.Instance.cohesionWeight;
        accel += BoidManager.Instance.Separation(this, m_currentTarget.transform) * BoidManager.Instance.separationWeight;
        accel += BoidManager.Instance.Alignment(this) * BoidManager.Instance.alignmentWeight;
        accel += BoidManager.Instance.LeaderInfluence(this, m_currentTarget.transform) * BoidManager.Instance.leaderInfluence;
        accel += BoidManager.Instance.WallInfluence(this) * BoidManager.Instance.wallInfluence;

        boidVelocity += accel * Time.deltaTime;
        boidVelocity = boidVelocity.normalized * unitScriptableObject.speed;

        transform.position += boidVelocity * Time.deltaTime;
    }

    private void Attack(GameObject defense)
    {
        if (!defense.TryGetComponent(out Defense defenseScript))
        {
            Debug.Log("defense dont");
            return;
        }

        if (defenseScript.IsDead() && !m_isBoid)
        {
            SetCurrentTarget();
            if (m_currentTarget == null)
            {
                foreach (var boid in m_boids)
                {
                    if (!boid.TryGetComponent(out UnitManager unitManager)) return;
                    unitManager.m_currentTarget = null;
                    unitManager.m_currentState = State.Idle;
                    m_animator.SetBool("isMoving", false);
                }
            }
            else
            {
                foreach (var boid in m_boids)
                {
                    if (!boid.TryGetComponent(out UnitManager unitManager)) return;
                    unitManager.m_currentTarget = this.gameObject;
                    unitManager.m_currentState = State.Move;
                    m_animator.SetBool("isMoving", true);
                }
            }
        }

        if (m_canAttack)
        {
            Debug.Log("--- can attack is true");
            if (!transform.GetChild(0).TryGetComponent(out LancerAnimationHandler lancerAnimationHandler)) return;
            Debug.Log("----- can attack is true 2222222");
            lancerAnimationHandler.attackAnimationIsPlaying = true;
            m_animator.Play("Attack");
            m_canAttack = false;
            defenseScript.TakeDamage(unitScriptableObject.damageDealt);
        }
    }

    private void SetCurrentTarget()
    {
        if (m_towerTargets.Count > 0)
        {
            m_currentState = State.Move;
            m_animator.SetBool("isMoving", true);
            m_currentTarget = m_towerTargets[0];
            m_towerTargets.RemoveAt(0);
            Debug.Log($"change target : {m_currentTarget}");

            if (!m_isBoid) CalculateAStar();
        }
        else
        {
            Debug.Log("no more targets");
            m_currentState = State.Idle;
            m_currentTarget = null;
        }
    }

    private void CalculateAStar()
    {
        Debug.Log("test---");
        Debug.Log(m_tilemaps);
        Debug.Log(m_tilemaps.Length);
        m_tilesToCross = AStar.Compute(m_tilemaps, Vector2Int.FloorToInt(transform.position), (Vector2)m_currentTarget.transform.position);
        Debug.Log(m_tilesToCross.Count);
        Debug.Log("remove in CalculateAStar astar");
        m_tilesToCross.RemoveAt(0);
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.transform.gameObject == m_currentTarget && m_currentTarget.TryGetComponent(out Defense _))
        {
            Debug.Log("good collision");

            if (!m_isBoid)
            {
                foreach (var boid in m_boids)
                {
                    if (!boid.TryGetComponent(out UnitManager unitManager)) return;
                    unitManager.m_currentTarget = m_currentTarget;
                }
            }

            m_currentState = State.Attack;
            m_animator.SetBool("isMoving", false);
        }
    }
}
