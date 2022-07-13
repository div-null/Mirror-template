using Mirror;
using UniRx;
using UnityEngine;

namespace Game.CodeBase.Player
{
    public class LobbyPlayer : NetworkBehaviour, IPlayer
    {
        public ReactiveCommand<int> IdChanged => BasePlayer.IdChanged;
        public ReactiveCommand<string> UsernameChanged => BasePlayer.UsernameChanged;
        public ReactiveCommand<Color> ColorChanged => BasePlayer.ColorChanged;
        public int Id => BasePlayer.Id;
        public string Username => BasePlayer.Username;
        public Color Color => BasePlayer.Color;

        public ReactiveCommand<bool> ReadyChanged;

        [SyncVar(hook = nameof(HandleReadyChanged))]
        public bool IsReady;

        [SyncVar] public BasePlayer BasePlayer;
        [SyncVar] public bool IsLeader;

        private void Awake()
        {
            ReadyChanged = new ReactiveCommand<bool>();
        }

        public override void OnStartAuthority()
        {
            CmdSetReadyStatus(IsLeader);
        }

        public void Initialize(BasePlayer basePlayer, bool isLeader)
        {
            BasePlayer = basePlayer;
            IsLeader = isLeader;
        }

        public void SetUsername(string username) =>
            BasePlayer.SetUsername(username);

        public void SetColor(Color value) =>
            BasePlayer.SetColor(value);

        public void SetReady(bool value)
        {
            if (!isServer) return;
            CmdSetReadyStatus(value);
        }

        [Command]
        private void CmdSetReadyStatus(bool isReady) =>
            ReadyChanged.Execute(IsReady = isReady);

        private void HandleReadyChanged(bool _, bool @new) =>
            ReadyChanged.Execute(@new);
    }
}