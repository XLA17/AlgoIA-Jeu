using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UIElements;

public class AI : MonoBehaviour
{
    [SerializeField] private int damageDealt;
    [SerializeField] private float delayBetweenAttacks;
    [SerializeField] private float speed = 2f;

    private Animator animator;
    private LancerAnimationHandler animationHandler;
    private Tilemap[] tilemaps;
    private GameObject spawn;
    private List<GameObject> m_targets = new();
    private GameObject currentTarget;
    private bool canAttack = true;
    private List<TileInfos> movementPath = new();

    private State currentState;

    enum State {
        Idle,
        Move,
        Attack
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        animator = transform.GetChild(0).GetComponent<Animator>();
        animationHandler = transform.GetChild(0).GetComponent<LancerAnimationHandler>();
        animator.SetBool("isMoving", true);
        currentState = State.Move;
    }

    // Update is called once per frame
    void Update()
    {
        canAttack = !animationHandler.attackAnimationIsPlaying;

        switch (currentState)
        {
            case State.Idle:
                // there is no tower to destroy -> the game is finished
                break;
            case State.Move:
                float dist = Vector3.Distance(currentTarget.transform.position, transform.position);
                if (dist < 2)
                {
                    animator.SetBool("isMoving", false);
                    currentState = State.Attack;
                    MoveUnitTo(movementPath[0].parent.Value + new Vector2(0.5f, 0.5f));
                    break;
                }

                MoveUnitTo(movementPath[0].parent.Value + new Vector2(0.5f, 0.5f));

                break;
            case State.Attack:
                if (currentTarget == null)
                {
                    currentState = State.Idle;
                    break;
                }

                dist = Vector3.Distance(currentTarget.transform.position, transform.position);
                if (dist > 2)
                {
                    animator.SetBool("isMoving", true);
                    currentState = State.Move;
                    break;
                }
                
                if (canAttack) Attack(currentTarget);
                
                break;
            default:
                break;
        }
    }

    public void SetSpawn(GameObject spawn)
    {
        this.spawn = spawn;
    }

    public void SetTargets(List<GameObject> targets)
    {
        m_targets = targets;

        SetCurrentTarget();
        CalculateAStar();
    }

    public void SetTilemaps(Tilemap[] tilemaps)
    {
        this.tilemaps = tilemaps;
    }

    public void Attack(GameObject defense)
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
            defenseScript.TakeDamage(damageDealt);
            if (defenseScript.IsDead())
            {
                SetCurrentTarget();
            }
        }
    }

    void SetCurrentTarget()
    {
        if (m_targets.Count > 0)
        {
            currentTarget = m_targets[0];
            m_targets.RemoveAt(0);
            Debug.Log($"change target : {currentTarget}");

            CalculateAStar();
        } else
        {
            currentTarget = null;
        }
    }

    void CalculateAStar()
    {
        Debug.Log("target: " + currentTarget);
        movementPath = AStar.Compute(tilemaps, Vector2Int.FloorToInt(transform.position), (Vector2)currentTarget.transform.position);
        Debug.Log("movementPath: " + movementPath.Count);
        movementPath.RemoveAt(0);
    }

    public void MoveUnitTo(Vector2 position)
    {
        if (Vector3.Distance(transform.position, position) >= 0.01f)
        {
            Vector3 dir = ((Vector3)position - transform.position).normalized;
            transform.position += dir * Time.deltaTime * speed;
        } else
        {
            movementPath.RemoveAt(0);
        }

    }

    public void AttackAnimationFinished()
    {
        canAttack = true;
    }
}
