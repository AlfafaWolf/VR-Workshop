using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class XROffsetGrabbable : XRGrabInteractable
{
    private class SavedTransform
    {
        public Vector3 OriginalPosition;
        public Quaternion OriginalRotation;
    }

    private Dictionary<IXRSelectInteractor, SavedTransform> _savedTransforms = new Dictionary<IXRSelectInteractor, SavedTransform>();
    private Rigidbody _rigidbody;

    protected override void Awake()
    {
        base.Awake();

        // The base class already grab it but don't expose it so have to grab it again
        _rigidbody = GetComponent<Rigidbody>();
    }

    protected override void OnSelectEntered(SelectEnterEventArgs args)
    {
        if (args.interactorObject is XRDirectInteractor)
        {
            SavedTransform savedTransform = new SavedTransform();
            
            savedTransform.OriginalPosition = args.interactorObject.GetAttachTransform(args.interactableObject).localPosition;
            savedTransform.OriginalRotation = args.interactorObject.GetAttachTransform(args.interactableObject).localRotation;

            _savedTransforms[args.interactorObject] = savedTransform;

            bool haveAttach = attachTransform != null;

            args.interactorObject.GetAttachTransform(args.interactableObject).position = haveAttach ? attachTransform.position : transform.position;
            args.interactorObject.GetAttachTransform(args.interactableObject).rotation = haveAttach ? attachTransform.rotation : transform.rotation;
        }
            
        base.OnSelectEntered(args);
    }

    protected override void OnSelectExited(SelectExitEventArgs args)
    {
        if (args.interactorObject is XRDirectInteractor)
        {
            if (_savedTransforms.TryGetValue(args.interactorObject, out SavedTransform savedTransform))
            {
                args.interactorObject.GetAttachTransform(args.interactableObject).localPosition = savedTransform.OriginalPosition;
                args.interactorObject.GetAttachTransform(args.interactableObject).localRotation = savedTransform.OriginalRotation;

                _savedTransforms.Remove(args.interactorObject);
            }
        }
        
        base.OnSelectExited(args);
    }
}
