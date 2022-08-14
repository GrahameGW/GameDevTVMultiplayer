using Mirror;
using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

namespace RTSTutorialGame
{
    public class Minimap : MonoBehaviour, IPointerDownHandler, IDragHandler
    {
        [SerializeField] RectTransform minimapRect;
        [SerializeField] float mapScale; // assumes map is square
        [SerializeField] float clickOffset;

        private Transform cameraTransform;


        private void Start()
        {
            cameraTransform = NetworkClient.connection.identity.GetComponent<RTSPlayer>().CameraTransform;
        }

        private void MoveCamera()
        {
            var mousePos = Mouse.current.position.ReadValue();

            if (RectTransformUtility.ScreenPointToLocalPointInRectangle(
                minimapRect, mousePos, null, out Vector2 point))
            {
                Vector2 lerp = new Vector2(
                    (point.x - minimapRect.rect.x) / minimapRect.rect.width,
                    (point.y - minimapRect.rect.y) / minimapRect.rect.height
                    );

                var newCameraPos = new Vector3(
                    Mathf.Lerp(-mapScale, mapScale, lerp.x),
                    cameraTransform.position.y,
                    Mathf.Lerp(-mapScale, mapScale, lerp.y)
                    );

                cameraTransform.position = newCameraPos + new Vector3(0f, 0f, clickOffset);
            }
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            MoveCamera();
        }

        public void OnDrag(PointerEventData eventData)
        {
            MoveCamera();
        }
    }
}

