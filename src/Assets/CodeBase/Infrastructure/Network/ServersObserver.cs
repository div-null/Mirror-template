using System;
using System.Collections;
using System.Collections.Generic;
using Mirror;
using Mirror.Discovery;
using UnityEngine;

public class ServersObserver : MonoBehaviour
{
    private  Dictionary<long, ServerResponse> _discoveredServers;
    public Action<Dictionary<long, ServerResponse>> FoundServers;
    public Action<ServerResponse> FoundServer;
    
    [SerializeField] private NetworkDiscovery _networkDiscovery;
    
    // Start is called before the first frame update
    void Start()
    {
        _networkDiscovery.OnServerFound.AddListener(OnDiscoveredServer);
        StartObservation();
    }

    public void StartObservation()
    {
        _discoveredServers = new Dictionary<long, ServerResponse>();
        StartCoroutine(WaitForObservation());
    }

    public IEnumerator WaitForObservation()
    {
        _networkDiscovery.StartDiscovery();
        yield return new WaitForSeconds(1);
        FoundServers?.Invoke(_discoveredServers);
    }

    void OnDiscoveredServer(ServerResponse serverResponse)
    {
        Debug.Log("Found server with port: " + serverResponse.uri.Port);
        _discoveredServers.Add(serverResponse.serverId, serverResponse);
        FoundServer?.Invoke(serverResponse);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void AdvertiseServer()
    {
        _networkDiscovery.AdvertiseServer();
    }

    public void Reload()
    {
        _networkDiscovery.StopDiscovery();
        StartObservation();
    }
}
