using UnityEngine;
using Mirror;
using UnityEngine.SceneManagement;
using System;
using Random = UnityEngine.Random;
using System.Collections.Generic;

namespace RTSTutorialGame
{
    public class RTSNetworkManager : NetworkManager
    {
        [SerializeField] GameObject unitBasePrefab;
        [SerializeField] GameOverHandler gameOverHandlerPrefab;

        public static event Action ClientOnConnected;
        public static event Action ClientOnDisconnected;

        public List<RTSPlayer> Players { get; private set; } = new();
        private bool isGameInProgress = false;

        #region Server

        public override void OnServerConnect(NetworkConnectionToClient conn)
        {
            if (isGameInProgress)
            {
                conn.Disconnect(); 
            }
        }

        public override void OnServerDisconnect(NetworkConnectionToClient conn)
        {
            var player = conn.identity.GetComponent<RTSPlayer>();

            Players.Remove(player);

            base.OnServerDisconnect(conn);
        }

        public override void OnStopServer()
        {
            Players.Clear();

            isGameInProgress = false;
        }

        public void StartGame()
        {
            if (Players.Count > 1)
            {
                isGameInProgress = true;

                ServerChangeScene("Map01");
            }
        }

        public override void OnServerAddPlayer(NetworkConnectionToClient conn)
        {
            base.OnServerAddPlayer(conn);

            var player = conn.identity.GetComponent<RTSPlayer>();
            player.DisplayName = $"Player {Players.Count + 1}";

            Players.Add(player);

            player.TeamColor = new Color(
                Random.Range(0f, 1f),
                Random.Range(0f, 1f),
                Random.Range(0f, 1f)
                );

            player.SetPartyOwner(Players.Count == 1);
        }

        public override void OnServerSceneChanged(string sceneName)
        {
            if (SceneManager.GetActiveScene().name.StartsWith("Map"))
            {
                var instance = Instantiate(gameOverHandlerPrefab);
                NetworkServer.Spawn(instance.gameObject);

                foreach (var player in Players)
                {
                    var baseInstance = Instantiate(
                        unitBasePrefab,
                        GetStartPosition().position,
                        Quaternion.identity);

                    NetworkServer.Spawn(baseInstance, player.connectionToClient);
                }
            }
        }

        #endregion

        #region Client

        public override void OnClientConnect()
        {
            base.OnClientConnect();
            ClientOnConnected?.Invoke();
        }

        public override void OnClientDisconnect()
        {
            base.OnClientDisconnect();
            ClientOnDisconnected?.Invoke();
        }

        public override void OnStopClient()
        {
            Players.Clear();
        }
        #endregion
    }
}

