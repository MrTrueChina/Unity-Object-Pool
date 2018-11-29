using UnityEngine;
using MtC.Tools.ObjectPool;

public class SpownCube : MonoBehaviour
{
    [SerializeField]
    GameObject _prefab;

    [SerializeField]
    float _minSpownInterval;
    [SerializeField]
    float _maxSpownInterval;

    [SerializeField]
    float _minSetDelay;
    [SerializeField]
    float _maxSetDelay;


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
        GameObject instance = Pool.Get(_prefab, position, Quaternion.identity);
        instance.GetComponent<DelaySet>().setTime = Random.Range(_minSetDelay, _maxSetDelay);
    }
}