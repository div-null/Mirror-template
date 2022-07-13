using Game.CodeBase.Model;
using Game.CodeBase.Player;
using Mirror;
using UnityEngine;

namespace Game.CodeBase.Game
{
    public class PlayerFactory
    {
        private readonly Transform _startPos;
        private readonly GameObject _playerPrefab;

        public PlayerFactory(Transform startPos, GameObject playerPrefab)
        {
            _startPos = startPos;
            _playerPrefab = playerPrefab;
        }

        public BasePlayer CreatePlayer(NetworkConnection conn, int id)
        {
            GameObject player = _startPos != null
                ? Object.Instantiate(_playerPrefab, _startPos.position, _startPos.rotation)
                : Object.Instantiate(_playerPrefab);
                
            player.name = $"{_playerPrefab.name} [connId={conn.connectionId}]";
                
            var playerProgress = (PlayerProgress) conn.authenticationData;
            BasePlayer basePlayer = player.GetComponent<BasePlayer>();
            basePlayer.Initialize(id, playerProgress);
            return basePlayer;
        }
    }
}