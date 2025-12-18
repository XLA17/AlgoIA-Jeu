using UnityEngine;

public class Health : MonoBehaviour
{
    [SerializeField] private int maxHealth;
    [SerializeField] private GameObject deadSkull;

    private int health;

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
        Debug.Log(gameObject + " take damage : " + damage);

        health -= damage;

        if (health < 0)
        {
            gameObject.SetActive(false);
            transform.position = new Vector3(-100000, -100000, 0);
            Instantiate(deadSkull, transform.position, Quaternion.identity);
        }
    }
}
