using Mirror;
using UnityEngine;
using UnityEngine.InputSystem;

namespace RTSTutorialGame
{
    public class CameraController : NetworkBehaviour
    {
        [SerializeField] Transform playerCameraTransform;
        [SerializeField] float speed = 20f;
        [SerializeField] float screenBorderThickness = 10f;
        [SerializeField] Vector2 screenXLimits;
        [SerializeField] Vector2 screenZLimits;

        private Vector2 previousInput;

        private Controls controls;


        public override void OnStartAuthority()
        {
            playerCameraTransform.gameObject.SetActive(true);

            controls = new Controls();

            controls.Player.MoveCamera.performed += SetPreviousInput;
            controls.Player.MoveCamera.canceled += SetPreviousInput;
            
            controls.Enable();
        }

        [ClientCallback]
        private void Update()
        {
            if (!hasAuthority || !Application.isFocused) { return; }

            UpdateCameraPosition();
        }

        private void UpdateCameraPosition()
        {
            var pos = playerCameraTransform.position;

            if (previousInput == Vector2.zero)
            {
                Vector2 cursorPosition = Mouse.current.position.ReadValue();
                Vector3 cursorMovement = Vector3.zero;

                if (cursorPosition.x >= Screen.width - screenBorderThickness)
                {
                    cursorMovement.x = 1f;
                }
                else if (cursorPosition.x <= screenBorderThickness)
                {
                    cursorMovement.x = -1f;
                }
                if (cursorPosition.y >= Screen.height - screenBorderThickness)
                {
                    cursorMovement.z = 1f;
                }
                else if (cursorPosition.y <= screenBorderThickness)
                {
                    cursorMovement.z = -1f;
                }

                pos += speed * Time.deltaTime * cursorMovement.normalized;
            }
            else
            {
                pos += speed * Time.deltaTime * new Vector3(previousInput.x, 0f, previousInput.y);
            }

            pos.x = Mathf.Clamp(pos.x, screenXLimits.x, screenXLimits.y);
            pos.z = Mathf.Clamp(pos.z, screenZLimits.x, screenZLimits.y);

            playerCameraTransform.position = pos;
        }

        private void SetPreviousInput(InputAction.CallbackContext ctx)
        {
            previousInput = ctx.ReadValue<Vector2>();
        }
    }
}

