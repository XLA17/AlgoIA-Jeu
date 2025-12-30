using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Tilemaps;

public class BoidManager : MonoBehaviour
{
    public static BoidManager Instance;

    //public int boidCount = 50;
    //public float spawnRadius = 1f;
    [SerializeField] private Tilemap[] tilemaps; 

    public float spriteWidth = 0.5f;

    public float cohesionWeight = 10f;
    public float separationWeight = 10000f;
    public float alignmentWeight = 10f;
    public float leaderInfluence = 100f;
    public float wallInfluence = 100000f;

    public float cohesionDistance = 10f;
    public float separationDistance = 1f;
    public float alignmentDistance = 2f;
    public float wallDistance = 1f;

    public float speed = 2f;

    // public List<Boid> boids = new List<Boid>();
    public List<UnitManager> boids = new();

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

    public Vector3 Cohesion(UnitManager b)
    {
        Vector3 center = Vector3.zero;
        int count = 0;

        foreach (var other in boids)
        {
            if (other == b) continue;
            if (Distance(other.transform.position, b.transform.position) < cohesionDistance)
            {
                center += other.transform.position;
                count++;
            }
        }

        if (count == 0) return Vector3.zero;

        center /= count;
        return (center - b.transform.position).normalized;
    }

    public Vector3 Separation(UnitManager b, Transform leader)
    {
        Vector3 force = Vector3.zero;

        foreach (var other in boids)
        {
            if (other == b) continue;

            float d = Distance(other.transform.position, b.transform.position);
            if (d < separationDistance + spriteWidth)
                force += (b.transform.position - other.transform.position).normalized / d;
        }

        float dLeader = Distance(b.transform.position, leader.transform.position);
        if (dLeader < separationDistance + spriteWidth)
            force += (b.transform.position - leader.transform.position).normalized / dLeader;

        return force;
    }

    public Vector3 Alignment(UnitManager b)
    {
        Vector3 avgDir = Vector3.zero;
        int count = 0;

        foreach (var other in boids)
        {
            if (other == b) continue;
            if (Distance(other.transform.position, b.transform.position) < alignmentDistance)
            {
                avgDir += other.boidVelocity;
                count++;
            }
        }

        if (count == 0) return Vector3.zero;

        return avgDir.normalized;
    }

    public Vector3 LeaderInfluence(UnitManager b, Transform leader)
    {
        Vector3 force = (leader.position - b.transform.position).normalized;

        return force;
    }

    public Vector3 WallInfluence(UnitManager b)
    {
        Vector3 force = new();

        bool right = false;
        Vector3 otherPosR = b.transform.position + new Vector3(spriteWidth, 0, 0);
        bool left = false;
        Vector3 otherPosL = b.transform.position + new Vector3(-spriteWidth, 0, 0);
        bool up = false;
        Vector3 otherPosU = b.transform.position + new Vector3(0, spriteWidth, 0);
        bool down = false;
        Vector3 otherPosD = b.transform.position + new Vector3(0, -spriteWidth, 0);

        foreach (var t in tilemaps)
        {
            Vector3Int cellPos = t.WorldToCell(otherPosR);
            if (t.HasTile(cellPos))
            {
                right = true;
                break;
            }
            
        }

        foreach (var t in tilemaps)
        {
            Vector3Int cellPos = t.WorldToCell(otherPosL);
            if (t.HasTile(cellPos))
            {
                left = true;
                break;
            }

        }

        foreach (var t in tilemaps)
        {
            Vector3Int cellPos = t.WorldToCell(otherPosU);
            if (t.HasTile(cellPos))
            {
                up = true;
                break;
            }

        }

        foreach (var t in tilemaps)
        {
            Vector3Int cellPos = t.WorldToCell(otherPosD);
            if (t.HasTile(cellPos))
            {
                down = true;
                break;
            }

        }

        if (!right)
        {
            force += Vector3.left;
        }
        if (!left)
        {
            force += Vector3.right;
        }
        if (!up)
        {
            force += Vector3.down;
        }
        if (!down)
        {
            force += Vector3.up;
        }

        return force;
    }
}
