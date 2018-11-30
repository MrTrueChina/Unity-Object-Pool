https://github.com/MrTrueChina/Unity-Object-Pool

# namespace： MtC.Tools.ObjectPool

## 方法：
### Pool.Set
    public static void Set(GameObject setObject, float delay = 0)

按照 Destroy 方法写的 Set 方法，为了模仿 Destroy 的在每一帧最后销毁的功能设置为通过 Camera.onPreCull 在剔除前进行存入，如果有其他脚本在这个时候存入池，我敬你是条汉子。</br>
</br>
### Pool.Get
    public static GameObject Get(GameObject prefab)
    public static GameObject Get(GameObject prefab, Transform parent)
    public static GameObject Get(GameObject prefab, Transform parent, bool instantiateInWorldSpace)
    public static GameObject Get(GameObject prefab, Vector3 position, Quaternion rotation)
    public static GameObject Get(GameObject prefab, Vector3 position, Quaternion rotation, Transform parent)

按照 Instantiate 方法写的 Get 方法，效果和 2018.2.16f1 版的 Instantiate 完全一致，鉴于 Instantiate 方法十分成熟，这个 Get 应该能和 Instantiate 保持一段时间的一致性。</br>
</br>

## 接口：
### ResetOnGetFromPool
实现这个接口中的 ResetOnGetFromPool() 方法，可以在从池里取出时由对象池调用该方法对组件进行重置。
