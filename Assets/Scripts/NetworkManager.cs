using Fusion;
using Fusion.Sockets;
using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;

public class NetworkManager : MonoBehaviour, INetworkRunnerCallbacks
{
    public event UnityAction<List<SessionInfo>> OnSessionUpdated;
    public event UnityAction<SessionInfo> OnSessionJoined;
    public UnityEvent<SceneType> OnConnection;

    [field: SerializeField] public NetworkRunner NetworkRunner {  get; private set; }

    private List<SessionInfo> _sessionInfos;
    //private bool _hasStarted = false;
    private void OnEnable()
    {
        NetworkRunner.AddCallbacks(this);
    }

    public async void StartSession(string sessionName, int maxPlayerCount)
    {
        //if (_hasStarted) return;
        //_hasStarted = true;

        var result = await NetworkRunner.StartGame(new StartGameArgs()
        {
            GameMode = GameMode.Shared,
            SessionName = sessionName,
            CustomLobbyName = NetworkRunner.LobbyInfo.Name,
            PlayerCount = maxPlayerCount,
            OnGameStarted = OnGameStarted,
        });

        if (result.Ok)
        {
            //OnConnection?.Invoke(SceneType.InGameMenuScene);
            OnSessionJoined?.Invoke(NetworkRunner.SessionInfo);
            Debug.Log("Game started successfully.");
        }
        else
        {
            Debug.LogError($"Failed to start game: {result.ShutdownReason}");
            if (!NetworkRunner)
            {
                NetworkRunner = this.AddComponent<NetworkRunner>();
                NetworkRunner.AddCallbacks(this);
            }
            OnConnection.Invoke(SceneType.LobbyMeneScene); // Return To Lobby
        }

        if (_sessionInfos != null)
            OnSessionUpdated?.Invoke(_sessionInfos);
    }

    public async void JoinSession(string sessionName)
    {
        //if (_hasStarted) return;
        //_hasStarted = true;

        var result = await NetworkRunner.StartGame(new StartGameArgs()
        {
            GameMode = GameMode.Shared,
            SessionName = sessionName,
            CustomLobbyName = NetworkRunner.LobbyInfo.Name,
            OnGameStarted = OnGameStarted,

        });

        if (result.Ok)
        {
            //OnConnection?.Invoke(SceneType.InGameMenuScene);
            OnSessionJoined?.Invoke(NetworkRunner.SessionInfo);
            Debug.Log("Game started successfully.");
        }
        else
        {
            Debug.LogError($"Failed to start game: {result.ShutdownReason}");
            if (!NetworkRunner) // Handle NetworkRunner destroyed
            {
                NetworkRunner = this.AddComponent<NetworkRunner>();
                NetworkRunner.AddCallbacks(this);
            }
            OnConnection.Invoke(SceneType.LobbyMeneScene); // Return To Lobby
        }

        if (_sessionInfos != null)
            OnSessionUpdated?.Invoke(_sessionInfos);
    }

    public async void JoinLobby(string lobbyName)
    {
        StartGameResult result =
            await NetworkRunner.JoinSessionLobby(SessionLobby.Custom, lobbyName);

        if (result.Ok)
        {
            OnConnection?.Invoke(SceneType.SessionMenuScene);
        }

        if (_sessionInfos != null)
            OnSessionUpdated?.Invoke(_sessionInfos);

    }

    public async void LeaveSession()
    {
        Debug.Log("Leaving session...");

        if (NetworkRunner != null)
        {
            await NetworkRunner.Shutdown();
        }
    }
    private void OnGameStarted(NetworkRunner obj)
    {
        Debug.Log("Game Started");
    }

    void OnValidate()
    {
        if (!NetworkRunner)
        {
            NetworkRunner = FindAnyObjectByType<NetworkRunner>();
        }
    }
    private void OnDisable()
    {
        if (NetworkRunner != null)
        {
            NetworkRunner.RemoveCallbacks(this);
        }
    }


    #region << Network Callbacks >>
    public void OnPlayerJoined(NetworkRunner runner, PlayerRef player)
    {

        bool isLocalPlayer = false;

        if(NetworkRunner.LocalPlayer == player)
            isLocalPlayer = true;

        Debug.Log($"{player.PlayerId} Joined, Local Player: {isLocalPlayer}");

        OnSessionJoined?.Invoke(NetworkRunner.SessionInfo);


    }

    public void OnPlayerLeft(NetworkRunner runner, PlayerRef player)
    {
        //OnConnection.Invoke(true);

        OnSessionJoined?.Invoke(NetworkRunner.SessionInfo);

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

        if (!NetworkRunner) // Handle NetworkRunner destroyed
        {
            NetworkRunner = this.AddComponent<NetworkRunner>();
            NetworkRunner.AddCallbacks(this);
        }

        OnConnection.Invoke(SceneType.LobbyMeneScene);
    }
    public void OnDisconnectedFromServer(NetworkRunner runner, NetDisconnectReason reason)
    {
        OnConnection.Invoke(SceneType.SessionMenuScene);
    }
    public void OnConnectedToServer(NetworkRunner runner)
    {
        OnConnection.Invoke(SceneType.InGameMenuScene);
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
