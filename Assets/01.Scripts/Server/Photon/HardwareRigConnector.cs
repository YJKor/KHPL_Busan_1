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

    // �� �������� ��Ʈ��ũ�� ���� ��� Ŭ���̾�Ʈ���� ����ȭ�˴ϴ�.
    [Networked] private Vector3 headPos { get; set; }
    [Networked] private Quaternion headRot { get; set; }
    [Networked] private Vector3 leftHandPos { get; set; }
    [Networked] private Quaternion leftHandRot { get; set; }
    [Networked] private Vector3 rightHandPos { get; set; }
    [Networked] private Quaternion rightHandRot { get; set; }

    public override void Spawned()
    {
        // �� ��ũ��Ʈ�� ���� �÷��̾��� �ƹ�Ÿ�� �پ����� ���� ����˴ϴ�.
        if (Object.HasInputAuthority)
        {
            // ���� �ִ� XR Origin�� ������Ʈ���� ã�Ƽ� �����մϴ�.
            // ���� �Ҵ��ϰų� FindObjectOfType ������ ã�� �� �ֽ��ϴ�.
            var rig = FindObjectOfType<XRRig>(); // XRRig�� ������� XR Origin ��ũ��Ʈ �̸��� ���� �޶��� �� �ֽ��ϴ�.
            if (rig != null)
            {
                hardwareHead = rig.cameraGameObject.transform;
                // Left/Right Hand Controller�� ã�� ���� �߰�
                // ��: hardwareLeftHand = rig.leftHand.transform;
            }
        }
    }

    public override void FixedUpdateNetwork()
    {
        // Input ������ �ִ� ���� �÷��̾ VR ����� �����͸� �о [Networked] ������ ���ϴ�.
        if (Object.HasInputAuthority)
        {
            if (hardwareHead != null)
            {
                headPos = hardwareHead.position;
                headRot = hardwareHead.rotation;
            }
            // (�޼�, �����տ� ���� �ڵ嵵 �����ϰ� �߰�)
            // if (hardwareLeftHand != null) { ... }
            // if (hardwareRightHand != null) { ... }
        }
    }

    public override void Render()
    {
        // ��� Ŭ���̾�Ʈ(����, ���� ���)���� ����˴ϴ�.
        // ����ȭ�� [Networked] ���� ���� �ƹ�Ÿ �𵨿� �����Ͽ� �������� �����ݴϴ�.
        avatarHead.SetPositionAndRotation(headPos, headRot);
        // (�޼�, �����տ� ���� �ڵ嵵 �����ϰ� �߰�)
        // avatarLeftHand.SetPositionAndRotation(leftHandPos, leftHandRot);
        // avatarRightHand.SetPositionAndRotation(rightHandPos, rightHandRot);
    }
}