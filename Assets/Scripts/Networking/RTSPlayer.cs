using System;
using System.Collections.Generic;
using System.Linq;
using Mirror;
using UnityEngine;


namespace RTSTutorialGame
{
    public class RTSPlayer : NetworkBehaviour
    {
        public List<Unit> Units { get; private set; }
        public List<Building> Buildings { get; private set; }
        public Color TeamColor { get; set; }
        [field: SerializeField]
        public Transform CameraTransform { get; private set; }
        public bool IsPartyOwner
        {
            get => isPartyOwner;
            private set
            {
                isPartyOwner = value;
            }
        }
        public string DisplayName
        {
            get => displayName;
            set
            {
                displayName = value;
            }
        }

        [SerializeField] Building[] buildingCatalog;
        [SerializeField] LayerMask buildingBlockLayer;
        [SerializeField] float buildingRangeLimit;

        [SyncVar(hook = nameof(ClientHandleResourcesUpdated))]
        [SerializeField] int resources = 550;
        [SyncVar(hook = nameof(AuthorityHandlePartyOwnerStateChanged))]
        private bool isPartyOwner = false;
        [SyncVar(hook = nameof(ClientHandleDisplayNameUpdated))]
        private string displayName;

        public event Action<int> ClientOnResourcesUpdated;

        public static event Action<bool> AuthorityOnPartyOwnerChanged;
        public static event Action ClientOnInfoUpdated;


        public bool CanPlaceBuilding(BoxCollider boxCollider, Vector3 position)
        {
            if (Physics.CheckBox(position + boxCollider.center, boxCollider.size / 2, Quaternion.identity, buildingBlockLayer))
            {
                return false;
            }

            return Buildings.Any(b =>
                (position - b.transform.position).sqrMagnitude <= buildingRangeLimit * buildingRangeLimit);
        }



        #region Server
        public override void OnStartServer()
        {
            Units = new List<Unit>();
            Buildings = new List<Building>();

            Unit.ServerOnUnitSpawned += ServerHandleUnitSpawned;
            Unit.ServerOnUnitDespawn += ServerHandleUnitDespawn;
            Building.ServerOnBuildingSpawned += ServerHandleBuildingSpawned;
            Building.ServerOnBuildingDespawn += ServerHandleBuildingDespawn;

            DontDestroyOnLoad(gameObject);
        }

        public override void OnStopServer()
        {
            Unit.ServerOnUnitSpawned -= ServerHandleUnitSpawned;
            Unit.ServerOnUnitDespawn -= ServerHandleUnitDespawn;
            Building.ServerOnBuildingSpawned -= ServerHandleBuildingSpawned;
            Building.ServerOnBuildingDespawn -= ServerHandleBuildingDespawn;
        }

        [Server]
        public void SetPartyOwner(bool state)
        {
            isPartyOwner = state;
        }

        private void ServerHandleUnitSpawned(Unit unit)
        {
            if (unit.connectionToClient.connectionId != connectionToClient.connectionId) { return; }
            
            Units.Add(unit);
        }

        private void ServerHandleUnitDespawn(Unit unit)
        {
            if (unit.connectionToClient.connectionId != connectionToClient.connectionId) { return; }

            Units.Remove(unit);
        }

        private void ServerHandleBuildingSpawned(Building building)
        {
            if (building.connectionToClient.connectionId != connectionToClient.connectionId) { return; }

            Buildings.Add(building);
        }

        private void ServerHandleBuildingDespawn(Building building)
        {
            if (building.connectionToClient.connectionId != connectionToClient.connectionId) { return; }

            Buildings.Remove(building);
        }

        [Command]
        public void CmdTryPlaceBuilding(int buildingId, Vector3 position)
        {
            if (buildingId == -1)
            {
                Debug.LogWarning("Passed a building without an ID. Have you set the Building ID on the prefab?");
                return;
            }

            var building = buildingCatalog.FirstOrDefault(b => b.Id == buildingId);

            if (building == null || resources < building.Price) { return; }

            if (!CanPlaceBuilding(building.GetComponent<BoxCollider>(), position)) { return; }

            GameObject instance = Instantiate(building.gameObject, position, building.transform.rotation);
            NetworkServer.Spawn(instance, connectionToClient);
            SetResources(resources - building.Price);

        }

        [Command]
        public void CmdStartGame()
        {
            if(!IsPartyOwner) { return; }

            ((RTSNetworkManager)NetworkManager.singleton).StartGame();
        }

        // syncvar getter
        public int GetResources()
        {
            return resources;
        }
        // syncvar setter
        public void SetResources(int resources)
        {
            this.resources = resources;
        }

        #endregion

        #region Client

        public override void OnStartAuthority()
        {
            if (NetworkServer.active) { return; } // if is server

            Unit.AuthorityOnUnitSpawned += AuthorityHandleUnitSpawned;
            Unit.AuthorityOnUnitDespawn += AuthorityHandleUnitDespawn;

            Building.AuthorityOnBuildingSpawned += AuthorityHandleBuildingSpawned;
            Building.AuthorityOnBuildingDespawn += AuthorityHandleBuildingDespawn;
        }

        public override void OnStartClient()
        {
            if (NetworkServer.active) { return; }

            // have to do a weird cast to get the derived class
            ((RTSNetworkManager)NetworkManager.singleton).Players.Add(this);

            DontDestroyOnLoad(gameObject);
        }

        public override void OnStopClient()
        {
            ClientOnInfoUpdated?.Invoke();

            if (!isClientOnly) { return; }
            
            // have to do a weird cast to get the derived class
            ((RTSNetworkManager)NetworkManager.singleton).Players.Add(this);

            if (!hasAuthority) { return; }

            Unit.AuthorityOnUnitSpawned -= AuthorityHandleUnitSpawned;
            Unit.AuthorityOnUnitDespawn -= AuthorityHandleUnitDespawn;
            Building.AuthorityOnBuildingSpawned -= AuthorityHandleBuildingSpawned;
            Building.AuthorityOnBuildingDespawn -= AuthorityHandleBuildingDespawn;
        }

        private void AuthorityHandleUnitSpawned(Unit unit)
        {
            Units.Add(unit);
        }

        private void AuthorityHandleUnitDespawn(Unit unit)
        {
            Units.Remove(unit);
        }

        private void AuthorityHandleBuildingSpawned(Building building)
        {
            Buildings.Add(building);
        }

        private void AuthorityHandleBuildingDespawn(Building building)
        {
            Buildings.Remove(building);
        }

        private void AuthorityHandlePartyOwnerStateChanged(bool oldState, bool newState)
        {
            if (!hasAuthority) { return; }

            AuthorityOnPartyOwnerChanged?.Invoke(newState);
        }

        private void ClientHandleResourcesUpdated(int oldResources, int newResources)
        {
            ClientOnResourcesUpdated?.Invoke(newResources);
        }

        private void ClientHandleDisplayNameUpdated(string oldName, string newName)
        {
            ClientOnInfoUpdated?.Invoke();
        }
        #endregion
    }
}

