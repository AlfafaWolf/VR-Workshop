using UnityEngine.XR;

namespace VRWorkshop.XR
{
    public static class XRHelpers
    {
        public static readonly string OculusTouchControllerName = "Oculus Touch Controller OpenXR";
        public static readonly string HTCViveControllerName = "HTC Vive Controller OpenXR";
        public static readonly string ValveIndexControllerName = "Index Controller OpenXR";
        
        public static XRControllerModelType GetInputDeviceControllerModel(string deviceName)
        {
            if (!string.IsNullOrEmpty(deviceName))
            {
                if (deviceName.Contains(HTCViveControllerName))
                {
                    return XRControllerModelType.HTCVive;
                }
                if (deviceName.Contains(OculusTouchControllerName))
                {
                    return XRControllerModelType.OculusTouch;
                }
                if (deviceName.Contains(ValveIndexControllerName))
                {
                    return XRControllerModelType.ValveIndex;
                }
            }
            
            return XRControllerModelType.None;
        }
        
        public static XRControllerModelType GetInputDeviceControllerModel(InputDevice device)
        {
            return GetInputDeviceControllerModel(device.name);
        }
    }
    
    public enum XRControllerModelType
    {
        None,
        
        /// <summary>
        /// Oculus Touch Controller.
        /// </summary>
        OculusTouch,
        
        /// <summary>
        /// HTC Vive Controller.
        /// </summary>
        HTCVive,
        
        /// <summary>
        /// Valve Index Controller
        /// </summary>
        ValveIndex
    }
}

