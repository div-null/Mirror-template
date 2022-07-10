using Cysharp.Threading.Tasks;
using Game.CodeBase.Infrastructure;
using Game.CodeBase.Player;
using Game.CodeBase.UI;
using Mirror;
using Unity.VisualScripting;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Game.CodeBase.Game.Lobby
{
    public class LobbyFactory
    {
        readonly AsyncLazy<Object> LobbyPlayerTask = UniTask.Lazy(() => Resources.LoadAsync<Object>(AssetPath.LobbyPlayer).ToUniTask());
        readonly AsyncLazy<Object> LobbyUITask = UniTask.Lazy(() => Resources.LoadAsync<Object>(AssetPath.LobbyUI).ToUniTask());

        public async UniTask<LobbyPlayer> CreatePlayer(NetworkConnection conn, BasePlayer basePlayer, bool isLeader)
        {
            var playerObj = (GameObject) await LobbyPlayerTask;
            var lobbyPlayer = Object.Instantiate(playerObj).GetComponent<LobbyPlayer>();
            lobbyPlayer.Initialize(basePlayer, isLeader);

            NetworkServer.Spawn(playerObj, conn);
            return lobbyPlayer;
        }

        public async UniTask<LobbyUI> CreateUI()
        {
            Object lobbyUI = await LobbyUITask;
            return Object.Instantiate(lobbyUI).GetComponent<LobbyUI>();
        }

        public async UniTask<Lobby> SpawnLobby()
        {
            GameObject gameObject = new GameObject("Server - Lobby");
            var lobby = gameObject.GetOrAddComponent<Lobby>();
            NetworkServer.Spawn(gameObject);
            return lobby;
        }
    }
}