using Mirror;
using System;
using UnityEngine;
using UnityEngine.Events;

namespace RTSTutorialGame
{
    public class Unit : NetworkBehaviour
    {
        [field: SerializeField] 
        public UnitMovement UnitMovement { get; private set; }
        [field: SerializeField]
        public Targeter Targeter { get; private set; }
        [field: SerializeField]
        public int ResourceCost { get; private set; }

        [SerializeField] Health health;
        [SerializeField] UnityEvent onSelected;
        [SerializeField] UnityEvent onDeselected;

        public static event Action<Unit> ServerOnUnitSpawned;
        public static event Action<Unit> ServerOnUnitDespawn;

        public static event Action<Unit> AuthorityOnUnitSpawned;
        public static event Action<Unit> AuthorityOnUnitDespawn;


        #region Server

        public override void OnStartServer()
        {
            ServerOnUnitSpawned?.Invoke(this);
            health.ServerOnDie += HandleServerOnDie;
        }

        public override void OnStopServer()
        {
            ServerOnUnitDespawn?.Invoke(this);
            health.ServerOnDie -= HandleServerOnDie;
        }

        [Server]
        private void HandleServerOnDie()
        {
            NetworkServer.Destroy(gameObject);
        }
        #endregion

        #region Client

        [Client]
        public void Select()
        {
            if (!hasAuthority) return;

            onSelected?.Invoke();
        }

        [Client]
        public void Deselect()
        {
            if (!hasAuthority) return;

            onDeselected?.Invoke();
        }

        [Client]
        public override void OnStartAuthority()
        {
            AuthorityOnUnitSpawned?.Invoke(this);
        }

        [Client]
        public override void OnStopClient()
        {
            if (!hasAuthority) { return; }

            AuthorityOnUnitDespawn?.Invoke(this);
        }

        #endregion
    }
}

