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
        XSFStartup.Instance.UI.SetInfoText("Start Load Schema ...");
    }

    private float m_fProgress;
    public override float CurrentProgress { get { return m_fProgress; } }

    public override bool OnLocalEvent(uint nEventID, uint nObjectID, object context)
    {   
        SchemaLoadEvent le = (SchemaLoadEvent)context;
        if(le.Left == 0)
        {
            //Debug.LogWarning($"RunStep.OnLocalEvent nEventID={nEventID}, nObjectID={nObjectID}");
            IsDone = true;
            return false;
        }
        else
        {
            m_fProgress = (float)(le.Total - le.Left)/le.Total;
        }

        return true;
    }
}