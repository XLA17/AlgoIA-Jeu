using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Node : MonoBehaviour
{
    [SerializeField] private List<GameObject> nextTowers;
    [SerializeField] private float value;

    public List<GameObject> GetNextTowers()
    {
        return nextTowers;
    }

    public float GetValue()
    {
        return value;
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnDrawGizmos()
    {
        foreach (GameObject o in nextTowers)
        {
            Gizmos.color = Color.red;
            Vector3 direction = o.transform.position - transform.position;
            Gizmos.DrawLine(transform.position, o.transform.position);

            // calcul des "ailes" de la flèche
            Vector3 right = Quaternion.LookRotation(direction) * Quaternion.Euler(0, 180 + 20f, 0) * Vector3.forward;
            Vector3 left = Quaternion.LookRotation(direction) * Quaternion.Euler(0, 180 - 20f, 0) * Vector3.forward;

            Gizmos.DrawLine(o.transform.position, o.transform.position + right * 0.5f);
            Gizmos.DrawLine(o.transform.position, o.transform.position + left * 0.5f);
        }
    }
}
