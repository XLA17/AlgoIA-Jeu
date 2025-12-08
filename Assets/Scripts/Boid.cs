using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Boid : MonoBehaviour
{
    private Vector2 position;
    private Vector2 velocity;

    public Boid(Vector2 position)
    {
        this.position = position;
        velocity = Vector2.zero;
    }

    void Update()
    {

    }

    public float Distance(Boid boid)
    {
        float distX = position.x - boid.position.x;
        float distY = position.y - boid.position.y;
        return Mathf.Sqrt(distX * distX + distY * distY);
    }

    public void MoveCloser(List<Boid> boids)
    {
        if (boids.Count < 1) return;

        float avgX = 0f;
        float avgY = 0f;

        foreach (Boid boid in boids)
        {
            if (boid.position.x == position.x && boid.position.y == position.y)
            {
                continue;
            }
            avgX += position.x - boid.position.x;
            avgY += position.y - boid.position.y;
        }
        avgX /= boids.Count;
        avgY /= boids.Count;

        velocity.x -= avgX / 100;
        velocity.y -= avgY / 100;
    }

    public void MoveWith(List<Boid> boids)
    {
        if (boids.Count < 1) return;

        float avgX = 0f;
        float avgY = 0f;

        foreach (Boid boid in boids)
        {
            avgX += boid.position.x;
            avgY += boid.position.y;
        }
        avgX /= boids.Count;
        avgY /= boids.Count;

        velocity.x += avgX / 40;
        velocity.y += avgY / 40;
    }

    public void MoveAway(List<Boid> boids, float minDistance)
    {
        if (boids.Count < 1) return;

        float distX = 0f;
        float distY = 0f;
        float numClose = 0f;

        foreach (Boid boid in boids)
        {
            float distance = Distance(boid);

            if (distance < minDistance)
            {
                numClose += 1;
                float xDiff = (position.x - boid.position.x);
                float yDiff = (position.y - boid.position.y);
                if (xDiff >= 0)
                {
                    xDiff = Mathf.Sqrt(minDistance) - xDiff;
                }
                else
                {
                    xDiff = -Mathf.Sqrt(minDistance) - xDiff;
                }
                if (yDiff >= 0)
                {
                    yDiff = Mathf.Sqrt(minDistance) - yDiff;
                }
                else
                {
                    yDiff = -Mathf.Sqrt(minDistance) - yDiff;
                }
                distX += xDiff;
                distY += yDiff;
            }
        }

        if (numClose == 0) {
            return;
        }
        velocity.x -= distX / 5;
        velocity.y -= distY / 5;
    }

    public void Move()
    {
        if (Mathf.Abs(velocity.x) > float.MaxValue || Mathf.Abs(velocity.y) > float.MaxValue)
        {
            float scaleFactor = float.MaxValue / Mathf.Max(Mathf.Abs(velocity.x), Mathf.Abs(velocity.y));
            velocity.x *= scaleFactor;
            velocity.y *= scaleFactor;
        }
        position.x += velocity.x;
        position.y += velocity.y;
    }
}
