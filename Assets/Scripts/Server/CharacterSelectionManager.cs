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

        public void PlayerJoined(PlayerRef player) => UpdateCharacterMarkStatusRPC();

        public void PlayerLeft(PlayerRef player)
        {
            FreePlayerCurrentCharacter(player);
            UpdateCharacterMarkStatusRPC();
        }

        public event Action<bool, int> OnCharacterRequestResponse;
        public event Action<List<int>> OnUpdateCharacterMarkedStatus;

        private void FreePlayerCurrentCharacter(PlayerRef player)
        {
            if (!_characterOwners.ContainsValue(player))
            {
                return;
            }

            var ownedCharacter = _characterOwners.First(kvp => kvp.Value.Equals(player));

            _characterOwners.Remove(ownedCharacter.Key);
            Debug.Log($"Player {player} freed character {ownedCharacter.Key}");
        }

        #region RPCs

        [Rpc(RpcSources.All, RpcTargets.StateAuthority, HostMode = RpcHostMode.SourceIsHostPlayer)]
        public void RequestCharacterRPC(int characterIdx, RpcInfo info = default)
        {
            Debug.Log("Requested character idx");
            bool isCharacterFreed = !_characterOwners.ContainsKey(characterIdx);

            FreePlayerCurrentCharacter(info.Source);

            if (isCharacterFreed)
            {
                _characterOwners.Add(characterIdx, info.Source);
            }

            ReturnCharacterRequestAnswerRPC(info.Source, isCharacterFreed, characterIdx);
            UpdateCharacterMarkStatusRPC();
        }

        [Rpc(RpcSources.All, RpcTargets.StateAuthority, HostMode = RpcHostMode.SourceIsHostPlayer)]
        private void RequestCharacterFreeRPC(int characterIdx, RpcInfo info = default)
        {
            if (!_characterOwners.TryGetValue(characterIdx, out var owner))
            {
                Debug.Log("Character is free, no need to return it");
                return;
            }

            if (owner != info.Source)
            {
                Debug.Log("Cannot free a character that isn't yours");
                return;
            }

            if (!_characterOwners.Remove(characterIdx))
            {
                Debug.Log("Character is free, no need to return it");
                return;
            }

            UpdateCharacterMarkStatusRPC();
        }

        [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
        private void ReturnCharacterRequestAnswerRPC([RpcTarget] PlayerRef player, bool canUseCharacter,
            int characterIdx) => OnCharacterRequestResponse?.Invoke(canUseCharacter, characterIdx);

        [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
        private void UpdateCharacterMarkStatusRPC() =>
            OnUpdateCharacterMarkedStatus?.Invoke(_characterOwners.Keys.ToList());

        #endregion
    }
}
