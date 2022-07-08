using Cysharp.Threading.Tasks;
using Game.CodeBase.Data;
using Game.CodeBase.Infrastructure;
using Game.CodeBase.Services.Network;
using Game.CodeBase.UI;
using Mirror;
using Unity.VisualScripting;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Game.CodeBase.Lobby
{
    public class LobbyFactory
    {
        readonly AsyncLazy<Object> LobbyPlayerTask = UniTask.Lazy(() => Resources.LoadAsync<Object>(AssetPath.LobbyPlayer).ToUniTask());
        readonly AsyncLazy<Object> LobbyUITask = UniTask.Lazy(() => Resources.LoadAsync<Object>(AssetPath.LobbyUI).ToUniTask());

        async UniTask<LobbyPlayer> CreatePlayer(NetworkConnection conn)
        {
            var lobbyPlayer = await LobbyPlayerTask;
            var player = Object.Instantiate(lobbyPlayer).GetComponent<LobbyPlayer>();
            return player;
        }

        async UniTask<LobbyUI> CreateUI()
        {
            Object lobbyUI = await LobbyUITask;
            return Object.Instantiate(lobbyUI).GetComponent<LobbyUI>();
        }
    }
}