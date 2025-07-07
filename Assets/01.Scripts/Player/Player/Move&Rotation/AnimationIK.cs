using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationIK : MonoBehaviour
{
    public Animator animator;
    public Transform leftHandTarget;
    public Transform rightHandTarget;

    public GameObject LeftHand;
    public GameObject RightHand;

    void OnAnimatorIK(int layerIndex)
    {
        if (animator == null) return;

        if (LeftHand.activeSelf)
        {
            // ¿Þ¼Õ
            animator.SetIKPositionWeight(AvatarIKGoal.LeftHand, 1.0f);
            animator.SetIKRotationWeight(AvatarIKGoal.LeftHand, 1.0f);
            animator.SetIKPosition(AvatarIKGoal.LeftHand, leftHandTarget.position);
            animator.SetIKRotation(AvatarIKGoal.LeftHand, leftHandTarget.rotation);
        }
        if (RightHand.activeSelf)
        { 
            // ¿À¸¥¼Õ
            animator.SetIKPositionWeight(AvatarIKGoal.RightHand, 1.0f);
            animator.SetIKRotationWeight(AvatarIKGoal.RightHand, 1.0f);
            animator.SetIKPosition(AvatarIKGoal.RightHand, rightHandTarget.position);
            animator.SetIKRotation(AvatarIKGoal.RightHand, rightHandTarget.rotation);
        }
        
    }
}
