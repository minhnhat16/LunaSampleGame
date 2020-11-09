using UnityEngine;

public class PlatformScript : MonoBehaviour
{

    public float jumpForce = 10f;
    
    private void OnCollisionEnter2D(Collision2D collision)
    {
//swap if statement with relativeVelocity condition with below commented line
        if(collision.rigidbody.velocity.y <= 1)
        {
            Rigidbody2D rb = collision.collider.GetComponent<Rigidbody2D>();
            if (rb != null)
            {
                Vector2 velocity = rb.velocity;
                velocity.y = 0;
                velocity.y = jumpForce;
                rb.velocity = velocity;
            }
        }
    }
}

//replace with this 
//collision.rigidbody.velocity.y <= 1

//this doesnt work in condition
//collision.relativeVelocity.y <= 0 