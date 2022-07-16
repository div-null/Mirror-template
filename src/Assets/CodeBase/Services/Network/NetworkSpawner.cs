using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using Mirror;
using UnityEngine;

namespace Game.CodeBase.Services.Network
{
    public class NetworkSpawner
    {
        private readonly List<uint> _deSpawned = new List<uint>();
        private readonly TimeoutController _timeoutController = new TimeoutController();
        private bool _isServer;

        public NetworkSpawner()
        {
            NetworkClient.RegisterHandler<ObjectDestroyMessage>(HandleResponse);
        }

        private void HandleResponse(ObjectDestroyMessage msg)
        {
            _deSpawned.Add(msg.netId);
        }

        public void Spawn(GameObject obj, GameObject ownerPlayer)
        {
            var identity = obj.GetComponent<NetworkIdentity>();
            NetworkServer.Spawn(obj, ownerPlayer);
        }

        public void Spawn(GameObject obj, NetworkConnection ownerConnection = null)
        {
            var identity = obj.GetComponent<NetworkIdentity>();
            NetworkServer.Spawn(obj, ownerConnection);
        }

        public void UnSpawn(GameObject obj)
        {
            var identity = obj.GetComponent<NetworkIdentity>();
            // _deSpawned.Add(identity.netId);
            NetworkServer.UnSpawn(obj);
        }

        public UniTask<TEntity> AwaitForNetworkEntity<TEntity>(uint netId, TimeSpan timeout) where TEntity : NetworkBehaviour
        {
            CancellationToken cancellationToken = _timeoutController.Timeout(timeout);
            return PullNetworkEntity<TEntity>(netId, cancellationToken);
        }

        private async UniTask<TEntity> PullNetworkEntity<TEntity>(uint netId, CancellationToken cancellationToken) where TEntity : NetworkBehaviour
        {
            if (_deSpawned.Contains(netId))
                return null;

            if (NetworkClient.isHostClient)
                return await TryGetComponent<TEntity>(netId, NetworkServer.spawned);

            while (!cancellationToken.IsCancellationRequested)
            {
                await UniTask.NextFrame(cancellationToken);

                var component = await TryGetComponent<TEntity>(netId, NetworkClient.spawned);
                if (component != null)
                    return component;
            }

            return null;
        }

        private static async UniTask<TEntity> TryGetComponent<TEntity>(uint netId, Dictionary<uint, NetworkIdentity> networkIdentities) where TEntity : NetworkBehaviour
        {
            if (networkIdentities.TryGetValue(netId, out NetworkIdentity identity))
            {
                var entity = identity.gameObject.GetComponent<TEntity>();
                if (entity == null)
                    await UniTask.FromException<NullReferenceException>(
                        new NullReferenceException($"Entity of type ({nameof(TEntity)}) is missing on object (netId={netId})"));

                return entity;
            }

            return null;
        }
    }
}