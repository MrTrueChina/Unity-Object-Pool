https://github.com/MrTrueChina/Unity-Object-Pool

# <a href="#Chinese">中文版</a> / <a href="#English">English</a>
<div id="Chinese"></div>

# namespace： MtC.Tools.ObjectPool

## 方法：
### MPool.Set
```C#
public static void Set(GameObject setObject, float delay = 0);
```
按照 Destroy 方法写的 Set 方法，为了模仿 Destroy 的在每一帧最后销毁的功能设置为通过 Camera.onPreCull 在剔除前进行存入，如果有其他脚本在这个时候存入池，我敬你是条汉子。</br>
</br>
### MPool.Get
```C#
public static GameObject Get(GameObject prefab);
public static GameObject Get(GameObject prefab, Transform parent);
public static GameObject Get(GameObject prefab, Transform parent, bool instantiateInWorldSpace);
public static GameObject Get(GameObject prefab, Vector3 position, Quaternion rotation);
public static GameObject Get(GameObject prefab, Vector3 position, Quaternion rotation, Transform parent);
```
按照 Instantiate 方法写的 Get 方法，使用方式和 Instantiate 相同。</br>
</br>

## 接口：
### ResetOnGetFromPool
实现这个接口中的 ResetOnGetFromPool() 方法，当从池里取出物体时对象池会自动调用该方法对组件进行重置。</br>
</br>

## 注意事项：
#### 这个对象池只能存入通过自己的 Get() 方法创建的物体。
</br>
</br>
</br>
</br>
</br>
</br>
</br>
</br>
</br>
</br>
</br>
</br>
</br>
</br>
</br>
</br>
</br>
</br>
</br>
</br>

# <a href="#English">English</a> / <a href="#Chinese">中文版</a>
<div id="English"></div>

# namespace： MtC.Tools.ObjectPool

## Methods：
### MPool.Set
```C#
public static void Set(GameObject setObject, float delay = 0);
```
According to the Set method written by the Destroy method, in order to simulate Destroy's function of destroying at the end of each frame, it is set to be stored before the culling by Camera.onPreCull. If other scripts are stored in the pool at this time, it is brave.</br>
</br>
### MPool.Get
```C#
public static GameObject Get(GameObject prefab);
public static GameObject Get(GameObject prefab, Transform parent);
public static GameObject Get(GameObject prefab, Transform parent, bool instantiateInWorldSpace);
public static GameObject Get(GameObject prefab, Vector3 position, Quaternion rotation);
public static GameObject Get(GameObject prefab, Vector3 position, Quaternion rotation, Transform parent);
```
According to the Get method written by the Instantiate method, it is used in the same way as Instantiate.</br>
</br>

## Interface ：
### ResetOnGetFromPool
Implement the ResetOnGetFromPool() method in this interface. When the object is taken out of the pool, the object pool will automatically call this method to reset the component.</br>
</br>

## Attention：
#### This object pool can only be stored in objects created by its own Get() method.
