using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowTheCamera : MonoBehaviour
{
    public Transform cameraTransform;

    void LateUpdate() // Animator�� ����⸦ �����Ϸ��� LateUpdate ��� ����
    {
        if (cameraTransform == null) return;

        // ���� ȸ���� ī�޶�� �����ϰ� ���߱�
        transform.rotation = cameraTransform.rotation;
        // ���� localRotation�� ����ϰ� ������, �ʿ��� �����ุ �����ϵ��� ���� �ʿ�
    }
}
