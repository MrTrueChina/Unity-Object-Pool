详细内容：https://github.com/MrTrueChina/Unity-Object-Pool

namespace： MtC.Tools.ObjectPool

方法：
	MPool.Set
		public static void Set(GameObject setObject, float delay = 0)

	按照 Destroy 方法写的 Set 方法，为了模仿 Destroy 的在每一帧最后销毁的功能设置为通过 Camera.onPreCull 在剔除前进行存入，如果有其他脚本在这个时候存入池，我敬你是条汉子。


	MPool.Get
		public static GameObject Get(GameObject prefab)
		public static GameObject Get(GameObject prefab, Transform parent)
		public static GameObject Get(GameObject prefab, Transform parent, bool instantiateInWorldSpace)
		public static GameObject Get(GameObject prefab, Vector3 position, Quaternion rotation)
		public static GameObject Get(GameObject prefab, Vector3 position, Quaternion rotation, Transform parent)

	按照 Instantiate 方法写的 Get 方法，使用方式和 Instantiate 相同。


接口：
	ResetOnGetFromPool

	实现这个接口中的 ResetOnGetFromPool() 方法，当从池里取出物体时对象池会自动调用该方法对组件进行重置。

注意事项：
这个对象池只能存入通过自己的 Get() 方法创建的物体。