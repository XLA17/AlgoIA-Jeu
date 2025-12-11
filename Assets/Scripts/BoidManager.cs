using UnityEngine;
using System.Collections.Generic;

public class BoidManager : MonoBehaviour
{
    public Boid boidPrefab;
    public Transform leader;

    public int boidCount = 50;
    public float spawnRadius = 10f;

    public float cohesionWeight = 0.5f;
    public float separationWeight = 2f;
    public float alignmentWeight = 1f;
    public float leaderInfluence = 3f;

    public float neighborDistance = 5f;
    public float separationDistance = 2f;

    List<Boid> boids = new List<Boid>();

    void Start()
    {
        for (int i = 0; i < boidCount; i++)
        {
            Vector3 pos = transform.position + (Vector3)Random.insideUnitCircle * spawnRadius;
            Boid b = Instantiate(boidPrefab, pos, Quaternion.identity);

            b.manager = this;
            b.leader = leader;
            b.velocity = Random.insideUnitCircle;

            boids.Add(b);
        }
    }

    public float Distance(Boid b1, Boid b2)
    {
        float distX = b2.transform.position.x - b1.transform.position.x;
        float distY = b2.transform.position.y - b1.transform.position.y;
        return Mathf.Sqrt(distX * distX + distY * distY);
    }

    // ----------- COMPORTEMENTS ----------------

    public Vector3 Cohesion(Boid b)
    {
        Vector3 center = Vector3.zero;
        int count = 0;

        foreach (var other in boids)
        {
            if (other == b) continue;
            if ((other.transform.position - b.transform.position).sqrMagnitude < neighborDistance * neighborDistance)
            {
                center += other.transform.position;
                count++;
            }
        }

        if (count == 0) return Vector3.zero;

        center /= count;
        return (center - b.transform.position).normalized;
    }

    public Vector3 Separation(Boid b)
    {
        Vector3 force = Vector3.zero;

        foreach (var other in boids)
        {
            if (other == b) continue;

            float d = Distance(other, b);
            if (d < separationDistance)
                force += (b.transform.position - other.transform.position).normalized / d;
        }

        return force;
    }

    public Vector3 Alignment(Boid b)
    {
        Vector3 avgDir = Vector3.zero;
        int count = 0;

        foreach (var other in boids)
        {
            if (other == b) continue;
            if ((other.transform.position - b.transform.position).sqrMagnitude < neighborDistance * neighborDistance)
            {
                avgDir += other.velocity;
                count++;
            }
        }

        if (count == 0) return Vector3.zero;

        return avgDir.normalized;
    }
}
