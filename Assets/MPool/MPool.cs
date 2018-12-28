/*
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
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace MtC.Tools.ObjectPool
{
    public class MPool : MonoBehaviour
    {
        static Transform poolParent     //所有池内物体的父级
        {
            get
            {
                if (_poolParent != null)
                    return _poolParent;

                GameObject poolObject = new GameObject("Object Pool");
                DontDestroyOnLoad(poolObject);                              //设置对象池物体不会随场景加载被销毁，这步很重要，能不能跨场景都在这了
                _poolParent = poolObject.transform;
                return _poolParent;

                //经典单例模式，保证场上有且只有一个对象池物体
            }
        }
        static Transform _poolParent;

        static MPool poolComponent       //挂载的对象池组件
        {
            get
            {
                if (_poolComponent != null)
                    return _poolComponent;

                _poolComponent = poolParent.gameObject.AddComponent<MPool>();
                return _poolComponent;
            }
        }
        static MPool _poolComponent;


        Dictionary<GameObject, GameObject> _poolObjects = new Dictionary<GameObject, GameObject>();                     //总表，所有对象池创建的物体，不管在池里还是在场上
        Dictionary<GameObject, Stack<GameObject>> _insidePoolObjects = new Dictionary<GameObject, Stack<GameObject>>(); //内表，对象池里的物体
        //字典（Dictionary）：通过键值查找元素的数据类型，创建格式是：Dictionary<Tkey, TValue> 前一个是键值类型，后一个是元素类型

        Stack<GameObject> _currentFrameSetObjects = new Stack<GameObject>();    //当前帧需要存入池的物体，为了模仿 Destroy 的销毁总在当前帧最后进行，把当前帧需要存入池的物体先暂存起来，到 SceneManager.sceneLoaded 再存入



        private void Awake()
        {
            SceneManager.sceneLoaded += OnSceneLoaded;      //订阅场景加载事件，OnLevelWasLoaded过时了，会在最近版本弃用，新方法暂时是订阅事件
            Camera.onPreCull += OnPerCull;                  //订阅摄像机开始剔除事件，剔除是渲染的第一步，这个事件是渲染前最后一个事件
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
            poolComponent._currentFrameSetObjects.Push(setObject);
        }

        void OnPerCull(Camera cam)
        {
            SetObjectsIntoPool();
        }
        void SetObjectsIntoPool()
        {
            while (_currentFrameSetObjects.Count > 0)
                SetAnObject(_currentFrameSetObjects.Pop());         //在 SceneManager.sceneLoaded 把当前帧需要存入池的物体全都存入池，如果其他脚本有在 SceneManager.sceneLoaded 存入池的情况，我敬他是条汉子。
        }
        void SetAnObject(GameObject setObject)
        {
            if (setObject == null) return;

            GameObject prefab = null;
            if (_poolObjects.TryGetValue(setObject, out prefab))    //通过总表获取预制，获取得到说明是池物体
            {
                if (InPool(setObject, prefab)) return;              //在内表里则不需要任何操作

                DoSet(setObject, prefab);
            }
            else
            {
                Destroy(setObject);                                 //没取到预制说明不是池物体，无法存入池，直接销毁
            }
        }
        bool InPool(GameObject go, GameObject prefab)
        {
            Stack<GameObject> stack;
            if (_insidePoolObjects.TryGetValue(prefab, out stack))
                return stack.Contains(go);
            return false;
        }
        void DoSet(GameObject setObject, GameObject prefab)
        {
            setObject.SetActive(false);                                     //禁用物体同时物体的所有协程也一起停止了，不用特别处理

            DoOnSet(setObject);

            if (!_insidePoolObjects.ContainsKey(prefab))                    //Dictionary.ContainsKey()：查找字典里有没有这个键值
                _insidePoolObjects.Add(prefab, new Stack<GameObject>());    //Dictionary.Add()：向字典里增加一对键值和元素，字典不会自动增加键值和元素，只能手动进行

            _insidePoolObjects[prefab].Push(setObject);                     //字典获取元素的方法类似于数组，是方括号里写键值：[键值]
            setObject.transform.SetParent(poolParent, false);               //将存入池的物体移到对象池物体下作为子物体
                                                                            //这一步很重要，如果不转移到对象池物体下的话有可能会因为原本的父物体销毁而导致对象池出现空位造成资源浪费
                                                                            //同时对象池物体已经设置加载场景时不销毁，他的子物体同样不会在加载场景时销毁，对象池就可以跨场景使用
        }

        void DoOnSet(GameObject setObject)
        {
            IOnSetIntoPool[] resetComponents = setObject.GetComponents<IOnSetIntoPool>();
            foreach (IOnSetIntoPool resetComponent in resetComponents)
                resetComponent.OnSetIntoPool();
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
                poolComponent._poolObjects.Add(instance, prefab);       //加入到总表，只要物体还存在就不会被移出总表，所以只要在实例化时加入一次
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
                poolComponent._poolObjects.Add(instance, prefab);
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
                poolComponent._poolObjects.Add(instance, prefab);
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
                poolComponent._poolObjects.Add(instance, prefab);
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
                poolComponent._poolObjects.Add(instance, prefab);
                return instance;
            }
        }

        
        static GameObject GetInactiveObjectFromPool(GameObject prefab)      //从池里获取未激活的物体，因为要处理后再激活
        {
            Stack<GameObject> stack;
            if (poolComponent._insidePoolObjects.TryGetValue(prefab, out stack) && stack.Count > 0)
            {
                GameObject instance = stack.Pop();
                if (instance != null)
                {
                    instance.transform.SetParent(null, false);      //首先把这个物体从对象池物体下移出来
                    CancelDontDestroyOnLoad(instance);              //取消物体的加载场景不销毁效果

                    DoOnGet(instance);                              //重置这个物体

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

        static void DoOnGet(GameObject setObject)           //在取出时重置物体，因为找不到重置整个物体的方法，所以通过获取所有需要重置的组件并调用重置方法来达到类似效果
        {
            IOnGetFromPool[] resetComponents = setObject.GetComponents<IOnGetFromPool>();
            foreach (IOnGetFromPool resetComponent in resetComponents)
                resetComponent.OnGetFromPool();
        }


        
        private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            StopAllCoroutines();                            //没有在加载场景前发出的事件，无法回收在场上的物体，中断所有协程，防止空存入
            ClearDestroyedObjectInPoolObjects();
        }

        void ClearDestroyedObjectInPoolObjects()
        {
            _poolObjects = (from kv in _poolObjects where kv.Key != null select kv).ToDictionary(kv => kv.Key, kv => kv.Value);     //用Linq清除总表里的键值为空的元素
        }



        private void OnDestroy()
        {
            SceneManager.sceneLoaded -= OnSceneLoaded;      //被销毁时取消对场景加载的订阅
            Camera.onPreCull -= OnPerCull;                  //取消对摄像机开始剔除事件的订阅
        }
    }



    /*
     *  需要在取出池时重置或者执行方法的组件可以实现这个接口
     *  
     *  
     *  我找了好长时间也找不到自带的在运行时重置物体或者组件的方法，然后我找了别人写的对象池发现也不能重置物体或组件，我猜测重置物体或组件的功能应该不会比实例化和挂载组件节省多少资源，所以官方才没有提供这个方法
     *  
     *  于是只能曲线救国了，写一个接口，写上重置方法之后让对象池在取出的时候调用，之所以是取出是因为对象在池里时有可能被以其他方式访问并修改，在取出时重置明显更安全
     */
    public interface IOnGetFromPool
    {
        void OnGetFromPool();
    }

    /*
     *  这个是在存入时执行的，可以用来取消订阅和引用，防止内存泄漏
     */
    public interface IOnSetIntoPool
    {
        void OnSetIntoPool();
    }
}