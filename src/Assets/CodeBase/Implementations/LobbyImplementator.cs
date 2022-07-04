using System;
using System.Collections;
using System.Collections.Generic;
using CodeBase;
using CodeBase.Infrastructure.Network;
using Mirror;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using Object = UnityEngine.Object;

public class LobbyImplementator : IImplementator
{
    public GameObject LobbyPlayerPrefab { get; }
    public string Scene { get; }
    private GameObject _lobbyUIPrefab;
    private LobbyUI _lobbyUI;
    private AdvancedNetworkManager _advancedNetworkManager;

    public Action<bool> ReadinessToStartChanged;
    
    public LobbyImplementator(AdvancedNetworkManager advancedNetworkManager)
    {
        _advancedNetworkManager = advancedNetworkManager;
        LobbyPlayerPrefab = Resources.Load<GameObject>("NetworkPrefabs/LobbyPlayerPrefab");
        _lobbyUIPrefab = Resources.Load<GameObject>("UtilityPrefabs/LobbyUIPrefab");
        Scene = "MainMenu";
    }

    public void Setup()
    {
        _lobbyUI = Object.Instantiate(_lobbyUIPrefab).GetComponent<LobbyUI>();
        //_advancedNetworkManager.OnClientConnected += OnClientConnect;
        _advancedNetworkManager.OnClientDisconnected += OnClientDisconnect;
        _advancedNetworkManager.OnClientConnectedOnServerSide += OnClientConnectOnServerSide;
        _advancedNetworkManager.OnClientDisconnectedOnServerSide += OnClientDisconnectOnServerSide;
        _advancedNetworkManager.OnServerAddedPlayer += OnServerAddPlayer;
        Debug.Log("Setup dependencies");
    }

    public void Start()
    {
        
    }

    public void OnClientConnect()
    {
        //layerInstance.Initialize(this, conn.identity.GetComponent<BasePlayer>(), _lobbyUI, _advancedNetworkManager.numPlayers == 1);
        //playerInstance.ReadyStatusChanged += NotifyHostOfReadyState;
    }
    
    public void OnClientDisconnect(NetworkConnection conn)
    {
        
    }

    public void OnClientConnectOnServerSide(NetworkConnectionToClient conn)
    {
        
    }

    public void OnClientDisconnectOnServerSide(NetworkConnectionToClient conn)
    {
        if (conn.identity != null)
        {
            var player = conn.identity.GetComponent<BasePlayer>();

            _advancedNetworkManager.Players[player.Id] = null;

            NotifyHostOfReadyState();
        }
    }

    public void OnServerAddPlayer(NetworkConnectionToClient conn)
    {
        BasePlayer basePlayer = conn.identity.GetComponent<BasePlayer>();
        LobbyPlayer playerInstance = Object.Instantiate(LobbyPlayerPrefab).GetComponent<LobbyPlayer>();
        NetworkServer.Spawn(playerInstance.gameObject, conn);
        //TODO: centralize it, because on line 95:  Disconnecting connId=0 to prevent exploits from an Exception in MessageHandler: NullReferenceException Object reference not set to an instance of an object
    }
    
    private void NotifyHostOfReadyState()
    {
        var lobbyPlayers = Object.FindObjectsOfType<LobbyPlayer>();
        foreach (var lobbyPlayer in lobbyPlayers)
        {
            if (!lobbyPlayer.IsReady)
            {
                ReadinessToStartChanged?.Invoke(false);
            }
        }

        ReadinessToStartChanged?.Invoke(true);
    }
}
