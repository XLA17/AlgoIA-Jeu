using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Boid : MonoBehaviour
{
    public Vector3 velocity;
    public float speed = 2f;

    public Transform leader;

    void Update()
    {
        Vector3 accel = Vector3.zero;

        // --- RÈGLE 1 : Cohésion ---
        accel += (BoidManager.Instance.Cohesion(this) * BoidManager.Instance.cohesionWeight);

        // --- RÈGLE 2 : Séparation ---
        accel += (BoidManager.Instance.Separation(this, leader) * BoidManager.Instance.separationWeight);

        // --- RÈGLE 3 : Alignement ---
        accel += (BoidManager.Instance.Alignment(this) * BoidManager.Instance.alignmentWeight);

        // --- RÈGLE 4 (optionnelle) : Attraction vers le leader ---
        accel += (BoidManager.Instance.LeaderInfluence(this, leader) * BoidManager.Instance.leaderInfluence);
        accel += (leader.position - transform.position).normalized * BoidManager.Instance.leaderInfluence;


        // Mise à jour vitesse / position
        velocity += accel * Time.deltaTime;
        velocity = (Vector2)velocity.normalized * speed;

        transform.position += velocity * Time.deltaTime;
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(transform.position, BoidManager.Instance.cohesionDistance);
        Gizmos.DrawWireSphere(transform.position, BoidManager.Instance.separationDistance);
        Gizmos.DrawWireSphere(transform.position, BoidManager.Instance.alignmentDistance);
    }
}

