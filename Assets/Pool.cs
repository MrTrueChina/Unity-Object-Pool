using System.Collections.Generic;
using UnityEngine;

public class Pool : MonoBehaviour
{
    static Dictionary<string, List<GameObject>> _pool = new Dictionary<string, List<GameObject>>();

    static Transform poolObject
    {
        get
        {
            if (_poolObject == null)
                _poolObject = new GameObject("Pool").transform;

            return _poolObject;
        }
    }
    static Transform _poolObject;


    public static void Set(GameObject setObject)
    {
        if (!_pool.ContainsKey(setObject.name))
            _pool.Add(setObject.name, new List<GameObject>());
        
        RequireResetComponent[] resetComponents = setObject.GetComponents<RequireResetComponent>();
        foreach (RequireResetComponent resetComponent in resetComponents)
            resetComponent.Reset();

        _pool[setObject.name].Add(setObject);

        setObject.transform.parent = poolObject;
    }


    public static GameObject Get(GameObject prefab)
    {
        return Get(prefab, Vector3.zero, Quaternion.identity);
    }
    public static GameObject Get(GameObject prefab, Transform parent)
    {
        GameObject instance = Get(prefab);
        instance.transform.SetParent(parent, false);
        return instance;
    }
    public static GameObject Get(GameObject prefab, Transform parent, bool instantiateInWorldSpace)
    {
        GameObject instance = Get(prefab);

        instance.transform.SetParent(parent, instantiateInWorldSpace);

        return instance;
    }
    public static GameObject Get(GameObject prefab, Vector3 position, Quaternion rotation)
    {
        GameObject instance = GetFromPool(prefab);

        if (instance != null)
        {
            instance.transform.position = position;
            instance.transform.rotation = rotation;
            instance.SetActive(true);
        }

        return GetNewObject(prefab, position, rotation);
    }
    public static GameObject Get(GameObject prefab, Vector3 position, Quaternion rotation, Transform parent)
    {
        GameObject instance = Get(prefab, position, rotation);

        instance.transform.SetParent(parent);

        return instance;
    }


    static GameObject GetFromPool(GameObject prefab)
    {
        if (_pool.ContainsKey(prefab.name))
        {
            List<GameObject> list = _pool[prefab.name];

            if (list != null && list.Count > 0)
            {
                GameObject instance = list[0];
                list.RemoveAt(0);
                return instance;
            }
        }

        return null;
    }

    static GameObject GetNewObject(GameObject prefab, Vector3 position, Quaternion rotation)
    {
        GameObject newObject = Instantiate(prefab, position, rotation);
        newObject.name = prefab.name;
        return newObject;
    }
}
