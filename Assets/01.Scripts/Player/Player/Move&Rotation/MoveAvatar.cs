using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveAvatar : MonoBehaviour
{
    public Transform cameraRotation;

    private Vector3 prevCameraPos;

    void Start()
    {
        prevCameraPos = cameraRotation.position;
    }

    void Update()
    {
        Vector3 cameraDelta = cameraRotation.position - prevCameraPos;

        Vector3 newPosition = transform.position + new Vector3(cameraDelta.x, 0, cameraDelta.z);
        transform.position = newPosition;

        Vector3 camEuler = cameraRotation.rotation.eulerAngles;
        transform.rotation = Quaternion.Euler(0, camEuler.y, 0);

        prevCameraPos = cameraRotation.position;
    }
}
