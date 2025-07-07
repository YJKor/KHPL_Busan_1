using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowTheCamera : MonoBehaviour
{
    public Transform cameraTransform;

    void LateUpdate() // Animator의 덮어쓰기를 방지하려면 LateUpdate 사용 권장
    {
        if (cameraTransform == null) return;

        // 목의 회전을 카메라와 동일하게 맞추기
        transform.rotation = cameraTransform.rotation;
        // 만약 localRotation을 사용하고 싶으면, 필요한 기준축만 복사하도록 연산 필요
    }
}
