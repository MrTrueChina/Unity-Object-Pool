using UnityEngine;
using MtC.Tools.ObjectPool;

public class MPoolSpownCube : MonoBehaviour
{
    [SerializeField]
    GameObject _prefab;

    [SerializeField]
    float _minSpownInterval;
    [SerializeField]
    float _maxSpownInterval;


    float nextSpown;


    private void Update()
    {
        if (Time.time >= nextSpown)
        {
            Spown();
            nextSpown = Time.time + Random.Range(_minSpownInterval, _maxSpownInterval);
        }
    }

    void Spown()
    {
        Vector3 position = new Vector3(Random.Range(-5f, 5f), Random.Range(-5f, 5f), Random.Range(-5f, 5f));
        MPool.Get(_prefab, position, Quaternion.identity);
    }
}