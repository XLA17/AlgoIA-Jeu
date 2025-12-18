using System.Collections.Generic;
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
    private bool m_isBoid = false;
    private GameObject m_currentTarget;
    private bool canAttack;

    private Animator animator;
    private LancerAnimationHandler animationHandler; // TODO: remove

    private enum State
    {
        Idle,
        Move,
        Attack
    }

    void Start()
    {
        // TODO: to improve
        animator = transform.GetChild(0).GetComponent<Animator>();
        animationHandler = transform.GetChild(0).GetComponent<LancerAnimationHandler>();
    }

    private void Update()
    {
        if (m_isBoid)
        {
            UpdateBoid();
        }
        else
        {
            UpdateAI();
        }
    }

    void UpdateBoid()
    {

    }

    void UpdateAI()
    {
        canAttack = !animationHandler.attackAnimationIsPlaying;

        switch (m_currentState)
        {
            case State.Idle:
                // there is no tower to destroy -> the game is finished
                return;

            case State.Move:
                if (Vector2.Distance(transform.position, m_tilesToCross[0].parent.Value) >= 0.01f)
                {
                    MoveToward(m_tilesToCross[0].parent.Value + new Vector2(0.5f, 0.5f));
                }
                else
                {
                    m_tilesToCross.RemoveAt(0);
                }
                return;

            case State.Attack:
                if (m_currentTarget == null)
                {
                    m_currentState = State.Idle;
                    //foreach (var b in boids)
                    //{
                    //    b.GetComponent<Boid>().mustAttack = false;
                    //    b.GetComponent<Boid>().isMoving = false;
                    //    b.GetComponent<Boid>().animator.SetBool("isMoving", false);
                    //}
                    return;
                }

                Attack(m_currentTarget);

                return;

            default:
                return;
        }
    }

    public void InitializeAI(List<GameObject> towerTargets, Tilemap[] tilemaps)
    {
        m_isBoid = false;
        m_towerTargets = towerTargets;
        m_tilemaps = tilemaps;
    }

    public void InitializeBoid()
    {
        m_isBoid = true;
    }

    private void MoveToward(Vector2 position)
    {
        Vector3 dir = (position - (Vector2)transform.position).normalized;
        transform.position += dir * Time.deltaTime * unitScriptableObject.speed;
    }

    private void Attack(GameObject defense)
    {
        if (!defense.TryGetComponent(out Defense defenseScript))
        {
            return;
        }

        if (canAttack)
        {
            if (!transform.GetChild(0).TryGetComponent(out LancerAnimationHandler lancerAnimationHandler))
            {
                return;
            }
            lancerAnimationHandler.attackAnimationIsPlaying = true;
            animator.Play("Attack");
            canAttack = false;
            defenseScript.TakeDamage(unitScriptableObject.damageDealt);
            if (defenseScript.IsDead())
            {
                SetCurrentTarget();
            }
        }
    }

    private void SetCurrentTarget()
    {
        if (m_towerTargets.Count > 0)
        {
            m_currentState = State.Move;
            animator.SetBool("isMoving", true);
            m_currentTarget = m_towerTargets[0];
            m_towerTargets.RemoveAt(0);
            Debug.Log($"change target : {m_currentTarget}");

            CalculateAStar();
        }
        else
        {
            m_currentState = State.Idle;
            m_currentTarget = null;
        }
    }

    private void CalculateAStar()
    {
        m_tilesToCross = AStar.Compute(m_tilemaps, Vector2Int.FloorToInt(transform.position), (Vector2)m_currentTarget.transform.position);
        m_tilesToCross.RemoveAt(0);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.transform.gameObject == m_currentTarget)
        {
            m_currentState = State.Attack;
            animator.SetBool("isMoving", false);
        }
    }
}
