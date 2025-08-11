using System;
using System.Collections.Generic;
using Fusion;
using Fusion.Sockets;
using Game.Client.Input;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

namespace Game.Client
{
    public enum PlayerInputButtons
    {
        SprintButton,
        PlaceBombButton
    }

    [RequireComponent(typeof(NetworkTransform))]
    public class PlayerInput : NetworkBehaviour, INetworkRunnerCallbacks
    {
        private InputSystemActions _inputSystemActions;

        private Vector2 _move;

        private void Awake() => _inputSystemActions = new InputSystemActions();

        private void OnEnable()
        {
            if (!HasInputAuthority || !HasStateAuthority) return;

            _inputSystemActions.Player.Move.performed += OnMovePerformed;
            _inputSystemActions.Player.Move.canceled += OnMoveCancelled;

            _inputSystemActions.Enable();

            Runner.AddCallbacks(this);
        }

        private void OnDisable()
        {
            if (!HasInputAuthority || !HasStateAuthority) return;

            _inputSystemActions.Disable();

            _inputSystemActions.Player.Move.performed += OnMovePerformed;
            _inputSystemActions.Player.Move.canceled += OnMoveCancelled;

            Runner.RemoveCallbacks(this);
        }

        private void OnMovePerformed(InputAction.CallbackContext ctx) => _move = ctx.ReadValue<Vector2>();
        private void OnMoveCancelled(InputAction.CallbackContext ctx) => _move = Vector2.zero;

        public void OnInput(NetworkRunner runner, NetworkInput input)
        {
            var inputState = new PlayerInputState { move = _move };
            inputState.buttons.Set(PlayerInputButtons.SprintButton, _inputSystemActions.Player.Sprint.IsPressed());
            inputState.buttons.Set(PlayerInputButtons.PlaceBombButton, _inputSystemActions.Player.PlaceBomb.IsPressed());
            input.Set(inputState);
        }

        #region Empty INetworkRunnerCallbacks functions
        // other network runner callbacks, intentionally empty
        public void OnObjectExitAOI(NetworkRunner runner, NetworkObject obj, PlayerRef player) {}

        public void OnObjectEnterAOI(NetworkRunner runner, NetworkObject obj, PlayerRef player) {}

        public void OnPlayerJoined(NetworkRunner runner, PlayerRef player) {}

        public void OnPlayerLeft(NetworkRunner runner, PlayerRef player) {}

        public void OnShutdown(NetworkRunner runner, ShutdownReason shutdownReason) {}

        public void OnDisconnectedFromServer(NetworkRunner runner, NetDisconnectReason reason) {}

        public void OnConnectRequest(NetworkRunner runner, NetworkRunnerCallbackArgs.ConnectRequest request, byte[] token) {}

        public void OnConnectFailed(NetworkRunner runner, NetAddress remoteAddress, NetConnectFailedReason reason) {}

        public void OnUserSimulationMessage(NetworkRunner runner, SimulationMessagePtr message) {}

        public void OnReliableDataReceived(NetworkRunner runner, PlayerRef player, ReliableKey key, ArraySegment<byte> data) {}

        public void OnReliableDataProgress(NetworkRunner runner, PlayerRef player, ReliableKey key, float progress) {}

        public void OnInputMissing(NetworkRunner runner, PlayerRef player, NetworkInput input) {}
        public void OnConnectedToServer(NetworkRunner runner) {}

        public void OnSessionListUpdated(NetworkRunner runner, List<SessionInfo> sessionList) {}

        public void OnCustomAuthenticationResponse(NetworkRunner runner, Dictionary<string, object> data) {}

        public void OnHostMigration(NetworkRunner runner, HostMigrationToken hostMigrationToken) {}

        public void OnSceneLoadDone(NetworkRunner runner) {}

        public void OnSceneLoadStart(NetworkRunner runner) {}
        #endregion
    }

    public class PlayerInputSender : MonoBehaviour, INetworkInput {}

    public struct PlayerInputState : INetworkInput
    {
        public Vector2 move;
        public NetworkButtons buttons;
    }
}
