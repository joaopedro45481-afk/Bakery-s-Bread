using UnityEngine;

public class CustomerController : MonoBehaviour
{
    public Transform target;
    public float speed = 3f;

    void Update()
    {
        transform.position = Vector2.MoveTowards(
            transform.position,
            target.position,
            speed * Time.deltaTime
        );
    }
}