using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit.Inputs;

namespace VRWorkshop.XR.Hand
{
    public class ActionBasedDefaultHandAnimator : HandAnimator
    {
        [SerializeField]
        private InputActionProperty handGripAction;
    
        [SerializeField]
        private InputActionProperty handTriggerAction;
    
        private readonly int animParamGripHash    = Animator.StringToHash("Grip");
        private readonly int animParamTriggerHash = Animator.StringToHash("Trigger");

        private float _gripValue;
        private float _triggerValue;
    
        private void OnEnable()
        {
            handGripAction.EnableDirectAction();
            handTriggerAction.EnableDirectAction();
        
            handGripAction.action.performed += UpdateGripValue;
            handGripAction.action.canceled += UpdateGripValue;
            handTriggerAction.action.performed += UpdateTriggerValue;
            handTriggerAction.action.canceled += UpdateTriggerValue;
        }

        private void OnDisable()
        {
            handGripAction.action.performed -= UpdateGripValue;
            handGripAction.action.canceled -= UpdateGripValue;
            handTriggerAction.action.performed -= UpdateTriggerValue;
            handTriggerAction.action.canceled -= UpdateTriggerValue;

            handGripAction.DisableDirectAction();
            handTriggerAction.DisableDirectAction();
        }

        private void UpdateTriggerValue(InputAction.CallbackContext context)
        {
            _triggerValue = context.ReadValue<float>();
        }

        private void UpdateGripValue(InputAction.CallbackContext context)
        {
            _gripValue = context.ReadValue<float>();
        }

        protected override void AnimateHand()
        {
            base.AnimateHand();
        
            animator.SetFloat(animParamGripHash, _gripValue);
            animator.SetFloat(animParamTriggerHash, _triggerValue);
        }
    }
}
