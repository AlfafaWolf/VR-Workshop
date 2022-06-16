using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit.Inputs;

namespace VRWorkshop.XR.Teleport
{
    public class XRTeleportEvents : MonoBehaviour
    {
        [SerializeField] private InputActionProperty leftHandActivateAction;
        [SerializeField] private InputActionProperty rightHandActivateAction;
        [Space]
        [SerializeField] private InputActionProperty leftHandCancelAction;
        [SerializeField] private InputActionProperty rightHandCancelAction;

        [Header("Events")]
        public UnityEvent onActivateTeleport;
        public UnityEvent onCancelTeleport;

        private bool _isLeftTeleportActive;
        private bool _isRightTeleportActive;

        public bool IsTeleportActive => _isLeftTeleportActive || _isRightTeleportActive;

        private void OnEnable()
        {
            leftHandActivateAction.EnableDirectAction();
            rightHandActivateAction.EnableDirectAction();
            leftHandCancelAction.EnableDirectAction();
            rightHandCancelAction.EnableDirectAction();

            leftHandActivateAction.action.performed += OnActivateLeftTeleport;
            rightHandActivateAction.action.performed += OnActivateRightTeleport;

            leftHandActivateAction.action.canceled += OnCancelLeftTeleport;
            rightHandActivateAction.action.canceled += OnCancelRightTeleport;
        }
        
        private void OnDisable()
        {
            leftHandActivateAction.action.performed -= OnActivateLeftTeleport;
            rightHandActivateAction.action.performed -= OnActivateRightTeleport;

            leftHandActivateAction.action.canceled -= OnCancelLeftTeleport;
            rightHandActivateAction.action.canceled -= OnCancelRightTeleport;
            
            leftHandActivateAction.DisableDirectAction();
            rightHandActivateAction.DisableDirectAction();
            leftHandCancelAction.DisableDirectAction();
            rightHandCancelAction.DisableDirectAction();
        }

        private void OnActivateLeftTeleport(InputAction.CallbackContext context)
        {
            if (!IsTeleportActive)
            {
                onActivateTeleport?.Invoke();
            }
            
            _isLeftTeleportActive = true;
        }
        
        private void OnActivateRightTeleport(InputAction.CallbackContext context)
        {
            if (!IsTeleportActive)
            {
                onActivateTeleport?.Invoke();
            }
            
            _isRightTeleportActive = true;
        }
        
        private void OnCancelLeftTeleport(InputAction.CallbackContext context)
        {
            if (!_isRightTeleportActive)
            {
                onCancelTeleport?.Invoke();
            }
            
            _isLeftTeleportActive = false;
        }
        
        private void OnCancelRightTeleport(InputAction.CallbackContext context)
        {
            if (!_isLeftTeleportActive)
            {
                onCancelTeleport?.Invoke();
            }
            
            _isRightTeleportActive = false;
        }
    }
}