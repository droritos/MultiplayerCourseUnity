using UnityEngine;
using UnityEngine.Events;

public class GameManager : MonoBehaviour
{
    [SerializeField] LobbyManager lobbyManager;
    [SerializeField] UIManager uiManager;

    private void Awake()
    {
        uiManager.OnEnterLobby += lobbyManager.JoinLobby;
    }

    private void OnValidate()
    {
        if(!lobbyManager)
            lobbyManager = FindAnyObjectByType<LobbyManager>();

        if(!uiManager)
            uiManager = FindAnyObjectByType<UIManager>();
    }
}
