using UnityEngine;
using Mirror;


namespace RTSTutorialGame
{
    public class Targeter : NetworkBehaviour
    {
        public Targetable Target { get; private set; }


        public override void OnStartServer()
        {
            GameOverHandler.ServerOnGameOver += ServerHandleGameOver;
        }

        public override void OnStopServer()
        {
            GameOverHandler.ServerOnGameOver -= ServerHandleGameOver;
        }

        [Command]
        public void CmdSetTarget(GameObject targetObj)
        {
            if (!targetObj.TryGetComponent(out Targetable newTarget)) { return; }

            Target = newTarget;
        }

        [Server]
        public void ClearTarget()
        {
            Target = null;
        }

        [Server]
        private void ServerHandleGameOver()
        {
            ClearTarget();
        }
    }
}

