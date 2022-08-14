using Mirror;
using UnityEngine;

namespace RTSTutorialGame
{
    public class MainMenu : MonoBehaviour
    {
        [SerializeField] GameObject landingPagePanel;

        public void HostLobby()
        {
            landingPagePanel.SetActive(false);

            NetworkManager.singleton.StartHost();
        }
    }
}

