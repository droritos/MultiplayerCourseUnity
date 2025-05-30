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
        if (currentPlayer >= maxPlayers)
            ButtonIteraction(false);
        else
            ButtonIteraction(true);

        this.sessionNameText.text = sesstionName;
        playersInfo.text = currentPlayer + "/" + maxPlayers;
    }
    public void SetJoinAction(string sessionName , NetworkManager networkManager, UnityAction unityAction = default)
    {
        enterSession.onClick.RemoveAllListeners();
        enterSession.onClick.AddListener(() => networkManager.JoinSession(sessionName));

        if(unityAction != default)
            enterSession.onClick.AddListener(unityAction);
    }

    public void ButtonIteraction(bool state)
    {
        enterSession.interactable = state;
    }
}
