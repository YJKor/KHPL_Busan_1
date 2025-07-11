using Fusion;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class HardwareRigConnector : NetworkBehaviour
{
    [Header("Hardware Rig Parts")]
    public Transform hardwareHead;
    public Transform hardwareLeftHand;
    public Transform hardwareRightHand;

    [Header("Avatar Parts")]
    public Transform avatarHead;
    public Transform avatarLeftHand;
    public Transform avatarRightHand;

    // 이 변수들이 네트워크를 통해 모든 클라이언트에게 동기화됩니다.
    [Networked] private Vector3 headPos { get; set; }
    [Networked] private Quaternion headRot { get; set; }
    [Networked] private Vector3 leftHandPos { get; set; }
    [Networked] private Quaternion leftHandRot { get; set; }
    [Networked] private Vector3 rightHandPos { get; set; }
    [Networked] private Quaternion rightHandRot { get; set; }

    public override void Spawned()
    {
        // 이 스크립트가 로컬 플레이어의 아바타에 붙어있을 때만 실행됩니다.
        if (Object.HasInputAuthority)
        {
            // 씬에 있는 XR Origin의 컴포넌트들을 찾아서 연결합니다.
            // 직접 할당하거나 FindObjectOfType 등으로 찾을 수 있습니다.
            var rig = FindObjectOfType<XRRig>(); // XRRig는 사용자의 XR Origin 스크립트 이름에 따라 달라질 수 있습니다.
            if (rig != null)
            {
                hardwareHead = rig.cameraGameObject.transform;
                // Left/Right Hand Controller를 찾는 로직 추가
                // 예: hardwareLeftHand = rig.leftHand.transform;
            }
        }
    }

    public override void FixedUpdateNetwork()
    {
        // Input 권한이 있는 로컬 플레이어만 VR 장비의 데이터를 읽어서 [Networked] 변수에 씁니다.
        if (Object.HasInputAuthority)
        {
            if (hardwareHead != null)
            {
                headPos = hardwareHead.position;
                headRot = hardwareHead.rotation;
            }
            // (왼손, 오른손에 대한 코드도 동일하게 추가)
            // if (hardwareLeftHand != null) { ... }
            // if (hardwareRightHand != null) { ... }
        }
    }

    public override void Render()
    {
        // 모든 클라이언트(로컬, 원격 모두)에서 실행됩니다.
        // 동기화된 [Networked] 변수 값을 아바타 모델에 적용하여 움직임을 보여줍니다.
        avatarHead.SetPositionAndRotation(headPos, headRot);
        // (왼손, 오른손에 대한 코드도 동일하게 추가)
        // avatarLeftHand.SetPositionAndRotation(leftHandPos, leftHandRot);
        // avatarRightHand.SetPositionAndRotation(rightHandPos, rightHandRot);
    }
}