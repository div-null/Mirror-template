using System;
using Game.CodeBase.Data;
using Mirror;
using VContainer;

namespace Game.CodeBase.Player
{
    public class BasePlayer : NetworkBehaviour
    {
        [SyncVar(hook = nameof(HandleUsernameChanged))]
        public string Username;
        [SyncVar(hook = nameof(HandleColorChanged))]
        public ColorType Color;
        public int Id;
        public bool IsOnline;

        public event Action<int, string> UsernameChanged;
        public event Action<ColorType> ColorChanged;
        public event Action Destroyed;
        
        private PlayerProgress _playerProgress;

        [Inject]
        public void Initialize(PlayerProgress playerProgress)
        {
            _playerProgress = playerProgress;
        }
        
        [Command]
        public void CmdChangeUsername(string username)
        {
            Username = username;
        }
        
        public void HandleUsernameChanged(string oldValue, string newValue)
        {
            UsernameChanged?.Invoke(Id, newValue);
        }
        
        public void HandleColorChanged(ColorType oldValue, ColorType newValue)
        {
            ColorChanged?.Invoke(newValue);
        }

        private void OnDestroy()
        {
            Destroyed?.Invoke();
        }

        public void Initialize(int availableId)
        {
            Id = availableId;
        }

        public override void OnStartAuthority()
        {
            CmdChangeUsername(_playerProgress.Username);
        }

        [Command]
        public void CmdRemoveAuthority()
        {
            GetComponent<NetworkIdentity>().RemoveClientAuthority();
        }
    }
}