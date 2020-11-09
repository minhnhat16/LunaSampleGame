using UnityEngine;

public class Shroom : MonoBehaviour
{
    public GameObject particle;
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            Instantiate(particle, transform.position, Quaternion.identity);
            gameObject.SetActive(false);
        }
    }
}
