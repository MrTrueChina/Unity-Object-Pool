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
	ResetOnGetFromPool

	Implement the ResetOnGetFromPool() method in this interface. When the object is taken out of the pool, the object pool will automatically call this method to reset the component.

Attention：
This object pool can only be stored in objects created by its own Get() method.
