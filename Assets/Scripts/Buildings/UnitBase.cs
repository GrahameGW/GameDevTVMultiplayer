using Mirror;
using System;
using UnityEngine;

namespace RTSTutorialGame
{
    public class UnitBase : NetworkBehaviour
    {
        [SerializeField] Health health;

        public static event Action<UnitBase> ServerOnBaseSpawned;
        public static event Action<UnitBase> ServerOnBaseDespawn;
        public static event Action<int> ServerOnPlayerDefeat;

        #region Server

        public override void OnStartServer()
        {
            health.ServerOnDie += HandleServerOnDie;

            ServerOnBaseSpawned?.Invoke(this);
        }

        public override void OnStopServer()
        {
            health.ServerOnDie -= HandleServerOnDie;

            ServerOnBaseDespawn?.Invoke(this);
        }

        [Server]
        public void HandleServerOnDie()
        {
            ServerOnPlayerDefeat?.Invoke(connectionToClient.connectionId);

            NetworkServer.Destroy(gameObject);
        }
        #endregion
        #region Client
        #endregion
    }
}



