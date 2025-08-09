using System.Linq;
using Fusion;
using UnityEngine;

namespace Game
{
    public class GameSessionStarter : MonoBehaviour
    {
        [SerializeField] private NetworkRunner networkRunnerPrefab;

        private async void Start()
        {
            var tags = Unity.Multiplayer.Playmode.CurrentPlayer.ReadOnlyTags().ToHashSet();
            var runner =  Instantiate(networkRunnerPrefab);
            var startGameArgs = new StartGameArgs { GameMode = GameMode.Host, SessionName = "TestSession" };

            if (tags.Contains("Server"))
            {
                Debug.Log("Configuration for player: Dedicated Server");
                startGameArgs.GameMode = GameMode.Server;
            }
            else if (tags.Contains("Host"))
            {
                Debug.Log("Configuration for player: Host");
                startGameArgs.GameMode = GameMode.Host;
            }
            else
            {
                Debug.Log("Configuration for player: Client");
                startGameArgs.GameMode = GameMode.Client;
            }

            var result = await runner.StartGame(startGameArgs);
            Debug.Log($"{result.Ok}, error: {result.ErrorMessage}, shutdown-reason{result.ShutdownReason}");
            if (startGameArgs.GameMode is GameMode.Server or GameMode.Host)
            {
                await runner.LoadScene("Level Scene");
            }
        }
    }
}
