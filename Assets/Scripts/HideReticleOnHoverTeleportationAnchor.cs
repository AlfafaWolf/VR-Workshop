using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

namespace VRWorkshop
{
    public class HideReticleOnHoverTeleportationAnchor : MonoBehaviour
    {
        [SerializeField] private GameObject reticleRender;
    
        private XRRayInteractor _xrRayInteractor;

        private void Awake()
        {
            _xrRayInteractor = GetComponent<XRRayInteractor>();
        }

        private void OnEnable()
        {
            _xrRayInteractor.hoverEntered.AddListener(OnHoverEntered);
            _xrRayInteractor.hoverExited.AddListener(OnHoverExited);
        }

        private void OnDisable()
        {
            _xrRayInteractor.hoverEntered.RemoveListener(OnHoverEntered);
            _xrRayInteractor.hoverExited.RemoveListener(OnHoverExited);
        }

        private void OnHoverEntered(HoverEnterEventArgs args)
        {
            if (args.interactableObject is TeleportationAnchor)
            {
                reticleRender.SetActive(false);
            }
        }
    
        private void OnHoverExited(HoverExitEventArgs args)
        {
            if (args.interactableObject is TeleportationAnchor)
            {
                reticleRender.SetActive(true);
            }
        }
    }
}