//////////////////////////////////////////////////////////////////////////
//
// 文件：Assets/Scripts/Game.cs
// 作者：Xoen Xie
// 时间：2023/06/15
// 描述：Game脚本
// 说明：
//
//////////////////////////////////////////////////////////////////////////

using XSF;
using XsfUI;
using XsfNet;

public class Game : IModuleRegister
{
    public void SetInitData()
    {
        XSFUI.Instance.SetInitData(new UIHelper(), XSFMain.Instance.UIRoot);
    }

    public void DoRegist()
    {
        XSFCore.ModuleRegist(XSFStartup.Instance);
        XSFCore.ModuleRegist(XSFNet.Instance);
    }
}