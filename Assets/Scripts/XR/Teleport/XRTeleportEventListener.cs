using UnityEngine;
using UnityEngine.Events;

namespace VRWorkshop.XR.Teleport
{
    public class XRTeleportEventListener : MonoBehaviour
    {
        [Header("Events")]
        [SerializeField] private UnityEvent onActivateTeleport;
        [SerializeField] private UnityEvent onCancelTeleport;
        
        private XRTeleportEvents _xrTeleportEvents;

        private void Awake()
        {
            _xrTeleportEvents = FindObjectOfType<XRTeleportEvents>();
        }

        private void OnEnable()
        {
            _xrTeleportEvents.onActivateTeleport.AddListener(OnActivateTeleport);
            _xrTeleportEvents.onCancelTeleport.AddListener(OnCancelTeleport);
        }

        private void OnDisable()
        {
            _xrTeleportEvents.onActivateTeleport.RemoveListener(OnActivateTeleport);
            _xrTeleportEvents.onCancelTeleport.RemoveListener(OnCancelTeleport);
        }

        private void OnActivateTeleport()
        {
            onActivateTeleport?.Invoke();
        }
        
        private void OnCancelTeleport()
        {
            onCancelTeleport?.Invoke();
        }
    }
}