using System.Collections.Generic;
using UnityEngine;

public class _FlockingManager : MonoBehaviour
{
    [SerializeField] private GameObject boidPrefab;

    private List<Boid> boids;
    private int boidCount = 10;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        boids = new();
        for (int i = 0; i < boidCount; i++)
        {
            var boid = Instantiate(boidPrefab, (Vector3)UnityEngine.Random.insideUnitCircle * 2, Quaternion.identity);
            if (!boid.TryGetComponent(out Boid boidScript))
            {
                Debug.LogError($"{boid} doesn't have a boid script.");
                return;
            }
            boids.Add(boidScript);
        }
    }

    // Update is called once per frame
    void Update()
    {
        //foreach (Boid boid in boids)
        //{
        //    List<Boid> closeBoids = new();

        //    foreach (Boid otherBoid in boids)
        //    {
        //        if (otherBoid == boid) continue;
        //        float distance = boid.Distance(otherBoid);
        //        if (distance < 200)
        //        {
        //            closeBoids.Add(otherBoid);
        //        }
        //    }

        //    boid.MoveCloser(closeBoids);
        //    boid.MoveWith(closeBoids);
        //    boid.MoveAway(closeBoids, 20);

        //    Vector3 screenPos = Camera.main.WorldToViewportPoint(boid.transform.position);

        //    if (screenPos.x < 0f || screenPos.x > 1f)
        //    {
        //        boid.velocity.x = -boid.velocity.x * Random.Range(0.8f, 1.2f);
        //    }

        //    if (screenPos.y < 0f || screenPos.y > 1f)
        //    {
        //        boid.velocity.y = -boid.velocity.y * Random.Range(0.8f, 1.2f);
        //    }

        //    boid.Move();
        //}
    }
}
