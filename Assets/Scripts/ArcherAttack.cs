using System.Collections;
using UnityEngine;

public class ArcherAttack : MonoBehaviour
{
    [SerializeField] private int damageDealt;

    private Animator animator;
    private bool canAttack = true;

    public void Start()
    {
        animator = transform.GetChild(0).GetComponent<Animator>();
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.gameObject.TryGetComponent<Health>(out Health unitHealthScript) && canAttack)
        {
            canAttack = false;
            animator.Play("Shoot");
            unitHealthScript.TakeDamage(damageDealt);
            StartCoroutine(AttackCoroutine());
        }
    }

    IEnumerator AttackCoroutine()
    {
        yield return new WaitForSeconds(0.8f);
        canAttack = true;
    }
}
