using Fusion;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(NetworkCharacterController))] // NCC�� �ʼ����� ���
public class PlayerController : NetworkBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float _speed = 5.0f;

    [Header("Input Actions")]
    [SerializeField] private InputActionReference _moveAction;

    [Header("Body Parts")]
    [SerializeField] private Transform _head;
    [SerializeField] private Transform _leftHand;
    [SerializeField] private Transform _rightHand;



    // --- ������ �κ� ---
    private NetworkCharacterController _cc; // NetworkCharacterController ����
    private Transform _hmdTransform; // ����(ī�޶�) Transform

    private void Awake()
    {
        // �̸� ������Ʈ�� �����ɴϴ�.
        _cc = GetComponent<NetworkCharacterController>();
    }

    public override void Spawned()
    {
        // �Է� ������ �ִ� �� ĳ���Ϳ��� ī�޶� ã�� �κ��丮�� �ε��մϴ�.
        if (HasInputAuthority)
        {
            _hmdTransform = Camera.main.transform;
            if (_hmdTransform == null)
            {
                Debug.LogError("Main Camera�� ã�� �� �����ϴ�! XR Origin�� Main Camera�� 'MainCamera' �±װ� �ִ��� Ȯ�����ּ���.");
            }

            Debug.Log("�÷��̾� ���� �Ϸ�. �κ��丮 �ý��۰� �����մϴ�.");
            InventoryManager.Instance.LoadInventory();
        }
    }

    public override void FixedUpdateNetwork()
    {
        if (HasInputAuthority == false)
        {
            return;
        }

        // �Է� �ҽ�(ī�޶�)�� ������ �̵� ������ �������� �ʽ��ϴ�.
        if (_hmdTransform == null)
        {
            Debug.LogWarning("����(ī�޶�) Transform�� �������� �ʾҽ��ϴ�. �̵� ������ �������� �ʽ��ϴ�.");
            return;
        }


        if (HardwareRig.Instance != null)
        {
            Transform rigRoot = HardwareRig.Instance.transform; // XR Rig�� ��Ʈ Transform

            // 1. XR Rig ��Ʈ�� �������� ���� �Ӹ��� '��� ��ġ/ȸ��'�� ����մϴ�.
            Vector3 localHeadPos = rigRoot.InverseTransformPoint(HardwareRig.Instance.head.position);
            Quaternion localHeadRot = Quaternion.Inverse(rigRoot.rotation) * HardwareRig.Instance.head.rotation;

            // 2. ���� '��� ��ġ/ȸ��'�� �ƹ�Ÿ�� �Ӹ��� �����մϴ�.
            _head.localPosition = localHeadPos;
            _head.localRotation = localHeadRot;

            
            Vector3 localLeftHandPos = rigRoot.InverseTransformPoint(HardwareRig.Instance.leftHand.position);
            Quaternion localLeftHandRot = Quaternion.Inverse(rigRoot.rotation) * HardwareRig.Instance.leftHand.rotation;
            _leftHand.localPosition = localLeftHandPos;
            _leftHand.localRotation = localLeftHandRot;

            Vector3 localRightHandPos = rigRoot.InverseTransformPoint(HardwareRig.Instance.rightHand.position);
            Quaternion localRightHandRot = Quaternion.Inverse(rigRoot.rotation) * HardwareRig.Instance.rightHand.rotation;
            _rightHand.localPosition = localRightHandPos;
            _rightHand.localRotation = localRightHandRot;

        }
        ControlPlayer();

    }



    private void ControlPlayer()
    {

        Vector2 moveInput = _moveAction.action.ReadValue<Vector2>();

        Vector3 forward = Vector3.ProjectOnPlane(_hmdTransform.forward, Vector3.up).normalized;
        Vector3 right = Vector3.ProjectOnPlane(_hmdTransform.right, Vector3.up).normalized;
        Vector3 moveDir = (forward * moveInput.y + right * moveInput.x).normalized;

        Debug.Log($"Final Move Command: Direction={moveDir}, Speed={_speed}");

        // ���� �̵� ��� ���� ����
        // transform.position�� ���� �ٲٴ� ��� NetworkCharacterController�� Move �Լ��� ����մϴ�.
        _cc.Move(moveDir * _speed * Runner.DeltaTime);



    }
}