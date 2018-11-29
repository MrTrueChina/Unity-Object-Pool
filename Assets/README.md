https://github.com/MrTrueChina/Unity-Object-Pool

# namespace： MtC.Tools.ObjectPool

## 方法：
### Pool.Set
public static void Set(GameObject setObject, float delay = 0)</br>

按照 Destroy 方法写的 Set 方法，为了模仿 Destroy 的在每一帧最后销毁的功能设置为在 LateUpdate 进行存入，如果有其他脚本在 LateUpdate 存入池可以通过调整脚本执行顺序保证对象池最后执行。</br>
</br>
### Pool.Get
public static GameObject Get(GameObject prefab)</br>
public static GameObject Get(GameObject prefab, Transform parent)</br>
public static GameObject Get(GameObject prefab, Transform parent, bool instantiateInWorldSpace)</br>
public static GameObject Get(GameObject prefab, Vector3 position, Quaternion rotation)</br>
public static GameObject Get(GameObject prefab, Vector3 position, Quaternion rotation, Transform parent)</br>

按照 Instantiate 方法写的 Get 方法，效果和 2018.2.16f1 版的 Instantiate 完全一致，鉴于 Instantiate 方法十分成熟，这个 Get 应该能和 Instantiate 保持一段时间的一致性。</br>
</br>

## 接口：
### ResetOnSetToPool
实现这个接口中的 ResetOnSetToPool() 方法，可以在存入池时由对象池调用该方法对组件进行重置。
