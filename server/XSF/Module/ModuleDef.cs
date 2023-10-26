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

        public virtual void OnOK() {}

        public virtual void OnClose() {}

        public virtual void OnUpdate(uint nDeltaTime) {}

        public virtual ModuleRunCode OnStartCheck() { return ModuleRunCode.OK; }
        public virtual ModuleRunCode OnCloseCheck() { return ModuleRunCode.OK; }
    }
}

