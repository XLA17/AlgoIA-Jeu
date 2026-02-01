using UnityEngine;

[System.Serializable]
public class BoidGenome
{
    public float cohesionWeight;
    public float alignmentWeight;
    public float leaderInfluence;

    public BoidGenome(float cohesion, float alignment, float leader)
    {
        cohesionWeight = cohesion;
        alignmentWeight = alignment;
        leaderInfluence = leader;
    }
}


public class Genetic : MonoBehaviour
{
    const float MAX_WEIGHT = 10000f; // separation weight and wall influence are at 100000

    public static BoidGenome CreateRandomGenome()
    {
        float cohesionWeight = Random.Range(0f, MAX_WEIGHT);
        float alignmentWeight = Random.Range(0f, MAX_WEIGHT);
        float leaderInfluence = Random.Range(0f, MAX_WEIGHT);

        return new BoidGenome(cohesionWeight, alignmentWeight, leaderInfluence);
    }

    public static BoidGenome Crossover(BoidGenome a, BoidGenome b)
    {
        float cohesionWeight = Random.value < 0.5f ? a.cohesionWeight : b.cohesionWeight;
        float alignmentWeight = Random.value < 0.5f ? a.alignmentWeight : b.alignmentWeight;
        float leaderInfluence = Random.value < 0.5f ? a.leaderInfluence : b.leaderInfluence;

        return new BoidGenome(cohesionWeight, alignmentWeight, leaderInfluence);
    }

    public static void Mutate(BoidGenome g)
    {
        float mutationRate = 0.2f;

        if (Random.value < mutationRate) g.cohesionWeight = Mathf.Clamp(g.cohesionWeight + Random.Range(-2000f, 2000f), 0f, MAX_WEIGHT);
        if (Random.value < mutationRate) g.alignmentWeight = Mathf.Clamp(g.alignmentWeight + Random.Range(-2000f, 2000f), 0f, MAX_WEIGHT);
        if (Random.value < mutationRate) g.leaderInfluence = Mathf.Clamp(g.leaderInfluence + Random.Range(-2000f, 2000f), 0f, MAX_WEIGHT);
    }
}
