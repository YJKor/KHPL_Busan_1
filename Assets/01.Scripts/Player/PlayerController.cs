using Fusion;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(NetworkCharacterController))] // NCC가 필수임을 명시
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



    // --- 수정된 부분 ---
    private NetworkCharacterController _cc; // NetworkCharacterController 참조
    private Transform _hmdTransform; // 헤드셋(카메라) Transform

    private void Awake()
    {
        // 미리 컴포넌트를 가져옵니다.
        _cc = GetComponent<NetworkCharacterController>();
    }

    public override void Spawned()
    {
        // 입력 권한이 있는 내 캐릭터에만 카메라를 찾고 인벤토리를 로드합니다.
        if (HasInputAuthority)
        {
            _hmdTransform = Camera.main.transform;
            if (_hmdTransform == null)
            {
                Debug.LogError("Main Camera를 찾을 수 없습니다! XR Origin의 Main Camera에 'MainCamera' 태그가 있는지 확인해주세요.");
            }

            Debug.Log("플레이어 스폰 완료. 인벤토리 시스템과 연동합니다.");
            InventoryManager.Instance.LoadInventory();
        }
    }

    public override void FixedUpdateNetwork()
    {
        if (HasInputAuthority == false)
        {
            return;
        }

        // 입력 소스(카메라)가 없으면 이동 로직을 실행하지 않습니다.
        if (_hmdTransform == null)
        {
            Debug.LogWarning("헤드셋(카메라) Transform이 설정되지 않았습니다. 이동 로직을 실행하지 않습니다.");
            return;
        }


        if (HardwareRig.Instance != null)
        {
            Transform rigRoot = HardwareRig.Instance.transform; // XR Rig의 루트 Transform

            // 1. XR Rig 루트를 기준으로 실제 머리의 '상대 위치/회전'을 계산합니다.
            Vector3 localHeadPos = rigRoot.InverseTransformPoint(HardwareRig.Instance.head.position);
            Quaternion localHeadRot = Quaternion.Inverse(rigRoot.rotation) * HardwareRig.Instance.head.rotation;

            // 2. 계산된 '상대 위치/회전'을 아바타의 머리에 적용합니다.
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

        // ▼▼▼ 이동 방식 수정 ▼▼▼
        // transform.position을 직접 바꾸는 대신 NetworkCharacterController의 Move 함수를 사용합니다.
        _cc.Move(moveDir * _speed * Runner.DeltaTime);



    }
}