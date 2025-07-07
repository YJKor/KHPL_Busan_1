using UnityEngine;
using System.Collections;
using Fusion;
using Fusion.Sockets;

[RequireComponent(typeof(NetworkObject), typeof(NetworkTransform))]
public class SimpleGemsAnim : NetworkBehaviour
{
    // 인스펙터 설정 변수들은 그대로 사용합니다.
    [Header("Rotation")]
    public bool isRotating = false;
    public bool rotateX = false;
    public bool rotateY = false;
    public bool rotateZ = false;
    public float rotationSpeed = 90f;

    [Header("Floating")]
    public bool isFloating = false;
    public bool useEasingForFloating = false;
    public float floatHeight = 1f;
    public float floatSpeed = 1f;

    [Header("Scaling")]
    public bool isScaling = false;
    public bool useEasingForScaling = false;
    public Vector3 endScaleMultiplier = new Vector3(1.2f, 1.2f, 1.2f);
    public float scaleLerpSpeed = 1f;

    // [Networked] float 변수로 타이머를 구현합니다.
    [Networked]
    private float NetworkedTimer { get; set; }

    // 로컬에서만 사용될 변수들
    private Vector3 _initialPosition;
    private Vector3 _initialScale;
    private Vector3 _startScale;
    private Vector3 _endScale;

    /// <summary>
    /// 네트워크 객체가 스폰될 때 모든 클라이언트에서 호출됩니다.
    /// </summary>
    public override void Spawned()
    {
        // 애니메이션 기준이 되는 초기 위치와 스케일 값을 저장합니다.
        _initialPosition = transform.position;
        _initialScale = transform.localScale;

        _startScale = _initialScale;
        _endScale = new Vector3(
            _initialScale.x * endScaleMultiplier.x,
            _initialScale.y * endScaleMultiplier.y,
            _initialScale.z * endScaleMultiplier.z
        );

        // TickTimer 관련 코드는 모두 삭제하여 깨끗한 상태로 둡니다.
    }

    /// <summary>
    /// Fusion의 고정 틱 업데이트 루프입니다.
    /// </summary>
    public override void FixedUpdateNetwork()
    {
        // 상태 권한이 없는 클라이언트(Proxy)는 아무것도 실행하지 않고 즉시 종료합니다.
        // 아래의 모든 코드는 상태 권한을 가진 클라이언트(서버/호스트)에서만 실행됩니다.
        if (!HasStateAuthority)
        {
            return;
        }

        // 네트워크 타이머에 매 틱 시간을 더해줍니다.
        NetworkedTimer += Runner.DeltaTime;

        // 동기화된 타이머 값을 사용합니다.
        float time = NetworkedTimer;

        // --- 회전 로직 ---
        if (isRotating)
        {
            Vector3 rotationVector = new Vector3(
                rotateX ? 1 : 0,
                rotateY ? 1 : 0,
                rotateZ ? 1 : 0
            );
            transform.Rotate(rotationVector * rotationSpeed * Runner.DeltaTime);
        }

        // --- 상하 이동(Floating) 로직 ---
        if (isFloating)
        {
            float floatTimer = time * floatSpeed;
            float t = Mathf.PingPong(floatTimer, 1f);
            if (useEasingForFloating) t = EaseInOutQuad(t);

            transform.position = _initialPosition + new Vector3(0, t * floatHeight, 0);
        }

        // --- 스케일링 로직 ---
        if (isScaling)
        {
            float scaleTimer = time * scaleLerpSpeed;
            float t = Mathf.PingPong(scaleTimer, 1f);
            if (useEasingForScaling) t = EaseInOutQuad(t);

            transform.localScale = Vector3.Lerp(_startScale, _endScale, t);
        }
    }

    private float EaseInOutQuad(float t)
    {
        return t < 0.5f ? 2 * t * t : 1 - Mathf.Pow(-2 * t + 2, 2) / 2;
    }
}