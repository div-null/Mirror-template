using System;
using Game.CodeBase.Data;
using Game.CodeBase.Infrastructure;
using Mirror;

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

        public Action<int, string> UsernameChanged;
        public Action<ColorType> ColorChanged;
        public Action Destroyed;

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
            CmdChangeUsername(AllServices.PlayerProgressData.Username);
        }

        [Command]
        public void CmdRemoveAuthority()
        {
            GetComponent<NetworkIdentity>().RemoveClientAuthority();
        }
    }
}