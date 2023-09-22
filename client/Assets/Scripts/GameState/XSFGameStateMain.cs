//////////////////////////////////////////////////////////////////////////
//
// 文件：Assets\XSF\GameState\XSFGameStateMain.cs
// 作者：Xiexx
// 时间：2022/04/11
// 描述：游戏状态机
// 说明：
//
//////////////////////////////////////////////////////////////////////////
using System;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

public class XSFGameStateMain : XSFGameState, ICharacterEvent, XSFAnimHandler
{
    enum MainSceneObjID
    {
        Character = 0,
        Lobby,
        Title,
        Max,
    }

    enum RunStatus
    {
        None = 0,
        LoadCharacter,
        LoadLobby,
        LoadTitle,
        ShowUI,
        Wait,
        Change,
    }

    private GameObject[] m_Objs;

    private RunStatus m_nStatus;

    public override XSFGSID mID { get { return XSFGSID.Main; } }

    private int m_CharacterIndex;
    public override bool Enter()
    {
        m_nStatus = RunStatus.LoadCharacter;
        m_CharacterIndex = 0;

        return true;
    }

    public override void Exit()
    {

    }

    public override void OnUpdate()
    {
        switch(m_nStatus)
        {
        case RunStatus.LoadCharacter:
            LoadAsset(MainSceneObjID.Character, "m_22");
            m_nStatus = RunStatus.None;
            break;

        case RunStatus.LoadLobby:
            LoadAsset(MainSceneObjID.Lobby, "lobby");
            m_nStatus = RunStatus.None;
            break;

        case RunStatus.LoadTitle:
            LoadAsset(MainSceneObjID.Title, "title");
            m_nStatus = RunStatus.None;
            break;

        case RunStatus.ShowUI:
            XSFUI.Instance.ShowUI((int)UIID.UIMain);
            m_nStatus = RunStatus.None;
            break;

        case RunStatus.Change:
            m_nStatus = RunStatus.Wait;
            var schema = XSFSchema.Instance.Get<SchemaGlobal>((int)SchemaID.Global) as SchemaGlobal;
            var array = schema.Get((uint)GlobalID.CharacterList).data as CSVData_SArray;
            if(m_CharacterIndex >= array.sarValue.Length)
            {
                m_CharacterIndex = 0;
            }

            LoadAsset(MainSceneObjID.Character, array.sarValue[m_CharacterIndex]);
            m_CharacterIndex ++;
            break;
        }
    }

    public void OnLobbyEnterDone()
    {
        if(m_nStatus == RunStatus.Wait)
        {
            m_nStatus = RunStatus.None;
        }
        else 
        {
            m_nStatus = RunStatus.LoadLobby;
        }
    }

    public void OnLobbyExitDone()
    {
        Addressables.ReleaseInstance(m_Objs[(int)MainSceneObjID.Character]);
        m_Objs[(int)MainSceneObjID.Character] = null;
        m_nStatus = RunStatus.Change;
    }

    public void ChangeCharacter()
    {
        if(m_nStatus != RunStatus.None)
            return;

        m_nStatus = RunStatus.Wait;
        var t = m_Objs[(int)MainSceneObjID.Character].transform;
        var mono = t.GetComponent<MonoCharacter>();
        mono.LobbyRunExit();
    }

    private void LoadAsset(MainSceneObjID id, string name)
    {
        var handle = Addressables.InstantiateAsync(name);
        handle.Completed += (op) =>
        {
            if (op.Status == AsyncOperationStatus.Succeeded)
            {
                if(m_Objs == null)
                    m_Objs = new GameObject[(int)MainSceneObjID.Max];

                m_Objs[(int)id] = op.Result;
                switch(id)
                {
                case MainSceneObjID.Character:
                    {
                        m_Objs[(int)id].SetActive(true);
                        var t = m_Objs[(int)id].transform;
                        var mono = t.GetComponent<MonoCharacter>();
                        mono.Init(this);
                        mono.LobbyBorn();
                    }
                    
                    break;

                case MainSceneObjID.Lobby:
                case MainSceneObjID.Title:
                    {
                        var t = m_Objs[(int)id].transform;
                        var mono = t.GetComponent<MonoLevel>();
                        mono.Init(this);
                    }
                    break;

                }
            }
            else
            {
                Debug.LogError("LoadingGameObject.Start error, name=" + name);
            }
        };
    }

    public void OnAnimFinish(GameObject obj, string param)
    {
        if(param == "LobbyShow")
        {
            m_nStatus = RunStatus.LoadTitle;
        }
        else if(param == "TitleShow")
        {
            m_nStatus = RunStatus.ShowUI;
        }
    }


}
