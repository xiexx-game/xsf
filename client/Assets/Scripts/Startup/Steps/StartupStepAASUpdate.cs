//////////////////////////////////////////////////////////////////////////
// 
// 文件：Assets\Scripts\Startup\Steps\StartupStepAASUpdate.cs
// 作者：Xoen Xie
// 时间：2023/06/24
// 描述：游戏启动 - AAS Update
// 说明：
//
//////////////////////////////////////////////////////////////////////////

public sealed class StartupStepAASUpdate : StartupStep
{
    public override void Start()
    {
#if UNITY_EDITOR
        if (XSFConfig.Instance.AASUpdateOpen)
        {
#endif
            XSF.Log("StartupStepAASUpdate Start ..");

            // todo: aas update progress
            IsDone = true;
#if UNITY_EDITOR
        }
        else
        {
            XSF.Log("StartupStepAASUpdate skip ..");
            IsDone = true;
        }
#endif
    }
}