using Mirror;
using UnityEngine;


namespace RTSTutorialGame
{
    public class UnitProjectile : NetworkBehaviour
    {
        [SerializeField] Rigidbody rb;
        [SerializeField] float destroyAfterSeconds = 5f;
        [SerializeField] float launchForce = 10f;
        [SerializeField] int damageToDeal = 20;

        private void Start()
        {
            rb.velocity = transform.forward * launchForce;
        }

        public override void OnStartServer()
        {
            Invoke(nameof(DestroySelf), destroyAfterSeconds);
        }

        [Server]
        private void DestroySelf()
        {
            NetworkServer.Destroy(gameObject);
        }

        [ServerCallback]
        private void OnTriggerEnter(Collider other)
        {
            if (other.TryGetComponent(out NetworkIdentity identity))
            {
                if (identity.connectionToClient == connectionToClient) { return; }
            }

            if (other.TryGetComponent(out Health health))
            {
                health.DealDamage(damageToDeal);
                DestroySelf();
            }
        }
    }
}

