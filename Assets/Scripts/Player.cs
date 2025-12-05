using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UIElements;

public class Player : MonoBehaviour
{
    [SerializeField] private int damageDealt;
    [SerializeField] private float delayBetweenAttacks;

    private Animator animator;
    private Tilemap[] tilemaps;
    private GameObject spawn;
    private List<GameObject> m_targets = new();
    private GameObject currentTarget;
    private bool canAttack = true;
    private List<TileInfos> movementPath = new();

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        animator = transform.GetChild(0).GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        if (currentTarget != null)
        {
            float dist = Vector3.Distance(currentTarget.transform.position, transform.position);
            if (dist < 2)
            {
                animator.SetBool("isMoving", false);
                Attack(currentTarget);
            } else
            {
                animator.SetBool("isMoving", true);
                Debug.Log("count: " + movementPath.Count);
                if (movementPath.Count > 0)
                {
                    MoveUnitTo(movementPath[0].parent.Value + new Vector2(0.5f, 0.5f));
                }
            }
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
            animator.Play("Attack");
            canAttack = false;
            StartCoroutine(DelayBetweenAttacks());
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
            transform.position += dir * Time.deltaTime * 5;
        } else
        {
            movementPath.RemoveAt(0);
        }

    }

    IEnumerator DelayBetweenAttacks()
    {
        yield return new WaitForSeconds(delayBetweenAttacks);
        canAttack = true;
    }
}
