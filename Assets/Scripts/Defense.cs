using UnityEngine;

public class Defense : MonoBehaviour
{
    [SerializeField] private int maxHealth;

    private int health;
    private bool isDead = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        health = maxHealth;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void TakeDamage(int damage)
    {
        if (isDead)
        {
            return;
        }
        health -= damage;
        if (health <= 0)
        {
            health = 0;
            isDead = true;
            gameObject.SetActive(false);
        }
    }

    public bool IsDead()
    {
        return isDead;
    }
}
