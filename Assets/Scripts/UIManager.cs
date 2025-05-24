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
    [SerializeField] RectTransform sesstionParent;


    private List<SessionInfoConverter> _sesstionsList;

    public void JoinLobby() // Button Method
    {
        if (lobbyInput.text.IsNullOrEmpty())
        {
            uxMessageToPlayer.SetActive(true);
            return;
        }
        else
            OnEnterLobby?.Invoke(lobbyInput.text);
    }

    public void ButtonClicked(Button button) // Button Method
    {
        button.interactable = false;
    }
    public void CreateSessions(List<SessionInfo> sessions)
    {
        for (int i = 0; i < sessions.Count; i++)
        {
            SessionInfoConverter newSesstion = Instantiate(sessionInfoPrefab, sesstionParent);
            newSesstion.UpdateSession(sessions[i].Name, sessions[i].PlayerCount, sessions[i].MaxPlayers);

            _sesstionsList.Add(newSesstion);
        }
    }

    public void SetTotalSessions(int amount = 0)
    {
        totalSessions.text = amount.ToString();
    }
}
