using Mirror;
using UnityEngine;

namespace RTSTutorialGame
{
    public class ResourceGenerator : NetworkBehaviour
    {
        [SerializeField] Health health;
        [SerializeField] int resourcesPerInterval;
        [SerializeField] float interval;

        private float timer;
        private RTSPlayer player;

        public override void OnStartServer()
        {
            timer = interval;
            player = connectionToClient.identity.GetComponent<RTSPlayer>();

            health.ServerOnDie += ServerHandleDie;
            GameOverHandler.ServerOnGameOver += ServerHandleGameOver;
        }

        public override void OnStopServer()
        {
            health.ServerOnDie -= ServerHandleDie;
            GameOverHandler.ServerOnGameOver -= ServerHandleGameOver;
        }

        [ServerCallback]
        private void Update()
        {
            timer -= Time.deltaTime;

            if (timer <= 0)
            {
                timer = interval;
                player.SetResources(player.GetResources() + resourcesPerInterval);
            }
        }

        private void ServerHandleGameOver()
        {
            enabled = false;
        }

        private void ServerHandleDie()
        {
            NetworkServer.Destroy(gameObject);
        }
    }
}



