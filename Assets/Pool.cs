/*
 *  单场景对象池，切换场景时对象池会一起销毁
 *  
 *  
 *  首先，U3D官方没有提供对象池，只提供了实例化（Instantiate）和销毁（Destroy），然而这两个方法的成本都很高，如果是物体生成销毁频率低的游戏还没多大问题，如果是一个严谨的枪战游戏，一把枪一秒钟10个子弹物体，算作场上10个人，这游戏没法玩了
 *  
 *  为了解决这个问题就要把对象池搬出来了
 *  
 *  Unity还有一个类似实例化和销毁的功能是 启用/禁用物体，也就是SetActive(bool)，禁用物体的效果和销毁类似，只不过内存继续占用，启用一个被禁用的物体效果则和实例化很像，重点在于这个操作成本特别低，一帧执行几千次也不会卡
 *  
 *  那么对象池的功能就很简单了：先准备一个容器用来存储物体的引用，场上有物体要销毁的时候不进行销毁而是禁用后把引用存起来，需要实例化的时候通过引用启用这个物体，这期间物体一直在场上不需要实例化和销毁
 *  
 *  
 *  为了达到替代 Instantiate 和 Destroy 的效果，按照 Instantiate 和 Destroy 的所有重载写了 Get 和 Set
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pool : MonoBehaviour
{
    Dictionary<string, List<GameObject>> _pool = new Dictionary<string, List<GameObject>>();        //准备一个字典对象，这个字典就是对象池里数据的容器
    //字典（Dictionary）：通过键值查找元素的数据类型，创建格式是：Dictionary<Tkey, TValue> 前一个是键值类型，后一个是元素类型

    static Transform poolTransform
    {
        get
        {
            if (_poolTransform != null)
                return _poolTransform;

            _poolTransform = new GameObject("Pool").transform;
            return _poolTransform;

            //经典的保证场景中只有一个需要的物体的方法，也就是没有则创建保存返回，有则直接返回，应用广泛
        }
    }
    static Transform _poolTransform;
    //对象池物体的 Transform 组件，所有存入对象池的物体都移动到这个物体下面，可以防止父级物体被销毁导致对象池里的物体一起被销毁
    
    static Pool poolObject
    {
        get
        {
            if (_poolObject != null)
                return _poolObject;

            _poolObject = poolTransform.gameObject.AddComponent<Pool>();        //直接把对象池挂载到对象池物体上，省下一个物体，而且当对象池物体销毁时对象池实例一起销毁，避免了对象池物体被销毁留下一个失效的对象池的情况
            return _poolObject;
        }
    }
    static Pool _poolObject;
    //对象池实例，因为协程必须由对象调用，要按照延时销毁的功能做出延时存入池的效果最直接的方法就是创建一个自己的对象存起来用来启动协程


    
    public static void Set(GameObject setObject)
    {
        setObject.SetActive(false);                                         //禁用物体，这一步最先进行，因为后续的重置物体会大规模影响到物体上的组件，但如果物体已经被禁用那么影响就好控制的多了
        ResetObject(setObject);

        if (!_poolObject._pool.ContainsKey(setObject.name))                 //Dictionary.ContainsKey()：查找字典里有没有这个键值
            poolObject._pool.Add(setObject.name, new List<GameObject>());   //Dictionary.Add()：向字典里增加一对键值和元素，字典不会自动增加键值和元素，只能手动进行

        poolObject._pool[setObject.name].Add(setObject);                    //字典获取元素的方法类似于数组，是方括号里写键值：[键值]

        setObject.transform.parent = poolTransform;                         //将存入池的物体移到对象池物体下作为子物体，如果不转移到对象池物体下的话有可能会因为原本的父物体销毁而导致对象池出现空位，进而导致取不出物体
    }
    static void ResetObject(GameObject setObject)       //重置物体，因为找不到重置整个物体的方法，所以通过获取所有需要重置的组件并调用重置方法来达到类似效果
    {
        RequireResetComponent[] resetComponents = setObject.GetComponents<RequireResetComponent>();
        foreach (RequireResetComponent resetComponent in resetComponents)
            resetComponent.Reset();
    }

    public static void Set(GameObject setObject, float delay)
    {
        poolObject.StartCoroutine(DelaySet(setObject, delay));
    }
    static IEnumerator DelaySet(GameObject setObject, float delay)
    {
        yield return new WaitForSeconds(delay);
        Set(setObject);
    }



    public static GameObject Get(GameObject prefab, Vector3 position, Quaternion rotation)
    {
        GameObject instance = GetFromPool(prefab);      //首先从池里获取出物体

        if (instance != null)
        {
            instance.transform.parent = null;           //首先把这个物体从对象池物体下移出来
            instance.transform.position = position;
            instance.transform.rotation = rotation;
            instance.SetActive(true);                   //设置完父级、位置、旋转角度后就可以激活了
            return instance;
        }

        return GetNewObject(prefab, position, rotation);
    }
    public static GameObject Get(GameObject prefab)
    {
        return Get(prefab, Vector3.zero, Quaternion.identity);
    }
    public static GameObject Get(GameObject prefab, Transform parent, bool instantiateInWorldSpace = false)
    {
        GameObject instance = Get(prefab);

        instance.transform.SetParent(parent, instantiateInWorldSpace);

        return instance;
    }
    public static GameObject Get(GameObject prefab, Vector3 position, Quaternion rotation, Transform parent)
    {
        GameObject instance = Get(prefab, position, rotation);

        instance.transform.SetParent(parent);

        return instance;
    }
    
    static GameObject GetFromPool(GameObject prefab)                //从池里获取物体
    {
        if (poolObject._pool.ContainsKey(prefab.name))              
        {
            List<GameObject> list = poolObject._pool[prefab.name];

            if (list != null && list.Count > 0)
            {
                GameObject instance = list[0];
                list.RemoveAt(0);                   //从池里取出物体时要把池里的引用一起移除掉
                return instance;
            }
        }

        return null;
    }

    static GameObject GetNewObject(GameObject prefab, Vector3 position, Quaternion rotation)    //创建一个新物体
    {
        GameObject newObject = Instantiate(prefab, position, rotation);     //创建新物体的基础还是实例化
        newObject.name = prefab.name;       //重命名实例化出来的新物体为预制的名字，这一步很重要，这个对象池用物体的名称做键值，但实例化出来的物体会有一个"(Clone)"的后缀，这导致物体和预制的名称不同，在取出时因为名称不同获取不到池里的物体
        return newObject;
    }



    private void OnDisable()
    {
        StopAllCoroutines();
    }
}
