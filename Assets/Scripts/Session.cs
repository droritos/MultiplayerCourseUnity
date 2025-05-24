using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using WebSocketSharp;

public class Session : MonoBehaviour
{
    public event UnityAction<string, int> OnEnterSession;


    [SerializeField] TMP_InputField sessionNameInput;
    [SerializeField] Slider maxPlayerAllowed;
    [SerializeField] TextMeshProUGUI maxPlayerText;
    [SerializeField] Button startSesstion;

    [SerializeField] GameObject uxMessageToPlayer;

    private void Start()
    {
        startSesstion.onClick.AddListener(OnStartSessionButtonClicked);
    }
    private void OnStartSessionButtonClicked() // Button Method
    {
        string sessionName = sessionNameInput.text;
        int maxPlayers = (int)maxPlayerAllowed.value;

        StartSesstion(sessionName, maxPlayers);
    }

    private void StartSesstion(string sessionName,int maxPlayers)
    {
        if (sessionNameInput.text.IsNullOrEmpty())
        {
            uxMessageToPlayer.SetActive(true);
            return;
        }
        else
        {
            startSesstion.interactable = false;
            sessionNameInput.interactable = false;
            maxPlayerAllowed.interactable = false;
            OnEnterSession?.Invoke(sessionName, maxPlayers);
        }
    }

    public void SetMaxPlayers()
    {
        maxPlayerText.text = maxPlayerAllowed.value.ToString();
    }
    public void TryStartSession()
    {
        uxMessageToPlayer.SetActive(true);
    }


}
