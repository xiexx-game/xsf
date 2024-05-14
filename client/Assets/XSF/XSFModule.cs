//////////////////////////////////////////////////////////////////////////
// 
// 文件：Assets/XSF/XSFModule.cs
// 作者：Xoen Xie
// 时间：2024/05/11
// 描述：模块接口
// 说明：
//
//////////////////////////////////////////////////////////////////////////


namespace XSF
{
    public interface IModuleRegister
    {
        void SetInitData();

        void DoRegist();
    }

    public interface IXSFModule
    {
        // 初始化操作
        void Init();

        // 在所有模块Init后调用
        void Start();

        // 释放操作
        void Release();

        // 热更完成后调用
        void OnContentUpdateDone();

        // 登录完成后调用
        void OnLogin();

        // 登出时调用
        void OnLogout();
    }
}