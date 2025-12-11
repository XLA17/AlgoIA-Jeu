using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Boid : MonoBehaviour
{
    public Vector3 velocity;
    public float speed = 5f;

    [HideInInspector] public Transform leader;
    [HideInInspector] public BoidManager manager;

    void Update()
    {
        Vector3 accel = Vector3.zero;

        // --- RÈGLE 1 : Cohésion ---
        accel += (manager.Cohesion(this) * manager.cohesionWeight);

        // --- RÈGLE 2 : Séparation ---
        accel += (manager.Separation(this) * manager.separationWeight);

        // --- RÈGLE 3 : Alignement ---
        accel += (manager.Alignment(this) * manager.alignmentWeight);

        // --- RÈGLE 4 (optionnelle) : Attraction vers le leader ---
        accel += (leader.position - transform.position).normalized * manager.leaderInfluence;

        // Mise à jour vitesse / position
        velocity += accel * Time.deltaTime;
        velocity = (Vector2)velocity.normalized * speed;

        transform.position += velocity * Time.deltaTime;
    }
}

