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
    public event UnityAction<string> OnEnterLobby;
    public event UnityAction<string, int> OnEnterSession
    {
        add { session.OnEnterSession += value; }
        remove { session.OnEnterSession -= value; }
    }

    #endregion


    [Header("Lobby Data")]
    [SerializeField] TMP_InputField lobbyInput;
    [SerializeField] Button enterLobby;
    [SerializeField] GameObject uxMessageToPlayer;

    [Header("Session Data")]
    [SerializeField] TextMeshProUGUI totalSessions;
    [SerializeField] Session session;
    [SerializeField] SessionInfoConverter sessionInfoPrefab;

    [Header("Transforms")]
    [SerializeField] RectTransform sessionParent;
    [SerializeField] RectTransform lobbyMenu;
    [SerializeField] RectTransform sessionMenu;
    [SerializeField] RectTransform inGameMenu;
    private List<SessionInfoConverter> _sesstionsList;

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
            Destroy(child.gameObject);

        _sesstionsList = new List<SessionInfoConverter>();
        for (int i = 0; i < sessions.Count; i++)
        {
            SessionInfoConverter newSesstion = Instantiate(sessionInfoPrefab, sessionParent);
            newSesstion.UpdateSession(sessions[i].Name, sessions[i].PlayerCount, sessions[i].MaxPlayers);
            _sesstionsList.Add(newSesstion);
        }
        Debug.Log($"Created {sessions.Count} Sessions Convertors");
    }

    public void SetSessionButton(NetworkManager networkManager)
    {
        foreach (var SessionInfoConverter in _sesstionsList)
        {
            SessionInfoConverter.SetJoinAction(SessionInfoConverter.SessionName, networkManager);
        }
    }


    public void SetTotalSessions(int amount = 0)
    {
        totalSessions.text = "Total Sesstion: " + amount.ToString();
    }

    public void HandleConnectionDisplay(bool state)
    {
        if (state)
        {
            sessionMenu.gameObject.SetActive(false);
            inGameMenu.gameObject.SetActive(true);



        }
        else
        {

        }
    }
}
