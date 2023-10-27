//////////////////////////////////////////////////////////////////////////
// 
// 文件：Assets\Scripts\Startup\Steps\StartupStepSchema.cs
// 作者：Xoen Xie
// 时间：2023/06/24
// 描述：游戏启动 - schema
// 说明：
//
//////////////////////////////////////////////////////////////////////////

using UnityEngine;
using XSF;


public sealed class StartupStepSchema : StartupStep
{
    public override void Start()
    {
        Debug.LogWarning("StartupStepSchema Start");
        XSFSchema.Instance.StartLoad(new GameSchemaHelper());
        Subscribe((EventID)XSFCore.SCHEMA_EVENT_ID, 0);
    }
}