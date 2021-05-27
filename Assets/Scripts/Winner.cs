using Luna.Unity;
using UnityEngine;

public class Winner : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log(other.name);
        if (other.gameObject.CompareTag("Player"))
        {
            GameManager.instance.endDescription.text = "You Win!";
            GameManager.instance.StartCoroutine("ShowEndCard");
            Analytics.LogEvent(Analytics.EventType.LevelWon);
        }
    }
}
