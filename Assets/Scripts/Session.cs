using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using WebSocketSharp;

public class Session : MonoBehaviour
{
    public event UnityAction OnSessestionStarted;

    [SerializeField] TMP_InputField sessionNameInput;
    [SerializeField] Slider maxPlayerAllowed;
    [SerializeField] TextMeshProUGUI maxPlayerText;
    [SerializeField] Button startSesstion;

    public void StartSesstion(LobbyManager lobbyManager)
    {
        if (sessionNameInput.text.IsNullOrEmpty())
        {
            // Apply UX effect that notify players that they must enter a name
            return;
        }

        lobbyManager.StartSession(sessionNameInput.text,(int)maxPlayerAllowed.value);
        OnSessestionStarted.Invoke();
    }
    public void SetMaxPlayers()
    {
        maxPlayerText.text = maxPlayerAllowed.value.ToString();
    }
    public void TryStartSession(TMP_InputField inputText)
    {
        if (sessionNameInput.text.IsNullOrEmpty())
            return;
    }


}
