using Fusion;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.Events;

public class InGameSession : MonoBehaviour
{
    public event UnityAction OnLeaveSession;
    private SessionInfo _currentSession;

    [SerializeField] TextMeshProUGUI sessionName;
    [SerializeField] TextMeshProUGUI currentPlayers;

    [SerializeField] Button returnToLobby;

    public void UpdateInGameInfo(NetworkRunner runner)
    {
        if (!runner)
        {
            Debug.Log("Session Is Null Cant Update UI In Game");
            return;
        }

        _currentSession = runner.SessionInfo;

        sessionName.text = "In Session: " + _currentSession.Name;
        currentPlayers.text = $"{_currentSession.PlayerCount}/{_currentSession.MaxPlayers}";

        returnToLobby.onClick.AddListener(OnLeaveSession);

    }
}
