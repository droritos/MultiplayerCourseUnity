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

        uiManager.OnEnterLobby += netsworkManager.JoinLobby;
        uiManager.OnEnterSession += netsworkManager.StartSession;


    }

    private void CreateSessions(List<SessionInfo> sessionInfos)
    {
        if (sessionInfos.Count <= 0 || sessionInfos == null) return;
        uiManager.SetTotalSessions(sessionInfos.Count);
        uiManager.CreateSessions(sessionInfos);

        uiManager.SetSessionButton(netsworkManager);
    }

    private void OnValidate()
    {
        if(!netsworkManager)
            netsworkManager = FindAnyObjectByType<NetworkManager>();

        if(!uiManager)
            uiManager = FindAnyObjectByType<UIManager>();
    }
}
