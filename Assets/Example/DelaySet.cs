using MtC.Tools.ObjectPool;

public class DelaySet : RequireResetComponent
{
    public float setTime;
    
    float originalSetTime;


    private void Awake()
    { 
        originalSetTime = setTime;          //在脚本刚创建的时候储存下原始时间，留在重置的时候使用
    }


    private void OnEnable()
    {
        Pool.Set(gameObject, setTime);
    }


    public override void Reset()            //覆写 RequireResetComponent 的重置方法，存入对象池的时候会由对象池调用
    {
        setTime = originalSetTime;          //把创建时存储的原始时间拿出来做重置
    }
}
