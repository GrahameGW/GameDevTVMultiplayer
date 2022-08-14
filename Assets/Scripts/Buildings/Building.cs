using Mirror;
using System;
using UnityEngine;

namespace RTSTutorialGame
{
    public class Building : NetworkBehaviour
    {
        [field: SerializeField] 
        public Sprite Icon { get; private set; }
        [field: SerializeField] 
        public int Price { get; private set; }
        [field: SerializeField]
        public int Id { get; private set; }
        [field: SerializeField]
        public GameObject Preview;


        public static event Action<Building> ServerOnBuildingSpawned;
        public static event Action<Building> ServerOnBuildingDespawn;

        public static event Action<Building> AuthorityOnBuildingSpawned;
        public static event Action<Building> AuthorityOnBuildingDespawn;

        #region Server


        public override void OnStartServer()
        {
            ServerOnBuildingSpawned?.Invoke(this);
        }

        public override void OnStopServer()
        {
            ServerOnBuildingDespawn?.Invoke(this);
        }

        #endregion

        #region Client

        [Client]
        public override void OnStartAuthority()
        {
            AuthorityOnBuildingSpawned?.Invoke(this);
        }

        [Client]
        public override void OnStopClient()
        {
            if (!hasAuthority) { return; }

            AuthorityOnBuildingDespawn?.Invoke(this);
        }
        #endregion
    }
}



