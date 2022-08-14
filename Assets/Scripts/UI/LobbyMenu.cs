using Mirror;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace RTSTutorialGame
{
    public class LobbyMenu : MonoBehaviour
    {
        [SerializeField] GameObject lobbyUI;
        [SerializeField] Button startGameButton;
        [SerializeField] TMP_Text[] playerNameTexts;


        private void OnEnable()
        {
            RTSNetworkManager.ClientOnConnected += HandleClientConnected;
            RTSPlayer.AuthorityOnPartyOwnerChanged += AuthorityHandlePartyOwnerChanged;
            RTSPlayer.ClientOnInfoUpdated += ClientHandleInfoUpdated;
        }


        private void OnDisable()
        {
            RTSNetworkManager.ClientOnConnected -= HandleClientConnected;
            RTSPlayer.AuthorityOnPartyOwnerChanged -= AuthorityHandlePartyOwnerChanged;
            RTSPlayer.ClientOnInfoUpdated -= ClientHandleInfoUpdated;
        }

        private void AuthorityHandlePartyOwnerChanged(bool state)
        {
            startGameButton.gameObject.SetActive(state);
        }

        private void HandleClientConnected()
        {
            lobbyUI.SetActive(true);
        }


        private void ClientHandleInfoUpdated()
        {
            var players = ((RTSNetworkManager)NetworkManager.singleton).Players;

            for (int i = 0; i < players.Count; i++)
            {
                playerNameTexts[i].text = players[i].DisplayName;
            }

            startGameButton.interactable = players.Count > 1;
        }

        public void LeaveLobby()
        {
            if (NetworkServer.active && NetworkClient.isConnected)
            {
                NetworkManager.singleton.StopHost();
            }
            else
            {
                NetworkManager.singleton.StopClient();

                SceneManager.LoadScene(0);
            }
        }

        public void StartGame()
        {
            NetworkClient.connection.identity.GetComponent<RTSPlayer>().CmdStartGame();
        }
    }
}

