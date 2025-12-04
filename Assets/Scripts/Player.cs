using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UIElements;

public class Player : MonoBehaviour
{
    [SerializeField] private int damageDealt;
    [SerializeField] private float delayBetweenAttacks;

    private Tilemap[] tilemaps;
    private GameObject spawn;
    private List<GameObject> m_targets = new();
    private GameObject currentTarget;
    private bool canAttack = true;
    private List<TileInfos> movementPath = new();

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        //if (Input.GetKeyDown(KeyCode.A))
        //{
        //    transform.Translate(new Vector3(-1, 0, 0));
        //}
        //if (Input.GetKeyDown(KeyCode.D))
        //{
        //    transform.Translate(new Vector3(1, 0, 0));
        //}
        //if (Input.GetKeyDown(KeyCode.Z))
        //{
        //    transform.Translate(new Vector3(0, 1, 0));
        //}
        //if (Input.GetKeyDown(KeyCode.S))
        //{
        //    transform.Translate(new Vector3(0, -1, 0));
        //}

        //StartCoroutine(MoveUnits());

        if (currentTarget != null)
        {
            float dist = Vector3.Distance(currentTarget.transform.position, transform.position);
            if (dist < 2)
            {
                Attack(currentTarget);
            } else
            {
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

    private void Move()
    {
        MoveUnits();
        Vector3 dir = Vector3.Normalize(new Vector3(movementPath[0].parent.Value.x - transform.position.x, movementPath[0].parent.Value.y - transform.position.y, 0));
        transform.position += dir * Time.deltaTime *5;
    }

    public void Attack(GameObject defense)
    {
        if (!defense.TryGetComponent(out Defense defenseScript)) 
        {
            //Debug.LogError($"Attack impossible! {defense} is not a defense.");
            return;
        }

        if (canAttack)
        {
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

    IEnumerator MoveUnits()
    {
        while (currentTarget != null)
        {


            yield return new WaitForSeconds(1);
        }
    }
}
