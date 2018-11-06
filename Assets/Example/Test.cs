using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test : MonoBehaviour
{
    [SerializeField]
    GameObject prefab;
    [SerializeField]
    Transform parent;


    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.T))
        {
            Pool.Get(prefab, parent, true);
            Instantiate(prefab, parent, true);
        }

        if (Input.GetKeyDown(KeyCode.F))
        {
            Pool.Get(prefab, parent, false);
            Instantiate(prefab, parent, false);
        }

        if (Input.GetKeyDown(KeyCode.I))
        {
            Pool.Get(prefab, parent);
            Instantiate(prefab, parent);
        }

        if (Input.GetKeyDown(KeyCode.P))
        {
            Pool.Get(prefab, Vector3.zero, Quaternion.identity, parent);
            Instantiate(prefab, Vector3.zero, Quaternion.identity, parent);
        }
    }
}
