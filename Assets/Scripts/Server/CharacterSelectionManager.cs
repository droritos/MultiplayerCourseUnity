using System;
using System.Collections.Generic;
using System.Linq;
using Fusion;
using UnityEngine;

namespace Game.Server
{
    public class CharacterSelectionManager : NetworkBehaviour, IPlayerJoined, IPlayerLeft
    {
        [SerializeField] private GameData gameData;
        private readonly Dictionary<int, PlayerRef> _characterOwners = new();

        public event Action<bool, int> OnCharacterRequestResponse;
        public event Action<int[]> OnUpdateCharacterMarkedStatus;

        #region Player Connection Callbacks
        public void PlayerJoined(PlayerRef player) => UpdateCharacterMarkStatus();

        public void PlayerLeft(PlayerRef player)
        {
            FreePlayerCurrentCharacter(player);
            UpdateCharacterMarkStatus();
        }
        #endregion

        #region Character Management
        private void FreePlayerCurrentCharacter(PlayerRef player)
        {
            if (!_characterOwners.ContainsValue(player))
            {
                return;
            }

            var ownedCharacter = _characterOwners.First(kvp => kvp.Value.Equals(player));

            _characterOwners.Remove(ownedCharacter.Key);
            Debug.Log($"[Server] Player {player} freed character {ownedCharacter.Key}");
        }

        private void UpdateCharacterMarkStatus()
        {
            int[] occupied = _characterOwners.Keys.ToArray();
            Debug.Log($"[Server] Broadcasting character status to all clients: {string.Join(",", occupied)}");
            BroadcastOccupiedCharactersRPC(occupied);
        }
        #endregion

        #region Inbound RPCs
        [Rpc(RpcSources.All, RpcTargets.StateAuthority, HostMode = RpcHostMode.SourceIsHostPlayer)]
        public void RequestCharacterRPC(int characterIdx, RpcInfo info = default)
        {
            Debug.Log($"[Server] Requested character \"{gameData.characters[characterIdx].name}\" ({characterIdx})");
            bool isCharacterAvailable = !_characterOwners.ContainsKey(characterIdx);

            if (isCharacterAvailable)
            {
                FreePlayerCurrentCharacter(info.Source);
                _characterOwners.Add(characterIdx, info.Source);
            }

            NotifyCharacterSelectionResultRPC(info.Source, isCharacterAvailable, characterIdx);
            UpdateCharacterMarkStatus();
        }
        #endregion

        #region Outbound RPCs
        [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
        private void NotifyCharacterSelectionResultRPC([RpcTarget] PlayerRef player, bool canUseCharacter,
            int characterIdx) => OnCharacterRequestResponse?.Invoke(canUseCharacter, characterIdx);

        [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
        private void BroadcastOccupiedCharactersRPC(int[] occupiedCharacterIndices) => OnUpdateCharacterMarkedStatus?.Invoke(occupiedCharacterIndices);
        #endregion
    }
}
