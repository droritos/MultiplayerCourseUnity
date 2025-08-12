using Fusion;
using UnityEngine;

namespace Game.Server
{
    public class GameManager : NetworkBehaviour
    {
        [SerializeField] private GameData gameData;
        [SerializeField] private GridData gridData;

        private void OnEnable() => GameManagerRequestBroker.OnRequestBomb += RequestBombAtLocationRPC;

        private void OnDisable() => GameManagerRequestBroker.OnRequestBomb -= RequestBombAtLocationRPC;

        [Rpc(RpcSources.All, RpcTargets.StateAuthority, HostMode = RpcHostMode.SourceIsHostPlayer)]
        private void RequestBombAtLocationRPC(Vector3 position)
        {
            Debug.Log("[Server] Placing bomb in location");
            position = gridData.AlignToClosestGridPosition(position);
            var bombInstance = Runner.Spawn(gameData.bombPrefab, position);
        }
    }
}
