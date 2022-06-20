using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Inputs;
using UnityEngine.XR.OpenXR.Features.Interactions;
using VRWorkshop.Extensions;

namespace VRWorkshop.XR.Teleport
{
    public class XRTeleportController : MonoBehaviour
    {
        [Header("Input")]
        [SerializeField] private InputActionProperty selectAction;
        [SerializeField] private InputActionProperty selectValueAction;
        [SerializeField] private InputActionProperty activateAction;

        [Header("Dependencies")]
        [SerializeField] private XRRayInteractor rayInteractor;
        [SerializeField] private XRInteractorLineVisual lineVisualInteractor;
        
        private InteractionLayerMask _interactionLayers;
        private bool _isTeleportActive;
        private bool _isPrimary2DAxisPressed;
        private Vector2 _primary2DAxis;
        private bool _isViveController;
        private bool _wasTeleportActivated;

        private const float TeleportSelectThreshold = 0.7f;

        private void Awake()
        {
            if (rayInteractor == null) 
                rayInteractor = GetComponent<XRRayInteractor>();

            if (lineVisualInteractor == null)
                lineVisualInteractor = GetComponent<XRInteractorLineVisual>();

            _interactionLayers = rayInteractor.interactionLayers;
        }

        private void Start()
        {
            DisableTeleport();
        }

        private void OnEnable()
        {
            selectAction.EnableDirectAction();
            activateAction.EnableDirectAction();
            selectValueAction.EnableDirectAction();
            
            activateAction.action.performed += OnTeleportActivate;
            activateAction.action.canceled += OnTeleportCancel;
        }

        private void OnDisable()
        {
            activateAction.action.performed -= OnTeleportActivate;
            activateAction.action.canceled -= OnTeleportCancel;

            selectAction.DisableDirectAction();
            activateAction.DisableDirectAction();
            selectValueAction.DisableDirectAction();
        }

        private void Update()
        {
            CheckTeleportSelectInput();
        }

        private void CheckTeleportSelectInput()
        {
            _isPrimary2DAxisPressed = selectAction.action?.IsPressed() ?? false;
            _primary2DAxis = selectValueAction.action?.ReadValue<Vector2>() ?? Vector2.zero;
            _isViveController = selectValueAction.action?.activeControl?.device is HTCViveControllerProfile.ViveController;

            if (_primary2DAxis.y >= TeleportSelectThreshold && !_isViveController)
            {
                _wasTeleportActivated = true;
            }
            else if (_isPrimary2DAxisPressed && _isViveController)
            {
                if (_primary2DAxis.y >= TeleportSelectThreshold)
                {
                    _wasTeleportActivated = true;
                }
            }
            else
            {
                if (_wasTeleportActivated)
                {
                    _wasTeleportActivated = false;
                    PerformTeleport();
                }
            }
        }

        private void PerformTeleport()
        {
            if (!_isTeleportActive)
                return;

            if (!rayInteractor.TryGetCurrent3DRaycastHit(out RaycastHit hit))
                return;

            if (!hit.transform.gameObject.TryGetComponentInParent(out BaseTeleportationInteractable interactable))
                return;

            bool hasInteractionLayerOverlap = (rayInteractor.interactionLayers & interactable.interactionLayers) != 0;
            
            if (!hasInteractionLayerOverlap)
                return;
            
            rayInteractor.interactionManager.SelectEnter(rayInteractor as IXRSelectInteractor, interactable as IXRSelectInteractable);
        }

        private void OnTeleportActivate(InputAction.CallbackContext callbackContext)
        {
            EnableTeleport();
        }
        
        private void OnTeleportCancel(InputAction.CallbackContext callbackContext)
        {
            DisableTeleport();
        }

        private void EnableTeleport()
        {
            rayInteractor.interactionLayers = _interactionLayers;
            lineVisualInteractor.enabled = true;
            lineVisualInteractor.reticle.SetActive(true);
            _isTeleportActive = true;
        }
        
        private void DisableTeleport()
        {
            _isTeleportActive = false;
            rayInteractor.interactionLayers = 0;
            lineVisualInteractor.enabled = false;
            lineVisualInteractor.reticle.SetActive(false);
        }
    }
}