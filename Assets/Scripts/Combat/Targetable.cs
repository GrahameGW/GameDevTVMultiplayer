using UnityEngine;
using Mirror;

namespace RTSTutorialGame
{
    public class Targetable : NetworkBehaviour
    {
        [field: SerializeField] 
        public Transform AimPoint { get; private set; }

    }
}

