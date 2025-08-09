using System.Linq;
using System.Threading.Tasks;
using Fusion;
using UnityEngine;

namespace Game
{
    public class GameSessionStarter : MonoBehaviour
    {
        [SerializeField] private NetworkRunner networkRunnerPrefab;

        private const string TestSessionName = "TestSession";
        private const float RetryDelaySeconds = 1f;

        private GameMode GetGameMode()
        {
            var tags = Unity.Multiplayer.Playmode.CurrentPlayer.ReadOnlyTags().ToHashSet();
            if (tags.Contains("Server"))
            {
                Debug.Log("Configuration for player: Dedicated Server");
                return GameMode.Server;
            }

            if (tags.Contains("Host"))
            {
                Debug.Log("Configuration for player: Host");
                return GameMode.Host;
            }

            Debug.Log("Configuration for player: Client");
            return GameMode.Client;
        }

        private async void Start()
        {
            var gameMode = GetGameMode();

            if (gameMode == GameMode.Client)
            {
                await RunClientMode(gameMode);
            }
            else
            {
                await RunHostOrServerMode(gameMode);
            }
        }

        private async Task RunClientMode(GameMode gameMode)
        {
            NetworkRunner runner = null;
            var startGameArgs = new StartGameArgs { GameMode = gameMode, SessionName = TestSessionName };
            StartGameResult result = null;

            while (result is not { Ok: true })
            {
                // Clean up previous runner if it exists
                if (runner != null)
                {
                    Destroy(runner);
                }

                runner = Instantiate(networkRunnerPrefab);

                Debug.Log("Attempting connection...");
                result = await runner.StartGame(startGameArgs);
                Debug.Log($"{result.Ok}, error: {result.ErrorMessage}, shutdown-reason: {result.ShutdownReason}");

                if (result is not { Ok: true })
                {
                    await Awaitable.WaitForSecondsAsync(RetryDelaySeconds);
                }
            }
        }

        private async Task RunHostOrServerMode(GameMode gameMode)
        {
            var startGameArgs = new StartGameArgs { GameMode = gameMode, SessionName = TestSessionName };
            var runner = Instantiate(networkRunnerPrefab);
            await runner.StartGame(startGameArgs);
            await runner.LoadScene("Level Scene");
        }
    }
}
