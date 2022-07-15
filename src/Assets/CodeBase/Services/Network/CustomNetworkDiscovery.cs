using System;
using System.Net;
using Mirror;
using Mirror.Discovery;
using UnityEngine;
using VContainer;

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

        [Tooltip("Invoked when a server is found")]
        public event Action<ServerInfo> OnServerFound;

        private IServerNotifier _serverNotifier;
        
        public override void Start()
        {
            ServerId = RandomLong();
            base.Start();
        }

        [Inject]
        public void Initialize(IServerNotifier serverNotifier)
        {
            _serverNotifier = serverNotifier;
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
            return _serverNotifier.Notify(ServerId, request, endpoint);
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