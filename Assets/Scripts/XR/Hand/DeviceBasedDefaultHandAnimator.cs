using UnityEngine;
using UnityEngine.XR;

namespace VRWorkshop.XR.Hand
{
    public class DeviceBasedDefaultHandAnimator : DeviceBasedHandAnimator
    {
        private readonly int animParamGripHash    = Animator.StringToHash("Grip");
        private readonly int animParamTriggerHash = Animator.StringToHash("Trigger");

        protected override void AnimateHand()
        {
            if (_device.TryGetFeatureValue(CommonUsages.trigger, out float triggerValue))
            {
                animator.SetFloat(animParamTriggerHash, triggerValue);
            }
            else
            {
                animator.SetFloat(animParamTriggerHash, 0.0f);
            }

            if (_device.TryGetFeatureValue(CommonUsages.grip, out float gripValue))
            {
                animator.SetFloat(animParamGripHash, gripValue);
            }
            else
            {
                animator.SetFloat(animParamGripHash, 0.0f);
            }
        }
    }
}
