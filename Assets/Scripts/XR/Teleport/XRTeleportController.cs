using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.XR;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Inputs;
using InputDevice = UnityEngine.XR.InputDevice;

namespace VRWorkshop.XR.Teleport
{
    public class XRTeleportController : MonoBehaviour
    {
        [Header("Settings")]
        [SerializeField] private InputDeviceCharacteristics controllerCharacteristics = InputDeviceCharacteristics.Controller;
        
        [Header("Input")]
        [SerializeField] private InputActionProperty primary2DAxisPressAction;
        [SerializeField] private InputActionProperty primary2DAxisValueAction;

        [Header("Dependencies")]
        [SerializeField] private XRRayInteractor rayInteractor;
        [SerializeField] private XRInteractorLineVisual lineVisualInteractor;
        
        [Header("Events")]
        public UnityEvent onActivateTeleport;
        public UnityEvent onCancelTeleport;
        
        private InteractionLayerMask _teleportInteractionLayers;
        private bool _isTeleportActive;
        private bool _isPrimary2DAxisPressed;
        private Vector2 _primary2DAxis;
        private bool _wasTeleportSelect;
        private InputDevice _device;
        private XRControllerModelType _controllerModel;

        private const float TeleportSelectThreshold = 0.7f;
        private const float TeleportActivateThreshold = 0.6f;

        private void Awake()
        {
            if (rayInteractor == null) 
                rayInteractor = GetComponent<XRRayInteractor>();

            if (lineVisualInteractor == null)
                lineVisualInteractor = GetComponent<XRInteractorLineVisual>();

            _teleportInteractionLayers = rayInteractor.interactionLayers;
        }

        private void Start()
        {
            InitDevice();
            DisableTeleportComponents();
        }

        private void OnEnable()
        {
            primary2DAxisPressAction.EnableDirectAction();
            primary2DAxisValueAction.EnableDirectAction();
            InputDevices.deviceConnected += RegisterDevice;
        }
        
        private void OnDisable()
        {
            primary2DAxisPressAction.DisableDirectAction();
            primary2DAxisValueAction.DisableDirectAction();
            InputDevices.deviceConnected -= RegisterDevice;
        }
        
        private void Update()
        {
            CheckTeleportActivateInput();
            CheckTeleportSelectInput();
        }

        private void InitDevice()
        {
            List<InputDevice> devices = new List<InputDevice>();
            InputDevices.GetDevicesWithCharacteristics(controllerCharacteristics, devices);
            _device = devices.FirstOrDefault();
            _controllerModel = XRHelpers.GetInputDeviceControllerModel(_device);
        }
        
        private void RegisterDevice(InputDevice connectedDevice)
        {
            if (!connectedDevice.isValid) return;
            
            if ((connectedDevice.characteristics & controllerCharacteristics) == controllerCharacteristics)
            {
                _device = connectedDevice;
                _controllerModel = XRHelpers.GetInputDeviceControllerModel(_device);
            }
        }

        private void CheckTeleportSelectInput()
        {
            if (_controllerModel == XRControllerModelType.HTCVive)
            {
                CheckTeleportSelectHtcViveInput();
            }
            else
            {
                CheckTeleportSelectOculusTouchInput();
            }
        }
        
        private void CheckTeleportActivateInput()
        {
            if (_controllerModel == XRControllerModelType.HTCVive)
            {
                CheckTeleportActivateHtcViveInput();
            }
            else
            {
                CheckTeleportActivateOculusTouchInput();
            }
        }

        private void CheckTeleportActivateHtcViveInput()
        {
            _isPrimary2DAxisPressed = primary2DAxisPressAction.action?.IsPressed() ?? false;
            _primary2DAxis = primary2DAxisValueAction.action?.ReadValue<Vector2>() ?? Vector2.zero;
            
            if (_isPrimary2DAxisPressed)
            {
                if (_primary2DAxis.y >= TeleportActivateThreshold)
                {
                    if (_isTeleportActive)
                        return;
                    
                    EnableTeleport();
                }
            }
            else
            {
                if (!_isTeleportActive)
                    return;

                if (_wasTeleportSelect)
                {
                    PerformTeleport();
                    _wasTeleportSelect = false;
                }
                
                DisableTeleport();
            }
        }

        private void CheckTeleportSelectHtcViveInput()
        {
            _isPrimary2DAxisPressed = primary2DAxisPressAction.action?.IsPressed() ?? false;
            _primary2DAxis = primary2DAxisValueAction.action?.ReadValue<Vector2>() ?? Vector2.zero;
            
            if (_isPrimary2DAxisPressed)
            {
                if (_primary2DAxis.y >= TeleportSelectThreshold)
                {
                    _wasTeleportSelect = true;
                }
            }
        }
        
        private void CheckTeleportActivateOculusTouchInput()
        {
            _primary2DAxis = primary2DAxisValueAction.action?.ReadValue<Vector2>() ?? Vector2.zero;
            
            if (_primary2DAxis.y >= TeleportActivateThreshold)
            {
                if (_isTeleportActive)
                    return;
                
                EnableTeleport();
            }
            else
            {
                if (!_isTeleportActive)
                    return;

                if (_wasTeleportSelect)
                {
                    PerformTeleport();
                    _wasTeleportSelect = false;
                }
                
                DisableTeleport();
            }
        }
        
        private void CheckTeleportSelectOculusTouchInput()
        {
            _primary2DAxis = primary2DAxisValueAction.action?.ReadValue<Vector2>() ?? Vector2.zero;
            
            if (_primary2DAxis.y >= TeleportSelectThreshold)
            {
                _wasTeleportSelect = true;
            }
        }

        private void PerformTeleport()
        {
            if (!_isTeleportActive)
                return;
            
            if (!rayInteractor.TryGetCurrent3DRaycastHit(out RaycastHit hit))
                return;

            var interactable = hit.transform.GetComponentInParent<BaseTeleportationInteractable>();
            if (interactable == null)
                return;

            bool hasInteractionLayerOverlap = (rayInteractor.interactionLayers & interactable.interactionLayers) != 0;
            
            if (!hasInteractionLayerOverlap)
                return;

            rayInteractor.interactionManager.SelectEnter(rayInteractor as IXRSelectInteractor, interactable as IXRSelectInteractable);
        }

        private void EnableTeleport()
        {
            rayInteractor.interactionLayers = _teleportInteractionLayers;
            lineVisualInteractor.enabled = true;
            lineVisualInteractor.reticle.SetActive(true);
            _isTeleportActive = true;
            onActivateTeleport?.Invoke();
        }
        
        private void DisableTeleport()
        {
            _isTeleportActive = false;
            DisableTeleportComponents();
            onCancelTeleport?.Invoke();
        }

        private void DisableTeleportComponents()
        {
            rayInteractor.interactionLayers = 0;
            lineVisualInteractor.enabled = false;
            lineVisualInteractor.reticle.SetActive(false);
        }
    }
}