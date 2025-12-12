using UnityEngine;
using System.Collections.Generic;

public class BoidManager : MonoBehaviour
{
    public static BoidManager Instance;

    public int boidCount = 50;
    public float spawnRadius = 1f;

    public float cohesionWeight = 1f;
    public float separationWeight = 1f;
    public float alignmentWeight = 1f;
    public float leaderInfluence = 100f;

    public float neighborDistance = 1f;
    public float separationDistance = 1f;

    public float speed = 2f;

    public List<Boid> boids = new List<Boid>();

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public float Distance(Vector3 v1, Vector3 v2)
    {
        float distX = v2.x - v1.x;
        float distY = v2.y - v1.y;
        return Mathf.Sqrt(distX * distX + distY * distY);
    }

    public Vector3 Cohesion(Boid b)
    {
        Vector3 center = Vector3.zero;
        int count = 0;

        foreach (var other in boids)
        {
            if (other == b) continue;
            if (Distance(other.transform.position, b.transform.position) < separationDistance)
            {
                center += other.transform.position;
                count++;
            }
        }

        if (count == 0) return Vector3.zero;

        center /= count;
        return (center - b.transform.position).normalized;
    }

    public Vector3 Separation(Boid b, Transform leader)
    {
        Vector3 force = Vector3.zero;

        foreach (var other in boids)
        {
            if (other == b) continue;

            float d = Distance(other.transform.position, b.transform.position);
            if (d < separationDistance)
                force += (b.transform.position - other.transform.position).normalized / d;
        }

        float dLeader = Distance(b.transform.position, leader.transform.position);
        if (dLeader < separationDistance)
            force += (b.transform.position - leader.transform.position).normalized / dLeader;

        return force;
    }

    public Vector3 Alignment(Boid b)
    {
        Vector3 avgDir = Vector3.zero;
        int count = 0;

        foreach (var other in boids)
        {
            if (other == b) continue;
            if (Distance(other.transform.position, b.transform.position) < neighborDistance)
            {
                avgDir += other.velocity;
                count++;
            }
        }

        if (count == 0) return Vector3.zero;

        return avgDir.normalized;
    }

    public Vector3 LeaderInfluence(Boid b, Transform leader)
    {
        Vector3 force = (leader.position - b.transform.position).normalized;

        return force;
    }
}
