using UnityEngine;

public class LeaderController : MonoBehaviour
{
    public float speed = 8f;

    void Update()
    {
        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");

        Vector3 dir = new Vector3(h, v, 0);

        transform.position += dir * speed * Time.deltaTime;
    }
}

