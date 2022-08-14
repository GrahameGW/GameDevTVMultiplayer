using UnityEngine;
using Mirror;
using System;


namespace RTSTutorialGame
{
    public class Health : NetworkBehaviour
    {
        [SerializeField] int maxHealth = 100;

        [SyncVar(hook = nameof(HandleHealthUpdated))]
        private int currentHealth;

        public event Action ServerOnDie;

        public event Action<int, int> ClientOnHealthUpdated;


        #region Server

        public override void OnStartServer()
        {
            currentHealth = maxHealth;

            UnitBase.ServerOnPlayerDefeat += ServerHandlePlayerDefeat;
        }

        public override void OnStopServer()
        {
            UnitBase.ServerOnPlayerDefeat -= ServerHandlePlayerDefeat;
        }

        [Server]
        public void DealDamage(int damage)
        {
            if (currentHealth == 0) { return; }

            currentHealth = Mathf.Max(currentHealth - damage, 0);
            if (currentHealth == 0)
            {
                ServerOnDie?.Invoke();
            }
        }

        [Server]
        private void ServerHandlePlayerDefeat(int connectionId)
        {
            if (connectionId == connectionToClient.connectionId)
            {
                DealDamage(currentHealth);
            }
        }

        #endregion

        #region Client

        private void HandleHealthUpdated(int oldHealth, int newHealth)
        {
            ClientOnHealthUpdated?.Invoke(currentHealth, maxHealth);
        }
        #endregion
    }
}

