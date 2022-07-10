using System;
using Mirror;

namespace Game.CodeBase.Player
{
    public class LobbyPlayer : NetworkBehaviour
    {
        public event Action<bool> ReadyChanged;

        [SyncVar(hook = nameof(handleReadyChanged))]
        public bool IsReady = false;

        [SyncVar] public BasePlayer BasePlayer;
        [SyncVar] private bool _isLeader;

        public void Initialize(BasePlayer basePlayer, bool isLeader)
        {
            BasePlayer = basePlayer;
            _isLeader = isLeader;
        }

        public override void OnStartAuthority()
        {
            CmdSetReadyStatus(_isLeader);
        }

        [Command]
        public void CmdSetReadyStatus(bool isReady)
        {
            IsReady = isReady;
        }

        private void handleReadyChanged(bool old, bool @new)
        {
            ReadyChanged?.Invoke(@new);
        }
    }
}