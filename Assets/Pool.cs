﻿/*
 *  跨场景对象池，切换场景时对象池保留
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
using UnityEngine.SceneManagement;

namespace MtC.Tools.ObjectPool
{
    public class Pool : MonoBehaviour
    {
        Dictionary<string, List<GameObject>> _pool = new Dictionary<string, List<GameObject>>();    //准备一个字典对象，这个字典就是对象池里数据的容器
        //字典（Dictionary）：通过键值查找元素的数据类型，创建格式是：Dictionary<Tkey, TValue> 前一个是键值类型，后一个是元素类型

        static Transform poolParent
        {
            get
            {
                if (_poolParent != null)
                    return _poolParent;

                GameObject poolObject = new GameObject("Object Pool");
                DontDestroyOnLoad(poolObject);                          //设置对象池物体不会随场景加载被销毁，这步很重要，能不能跨场景都在这了
                _poolParent = poolObject.transform;
                return _poolParent;

                //经典单例模式，保证场上有且只有一个对象池物体
            }
        }
        static Transform _poolParent;

        static Pool poolComponent
        {
            get
            {
                if (_poolComponent != null)
                    return _poolComponent;

                _poolComponent = poolParent.gameObject.AddComponent<Pool>();
                return _poolComponent;
            }
        }
        static Pool _poolComponent;


        static List<GameObject> _currentFrameSetObjects = new List<GameObject>();       //当前帧需要存入池的对象，为了模仿Destroy的销毁总在当前帧最后进行，把当前帧需要存入池的对象先暂存起来，到 LateUpdate 再存入



        private void Awake()
        {
            SceneManager.sceneLoaded += OnSceneLoaded;      //订阅场景加载事件，OnLevelWasLoaded过时了，会在最近版本弃用，新方法暂时是订阅事件
        }



        //存入
        public static void Set(GameObject setObject, float delay = 0)
        {
            poolComponent.StartCoroutine(DelaySet(setObject, delay));
        }
        static IEnumerator DelaySet(GameObject setObject, float delay)
        {
            if (delay > 0)
                yield return new WaitForSeconds(delay);
            _currentFrameSetObjects.Add(setObject);
        }

        
        private void LateUpdate()
        {
            foreach (GameObject setObject in _currentFrameSetObjects)   //在 LateUpdate 把当前帧需要存入池的物体全都存入池，如果其他脚本有在 LateUpdate 存入池的情况，可以设置脚本执行顺序到最后
                DoSet(setObject);
            _currentFrameSetObjects.Clear();        //全部存入后一定要记得清除List
        }

        static void DoSet(GameObject setObject)
        {
            setObject.SetActive(false);                                             //禁用物体，这一步最先进行，因为后续的重置物体会大规模影响到物体上的组件，但如果物体已经被禁用那么影响就好控制的多了
            ResetObject(setObject);

            if (!poolComponent._pool.ContainsKey(setObject.name))                   //Dictionary.ContainsKey()：查找字典里有没有这个键值
                poolComponent._pool.Add(setObject.name, new List<GameObject>());    //Dictionary.Add()：向字典里增加一对键值和元素，字典不会自动增加键值和元素，只能手动进行

            poolComponent._pool[setObject.name].Add(setObject);                     //字典获取元素的方法类似于数组，是方括号里写键值：[键值]

            setObject.transform.parent = poolParent;                                //将存入池的物体移到对象池物体下作为子物体
            //这一部很重要，如果不转移到对象池物体下的话有可能会因为原本的父物体销毁而导致对象池出现空位，进而导致取不出物体
            //同时对象池物体已经设置加载场景时不销毁，他的子物体同样不会在加载场景时销毁，对象池就可以跨场景使用
        }
        static void ResetObject(GameObject setObject)       //重置物体，因为找不到重置整个物体的方法，所以通过获取所有需要重置的组件并调用重置方法来达到类似效果
        {
            ResetOnSetToPool[] resetComponents = setObject.GetComponents<ResetOnSetToPool>();
            foreach (ResetOnSetToPool resetComponent in resetComponents)
                resetComponent.ResetOnSetToPool();
        }



        //取出
        public static GameObject Get(GameObject prefab)
        {
            GameObject instance = GetInactiveObjectFromPool(prefab);

            if (instance != null)
            {
                instance.transform.position = prefab.transform.position;
                instance.transform.rotation = prefab.transform.rotation;
                instance.transform.localScale = prefab.transform.localScale;
                instance.SetActive(true);
                return instance;
            }
            else
            {
                instance = Instantiate(prefab);
                instance.name = prefab.name;
                return instance;
            }
        }
        public static GameObject Get(GameObject prefab, Transform parent)
        {
            GameObject instance = GetInactiveObjectFromPool(prefab);

            if (instance != null)
            {
                instance.transform.position = prefab.transform.position;
                instance.transform.rotation = prefab.transform.rotation;
                instance.transform.localScale = prefab.transform.localScale;
                instance.transform.SetParent(parent, false);
                instance.SetActive(true);
                return instance;
            }
            else
            {
                instance = Instantiate(prefab, parent);
                instance.name = prefab.name;
                return instance;
            }
        }
        public static GameObject Get(GameObject prefab, Transform parent, bool instantiateInWorldSpace)
        {
            GameObject instance = GetInactiveObjectFromPool(prefab);

            if (instance != null)
            {
                instance.transform.position = prefab.transform.position;
                instance.transform.rotation = prefab.transform.rotation;
                instance.transform.localScale = prefab.transform.localScale;
                instance.transform.SetParent(parent, instantiateInWorldSpace);
                instance.SetActive(true);
                return instance;
            }
            else
            {
                instance = Instantiate(prefab, parent, instantiateInWorldSpace);
                instance.name = prefab.name;
                return instance;
            }            
        }
        public static GameObject Get(GameObject prefab, Vector3 position, Quaternion rotation)
        {
            GameObject instance = GetInactiveObjectFromPool(prefab);

            if (instance != null)
            {
                instance.transform.position = position;
                instance.transform.rotation = rotation;
                instance.transform.localScale = prefab.transform.localScale;
                instance.SetActive(true);
                return instance;
            }
            else
            {
                instance = Instantiate(prefab, position, rotation);
                instance.name = prefab.name;
                return instance;
            }
        }
        public static GameObject Get(GameObject prefab, Vector3 position, Quaternion rotation, Transform parent)
        {
            GameObject instance = GetInactiveObjectFromPool(prefab);

            if (instance != null)
            {
                instance.transform.position = position;
                instance.transform.rotation = rotation;
                instance.transform.SetParent(parent);
                instance.transform.localScale = prefab.transform.localScale;
                instance.SetActive(true);
                return instance;
            }
            else
            {
                instance = Instantiate(prefab, position, rotation, parent);
                instance.name = prefab.name;
                return instance;
            }
        }

        
        static GameObject GetInactiveObjectFromPool(GameObject prefab)                //从池里获取未激活的物体，因为要处理后再激活
        {
            if (poolComponent._pool.ContainsKey(prefab.name))
            {
                List<GameObject> list = poolComponent._pool[prefab.name];

                if (list != null && list.Count > 0)
                {
                    GameObject instance = list[list.Count - 1];     //从最后一个移除似乎比从第一个移除快，我觉得应该是跟元素的移动有关系
                    list.RemoveAt(list.Count - 1);                  //从池里取出物体时要把池里的引用一起移除掉

                    instance.transform.parent = null;               //首先把这个物体从对象池物体下移出来
                    CancelDontDestroyOnLoad(instance);              //取消物体的加载场景不销毁效果

                    return instance;
                }
            }

            return null;
        }

        static void CancelDontDestroyOnLoad(GameObject go)
        {
            SceneManager.MoveGameObjectToScene(go, SceneManager.GetActiveScene());
            //把物体移动到当前激活场景就能取消 DontDestroyOnLoad 效果，具体原理见 https://github.com/MrTrueChina/Unity-Cancel-DontDestroyOnLoad
        }



        //场景加载时停止所有协程，也就是取消所有延时存入池，因为场景加载时还没存入池的物体都被销毁了，不停止协程的话会出错
        private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            StopAllCoroutines();
        }



        private void OnDestroy()
        {
            SceneManager.sceneLoaded -= OnSceneLoaded;      //被销毁时取消对场景加载的订阅，这一步意义不大，一般来说销毁对象池的时候就是游戏关闭的时候
        }
    }


    /*
     *  需要在存入池时重置的组件需要实现这个接口
     *  
     *  
     *  我找了好长时间也找不到自带的在运行时重置物体或者组件的方法，然后我找了别人写的对象池发现也不能重置物体或组件，我猜测重置物体或组件的功能应该不会比实例化和挂载组件节省多少资源，所以官方才没有提供这个方法
     *  
     *  于是只能曲线救国了，写一个接口，写上重置方法之后让对象池在存入的时候调用
     */
    public interface ResetOnSetToPool
    {
        void ResetOnSetToPool();
    }
}