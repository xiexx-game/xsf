//////////////////////////////////////////////////////////////////////////
// 
// 文件：XSF/Module/ModuleDef.cs
// 作者：Xoen Xie
// 时间：2023/07/01
// 描述：模块相关定义
// 说明：
//
//////////////////////////////////////////////////////////////////////////

namespace XSF
{
    public enum ModuleRunCode
    {
        OK = 0,
        Error,
        Wait,
    }

    internal enum ModuleRunStatus
    {
        None = 0,
        Running,
        WaitStart,
        Start,
        Stop,
        WaitStop,
        WaitOK,
    }

    public class ModuleInit
    {
        public int ID;
        public string Name;

        public bool NoWaitStart;

        public ModuleInit()
        {
            Name = "Unknow";
        }
    }

    public abstract class IModule
    {
        public int ID { get; private set; }
        public string Name { get; private set; }

        internal bool NoWaitStart;

        public IModule()
        {
            ID = 0;
            Name = "";
        }

        public virtual bool Init(ModuleInit init)
        {
            ID = init.ID;
            Name = init.Name;
            NoWaitStart = init.NoWaitStart;

            return true;
        }
        
        public virtual void Release() {}

        public virtual void DoRegist() { }

        public virtual bool Start() { return true; }

        // 服务器所有模块开启完毕后调用
        public virtual void OnOK() {}

        // 服务器所有模块都检测关闭完成后调用
        public virtual void DoClose() {}

        // 服务器关服前调用
        public virtual void OnStartClose() {}

        public virtual void OnUpdate(uint nDeltaTime) {}

        // 检查本模块是否开启完成
        public virtual ModuleRunCode OnStartCheck() { return ModuleRunCode.OK; }

        // 检查本模块是否关闭完成
        public virtual ModuleRunCode OnCloseCheck() { return ModuleRunCode.OK; }
    }
}

