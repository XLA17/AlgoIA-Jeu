using UnityEngine;
using System.Collections.Generic;

public class BoidManager : MonoBehaviour
{
    public Boid boidPrefab;
    public Transform leader;

    public int boidCount = 50;
    public float spawnRadius = 1f;

    public float cohesionWeight = 1f;
    public float separationWeight = 1f;
    public float alignmentWeight = 1f;
    public float leaderInfluence = 1000f;

    public float neighborDistance = 1f;
    public float separationDistance = 1f;

    public float speed = 2f;

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

    public Vector3 Cohesion(Boid b)
    {
        Vector3 center = Vector3.zero;
        int count = 0;

        foreach (var other in boids)
        {
            if (other == b) continue;
        //transform.forward = velocity;
            if (Distance(other, b) < separationDistance)
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
            if (Distance(other, b) < neighborDistance)
            {
                avgDir += other.velocity;
                count++;
            }
        }

        if (count == 0) return Vector3.zero;

        return avgDir.normalized;
    }
}
