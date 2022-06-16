using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Inputs;
using VRWorkshop.Extensions;

namespace VRWorkshop.XR.Teleport
{
    public class XRTeleportController : MonoBehaviour
    {
        [Header("Input")]
        [SerializeField] private InputActionProperty selectAction;
        [Space]
        [SerializeField] private InputActionProperty activateAction;

        [Header("Dependencies")]
        [SerializeField] private XRRayInteractor rayInteractor;
        [SerializeField] private XRInteractorLineVisual lineVisualInteractor;

        private bool _isTeleportActive;
        private InteractionLayerMask _interactionLayers;

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

            selectAction.action.canceled += OnTeleportSelect;
            activateAction.action.performed += OnTeleportActivate;
            activateAction.action.canceled += OnTeleportCancel;
        }

        private void OnDisable()
        {
            selectAction.action.canceled -= OnTeleportSelect;
            activateAction.action.performed -= OnTeleportActivate;
            activateAction.action.canceled -= OnTeleportCancel;

            selectAction.DisableDirectAction();
            activateAction.DisableDirectAction();
        }

        public void OnTeleportActivate(InputAction.CallbackContext callbackContext)
        {
            EnableTeleport();
        }
        
        public void OnTeleportSelect(InputAction.CallbackContext callbackContext)
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