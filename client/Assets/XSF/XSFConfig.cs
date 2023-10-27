//////////////////////////////////////////////////////////////////////////
//
// 文件：Assets\XSF\XSFConfig.cs
// 作者：Xoen Xie
// 时间：2023/06/16
// 描述：XSF框架配置
// 说明：
//
//////////////////////////////////////////////////////////////////////////

using UnityEngine;
namespace XSF
{

    public sealed class XSFConfig : MonoSingleton<XSFConfig>
    {
        [Header("游戏默认帧率")][Range(30, 60)] public int TargetFrameRate = 60;



        [Header("从文件加载配置")] public bool LoadScpInFiles;

        [Header("网络心跳间隔（秒）")] public float HeartbeatInterval;

        [Header("是否开启资源更新")] public bool AASUpdateOpen;


        protected override void Awake()
        {
            base.Awake();

#if UNITY_EDITOR
        
#else
            LoadScpInFiles = false;
            AASUpdateOpen = true;
#endif
            Application.targetFrameRate = TargetFrameRate;
        }



        void Start()
        {

        }


    }
}