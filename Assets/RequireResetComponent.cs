using UnityEngine;

/// <summary>
/// 需要在存入池时重置的组件需继承自这个类
/// </summary>
public class RequireResetComponent : MonoBehaviour
{
    public virtual void Reset() { }
}
