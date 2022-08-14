using UnityEngine;
using Mirror;

namespace RTSTutorialGame
{
    public class TeamColorSetter : NetworkBehaviour
    {
        [SerializeField] Renderer[] renderers;

        [SyncVar(hook = nameof(HandleTeamColorUpdated))]
        private Color teamColor;

        #region Server

        public override void OnStartServer()
        {
            var player = connectionToClient.identity.GetComponent<RTSPlayer>();

            teamColor = player.TeamColor;
        }

        #endregion

        #region Client

        private void HandleTeamColorUpdated(Color oldColor, Color newColor)
        {
            for (int i = 0; i < renderers.Length; i++)
            {
                renderers[i].material.SetColor("_BaseColor", newColor);
            }
        }
        #endregion  
    }
}

