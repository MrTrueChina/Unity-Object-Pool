using UnityEngine;
using MtC.Tools.ObjectPool;
using System;

public class MPoolDelaySet : MonoBehaviour, IOnGetFromPool, IOnSetIntoPool
{
    Transform _transform;

    Vector3 _originScale;


    private void Awake()
    {
        _transform = transform;

        _originScale = transform.localScale;    //记录原始缩放值，在取出时用于重置
    }


    private void Update()
    {
        _transform.localScale *= 1 - Time.deltaTime;

        if (_transform.localScale.x <= 0.1f)
            MPool.Set(gameObject);
    }



    public void OnGetFromPool()            //实现 ResetOnSetToPool() 方法，从对象池里取出时对象池会调用
    {
        _transform.localScale = _originScale;   //把前面储存的原始缩放值存回 Transform
    }

    public void OnSetIntoPool()
    {
        _transform.localScale = _originScale;
    }
}
