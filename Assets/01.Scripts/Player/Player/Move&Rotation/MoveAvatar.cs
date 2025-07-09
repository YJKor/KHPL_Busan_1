using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveAvatar : MonoBehaviour
{
    [Tooltip("이동 및 회전을 추적할 XR Origin Transform")]
    public Transform xrOriginTransform;
    [Tooltip("Camera Transform (Y축 회전 기준, HMD 또는 MainCamera)")]
    public Transform cameraTransform;

    private Vector3 prevXROriginPosition;

    private void Start()
    {
        if (xrOriginTransform == null || cameraTransform == null)
        {
            Debug.LogError("XR Origin과 Camera Transform을 모두 할당하세요.");
            enabled = false;
            return;
        }

        // 초기 위치 저장
        prevXROriginPosition = xrOriginTransform.position;
    }

    private void LateUpdate()
    {
        // XR Origin의 이동 거리 계산
        Vector3 delta = xrOriginTransform.position - prevXROriginPosition;
        // 그 이동 거리만큼 캐릭터 오브젝트를 이동
        transform.position += delta;

        // 카메라의 Y축 회전을 따라감 (상하 회전 무시)
        Vector3 cameraEuler = cameraTransform.rotation.eulerAngles;
        transform.rotation = Quaternion.Euler(0f, cameraEuler.y, 0f);

        // 다음 프레임 대비 XR Origin 위치 저장
        prevXROriginPosition = xrOriginTransform.position;
    }
}
