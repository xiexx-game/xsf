//////////////////////////////////////////////////////////////////////////
// 
// 文件：Assets\Scripts\Startup\Steps\StartupStep.cs
// 作者：Xoen Xie
// 时间：2023/06/24
// 描述：游戏启动步骤基类
// 说明：
//
//////////////////////////////////////////////////////////////////////////

using UnityEngine;
using XSF;

public enum StartupStepID
{
    None = 0,
    AASUpdate,
    Schema,
    ShowFirstUI,
    Max,
}

public abstract class StartupStep : IEventSink
{
    public abstract void Start();

    public virtual void End()
    {
        
    }

    public void Subscribe(EventID nEventID, uint nObjectID)
    {
        XSFEvent.Instance.Subscribe(this, (uint)nEventID, nObjectID);
    }

    public bool OnEvent( uint nEventID, uint nObjectID, object context )
    {
        return OnLocalEvent(nEventID, nObjectID, context);
    }

    public virtual bool OnLocalEvent(uint nEventID, uint nObjectID, object context)
    {
        Debug.LogWarning($"RunStep.OnLocalEvent nEventID={nEventID}, nObjectID={nObjectID}");
        IsDone = true;
        return false;
    }

    public bool IsDone { get; protected set; }
}