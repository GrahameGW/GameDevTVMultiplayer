using Mirror;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;

namespace RTSTutorialGame
{
    public class BuildingButton : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
    {
        [SerializeField] Building building;
        [SerializeField] Image iconImage;
        [SerializeField] TMP_Text priceText;
        [SerializeField] LayerMask floorMask;

        private Camera _camera;
        private RTSPlayer player;
        private GameObject buildingPreviewInstance;
        private MeshRenderer buildingRendInstance;
        private BoxCollider buildingCollider;


        private void Start()
        {
            _camera = Camera.main;
            iconImage.sprite = building.Icon;
            priceText.text = building.Price.ToString();
            buildingCollider = building.GetComponent<BoxCollider>();
            player = NetworkClient.connection.identity.GetComponent<RTSPlayer>();
        }

        private void Update()
        {
            if (buildingPreviewInstance != null)
            {
                UpdateBuildingPreview();
            }
        }
        public void OnPointerDown(PointerEventData eventData)
        {
            if (eventData.button != PointerEventData.InputButton.Left) { return; }

            if (player.GetResources() < building.Price) { return; }

            buildingPreviewInstance = Instantiate(building.Preview);
            buildingRendInstance = buildingPreviewInstance.GetComponentInChildren<MeshRenderer>();

            buildingPreviewInstance.SetActive(false);
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            if (buildingPreviewInstance == null) { return; }

            Ray ray = _camera.ScreenPointToRay(Mouse.current.position.ReadValue());

            if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, floorMask))
            {
                player.CmdTryPlaceBuilding(building.Id, hit.point);
            }

            Destroy(buildingPreviewInstance);
        }

        private void UpdateBuildingPreview()
        {
            Ray ray = _camera.ScreenPointToRay(Mouse.current.position.ReadValue());

            if (!Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, floorMask)) { return; }

            buildingPreviewInstance.transform.position = hit.point;

            if (!buildingPreviewInstance.activeSelf)
            {
                buildingPreviewInstance.SetActive(true);
            }

            Color color = player.CanPlaceBuilding(buildingCollider, hit.point) ? Color.green : Color.red;

            buildingRendInstance.material.SetColor("_BaseColor", color);
        }
    }
}



