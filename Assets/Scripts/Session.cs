using Fusion;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using WebSocketSharp;

public class Session : MonoBehaviour
{
    public event UnityAction<string,int> OnEnterSession;


    [SerializeField] TMP_InputField sessionNameInput;
    [SerializeField] Slider maxPlayerAllowed;
    [SerializeField] TextMeshProUGUI maxPlayerText;
    [SerializeField] Button startSesstion;

    [SerializeField] GameObject uxMessageToPlayer;

    private void OnEnable()
    {
        startSesstion.onClick.AddListener(OnStartSessionButtonClicked);
    }

    public void SetInteractables(bool state)
    {
        startSesstion.interactable = state;
        sessionNameInput.interactable = state;
        maxPlayerAllowed.interactable = state;
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
            SetInteractables(false);
            OnEnterSession?.Invoke(sessionName, maxPlayers);
        }
    }
}
