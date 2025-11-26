using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class Player : MonoBehaviour
{
    [SerializeField] private int damageDealt;
    [SerializeField] private float delayBetweenAttacks;


    private GameObject spawn;
    private List<GameObject> targets;
    private GameObject currentTarget;
    private bool canAttack = true;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        targets = new List<GameObject>();
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
            if (dist < 1)
            {
                Attack(currentTarget);
            }

            // move tmp
            Move();
        }

    }

    public void SetSpawn(GameObject spawn)
    {
        this.spawn = spawn;
    }

    public void SetTargets(List<GameObject> targets)
    {
        this.targets = targets;

        this.currentTarget = targets[0];
    }

    private void Move()
    {
        Vector3 dir = Vector3.Normalize(currentTarget.transform.position - transform.position);
        transform.position += dir * Time.deltaTime;
    }

    public void Attack(GameObject defense)
    {
        if (!defense.TryGetComponent(out Defense defenseScript)) 
        {
            //Debug.LogError($"Attack impossible! {defense} is not a defense.");
            return;
        }

        canAttack = false;
        defenseScript.TakeDamage(damageDealt);
        if (defenseScript.IsDead())
        {
            ChangeTarget();
        }

        StartCoroutine(DelayBetweenAttacks());
    }

    void ChangeTarget()
    {
        targets.RemoveAt(0);
        currentTarget = targets.Count > 0 ? targets[0] : null;
        Debug.Log($"change target : {currentTarget}");
    }

    IEnumerator DelayBetweenAttacks()
    {
        yield return new WaitForSeconds(delayBetweenAttacks);
        canAttack = true;
    }

    //IEnumerator MoveUnits()
    //{
    //    while (currentTarget != null)
    //    {
    //        yield return new WaitForSeconds(1);
    //    }
    //}
}
