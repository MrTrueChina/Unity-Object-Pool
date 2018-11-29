using UnityEngine;

namespace MtC.Tools.ObjectPool
{
    public class DelaySet : MonoBehaviour, ResetOnSetToPool
    {
        public float setTime
        {
            get { return _setTime; }
            set { _setTime = value; }
        }
        [SerializeField]
        float _setTime;

        float originalSetTime;


        private void Awake()
        {
            originalSetTime = _setTime;          //在脚本刚创建的时候储存下原始时间，留在重置的时候使用
        }


        private void OnEnable()
        {
            Pool.Set(gameObject, _setTime);
        }



        public void ResetOnSetToPool()          //实现 ResetOnSetToPool() 方法，存入对象池时对象池会调用
        {
            _setTime = originalSetTime;          //把创建时存储的原始时间拿出来做重置
        }
    }
}