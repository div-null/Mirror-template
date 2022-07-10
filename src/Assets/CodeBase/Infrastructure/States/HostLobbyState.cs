using System.Threading.Tasks;
using Game.CodeBase.Game.Lobby;
using Game.CodeBase.Services.Network;
using kcp2k;
using Mirror;

namespace Game.CodeBase.Infrastructure.States
{
    public class HostLobbyState : IPayloadedState<ushort>
    {
        private readonly LobbyFactory _lobbyFactory;
        private readonly ServersObserver _serversObserver;
        private readonly NetworkManager _networkManager;
        private Lobby _lobby;

        public HostLobbyState(LobbyFactory lobbyFactory, ServersObserver serversObserver, NetworkManager networkManager)
        {
            _networkManager = networkManager;
            _serversObserver = serversObserver;
            _lobbyFactory = lobbyFactory;
        }

        public async void Enter(ushort server)
        {
            _networkManager.gameObject.GetComponent<KcpTransport>().Port = server;
            _networkManager.StartHost();
            await InitializeLobby();
            _serversObserver.AdvertiseServer();
        }

        public void Exit()
        {
            NetworkServer.Destroy(_lobby.gameObject);
        }

        private async Task InitializeLobby()
        {
            var lobbyUI = await _lobbyFactory.CreateUI();
            _lobby = await _lobbyFactory.SpawnLobby();
            _lobby.Initialize(_lobbyFactory, lobbyUI);
        }
    }
}