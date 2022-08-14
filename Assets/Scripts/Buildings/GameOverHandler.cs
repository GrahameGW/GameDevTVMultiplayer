using Mirror;
using System.Collections.Generic;
using System;

namespace RTSTutorialGame
{
    public class GameOverHandler : NetworkBehaviour
    {
        public static event Action ServerOnGameOver;
        public static event Action<string> ClientOnGameOver;
        
        private List<UnitBase> bases;

        #region Server

        public override void OnStartServer()
        {
            bases = new List<UnitBase>();

            UnitBase.ServerOnBaseSpawned += ServerHandleBaseSpawned;
            UnitBase.ServerOnBaseDespawn += ServerHandleBaseDespawn;
        }

        public override void OnStopServer()
        {
            UnitBase.ServerOnBaseSpawned -= ServerHandleBaseSpawned;
            UnitBase.ServerOnBaseDespawn -= ServerHandleBaseDespawn;
        }

        [Server]
        private void ServerHandleBaseSpawned(UnitBase unitBase)
        {
            bases.Add(unitBase);
        }

        [Server]
        private void ServerHandleBaseDespawn(UnitBase unitBase)
        {
            bases.Remove(unitBase);

            if (bases.Count != 1) { return; }

            int playerId = bases[0].connectionToClient.connectionId;

            RpcGameOver($"Player {playerId}");

            ServerOnGameOver?.Invoke();
        }

        #endregion
        #region Client

        [ClientRpc]
        private void RpcGameOver(string winner)
        {
            ClientOnGameOver?.Invoke(winner);
        }

        #endregion
    }
}

