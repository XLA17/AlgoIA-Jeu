using NUnit.Framework;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Boid : MonoBehaviour
{
    [SerializeField] private int damageDealt;
    [SerializeField] private float delayBetweenAttacks;
    [SerializeField] private float speed = 2f;

    public Vector3 velocity;
    public bool mustAttack = false;
    public bool isMoving = true;
    public Transform leader; // TODO: change name
    public Animator animator;

    private LancerAnimationHandler animationHandler;
    private bool canAttack = false;

    private void Start()
    {
        animator = transform.GetChild(0).GetComponent<Animator>();
        animationHandler = transform.GetChild(0).GetComponent<LancerAnimationHandler>();
        animator.SetBool("isMoving", true);
    }

    void Update()
    {
        canAttack = !animationHandler.attackAnimationIsPlaying;

        if (mustAttack)
        {
            float dist = Vector3.Distance(leader.transform.position, transform.position);
            if (dist < 2f)
            {
                Attack();
                isMoving = false;
                animator.SetBool("isMoving", false);
            }
        }

        if (isMoving)
        {
            Vector3 accel = Vector3.zero;

            accel += (BoidManager.Instance.Cohesion(this) * BoidManager.Instance.cohesionWeight);

            accel += (BoidManager.Instance.Separation(this, leader) * BoidManager.Instance.separationWeight);

            accel += (BoidManager.Instance.Alignment(this) * BoidManager.Instance.alignmentWeight);

            accel += (BoidManager.Instance.LeaderInfluence(this, leader) * BoidManager.Instance.leaderInfluence);
            //accel += (leader.position - transform.position).normalized * BoidManager.Instance.leaderInfluence;

            accel += (BoidManager.Instance.WallInfluence(this) * BoidManager.Instance.wallInfluence);

            velocity += accel * Time.deltaTime;
            velocity = (Vector2)velocity.normalized * speed;

            transform.position += velocity * Time.deltaTime;
        }
    }

    void Attack()
    {
        if (!leader.TryGetComponent(out Defense defenseScript))
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
                mustAttack = false;
            }
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(transform.position, BoidManager.Instance.cohesionDistance);
        Gizmos.DrawWireSphere(transform.position, BoidManager.Instance.separationDistance);
        Gizmos.DrawWireSphere(transform.position, BoidManager.Instance.alignmentDistance);
    }
}

