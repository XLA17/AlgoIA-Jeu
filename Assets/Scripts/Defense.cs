using TMPro;
using UnityEngine;

public class Defense : MonoBehaviour
{
    [SerializeField] private int maxHealth;
    [SerializeField] private TextMeshPro health_UI;
    [SerializeField] private GameObject deadTowerPrefab;

    private int health;
    private bool isDead = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        health = maxHealth;
        health_UI.text = health.ToString();
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

            Instantiate(deadTowerPrefab, gameObject.transform.position, Quaternion.identity);
            gameObject.SetActive(false);
        }

        health_UI.text = health.ToString();
    }

    public bool IsDead()
    {
        return isDead;
    }
}
