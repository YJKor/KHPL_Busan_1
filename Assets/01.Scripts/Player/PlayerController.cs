using Fusion;
using UnityEngine;
using UnityEngine.InputSystem; // Unity의 새 Input System 사용을 위해 추가

// NetworkBehaviour를 계속 상속받습니다.
public class PlayerController : NetworkBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float _speed = 5.0f;

    [Header("Input Actions")]
    [SerializeField] private InputActionReference _moveAction; // 조이스틱 입력 액션을 연결할 필드

    [Header("XR Rig References")]
    [SerializeField] private Transform _hmdTransform; // XR Origin의 메인 카메라 Transform을 연결할 필드


    public override void Spawned()
    {
        _hmdTransform = Camera.main.transform;

        if (_hmdTransform == null)
        {
            Debug.LogError("Main Camera를 찾을 수 없습니다! XR Origin의 Main Camera에 'MainCamera' 태그가 있는지 확인해주세요.");
        }

        if (Object.HasInputAuthority)
        {
            // 이 클라이언트가 조종 권한을 가진 플레이어일 경우
            // InventoryManager에게 인벤토리 로드를 요청합니다.
            // (InventoryManager는 로그인 상태 변경 시 자동으로 로드하므로,
            // 이 코드가 없어도 되지만 명시적으로 호출해 줄 수도 있습니다.)
            Debug.Log("플레이어 스폰 완료. 인벤토리 시스템과 연동합니다.");
            //InventoryManager.Instance.LoadInventory();
        }

    }
    public override void FixedUpdateNetwork()
    {
        // 이 오브젝트가 내 것이 아니면 조종할 수 없도록 막는 부분 (매우 중요!)
        if (HasInputAuthority == false)
        {
            return;
        }
        if (_hmdTransform == null)
        {
            return;
        }
        // --- 여기부터 Quest 3 컨트롤러 입력 처리 ---

        // 1. 왼쪽 컨트롤러 조이스틱의 2D 축(Vector2) 값을 읽어옵니다.
        Vector2 moveInput = _moveAction.action.ReadValue<Vector2>();

        // 2. VR에서는 헤드셋이 바라보는 방향 기준으로 움직여야 자연스럽습니다.
        //    카메라의 정면 방향을 가져오되, 위아래(Y축)는 무시하여 수평 방향만 사용합니다.
        Vector3 forward = Vector3.ProjectOnPlane(_hmdTransform.forward, Vector3.up).normalized;
        Vector3 right = Vector3.ProjectOnPlane(_hmdTransform.right, Vector3.up).normalized;

        // 3. 조이스틱 입력과 카메라 방향을 조합하여 최종 이동 방향을 계산합니다.
        Vector3 moveDir = (forward * moveInput.y + right * moveInput.x).normalized;

        // 4. 계산된 방향으로 캐릭터를 이동시킵니다.
        transform.position += moveDir * _speed * Runner.DeltaTime;
    }
}