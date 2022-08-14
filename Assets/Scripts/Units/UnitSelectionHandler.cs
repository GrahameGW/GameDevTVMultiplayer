using Mirror;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;


namespace RTSTutorialGame
{
    public class UnitSelectionHandler : MonoBehaviour
    {
        public List<Unit> SelectedUnits { get; private set; }

        [SerializeField] LayerMask layerMask;
        [SerializeField] RectTransform selectionBox;
        
        private Camera _camera;
        private RTSPlayer player;
        private Vector2 startPosition;


        private void Start()
        {
            _camera = Camera.main;
            SelectedUnits = new List<Unit>();
            player = NetworkClient.connection.identity.GetComponent<RTSPlayer>();
            Unit.AuthorityOnUnitDespawn += HandleAuthorityOnUnitDespawn;
            GameOverHandler.ClientOnGameOver += ClientHandleGameOver;
        }

        private void OnDestroy()
        {
            Unit.AuthorityOnUnitDespawn -= HandleAuthorityOnUnitDespawn;
            GameOverHandler.ClientOnGameOver -= ClientHandleGameOver;
        }

        private void Update()
        {
            if (Mouse.current.leftButton.wasPressedThisFrame)
            {
                StartSelectionArea();
            }
            else if (Mouse.current.leftButton.isPressed)
            {
                UpdateSelectionArea();
            }
            else if (Mouse.current.leftButton.wasReleasedThisFrame)
            {
                ClearSelectionArea();
            }
        }


        private void StartSelectionArea()
        {
            if (!Keyboard.current.leftShiftKey.isPressed)
            {
                for (int i = 0; i < SelectedUnits.Count; i++)
                {
                    SelectedUnits[i].Deselect();
                }
                SelectedUnits.Clear();
            }

            selectionBox.gameObject.SetActive(true);
            startPosition = Mouse.current.position.ReadValue();
            UpdateSelectionArea();
        }


        private void UpdateSelectionArea()
        {
            Vector2 mousePos = Mouse.current.position.ReadValue();

            float width = mousePos.x - startPosition.x;
            float height = mousePos.y - startPosition.y;

            selectionBox.sizeDelta = new Vector2(Mathf.Abs(width), Mathf.Abs(height));
            selectionBox.anchoredPosition = startPosition + new Vector2(width * 0.5f, height * 0.5f) ;
        }

        private void ClearSelectionArea()
        {
            selectionBox.gameObject.SetActive(false);

            if (selectionBox.sizeDelta.magnitude == 0) // single select
            {
                Ray ray = _camera.ScreenPointToRay(Mouse.current.position.ReadValue());

                if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, layerMask))
                {
                    if (hit.collider.TryGetComponent(out Unit unit))
                    {
                        if (!unit.hasAuthority) return;

                        SelectedUnits.Add(unit);

                        for (int i = 0; i < SelectedUnits.Count; i++)
                        {
                            SelectedUnits[i].Select();
                        }
                    }
                }

                return;
            }

            Vector2 min = selectionBox.anchoredPosition - selectionBox.sizeDelta * 0.5f;
            Vector2 max = selectionBox.anchoredPosition + selectionBox.sizeDelta * 0.5f;

            foreach (Unit unit in player.Units)
            {
                if (SelectedUnits.Contains(unit))
                {
                    continue;
                }
                
                var screenPos = _camera.WorldToScreenPoint(unit.transform.position);

                if (screenPos.x > min.x && screenPos.x < max.x 
                    && screenPos.y > min.y && screenPos.y < max.y)
                {
                    SelectedUnits.Add(unit);
                    unit.Select();
                }
            }
        }

        private void HandleAuthorityOnUnitDespawn(Unit unit)
        {
            SelectedUnits.Remove(unit);
        }


        private void ClientHandleGameOver(string winner)
        {
            enabled = false;
        }

    }
}

