using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.XR;

namespace VRWorkshop.XR.Hand
{
    [RequireComponent(typeof(Animator))]
    public abstract class DeviceBasedHandAnimator : MonoBehaviour
    {
        [SerializeField] 
        protected Animator animator;
        
        [SerializeField] 
        protected InputDeviceCharacteristics controllerCharacteristics = InputDeviceCharacteristics.Controller;
    
        protected InputDevice _device;
        
        protected virtual void Awake()
        {
            if (animator == null)
            {
                animator = GetComponent<Animator>();
            }

            InitDevice();
        }

        protected virtual void OnEnable()
        {
            InputDevices.deviceConnected += RegisterDevice;
        }

        protected virtual void OnDisable()
        {
            InputDevices.deviceConnected -= RegisterDevice;
        }

        protected virtual void Update()
        {
            if (!_device.isValid) return;
            AnimateHand();
        }

        private void RegisterDevice(InputDevice connectedDevice)
        {
            if (connectedDevice.isValid)
            {
                if ((connectedDevice.characteristics & controllerCharacteristics) == controllerCharacteristics)
                {
                    _device = connectedDevice;
                }
            }
        }

        private void InitDevice()
        {
            List<InputDevice> devices = new List<InputDevice>();
            InputDevices.GetDevicesWithCharacteristics(controllerCharacteristics, devices);
            _device = devices.FirstOrDefault();
        }

        protected abstract void AnimateHand();
    }
}
