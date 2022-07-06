using System.Collections.Generic;
using System.Linq;
using kcp2k;
using Mirror;
using Mirror.Discovery;
using UnityEngine;

namespace Game.CodeBase.Infrastructure.Network
{
    public class LobbyPreparer : MonoBehaviour
    {
        public bool ConnectToAvailableServerAutomatically = false;
        
        [SerializeField] private ServersObserver _serversObserver;

        private void Awake()
        {
            _serversObserver.FoundServers += JoinOrHost;
        }

        private void JoinOrHost(Dictionary<long, ServerResponse> foundServers)
        {
            if (foundServers.Count == 0)
            {
                NetworkManager.singleton.StartHost();
            }
            else
            {
                if (ConnectToAvailableServerAutomatically)
                {
                    //TODO: Change response information to find out if the server is available
                    var firstServerInfo = foundServers.First().Value;
                    NetworkManager.singleton.StartClient(firstServerInfo.uri);
                }
                else
                {
                    ushort newPort = FindEmptyPort(foundServers);
                    NetworkManager.singleton.gameObject.GetComponent<KcpTransport>().Port = newPort;
                    NetworkManager.singleton.StartHost();
                }
            }
        }

        private ushort FindEmptyPort(Dictionary<long, ServerResponse> foundServers)
        {
            string myIp = NetworkManager.singleton.networkAddress;
            ushort myPort = 7777;
            bool isUnique = false;
            while (!isUnique)
            {
                foreach (var server in foundServers)
                {
                    if (server.Value.EndPoint.Address.ToString() == myIp && server.Value.uri.Port == myPort)
                    {
                        isUnique = false;
                        myPort++;
                        break;
                    }
                }

                isUnique = true;
            }

            return myPort;
        }
    }
}
