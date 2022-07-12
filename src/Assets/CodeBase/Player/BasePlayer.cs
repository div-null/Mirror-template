using System;
using Game.CodeBase.Data;
using Mirror;
using UniRx;
using UnityEngine;

namespace Game.CodeBase.Player
{
    public class BasePlayer : NetworkBehaviour
    {
        public ReactiveCommand<int> IdChanged { get; private set; }
        public ReactiveCommand<string> UsernameChanged { get; private set; }
        public ReactiveCommand<Color> ColorChanged { get; private set; }
        public ReactiveCommand Spawned { get; private set; }
        public ReactiveCommand Destroyed { get; private set; }

        [SyncVar(hook = nameof(HandleIdChanged))]
        public int Id;

        [SyncVar(hook = nameof(HandleUsernameChanged))]
        public string Username;

        [SyncVar(hook = nameof(HandleColorChanged))]
        public Color Color;

        [SyncVar] public bool IsSpawned;

        private PlayerProgress _playerProgress;

        private void Awake()
        {
            IdChanged = new ReactiveCommand<int>();
            UsernameChanged = new ReactiveCommand<string>();
            ColorChanged = new ReactiveCommand<Color>();
            Spawned = new ReactiveCommand();
            Destroyed = new ReactiveCommand();
            IdChanged.Subscribe((value) => Debug.Log($"New id={value}"));
        }

        public override void OnStartClient()
        {
            CmdSpawned();
        }

        public void Initialize(int availableId, PlayerProgress playerProgress)
        {
            Id = availableId;
            Username = playerProgress.Username;
            Color = playerProgress.ColorData.Color;
            _playerProgress = playerProgress;
        }

        [Command]
        private void CmdSpawned()
        {
            IsSpawned = true;
            Spawned.Execute();
        }

        [Command]
        public void CmdChangeUsername(string username)
        {
            Username = username;
            UsernameChanged.Execute(username);
        }

        [Command]
        public void CmdChangeId(int newId)
        {
            Id = newId;
            IdChanged.Execute(newId);
        }

        [Command]
        public void CmdSetColor(Color newColor)
        {
            Color = newColor;
            ColorChanged.Execute(newColor);
        }

        public void HandleUsernameChanged(string oldValue, string newValue)
        {
            UsernameChanged.Execute(newValue);
        }

        public void HandleIdChanged(int oldValue, int newValue)
        {
            IdChanged.Execute(newValue);
        }

        public void HandleColorChanged(Color oldValue, Color newValue)
        {
            ColorChanged.Execute(newValue);
        }

        private void OnDestroy()
        {
            Destroyed.Execute();
        }
    }
}