using System.Threading.Tasks;
using kcp2k;
using UniRx;

using Game.CodeBase.Game;
using Game.CodeBase.Game.Lobby;
using Game.CodeBase.Player;
using Game.CodeBase.Services;
using Game.CodeBase.Services.Network;

namespace Game.CodeBase.Infrastructure.States
{
    public class HostLobbyState : IPayloadedState<ushort>
    {
        private readonly LobbyFactory _lobbyFactory;
        private readonly ServersObserver _serversObserver;
        private readonly CustomNetworkManager _networkManager;
        private readonly PlayerProgressData _playerProgressData;
        private readonly GameState _gameState;

        private Lobby _lobby;
        private CompositeDisposable _disposable;

        public HostLobbyState(LobbyFactory lobbyFactory, 
            ServersObserver serversObserver,
            CustomNetworkManager networkManager,
            GameState gameState,
            PlayerProgressData playerProgressData)
        {
            _gameState = gameState;
            _playerProgressData = playerProgressData;
            _networkManager = networkManager;
            _serversObserver = serversObserver;
            _lobbyFactory = lobbyFactory;
        }

        public async void Enter(ushort server)
        {
            _disposable = new CompositeDisposable();
            _networkManager.gameObject.GetComponent<KcpTransport>().Port = server;

            _networkManager.StartHost();
            await InitializeLobby();

            _serversObserver.AdvertiseServer();
        }

        private void OnAddPlayer(BasePlayer player)
        {
            _lobby.CreatePlayerAsync(player);
        }

        public void Exit()
        {
            _disposable?.Dispose();
            // NetworkServer.Destroy(_lobby.gameObject);
        }

        private async Task InitializeLobby()
        {
            var lobbyUI = await _lobbyFactory.CreateUI();
            _lobby = await _lobbyFactory.SpawnLobby();
            _lobby.Initialize(_lobbyFactory, lobbyUI, _playerProgressData);
            _gameState.AddPlayerBuffered.Subscribe(OnAddPlayer).AddTo(_disposable);
        }
    }
}