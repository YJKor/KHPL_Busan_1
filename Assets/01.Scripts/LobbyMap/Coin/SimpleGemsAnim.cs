using UnityEngine;
using System.Collections;
using Fusion;
using Fusion.Sockets;

[RequireComponent(typeof(NetworkObject), typeof(NetworkTransform))]
public class SimpleGemsAnim : NetworkBehaviour
{
    // �ν����� ���� �������� �״�� ����մϴ�.
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

    // [Networked] float ������ Ÿ�̸Ӹ� �����մϴ�.
    [Networked]
    private float NetworkedTimer { get; set; }

    // ���ÿ����� ���� ������
    private Vector3 _initialPosition;
    private Vector3 _initialScale;
    private Vector3 _startScale;
    private Vector3 _endScale;

    /// <summary>
    /// ��Ʈ��ũ ��ü�� ������ �� ��� Ŭ���̾�Ʈ���� ȣ��˴ϴ�.
    /// </summary>
    public override void Spawned()
    {
        // �ִϸ��̼� ������ �Ǵ� �ʱ� ��ġ�� ������ ���� �����մϴ�.
        _initialPosition = transform.position;
        _initialScale = transform.localScale;

        _startScale = _initialScale;
        _endScale = new Vector3(
            _initialScale.x * endScaleMultiplier.x,
            _initialScale.y * endScaleMultiplier.y,
            _initialScale.z * endScaleMultiplier.z
        );

        // TickTimer ���� �ڵ�� ��� �����Ͽ� ������ ���·� �Ӵϴ�.
    }

    /// <summary>
    /// Fusion�� ���� ƽ ������Ʈ �����Դϴ�.
    /// </summary>
    public override void FixedUpdateNetwork()
    {
        // ���� ������ ���� Ŭ���̾�Ʈ(Proxy)�� �ƹ��͵� �������� �ʰ� ��� �����մϴ�.
        // �Ʒ��� ��� �ڵ�� ���� ������ ���� Ŭ���̾�Ʈ(����/ȣ��Ʈ)������ ����˴ϴ�.
        if (!HasStateAuthority)
        {
            return;
        }

        // ��Ʈ��ũ Ÿ�̸ӿ� �� ƽ �ð��� �����ݴϴ�.
        NetworkedTimer += Runner.DeltaTime;

        // ����ȭ�� Ÿ�̸� ���� ����մϴ�.
        float time = NetworkedTimer;

        // --- ȸ�� ���� ---
        if (isRotating)
        {
            Vector3 rotationVector = new Vector3(
                rotateX ? 1 : 0,
                rotateY ? 1 : 0,
                rotateZ ? 1 : 0
            );
            transform.Rotate(rotationVector * rotationSpeed * Runner.DeltaTime);
        }

        // --- ���� �̵�(Floating) ���� ---
        if (isFloating)
        {
            float floatTimer = time * floatSpeed;
            float t = Mathf.PingPong(floatTimer, 1f);
            if (useEasingForFloating) t = EaseInOutQuad(t);

            transform.position = _initialPosition + new Vector3(0, t * floatHeight, 0);
        }

        // --- �����ϸ� ���� ---
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