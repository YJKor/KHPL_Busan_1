using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveAvatar : MonoBehaviour
{
    [Tooltip("�̵� �� ȸ���� ������ XR Origin Transform")]
    public Transform xrOriginTransform;
    [Tooltip("Camera Transform (Y�� ȸ�� ����, HMD �Ǵ� MainCamera)")]
    public Transform cameraTransform;

    private Vector3 prevXROriginPosition;

    private void Start()
    {
        if (xrOriginTransform == null || cameraTransform == null)
        {
            Debug.LogError("XR Origin�� Camera Transform�� ��� �Ҵ��ϼ���.");
            enabled = false;
            return;
        }

        // �ʱ� ��ġ ����
        prevXROriginPosition = xrOriginTransform.position;
    }

    private void LateUpdate()
    {
        // XR Origin�� �̵� �Ÿ� ���
        Vector3 delta = xrOriginTransform.position - prevXROriginPosition;
        // �� �̵� �Ÿ���ŭ ĳ���� ������Ʈ�� �̵�
        transform.position += delta;

        // ī�޶��� Y�� ȸ���� ���� (���� ȸ�� ����)
        Vector3 cameraEuler = cameraTransform.rotation.eulerAngles;
        transform.rotation = Quaternion.Euler(0f, cameraEuler.y, 0f);

        // ���� ������ ��� XR Origin ��ġ ����
        prevXROriginPosition = xrOriginTransform.position;
    }
}
