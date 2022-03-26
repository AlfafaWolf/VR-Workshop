using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Inputs;

namespace VRWorkshop.XR
{
    public class XRTeleportController : MonoBehaviour
    {
        [Header("Input")]
        [SerializeField] private bool useCanceledPhaseInSelectAction;
        [SerializeField] private InputActionProperty selectAction;
        [Space]
        [SerializeField] private InputActionProperty activateAction;
        [Space]
        [SerializeField] private bool useCanceledPhaseInCancelAction;
        [SerializeField] private InputActionProperty cancelAction;
        
        [Header("Dependencies")]
        [SerializeField] private XRRayInteractor rayInteractor;
        [SerializeField] private XRInteractorLineVisual lineVisualInteractor;

        private bool _isTeleportActive;

        private void Awake()
        {
            if (rayInteractor == null) 
                rayInteractor = GetComponent<XRRayInteractor>();

            if (lineVisualInteractor == null)
                lineVisualInteractor = GetComponent<XRInteractorLineVisual>();
        }

        private void Start()
        {
            DisableTeleport();
        }

        private void OnEnable()
        {
            selectAction.EnableDirectAction();
            activateAction.EnableDirectAction();
            cancelAction.EnableDirectAction();
            
            if (!useCanceledPhaseInSelectAction)
                selectAction.action.performed += OnTeleportSelect;
            else
                selectAction.action.canceled += OnTeleportSelect;
            
            activateAction.action.performed += OnTeleportActivate;

            if (!useCanceledPhaseInCancelAction)
                cancelAction.action.performed += OnTeleportCancel;
            else
                cancelAction.action.canceled += OnTeleportCancel;
        }

        private void OnDisable()
        {
            if (!useCanceledPhaseInSelectAction)
                selectAction.action.performed -= OnTeleportSelect;
            else
                selectAction.action.canceled -= OnTeleportSelect;

            activateAction.action.performed -= OnTeleportActivate;

            if (!useCanceledPhaseInCancelAction)
                cancelAction.action.performed -= OnTeleportCancel;
            else
                cancelAction.action.canceled -= OnTeleportCancel;
            
            selectAction.DisableDirectAction();
            activateAction.DisableDirectAction();
            cancelAction.EnableDirectAction();
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

            if (!hit.transform.TryGetComponent(out BaseTeleportationInteractable interactable))
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
            rayInteractor.enabled = true;
            _isTeleportActive = true;
        }
        
        private void DisableTeleport()
        {
            rayInteractor.enabled = false;
            _isTeleportActive = false;
            lineVisualInteractor.reticle.SetActive(false);
        }
    }
    
}