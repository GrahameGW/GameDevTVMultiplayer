using Mirror;
using UnityEngine;
using TMPro;

namespace RTSTutorialGame
{
    public class GameOverDisplay : MonoBehaviour
    {
        [SerializeField] GameObject gameOverDisplayRoot;
        [SerializeField] TMP_Text winnerNameText;

        private void OnEnable()
        {
            GameOverHandler.ClientOnGameOver += ClientHandleGameOver;
        }

        private void OnDisable()
        {
            GameOverHandler.ClientOnGameOver -= ClientHandleGameOver;
        }

        public void LeaveGame()
        {
            if (NetworkServer.active && NetworkClient.isConnected)
            {
                // stop hosting
                NetworkManager.singleton.StopHost();
            }
            else
            {
                // stop client
                NetworkManager.singleton.StopClient();
            }
        }

        private void ClientHandleGameOver(string winner)
        {
            winnerNameText.text = $"{winner} Has Won!";

            gameOverDisplayRoot.SetActive(true);
        }
    }
}

