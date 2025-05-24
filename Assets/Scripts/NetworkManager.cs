using Fusion;
using Fusion.Sockets;
using System;
using System.Collections.Generic;
using UnityEngine;

public class NetworkManager : MonoBehaviour, INetworkRunnerCallbacks
{
    [SerializeField] NetworkRunner networkRunner;

    private bool _hasStarted = false;

    private const string _sessionName = "Funny Server";
    public List<SessionInfo> SessionList {  get; private set; }

    private void OnEnable()
    {
        networkRunner.AddCallbacks(this);
    }

    public async void StartSession(string SesstionName, int MaxPlayerCount)
    {
        if (_hasStarted) return;
        _hasStarted = true;

        var result = await networkRunner.StartGame(new StartGameArgs()
        {
            GameMode = GameMode.Shared,
            SessionName = SesstionName,
            CustomLobbyName = networkRunner.LobbyInfo.Name,
            PlayerCount = MaxPlayerCount,
            OnGameStarted = OnGameStarted,

        });

        if (result.Ok)
        {
            Debug.Log("Game started successfully.");
        }
        else
        {
            Debug.LogError($"Failed to start game: {result.ShutdownReason}");
        }
    }
    private void OnGameStarted(NetworkRunner obj)
    {
        Debug.Log("Game Started");
    }

    public async void JoinLobby(string lobbyName)
    {
        StartGameResult result =
            await networkRunner.JoinSessionLobby(SessionLobby.Custom, lobbyName);

        if (result.Ok)
            Debug.Log($"Joined Lobby: {lobbyName}");
    }

    void OnValidate()
    {
        if (!networkRunner)
        {
            networkRunner = FindAnyObjectByType<NetworkRunner>();
        }
    }


    #region << Network Callbacks >>

    public void OnPlayerJoined(NetworkRunner runner, PlayerRef player)
    {

        bool isLocalPlayer = false;

        if(networkRunner.LocalPlayer == player)
            isLocalPlayer = true;


        Debug.Log($"{player.PlayerId} Joined, Local Player: {isLocalPlayer}");

        // Apply Update UI
    }

    public void OnPlayerLeft(NetworkRunner runner, PlayerRef player)
    {
        Debug.Log("Player Left");
    }

    public void OnSessionListUpdated(NetworkRunner runner, List<SessionInfo> sessionList)
    {
        SessionList = sessionList;

        Debug.Log($"Session List Updated {sessionList.Count} Sessions");

        foreach (SessionInfo sessionInfo in sessionList)
        {
            //_sessionList.Add(sessionInfo);
            Debug.Log($"Session Name: {sessionInfo.Name}, Player Count: {sessionInfo.PlayerCount}");
        }
    }
    public void OnConnectFailed(NetworkRunner runner, NetAddress remoteAddress, NetConnectFailedReason reason)
    {
        Debug.Log("Connect Failed");
    }
    public void OnShutdown(NetworkRunner runner, ShutdownReason shutdownReason)
    {

    }
    public void OnDisconnectedFromServer(NetworkRunner runner, NetDisconnectReason reason)
    {
        throw new NotImplementedException();
    }
    public void OnConnectedToServer(NetworkRunner runner)
    {
        //throw new NotImplementedException();
    }
    public void OnObjectExitAOI(NetworkRunner runner, NetworkObject obj, PlayerRef player)
    {
        throw new NotImplementedException();
    }
    public void OnObjectEnterAOI(NetworkRunner runner, NetworkObject obj, PlayerRef player)
    {
        throw new NotImplementedException();
    }
    public void OnConnectRequest(NetworkRunner runner, NetworkRunnerCallbackArgs.ConnectRequest request, byte[] token)
    {
        throw new NotImplementedException();
    }
    public void OnUserSimulationMessage(NetworkRunner runner, SimulationMessagePtr message)
    {
        throw new NotImplementedException();
    }
    public void OnReliableDataReceived(NetworkRunner runner, PlayerRef player, ReliableKey key, ArraySegment<byte> data)
    {
        throw new NotImplementedException();
    }
    public void OnReliableDataProgress(NetworkRunner runner, PlayerRef player, ReliableKey key, float progress)
    {
        throw new NotImplementedException();
    }
    public void OnInput(NetworkRunner runner, NetworkInput input)
    {
        // Do Nothing no players yet
        //throw new NotImplementedException();
    }
    public void OnInputMissing(NetworkRunner runner, PlayerRef player, NetworkInput input)
    {
        throw new NotImplementedException();
    }
    public void OnCustomAuthenticationResponse(NetworkRunner runner, Dictionary<string, object> data)
    {
        throw new NotImplementedException();
    }
    public void OnHostMigration(NetworkRunner runner, HostMigrationToken hostMigrationToken)
    {
        throw new NotImplementedException();
    }
    public void OnSceneLoadDone(NetworkRunner runner)
    {
        throw new NotImplementedException();
    }
    public void OnSceneLoadStart(NetworkRunner runner)
    {
        throw new NotImplementedException();
    }
    #endregion
}
