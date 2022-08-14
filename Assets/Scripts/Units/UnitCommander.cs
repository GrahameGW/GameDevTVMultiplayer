using UnityEngine;
using UnityEngine.InputSystem;

namespace RTSTutorialGame
{
    public class UnitCommander :  MonoBehaviour
    {
        [SerializeField] UnitSelectionHandler selectionHandler;
        [SerializeField] LayerMask layerMask;

        private Camera _camera;


        private void Start()
        {
            _camera = Camera.main;
            GameOverHandler.ClientOnGameOver += ClientHandleGameOver;
        }

        private void OnDestroy()
        {
            GameOverHandler.ClientOnGameOver -= ClientHandleGameOver;
        }
        private void Update()
        {
            if (!Mouse.current.rightButton.wasPressedThisFrame) return;

            Ray ray = _camera.ScreenPointToRay(Mouse.current.position.ReadValue());

            if (!Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, layerMask)) { return; }
            
            if (hit.collider.TryGetComponent(out Targetable target))
            {
                if (!target.hasAuthority)
                {
                    TryTarget(target);
                    return;
                }
            }

            TryMove(hit.point);

        }

        private void TryMove(Vector3 destination)
        {
            foreach (Unit unit in selectionHandler.SelectedUnits)
            {
                unit.UnitMovement.CmdMove(destination);
            }
        }

        private void TryTarget(Targetable target)
        {
            foreach (Unit unit in selectionHandler.SelectedUnits)
            {
                unit.Targeter.CmdSetTarget(target.gameObject);
            }
        }

        private void ClientHandleGameOver(string winner)
        {
            enabled = false;
        }
    }
}

