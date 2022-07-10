using System;
using Game.CodeBase.Data;
using Mirror;
using UnityEngine;

namespace Game.CodeBase.Player
{
    public class BasePlayer : NetworkBehaviour
    {
        public event Action<int, string> UsernameChanged;
        public event Action<int> IdChanged;
        public event Action<Color> ColorChanged;
        public event Action Destroyed;
        
        [SyncVar(hook = nameof(HandleIdChanged))]
        public int Id;
        [SyncVar(hook = nameof(HandleUsernameChanged))]
        public string Username;
        [SyncVar(hook = nameof(HandleColorChanged))]
        public Color Color;

        private PlayerProgress _playerProgress;

        private void Awake()
        {
            IdChanged += (value) => Debug.Log($"New id={value}");
        }

        public void Initialize(int availableId, PlayerProgress playerProgress)
        {
            Id = availableId;
            Username = playerProgress.Username;
            Color = playerProgress.ColorData.Color;
            _playerProgress = playerProgress;
        }

        [Command]
        public void CmdChangeUsername(string username)
        {
            Username = username;
            UsernameChanged?.Invoke(Id, username);
        }

        [Command]
        public void CmdChangeId(int newId)
        {
            Id = newId;
            IdChanged?.Invoke(newId);
        }

        public void HandleUsernameChanged(string oldValue, string newValue)
        {
            UsernameChanged?.Invoke(Id, newValue);
        }

        public void HandleIdChanged(int oldValue, int newValue)
        {
            IdChanged?.Invoke(newValue);
        }

        public void HandleColorChanged(Color oldValue, Color newValue)
        {
            ColorChanged?.Invoke(newValue);
        }

        private void OnDestroy()
        {
            Destroyed?.Invoke();
        }
    }
}