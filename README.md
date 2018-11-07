# Unity对象池
Unity自带的实例化和销毁方法太过昂贵，同时Unity又没有提供对象池，那就只好自写对象池了<br/>
<br/>
除了对象池还有一个配对的需要在入池的时候重置的组件的基类（RequireResetComponent），因为找不到重置整个物体的方法，只能用这个办法来绕一绕<br/>
#### 再说一遍，这个对象池有一个不太好用但能用的重置物体方法，不要只用池把重置扔了
### 文件夹内容：
| 文件夹 | 内容 |
| ------------- |:-------------| 
| ObjectPool.unitypackage | 资源包，直接导入就能用 |
| Assets/Pool.cs | 对象池脚本 |
| Assets/RequireResetComponent.cs | 需要重置的组件的基类脚本 |
| Assets/Example | 演示场景、预制和脚本 |
