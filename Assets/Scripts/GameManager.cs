using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class GameManager : MonoBehaviour
{
    [SerializeField] NetworkManager netsworkManager;
    [SerializeField] UIManager uiManager;


    private void Awake()
    {
        uiManager.OnEnterLobby += netsworkManager.JoinLobby;
        uiManager.OnEnterSession += netsworkManager.StartSession;

        if (netsworkManager.SessionList != null) // Create Session if is there any
        {
            uiManager.CreateSessions(netsworkManager.SessionList);
            uiManager.SetTotalSessions(netsworkManager.SessionList.Count);
        }
        else
        {
            uiManager.SetTotalSessions(0);
        }
    }

    private void OnValidate()
    {
        if(!netsworkManager)
            netsworkManager = FindAnyObjectByType<NetworkManager>();

        if(!uiManager)
            uiManager = FindAnyObjectByType<UIManager>();
    }
}
