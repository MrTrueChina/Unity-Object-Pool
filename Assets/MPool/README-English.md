Details：https://github.com/MrTrueChina/Unity-Object-Pool

namespace： MtC.Tools.ObjectPool

Methods：
	MPool.Set
		public static void Set(GameObject setObject, float delay = 0)

	According to the Set method written by the Destroy method, in order to simulate Destroy's function of destroying at the end of each frame, 
	it is set to be stored before the culling by Camera.onPreCull. If other scripts are stored in the pool at this time, it is brave.


	MPool.Get
		public static GameObject Get(GameObject prefab)
		public static GameObject Get(GameObject prefab, Transform parent)
		public static GameObject Get(GameObject prefab, Transform parent, bool instantiateInWorldSpace)
		public static GameObject Get(GameObject prefab, Vector3 position, Quaternion rotation)
		public static GameObject Get(GameObject prefab, Vector3 position, Quaternion rotation, Transform parent)

	According to the Get method written by the Instantiate method, it is used in the same way as Instantiate.


Interface ：
	
	IOnGetFromPool
	
	Implement the OnGetFromPool() method in this interface. The object pool automatically calls this method when the object is removed from the pool. Can be used to reset components.


	IOnSetIntoPool
	
	Implement the OnSetIntoPool() method in this interface. The object pool automatically calls this method when an object is stored in the object pool. Can be used to unsubscribe and reference to prevent memory leaks


Attention：
This object pool can only be stored in objects created by its own Get() method.
