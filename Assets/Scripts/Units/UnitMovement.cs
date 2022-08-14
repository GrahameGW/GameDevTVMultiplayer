using Mirror;
using UnityEngine;
using UnityEngine.AI;


namespace RTSTutorialGame
{
    public class UnitMovement : NetworkBehaviour
    {
        [SerializeField] NavMeshAgent agent;
        [SerializeField] Targeter targeter;
        [SerializeField] float chaseRange = 10f;


        #region Server

        public override void OnStartServer()
        {
            GameOverHandler.ServerOnGameOver += ServerHandleGameOver;
        }

        public override void OnStopServer()
        {
            GameOverHandler.ServerOnGameOver -= ServerHandleGameOver;
        }

        [ServerCallback]
        private void Update()
        {
            if (targeter.Target != null)
            {
                ChaseTarget(targeter.Target);
                return;
            }

            if (agent.hasPath && agent.remainingDistance <= agent.stoppingDistance)
            {
                agent.ResetPath();
            }
        }

        private void ChaseTarget(Targetable target)
        {
            if ((target.transform.position - transform.position).sqrMagnitude > chaseRange * chaseRange)
            {
                agent.SetDestination(target.transform.position);
            }
            else if (agent.hasPath)
            {
                agent.ResetPath();
            }
        }

        [Command] 
        public void CmdMove(Vector3 position)
        {
            ServerMove(position);
        }

        [Server]
        public void ServerMove(Vector3 position)
        {
            targeter.ClearTarget();

            if (!NavMesh.SamplePosition(position, out NavMeshHit hit, 1f, NavMesh.AllAreas))
            {
                return;
            }

            agent.SetDestination(hit.position);
        }

        [Server]
        private void ServerHandleGameOver()
        {
            agent.ResetPath();
        }

        #endregion
    }
}

