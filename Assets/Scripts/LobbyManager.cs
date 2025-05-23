using Fusion;
using UnityEngine;

public class LobbyManager : MonoBehaviour
{
    [SerializeField] NetworkRunner networkRunner;
    private const string SessionName = "Funny Server";
    private void OnEnable()
    {
        StartSession();
    }
    public void StartSession()
    {
        networkRunner.StartGame(new StartGameArgs()
        {
            GameMode = GameMode.Shared,
            SessionName = SessionName,
            OnGameStarted = OnGameStarted
        });
    }

    private void OnGameStarted(NetworkRunner obj)
    {
        Debug.Log("Game Started");
    }

     void OnValidate()
    {
      if (!networkRunner)
        {
            networkRunner = FindAnyObjectByType<NetworkRunner>();
        }
    }
}
