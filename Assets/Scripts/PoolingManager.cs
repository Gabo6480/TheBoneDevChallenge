using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoolingManager : MonoBehaviour
{
    #region Singleton class : PoolingManager
    public static PoolingManager Instance { get; private set; }


    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }

        AfterAwake();
    }

    #endregion

    [System.Serializable]
    struct Pool{
        public GameObject pooledPrefab;
        public int poolSize;
    }

    [SerializeField] List<Pool> bakedPools; 

    Dictionary<string, Queue<GameObject>> poolDictionary = new Dictionary<string, Queue<GameObject>>();

    void AfterAwake()
    {
        foreach(var pool in bakedPools)
        {
            string poolType;
            IPoolable poolable;
            if (TryGetPoolable(pool.pooledPrefab, out poolable))
            {
                poolType = poolable.getName();
            }
            else
                continue;

            CreatePoolOfType(poolType, pool.pooledPrefab, pool.poolSize);
        }
    }

    //[ContextMenu("Update Baked Pools")]
    //private void UpdateBakedPools()
    //{

    //}

    public GameObject Spawn(GameObject gameObjectPrefab)
    {
        IPoolable poolable;
        GameObject result;

        if (TryGetPoolable(gameObjectPrefab, out poolable))
        {
            string poolType = poolable.getName();

            if (!poolDictionary.ContainsKey(poolType))
            {
                Debug.LogWarning("IPoolable GameObject without baked pool has requested Spawning, creating pool!");
                CreatePoolOfType(poolType, gameObjectPrefab, 5);
            }

            if (poolDictionary[poolType].Count > 0)
            {
                result = poolDictionary[poolType].Dequeue();
            }
            else
            {
                Debug.Log("New pool item created for: " + gameObjectPrefab.name);
                result = Instantiate(gameObjectPrefab) as GameObject;
            }
            result.SetActive(true);
            result.transform.SetParent(null, false);
            result.GetComponent<IPoolable>().OnSpawned();
        }
        else
        {
            result = Instantiate(gameObjectPrefab) as GameObject;
        }

        return result;
    }

    public void Despawn(GameObject gameObject)
    {
        IPoolable poolable;

        if (TryGetPoolable(gameObject, out poolable))
        {
            string poolType = poolable.getName();

            if (!poolDictionary.ContainsKey(poolType))
            {
                Debug.LogWarning("IPoolable GameObject without baked pool has requested Despawning, creating pool! (¡¿How did this happen?!)");
                CreatePoolOfType(poolType, gameObject, 5);
            }

            //poolable.OnDespawned();
            gameObject.SetActive(false);
            gameObject.transform.SetParent(transform, true);
            poolDictionary[poolType].Enqueue(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    bool TryGetPoolable(GameObject gameobject, out IPoolable poolable)
    {
        if (gameobject.TryGetComponent<IPoolable>(out poolable))
        {
            return true;
        }
        
        Debug.LogError("GameObject without IPoolable Component (" + gameobject.name + ") was trying to interact with the PoolingManager!");
        return false;
        
    }

    void CreatePoolOfType(string poolType, GameObject pooledPrefab, int poolSize)
    {
        Queue<GameObject> gameObjects = new Queue<GameObject>();

        for (int i = 0; i < poolSize; i++)
        {
            var a = Instantiate(pooledPrefab) as GameObject;
            a.SetActive(false);
            a.transform.SetParent(transform, true);
            gameObjects.Enqueue(a);
        }

        poolDictionary.Add(poolType, gameObjects);
    }
}
