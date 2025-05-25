using NUnit.Framework;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using WebSocketSharp;
using System.Collections.Generic;
using Fusion;

public class UIManager : MonoBehaviour
{
    #region << Events >>
    public event UnityAction OnJoinedSession;
    public event UnityAction<string> OnEnterLobby;
    public event UnityAction<string, int> OnEnterSession
    {
        add { session.OnEnterSession += value; }
        remove { session.OnEnterSession -= value; }
    }

    public event UnityAction OnLeaveSession
    {
        add { InGameSessionMenu.OnLeaveSession += value; }
        remove { InGameSessionMenu.OnLeaveSession -= value; }
    }

    #endregion

    [SerializeField] SceneType currentScene;

    [Header("Lobby Data")]
    [SerializeField] TMP_InputField lobbyInput;
    [SerializeField] Button enterLobby;
    [SerializeField] GameObject uxMessageToPlayer;

    [Header("Session Data")]
    [SerializeField] TextMeshProUGUI totalSessions;
    [SerializeField] Session session;
    [SerializeField] SessionInfoConverter sessionInfoPrefab;

    [Header("Transforms")]
    [field: SerializeField] public InGameSession InGameSessionMenu { get; private set; }
    [SerializeField] RectTransform sessionParent;
    [SerializeField] RectTransform lobbyMenu;
    [SerializeField] RectTransform sessionMenu;
    private List<SessionInfoConverter> _sessionsList = new List<SessionInfoConverter>();

    public void JoinLobby() // Button Method
    {
        if (lobbyInput.text.IsNullOrEmpty())
        {
            uxMessageToPlayer.SetActive(true);
            return;
        }
        else
        {
            OnEnterLobby?.Invoke(lobbyInput.text);
            enterLobby.interactable = false;
        }
    }

    public void CreateSessions(List<SessionInfo> sessions)
    {
        // Clear existing UI
        foreach (Transform child in sessionParent)
        {
            Destroy(child.gameObject);
        }
        _sessionsList.Clear();

        for (int i = 0; i < sessions.Count; i++)
        {
            SessionInfoConverter newSesstion = Instantiate(sessionInfoPrefab, sessionParent);
            newSesstion.UpdateSession(sessions[i].Name, sessions[i].PlayerCount, sessions[i].MaxPlayers);
            _sessionsList.Add(newSesstion);
        }
        Debug.Log($"Created {sessions.Count} Sessions Convertors");
    }

    public void SetSessionButton(NetworkManager networkManager)
    {
        foreach (var SessionInfoConverter in _sessionsList)
        {
            SessionInfoConverter.SetJoinAction(SessionInfoConverter.SessionName, networkManager);
        }
    }


    public void SetTotalSessions(int amount = 0)
    {
        totalSessions.text = "Total Sesstion: " + amount.ToString();
    }

    public void HandleConnectionDisplay(SceneType state)
    {
        Debug.Log($"HandleConnectionDisplay called with state: {state}");

        if (sessionMenu == null)
        {
            Debug.LogError("sessionMenu reference is null!");
            return;
        }

        if (InGameSessionMenu == null)
        {
            Debug.LogError("InGameSessionMenu reference is null!");
            return;
        }

        if (lobbyMenu == null)
        {
            Debug.LogError("lobbyMenu reference is null!");
            return;
        }

        switch (state)
        {
            case SceneType.LobbyMeneScene:
                //Debug.Log("Switching to Lobby Menu Scene");
                sessionMenu.gameObject.SetActive(false);
                InGameSessionMenu.gameObject.SetActive(false);
                lobbyMenu.gameObject.SetActive(true);

                enterLobby.interactable = true;

                break;

            case SceneType.SessionMenuScene:
                //Debug.Log("Switching to Session Menu Scene");
                lobbyMenu.gameObject.SetActive(false);
                InGameSessionMenu.gameObject.SetActive(false);
                sessionMenu.gameObject.SetActive(true);

                foreach (var session in _sessionsList)
                {
                    session.ButtonIteraction(true);
                }

                session.SetInteractables(true);
                break;

            case SceneType.InGameMenuScene:
                //Debug.Log("Switching to In Game Menu Scene");
                lobbyMenu.gameObject.SetActive(false);
                InGameSessionMenu.gameObject.SetActive(true);
                sessionMenu.gameObject.SetActive(false);

                break;
        }

        //Debug.Log($"After switch - sessionMenu active: {sessionMenu.gameObject.activeSelf}, InGameSessionMenu active: {InGameSessionMenu.gameObject.activeSelf}, lobbyMenu active: {lobbyMenu.gameObject.activeSelf}");
    }
}