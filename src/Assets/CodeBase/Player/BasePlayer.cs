using CodeBase.Model;
using Mirror;
using UniRx;
using UnityEngine;

namespace Game.CodeBase.Player
{
    public class BasePlayer : NetworkBehaviour, IPlayer
    {
        public ReactiveCommand<int> IdChanged { get; private set; }
        public ReactiveCommand<string> UsernameChanged { get; private set; }
        public ReactiveCommand<Color> ColorChanged { get; private set; }
        public ReactiveCommand Spawned { get; private set; }
        public ReactiveCommand Destroyed { get; private set; }


        [field: SyncVar(hook = nameof(HandleIdChanged))]
        public int Id { get; private set; }

        [field: SyncVar(hook = nameof(HandleUsernameChanged))]
        public string Username { get; private set; }

        [field: SyncVar(hook = nameof(HandleColorChanged))]
        public Color Color { get; private set; }

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

        public void SetUsername(string username)
        {
            if (!isServer) return;
            CmdChangeUsername(username);
        }

        public void SetColor(Color value)
        {
            if (!isServer) return;
            CmdSetColor(value);
        }

        [Command]
        private void CmdSpawned()
        {
            IsSpawned = true;
            Spawned.Execute();
        }

        [Command]
        private void CmdChangeUsername(string username) =>
            UsernameChanged.Execute(Username = username);

        [Command]
        private void CmdSetColor(Color newColor) =>
            ColorChanged.Execute(Color = newColor);

        public void HandleUsernameChanged(string oldValue, string newValue) =>
            UsernameChanged.Execute(newValue);

        public void HandleIdChanged(int oldValue, int newValue) =>
            IdChanged.Execute(newValue);

        public void HandleColorChanged(Color oldValue, Color newValue) =>
            ColorChanged.Execute(newValue);

        private void OnDestroy() =>
            Destroyed.Execute();
    }
}