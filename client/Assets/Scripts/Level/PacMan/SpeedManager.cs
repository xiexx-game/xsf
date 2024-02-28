//////////////////////////////////////////////////////////////////////////
//
// 文件：Assets/Scripts/Level/PacMan/SpeedManager.cs
// 作者：Xoen Xie
// 时间：2023/06/21
// 描述：速度管理器
// 说明：
//
//////////////////////////////////////////////////////////////////////////
using UnityEngine;

public class SpeedManager
{
    public float CurrentSpeed {
        get {
            float speed = StdSpeed * BaseSpeed * StackSpeed;
            //Debug.Log("SpeedManager speed=" + speed);
            return speed;
        }
    }

    private bool m_IsGhost;

    public void Init(bool IsGhost)
    {
        m_IsGhost = IsGhost;

        var data = XSFSchema.Instance.Get<SchemaGlobal>((int)SchemaID.Global).Get((int)GlobalID.StdSpeed);
        StdSpeed = (data.data as CSVData_Float).fValue;
        
        if(m_IsGhost)
        {
            BaseSpeed = LevelGamePackMan.Instance.ScpLevels.fGhostSpeed;
        }
        else
        {
            BaseSpeed = LevelGamePackMan.Instance.ScpLevels.fMoveSpeed;
        }

        StackSpeed = 1.0f;

        StackSpeedTime = -1f;
        BaseSpeedTime = -1f;
    }   

    private float StdSpeed;

    private float BaseSpeed;
    private float BaseSpeedTime;

    private float StackSpeed;
    private float StackSpeedTime;

    public bool HasEnergy;

    public void ResetBaseSpeed()
    {
        if(m_IsGhost)
        {
            BaseSpeed = LevelGamePackMan.Instance.ScpLevels.fGhostSpeed;
        }
        else
        {
            BaseSpeed = LevelGamePackMan.Instance.ScpLevels.fMoveSpeed;
        }

        BaseSpeedTime = -1;
    }

    public void EatBean()
    {
        if(!m_IsGhost)
        {
            StackSpeed = LevelGamePackMan.Instance.ScpLevels.fBeanMoveSpeed;

            var data = XSFSchema.Instance.Get<SchemaGlobal>((int)SchemaID.Global).Get((int)GlobalID.SpeedDownTime);
            StackSpeedTime = (data.data as CSVData_Float).fValue;
        }
    }

    public void EnterTunnel()
    {
        if(m_IsGhost)
        {
            StackSpeed = LevelGamePackMan.Instance.ScpLevels.fGhostTunnelMoveSpeed;
            StackSpeedTime = -1;
        }
    }

    public void ExitTunnel()
    {
        if(m_IsGhost)
        {
            StackSpeed = 1;
            StackSpeedTime = -1;
        }
    }

    public void OnEnergy()
    {
        if(m_IsGhost)
        {
            BaseSpeed = LevelGamePackMan.Instance.ScpLevels.fEnergyGhostSpeed;
        }
        else
        {
            HasEnergy = true;
            BaseSpeed = LevelGamePackMan.Instance.ScpLevels.fEnergyMoveSpeed;
        }

        BaseSpeedTime = LevelGamePackMan.Instance.ScpLevels.fEnergyTime;
    }

    public void Update()
    {
        if(StackSpeedTime > 0)
        {
            StackSpeedTime -= Time.deltaTime;
            if(StackSpeedTime < 0)
            {
                StackSpeed = 1.0f;
            }
        }

        if(BaseSpeedTime > 0)
        {
            BaseSpeedTime -= Time.deltaTime;
            if(BaseSpeedTime < 0)
            {
                HasEnergy = false;
                ResetBaseSpeed();
            }
        }
    }
}