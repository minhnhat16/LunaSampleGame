using UnityEngine;
using Random = UnityEngine.Random;

public class LevelGenerator : MonoBehaviour
{

    public GameObject platform;
    public GameObject mushroom;

    public float spawnHeight;
    public int division = 3;

    public int numberOfPlatform;
    public float levelWidth = 3f;
    public float minY = 1f;
    public float maxY = 1.5f;
   
    void Start()
    {
        Vector3 spawnPos = new Vector3(0, spawnHeight,0);
        
        for (int i = 0; i < numberOfPlatform; i++)
        {
            spawnPos.y += Random.Range(minY, maxY);
            spawnPos.x = Random.Range(-levelWidth, levelWidth);
            Instantiate(platform, spawnPos, Quaternion.Euler(0,0,0));
            if (i % division == 0)
            {
                spawnPos.y += 0.4f;
                Instantiate(mushroom, spawnPos, Quaternion.Euler(0,0,0));
            }
        }
        
    }
}
