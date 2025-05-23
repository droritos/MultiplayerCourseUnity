using NUnit.Framework;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using WebSocketSharp;
using System.Collections.Generic;

public class UIManager : MonoBehaviour
{
    public event UnityAction<string> OnEnterLobby;
    public event UnityAction OnEmptyOrNullInput;

    [SerializeField] TMP_InputField lobbyInput;
    [SerializeField] Button enterLobby;

    [SerializeField] GameObject uxMessageToPlayer;

    [SerializeField] List<Session> sesstionsList;

    public void JoinLobby()
    {
        EmptyOrNullInput(lobbyInput.text);

        OnEnterLobby?.Invoke(lobbyInput.text);
    }

    public void ButtonClicked(Button button)
    {
        button.interactable = false;
    }

    public void EmptyOrNullInput(string text)
    {
        if (text.IsNullOrEmpty())
        {
            uxMessageToPlayer.SetActive(true);
            OnEmptyOrNullInput.Invoke();
        }
    }

    private void HandleEmptyInput()
    {

    }

}
