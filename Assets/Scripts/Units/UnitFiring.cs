using Mirror;
using UnityEngine;


namespace RTSTutorialGame
{
    public class UnitFiring : NetworkBehaviour
    {
        [SerializeField] Targeter targeter;
        [SerializeField] GameObject projectilePrefab;
        [SerializeField] Transform projectileSpawnPoint;
        [SerializeField] float fireRange = 5f;
        [SerializeField] float fireRate = 1f;
        [SerializeField] float rotationSpeed = 20f;

        private float lastFireTime;


        [ServerCallback]
        private void Update()
        {
            var target = targeter.Target;

            if (target == null) { return; }
            if (!CanFireAtTarget()) { return; }

            var targetRotation = 
                Quaternion.LookRotation(target.transform.position - transform.position);
            transform.rotation = 
                Quaternion.RotateTowards(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);

            if (Time.time > (1f / fireRate) + lastFireTime)
            {
                var projectileRotation = 
                    Quaternion.LookRotation(target.AimPoint.position - projectileSpawnPoint.position);
                var projectile = Instantiate(projectilePrefab, projectileSpawnPoint.position, projectileRotation);

                NetworkServer.Spawn(projectile, connectionToClient);

                lastFireTime = Time.time;
            }
        }

        [Server]
        private bool CanFireAtTarget()
        {
            return (targeter.Target.transform.position - transform.position).sqrMagnitude 
                <= fireRange * fireRange;
        }
    }
}

