using Fusion;
using Fusion.Sockets;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class NetworkManager : MonoBehaviour, INetworkRunnerCallbacks
{
    public event UnityAction<List<SessionInfo>> OnSessionUpdated;
    public UnityEvent OnFinishConnection;
    public UnityEvent<bool> OnConnection;


    [SerializeField] NetworkRunner networkRunner;

    private List<SessionInfo> _sessionInfos;
    private bool _hasStarted = false;
    private void OnEnable()
    {
        networkRunner.AddCallbacks(this);
    }

    public async void StartSession(string sessionName, int maxPlayerCount)
    {
        if (_hasStarted) return;
        _hasStarted = true;

        var result = await networkRunner.StartGame(new StartGameArgs()
        {
            GameMode = GameMode.Shared,
            SessionName = sessionName,
            CustomLobbyName = networkRunner.LobbyInfo.Name,
            PlayerCount = maxPlayerCount,
            OnGameStarted = OnGameStarted,

        });

        if (result.Ok)
        {
            OnFinishConnection?.Invoke();

            Debug.Log("Game started successfully.");
        }
        else
        {
            Debug.LogError($"Failed to start game: {result.ShutdownReason}");
        }

        if(_sessionInfos != null)
            OnSessionUpdated?.Invoke(_sessionInfos);
    }

    public async void JoinSession(string sessionName)
    {
        if (_hasStarted) return;
        _hasStarted = true;

        var result = await networkRunner.StartGame(new StartGameArgs()
        {
            GameMode = GameMode.Shared,
            SessionName = sessionName,
            CustomLobbyName = networkRunner.LobbyInfo.Name,
            OnGameStarted = OnGameStarted,

        });

        if (result.Ok)
        {
            OnFinishConnection?.Invoke();

            Debug.Log("Game started successfully.");
        }
        else
        {
            Debug.LogError($"Failed to start game: {result.ShutdownReason}");
        }

        if (_sessionInfos != null)
            OnSessionUpdated?.Invoke(_sessionInfos);
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
        {
            OnFinishConnection?.Invoke();
        }

        if (_sessionInfos != null)
            OnSessionUpdated?.Invoke(_sessionInfos);

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

        OnConnection.Invoke(true);
    }

    public void OnPlayerLeft(NetworkRunner runner, PlayerRef player)
    {
        OnConnection.Invoke(true);
        Debug.Log("Player Left");
    }

    public void OnSessionListUpdated(NetworkRunner runner, List<SessionInfo> sessionList)
    {
        _sessionInfos = sessionList;
        OnSessionUpdated?.Invoke(_sessionInfos);
        Debug.Log("Session Updated Invoke Event!");
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
        OnConnection.Invoke(false);
    }
    public void OnConnectedToServer(NetworkRunner runner)
    {
        OnConnection.Invoke(true);
        // Apply UI of the Players in the sesstion
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
