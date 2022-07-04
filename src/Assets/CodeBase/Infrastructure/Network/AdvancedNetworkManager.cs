
using System;
using System.Linq;
using CodeBase;
using Mirror;
using UnityEngine;
using UnityEngine.SceneManagement;

public class AdvancedNetworkManager : NetworkManager
{
    [Header("Base information")]
    public int MinConnections = 2;
    [Scene] public string CurrentScene = string.Empty;
    
    public bool ConnectToAvailableServerAutomatically = false;
    
    public BasePlayer[] Players { get; } = new BasePlayer[4];

    private IImplementator _implementator;
    
    //TODO: Идея новой системы в том, что в ней есть стейт машина, которая дает приказы network manager'у, чтобы тот переключил IUtility и мб еще что-то
    //TODO: а объект IUtility занимается настройкой взаимосвязей между компонентами на сцене, спавном префабов в нужной последовательности в нужных местах, перестройкой ивентов (к слову, нужно сделать все ивенты под основные метды, по типу OnClientConnected и т.д. либо прокинуть методы)
    //TODO: кстати о прокидовани методов. можно же сделать расширение интерфейсов под нужды, например, IUtilityConnections, IUtilitySpawner
    //TODO: т.е. объект IUtility может наследовать еще и дополнительные интерфейсы
    //TODO; соответственно, networkmanager в нужных местах будет пробовать кастить объект IUtility в определенные интерфейсы
    //TODO: и если у него получится, то можно привязать ивент и всё хорошо
    //TODO: у стейта должен быть выход, чтобы перестроить со старого стейта связи в новый (как это было в видео, когда у NetworkServer меняли привязку к префабам)
    //TODO: еще есть мысль сделать объекты IUtility NetworkBehaviour, чтобы они были у всех и делали свои вещи как на стороне клиента, так и на стороне сервера
    
    //TODO: NetworkManager создает StateMachine, которая им в дальнейшем будет управлять
    //TODO: пока что стейт машина начинает свой пут с LobbyUtility

    public Action<NetworkConnection> OnClientConnected;
    public Action<NetworkConnection> OnClientDisconnected;
    public Action<NetworkConnectionToClient> OnClientConnectedOnServerSide;
    public Action<NetworkConnectionToClient> OnClientDisconnectedOnServerSide;
    public Action<NetworkConnectionToClient> OnServerAddedPlayer;

    public override void OnStartServer()
    {
        spawnPrefabs = Resources.LoadAll<GameObject>("NetworkPrefabs").ToList();
    }

    public override void OnStartClient()
    {
        var spawnableObjects = Resources.LoadAll<GameObject>("NetworkPrefabs");

        foreach (var obj in spawnableObjects)
        {
            NetworkClient.RegisterPrefab(obj);
        }
    }

    public override void OnClientConnect(NetworkConnection conn)
    {
        base.OnClientConnect(conn);
        
        OnClientConnected?.Invoke(conn);
    }

    public override void OnClientDisconnect(NetworkConnection conn)
    {
        base.OnClientDisconnect(conn);
        
        OnClientDisconnected?.Invoke(conn);
    }

    public override void OnServerConnect(NetworkConnectionToClient conn)
    {
        if (numPlayers > maxConnections)
        {
            conn.Disconnect();
        }

        //TODO: Сделать возможность проверки на повторяющиеся ники + реконнект и пауза для ожидания реконнекта
        OnClientConnectedOnServerSide?.Invoke(conn);
    }

    public override void OnServerDisconnect(NetworkConnectionToClient conn)
    {
        OnClientDisconnectedOnServerSide?.Invoke(conn);
        base.OnServerDisconnect(conn);
    }

    public override void OnServerAddPlayer(NetworkConnectionToClient conn)
    {
        int availableId = GetAvailableId();
        if (availableId != -1 && CurrentScene == SceneManager.GetActiveScene().name)
        {
            base.OnServerAddPlayer(conn);
            BasePlayer basePlayer = conn.identity.GetComponent<BasePlayer>();
            basePlayer.Initialize(availableId);
            Players[availableId] = basePlayer;
            OnServerAddedPlayer?.Invoke(conn);
        }
    }

    public override void OnStopServer()
    {
        foreach (var player in Players)
        {
            //if (player != null) player.Destroy();
        }
    }

    public void SetImplementator(IImplementator implementator)
    {
        _implementator = implementator;
        CurrentScene = _implementator.Scene;
        _implementator.Setup();
    }

    public int GetAvailableId()
    {
        for (int i = 0; i < 4; i++)
        {
            if (Players[i] == null)
                return i;
        }

        
        Debug.Log("Its not empty!");
        return -1;
    }
}
