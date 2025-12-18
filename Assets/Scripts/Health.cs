using UnityEngine;

public class Health : MonoBehaviour
{
    [SerializeField] private UnitScriptableObject unitScriptableObject;
    [SerializeField] private GameObject deathIcon;

    private int health;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        health = unitScriptableObject.maxHealth;
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
            Instantiate(deathIcon, transform.position, Quaternion.identity);
        }
    }
}
