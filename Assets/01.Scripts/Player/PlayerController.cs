using Fusion;
using UnityEngine;
using UnityEngine.InputSystem; // Unity�� �� Input System ����� ���� �߰�

// NetworkBehaviour�� ��� ��ӹ޽��ϴ�.
public class PlayerController : NetworkBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float _speed = 5.0f;

    [Header("Input Actions")]
    [SerializeField] private InputActionReference _moveAction; // ���̽�ƽ �Է� �׼��� ������ �ʵ�

    [Header("XR Rig References")]
    [SerializeField] private Transform _hmdTransform; // XR Origin�� ���� ī�޶� Transform�� ������ �ʵ�


    public override void Spawned()
    {
        _hmdTransform = Camera.main.transform;

        if (_hmdTransform == null)
        {
            Debug.LogError("Main Camera�� ã�� �� �����ϴ�! XR Origin�� Main Camera�� 'MainCamera' �±װ� �ִ��� Ȯ�����ּ���.");
        }

        if (Object.HasInputAuthority)
        {
            // �� Ŭ���̾�Ʈ�� ���� ������ ���� �÷��̾��� ���
            // InventoryManager���� �κ��丮 �ε带 ��û�մϴ�.
            // (InventoryManager�� �α��� ���� ���� �� �ڵ����� �ε��ϹǷ�,
            // �� �ڵ尡 ��� ������ ��������� ȣ���� �� ���� �ֽ��ϴ�.)
            Debug.Log("�÷��̾� ���� �Ϸ�. �κ��丮 �ý��۰� �����մϴ�.");
            //InventoryManager.Instance.LoadInventory();
        }

    }
    public override void FixedUpdateNetwork()
    {
        // �� ������Ʈ�� �� ���� �ƴϸ� ������ �� ������ ���� �κ� (�ſ� �߿�!)
        if (HasInputAuthority == false)
        {
            return;
        }
        if (_hmdTransform == null)
        {
            return;
        }
        // --- ������� Quest 3 ��Ʈ�ѷ� �Է� ó�� ---

        // 1. ���� ��Ʈ�ѷ� ���̽�ƽ�� 2D ��(Vector2) ���� �о�ɴϴ�.
        Vector2 moveInput = _moveAction.action.ReadValue<Vector2>();

        // 2. VR������ ������ �ٶ󺸴� ���� �������� �������� �ڿ��������ϴ�.
        //    ī�޶��� ���� ������ ��������, ���Ʒ�(Y��)�� �����Ͽ� ���� ���⸸ ����մϴ�.
        Vector3 forward = Vector3.ProjectOnPlane(_hmdTransform.forward, Vector3.up).normalized;
        Vector3 right = Vector3.ProjectOnPlane(_hmdTransform.right, Vector3.up).normalized;

        // 3. ���̽�ƽ �Է°� ī�޶� ������ �����Ͽ� ���� �̵� ������ ����մϴ�.
        Vector3 moveDir = (forward * moveInput.y + right * moveInput.x).normalized;

        // 4. ���� �������� ĳ���͸� �̵���ŵ�ϴ�.
        transform.position += moveDir * _speed * Runner.DeltaTime;
    }
}