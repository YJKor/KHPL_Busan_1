using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveAvatar : MonoBehaviour
{
    public Transform cameraRotation;

    private Vector3 prevCameraPos;

    void Start()
    {
        // 시작 시점에 카메라 위치를 저장
        prevCameraPos = cameraRotation.position;
    }

    void Update()
    {
        // 카메라의 현재 위치와 이전 위치 차이(이동분)를 구함
        Vector3 cameraDelta = cameraRotation.position - prevCameraPos;

        // 캐릭터의 Y값은 유지, X와 Z만 이동분 더하기
        Vector3 newPosition = transform.position + new Vector3(cameraDelta.x, 0, cameraDelta.z);
        transform.position = newPosition;

        // 회전은 Y축만 따라감
        Vector3 camEuler = cameraRotation.rotation.eulerAngles;
        transform.rotation = Quaternion.Euler(0, camEuler.y, 0);

        // 카메라 위치 갱신
        prevCameraPos = cameraRotation.position;
    }
}
