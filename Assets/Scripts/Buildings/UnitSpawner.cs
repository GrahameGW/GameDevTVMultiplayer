using Mirror;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace RTSTutorialGame
{
    public class UnitSpawner : NetworkBehaviour, IPointerClickHandler
    {
        [SerializeField] Unit unitPrefab;
        [SerializeField] Transform spawnPoint;
        [SerializeField] Health health;
        [SerializeField] TMP_Text remainingUnitsText; // wrong way around
        [SerializeField] Image unitProgressImage;
        [SerializeField] int maxUnitQueue;
        [SerializeField] float spawnMoveRange;
        [SerializeField] float unitSpawnDuration;

        [SyncVar(hook = nameof(ClientHandleQueuedUnitsUpdated))]
        private int queuedUnits;
        [SyncVar]
        private float unitTimer;

        private float imageFillVelocity;


        private void Update()
        {
            if (isServer)
            {
                ProduceUnits();
            }
            if (isClient)
            {
                UpdateTimerDisplay();
            }
        }

        [Server]
        private void ServerSpawnUnit()
        {
            var instance = Instantiate(unitPrefab, spawnPoint.position, spawnPoint.rotation);
            NetworkServer.Spawn(instance.gameObject, connectionToClient);

            Vector3 spawnOffset = Random.insideUnitSphere * spawnMoveRange;
            spawnOffset.y = spawnPoint.position.y;

            var movement = instance.GetComponent<UnitMovement>();
            movement.ServerMove(spawnPoint.position + spawnOffset);

            queuedUnits--;
            unitTimer = 0f;
        }


        #region Server

        public override void OnStartServer()
        {
            health.ServerOnDie += ServerHandleOnDie;
        }

        public override void OnStopServer()
        {
            health.ServerOnDie -= ServerHandleOnDie;
        }

        [Server]
        private void ProduceUnits()
        {
            if (queuedUnits == 0) { return; }
            unitTimer += Time.deltaTime;

            if (unitTimer >= unitSpawnDuration)
            {
                ServerSpawnUnit();
            }
        }

        [Command]
        private void CmdSpawnUnit()
        {
            if (queuedUnits == maxUnitQueue) { return; }

            var player = connectionToClient.identity.GetComponent<RTSPlayer>();

            if (player.GetResources() >= unitPrefab.ResourceCost)
            {
                queuedUnits++;
                player.SetResources(player.GetResources() - unitPrefab.ResourceCost);
            }
        }

        [Server]
        private void ServerHandleOnDie()
        {
            //NetworkServer.Destroy(gameObject);
        }
        #endregion

        #region Client

        public void OnPointerClick(PointerEventData eventData)
        {
            if (eventData.button != PointerEventData.InputButton.Left) return;
            if (!hasAuthority) return;

            CmdSpawnUnit();
        }

        private void ClientHandleQueuedUnitsUpdated(int oldQueue, int newQueue)
        {
            remainingUnitsText.text = newQueue.ToString();
        }

        private void UpdateTimerDisplay()
        {
            float newProgress = unitTimer / unitSpawnDuration;

            if (newProgress < unitProgressImage.fillAmount)
            {
                unitProgressImage.fillAmount = newProgress;
            }
            else
            {
                unitProgressImage.fillAmount = Mathf.SmoothDamp(
                    unitProgressImage.fillAmount,
                    newProgress,
                    ref imageFillVelocity,
                    0.1f
                    );
            }
        }
        #endregion 
    }
}

