using System;
using System.Net;
using Mirror;
using Mirror.Discovery;
using UnityEngine;

namespace Game.CodeBase.Services.Network
{
    public class DiscoveryRequest : NetworkMessage
    {
    }

    public class ServerInfo : NetworkMessage
    {
        // client fills this up after we receive it
        public IPEndPoint EndPoint { get; set; }
        public Uri Uri;
        public long ServerId;

        // custom fields
        public string ServerName;
        public int MaxPlayers;
        public int PlayersCount;
    }

    public class CustomNetworkDiscovery : NetworkDiscoveryBase<DiscoveryRequest, ServerInfo>, INetworkDiscovery
    {
        public long ServerId { get; private set; }

        [Tooltip("Transport to be advertised during discovery")]
        public Transport transport;

        private string _serverName;
        private int _maxPlayers;
        private int _playersCount;

        [Tooltip("Invoked when a server is found")]
        public event Action<ServerInfo> OnServerFound;


        public void Initialize(string serverName, int playersCount = 1, int maxPlayers = 4)
        {
            _playersCount = playersCount;
            _maxPlayers = maxPlayers;
            _serverName = serverName;
        }

        public void UpdatePlayersCount(int playersCount, int maxPlayers = 4)
        {
            _playersCount = playersCount;
            _maxPlayers = maxPlayers;
        }

        public override void Start()
        {
            ServerId = RandomLong();

            if (transport == null)
                transport = Transport.activeTransport;

            base.Start();
        }

        /// <summary>
        /// Process the request from a client
        /// </summary>
        /// <remarks>
        /// Override if you wish to provide more information to the clients
        /// such as the name of the host player
        /// </remarks>
        /// <param name="request">Request coming from client</param>
        /// <param name="endpoint">Address of the client that sent the request</param>
        /// <returns>A message containing information about this server</returns>
        protected override ServerInfo ProcessRequest(DiscoveryRequest request, IPEndPoint endpoint)
        {
            try
            {
                // this is an example reply message,  return your own
                // to include whatever is relevant for your game
                return new ServerInfo
                {
                    ServerId = ServerId,
                    Uri = transport.ServerUri(),
                    ServerName = _serverName,
                    MaxPlayers = _maxPlayers,
                    PlayersCount = _playersCount
                };
            }
            catch (NotImplementedException)
            {
                Debug.LogError($"Transport {transport} does not support network discovery");
                throw;
            }
        }


        /// <summary>
        /// Create a message that will be broadcasted on the network to discover servers
        /// </summary>
        /// <remarks>
        /// Override if you wish to include additional data in the discovery message
        /// such as desired game mode, language, difficulty, etc... </remarks>
        /// <returns>An instance of ServerRequest with data to be broadcasted</returns>
        protected override DiscoveryRequest GetRequest() =>
            new DiscoveryRequest();

        /// <summary>
        /// Process the answer from a server
        /// </summary>
        /// <remarks>
        /// A client receives a reply from a server, this method processes the
        /// reply and raises an event
        /// </remarks>
        /// <param name="response">Response that came from the server</param>
        /// <param name="endpoint">Address of the server that replied</param>
        protected override void ProcessResponse(ServerInfo response, IPEndPoint endpoint)
        {
            response.EndPoint = endpoint;

            UriBuilder realUri = new UriBuilder(response.Uri)
            {
                Host = response.EndPoint.Address.ToString()
            };
            response.Uri = realUri.Uri;

            OnServerFound?.Invoke(response);
        }
    }
}