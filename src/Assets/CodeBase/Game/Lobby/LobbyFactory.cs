using Cysharp.Threading.Tasks;
using Game.CodeBase.Infrastructure;
using Game.CodeBase.Player;
using Game.CodeBase.Services.Network;
using Game.CodeBase.UI;
using Mirror;
using Unity.VisualScripting;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Game.CodeBase.Game.Lobby
{
    public class LobbyFactory
    {
        private readonly AsyncLazy<Object> _lobbyPlayerTask = UniTask.Lazy(() => Resources.LoadAsync<Object>(AssetPath.LobbyPlayer).ToUniTask());
        private readonly AsyncLazy<Object> _lobbyUITask = UniTask.Lazy(() => Resources.LoadAsync<Object>(AssetPath.LobbyUI).ToUniTask());
        private readonly AsyncLazy<Object> _lobbyTask = UniTask.Lazy(() => Resources.LoadAsync<Object>(AssetPath.Lobby).ToUniTask());
        private NetworkSpawner _spawner;

        public LobbyFactory(NetworkSpawner spawner) => 
            _spawner = spawner;

        public async UniTask<LobbyPlayer> CreatePlayer(BasePlayer basePlayer, bool isLeader)
        {
            var playerObj = (GameObject) await _lobbyPlayerTask;
            var lobbyPlayer = Object.Instantiate(playerObj).GetComponent<LobbyPlayer>();
            lobbyPlayer.Initialize(basePlayer, isLeader);

            _spawner.Spawn(lobbyPlayer.gameObject, basePlayer.gameObject);
            return lobbyPlayer;
        }

        public async UniTask<LobbyUI> CreateUI()
        {
            Object lobbyUI = await _lobbyUITask;
            return Object.Instantiate(lobbyUI).GetComponent<LobbyUI>();
        }

        public async UniTask<Lobby> SpawnLobby()
        {
            Object lobbyUI = await _lobbyTask;
            var gameObject = (GameObject) Object.Instantiate(lobbyUI);
            var lobby = gameObject.GetOrAddComponent<Lobby>();
            _spawner.Spawn(gameObject);
            return lobby;
        }
    }
}