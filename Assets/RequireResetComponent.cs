/*
 *  需要在存入池时重置的组件需要继承这个类
 *  
 *  
 *  我找了好长时间也找不到自带的在运行时重置物体或者组件的方法，然后我找了别人写的对象池发现也不能重置物体或组件，我猜测重置物体或组件的功能应该不会比实例化和挂载组件节省多少资源，所以官方才没有提供这个方法
 *  
 *  于是只能曲线救国了，写一个基类，写上重置方法之后让对象池在存入的时候调用
 */

using UnityEngine;

namespace MtC.Tools.ObjectPool
{
    public class RequireResetComponent : MonoBehaviour
    {
        public virtual void Reset() { }
    }
}