# <a href="#Chinese">中文版</a> / <a href="#English">English</a>
<div id="Chinese"></div>

# Unity对象池
Unity自带的实例化和销毁方法太过昂贵，同时Unity又没有提供对象池，那就只好自写对象池了<br/>
<br/>
<br/>

# namespace： MtC.Tools.ObjectPool

## 方法：
### Pool.Set
```C#
public static void Set(GameObject setObject, float delay = 0);
```
按照 Destroy 方法写的 Set 方法，为了模仿 Destroy 的在每一帧最后销毁的功能设置为通过 Camera.onPreCull 在剔除前进行存入，如果有其他脚本在这个时候存入池，我敬你是条汉子。</br>
</br>
### Pool.Get
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

## 文件夹内容：
| 文件夹 | 内容 |
| ------------- |:-------------| 
| Assets/MPool/Pool.cs | 对象池脚本 |
| Assets/MPool/Example | 演示场景、预制和脚本 |
| BuildingPackages | 打好的资源包 |


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

# Unity object pool
Unity's Instantiate and Destroy methods are too expensive, and Unity does not provide a pool of objects, so I have to write the object pool.<br/>
<br/>
<br/>

# namespace： MtC.Tools.ObjectPool

## Methods：
### Pool.Set
```C#
public static void Set(GameObject setObject, float delay = 0);
```
According to the Set method written by the Destroy method, in order to simulate Destroy's function of destroying at the end of each frame, it is set to be stored before the culling by Camera.onPreCull. If other scripts are stored in the pool at this time, it is brave.</br>
</br>
### Pool.Get
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
</br>

## Contents of folder：
| Folder | Contents |
| ------------- |:-------------| 
| Assets/MPool/Pool.cs | Object pool script file |
| Assets/MPool/Example | Example scene,prefab and scripts |
| BuildingPackages | Packaged resource |
