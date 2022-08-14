using System.Collections;
using Mirror;
using TMPro;
using UnityEngine;


namespace RTSTutorialGame
{
    public class ResourcesDisplay : MonoBehaviour
    {
        [SerializeField] TMP_Text resourcesText;
        
        private RTSPlayer player;

        private void Start()
        {
            player = NetworkClient.connection.identity.GetComponent<RTSPlayer>();
            ClientHandleResourcesUpdated(player.GetResources());
            player.ClientOnResourcesUpdated += ClientHandleResourcesUpdated;
        }

        private void OnDestroy()
        {
            player.ClientOnResourcesUpdated -= ClientHandleResourcesUpdated;
        }

        private void ClientHandleResourcesUpdated(int resources)
        {
            resourcesText.text = $"Resources: {resources}";
        }
    }
}

