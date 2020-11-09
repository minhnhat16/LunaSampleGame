using UnityEngine;

public class CamraFollow : MonoBehaviour
{

    public Transform target;
    public float smoothSpeed = 0.3f;

    void FixedUpdate()
    {
        if (target.position.y > transform.position.y)
        {
            Vector3 position = transform.position;
            Vector3 newPos = new Vector3(position.x, target.position.y, position.z);
            position = Vector3.Lerp(position, newPos, smoothSpeed * Time.deltaTime);
            transform.position = position;
        }
    }
    
}
