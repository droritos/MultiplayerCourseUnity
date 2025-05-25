using Fusion;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class GameManager : MonoBehaviour
{
    [SerializeField] NetworkManager netsworkManager;
    [SerializeField] UIManager uiManager;


    private void Awake()
    {
        netsworkManager.OnSessionUpdated += CreateSessions;
        netsworkManager.OnConnection.AddListener(uiManager.HandleConnectionDisplay);

        netsworkManager.OnSessionJoined += (sessionInfo) =>
        {
            uiManager.InGameSessionMenu.UpdateInGameInfo(netsworkManager.NetworkRunner);
        };

        uiManager.OnEnterLobby += netsworkManager.JoinLobby;
        uiManager.OnEnterSession += netsworkManager.StartSession;
        uiManager.OnLeaveSession += netsworkManager.LeaveSession;
    }
    private void OnDisable()
    {
        netsworkManager.OnSessionUpdated -= CreateSessions;
        netsworkManager.OnConnection.RemoveListener(HandleConnectionDisplay);

        netsworkManager.OnSessionJoined -= (sessionInfo) =>
        {
            uiManager.InGameSessionMenu.UpdateInGameInfo(netsworkManager.NetworkRunner);
        };

        uiManager.OnEnterLobby -= netsworkManager.JoinLobby;
        uiManager.OnEnterSession -= netsworkManager.StartSession;
        uiManager.OnLeaveSession -= netsworkManager.LeaveSession;
    }

    private void CreateSessions(List<SessionInfo> sessionInfos)
    {
        if (sessionInfos.Count <= 0 || sessionInfos == null) return;
        uiManager.SetTotalSessions(sessionInfos.Count);
        uiManager.CreateSessions(sessionInfos);

        uiManager.SetSessionButton(netsworkManager);
    }

    private void HandleConnectionDisplay(SceneType sceneType)
    {
        uiManager.HandleConnectionDisplay(sceneType);
        uiManager.InGameSessionMenu.UpdateInGameInfo(netsworkManager.NetworkRunner);
    }

    private void OnValidate()
    {
        if(!netsworkManager)
            netsworkManager = FindAnyObjectByType<NetworkManager>();

        if(!uiManager)
            uiManager = FindAnyObjectByType<UIManager>();
    }
}

public enum SceneType
{
    LobbyMeneScene,
    SessionMenuScene,
    InGameMenuScene
};