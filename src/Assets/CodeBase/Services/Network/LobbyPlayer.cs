using System;
using Game.CodeBase.Implementations;
using Game.CodeBase.Player;
using Game.CodeBase.UI;
using Mirror;

namespace Game.CodeBase.Services.Network
{
    public class LobbyPlayer : NetworkBehaviour
    {
        [SyncVar(hook = nameof(HandleReadyStatusChanged))]
        public bool IsReady = false;

        private BasePlayer _basePlayer;
        private bool _isLeader;
        private LobbyUI _lobbyUI;
        private LobbyImplementator _lobbyImplementator;

        public Action ReadyStatusChanged;

        public void Initialize(LobbyImplementator lobbyImplementator, BasePlayer basePlayer, LobbyUI lobbyUI, bool isLeader)
        {
            _lobbyImplementator = lobbyImplementator;
            _basePlayer = basePlayer;
            _lobbyUI = lobbyUI;
            _isLeader = isLeader;
            basePlayer.UsernameChanged += lobbyUI.ChangeUsernameToPlayer;
        }

        public override void OnStartAuthority()
        {
            _lobbyUI.UsernameChanged += _basePlayer.CmdChangeUsername;
            _lobbyUI.ReadyChanged += CmdChangeReadyStatus;
            
            if (_isLeader)
            {
                _lobbyImplementator.ReadinessToStartChanged += _lobbyUI.SetStartGameButtonAvailability;
                _lobbyUI.SetHostButtons();
                CmdChangeReadyStatus(true);
            }
            else
            {
                CmdChangeReadyStatus(false);
            }
        }

        // private void Start()
        // {
        //     basePlayer.UsernameChanged += lobbyUI.ChangeUsernameToPlayer;
        //     
        //     if (hasAuthority)
        //     {
        //         _lobbyUI.UsernameChanged += _basePlayer.CmdChangeUsername;
        //         _lobbyUI.ReadyChanged += CmdChangeReadyStatus;
        //     }
        // }

        [Command]
        public void CmdChangeReadyStatus()
        {
            IsReady = !IsReady;
        }
        
        [Command]
        public void CmdChangeReadyStatus(bool isReady)
        {
            IsReady = isReady;
        }

        public void HandleReadyStatusChanged(bool oldValue, bool newValue)
        {
            _lobbyUI.ChangeReadyStatusToPlayer(newValue, _basePlayer.Id);
            ReadyStatusChanged?.Invoke();
        }
    }
}