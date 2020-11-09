using System.Collections.Generic;
using UnityEngine;

public class ObjectPool : MonoBehaviour
{
    public static ObjectPool instance;

    public List<GameObject> pooledObjects;
    public GameObject objectToPool;
    public int amountToPool;
    
    void Awake() {
        instance = this;
    }
    
    // Start is called before the first frame update
    void Start()
    {
        pooledObjects = new List<GameObject>();
        
        for (int i = 0; i < amountToPool; i++) 
        {
            GameObject obj = Instantiate(objectToPool);
            obj.SetActive(false); 
            pooledObjects.Add(obj);
        }
    }

    public GameObject GetPooledObject() {

        for (int i = 0; i < pooledObjects.Count; i++) 
        {
            if (!pooledObjects[i].activeInHierarchy) 
            {
                return pooledObjects[i];
            }
        }
        return null;
    }
    
    //Replace instantiate calls with the code below to replace platforms
    
    // GameObject plaformPrefab = ObjectPool.instance.GetPooledObject();
    //     if (plaformPrefab != null)
    // {
    //     plaformPrefab.transform.position = spawnPos;
    //         
    //     plaformPrefab.SetActive(true);
    // }
}
