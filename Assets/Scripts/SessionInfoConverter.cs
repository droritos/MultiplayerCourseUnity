using UnityEngine.UI;
using UnityEngine;
using TMPro;
using Fusion;
using UnityEngine.Events;

public class SessionInfoConverter : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI sessionNameText;
    public string SessionName => sessionNameText.text.ToString();

    [SerializeField] TextMeshProUGUI playersInfo;
    [SerializeField] Button enterSession;

    public void UpdateSession(string sesstionName , int currentPlayer ,int maxPlayers)
    {
        this.sessionNameText.text = sesstionName;
        playersInfo.text = currentPlayer + "/" + maxPlayers;
    }
    public void SetJoinAction(string sessionName , NetworkManager networkManager)
    {
        enterSession.onClick.RemoveAllListeners();
        enterSession.onClick.AddListener(() => networkManager.JoinSession(sessionName));
    }
}
