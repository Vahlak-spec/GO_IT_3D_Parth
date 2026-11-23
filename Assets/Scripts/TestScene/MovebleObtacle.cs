using UnityEngine;

public class MovebleObtacle : MonoBehaviour
{
    [SerializeField] private Transform[] points;
    [SerializeField] private float speed = 1f;

    private int currentPointIndex = 0;

    private void Update()
    {
        transform.position = Vector3.MoveTowards(transform.position, points[currentPointIndex].position, speed * Time.deltaTime);

        if (Vector3.Distance(transform.position, points[currentPointIndex].position) < 0.1f)
        {
            currentPointIndex = (currentPointIndex + 1) % points.Length;
        }
    }
}
