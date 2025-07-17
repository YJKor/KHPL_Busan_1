using UnityEngine;

// �� ��ũ��Ʈ�� Line Renderer�� �ִ� ���� ������Ʈ�� �߰��մϴ�.
[RequireComponent(typeof(LineRenderer))]
public class BowstringController : MonoBehaviour
{
    private LineRenderer lineRenderer;

    // Ȱ�� �� ������ ���� ��ġ�� �޾ƿ� Transform
    public Transform topPoint;
    public Transform bottomPoint;
    public Transform handPoint; // Ȱ������ ���� ��

    private bool isPulled = false; // �ӽ÷� ���� '�����' ���� ����

    void Awake()
    {
        lineRenderer = GetComponent<LineRenderer>();
    }

    void Update()
    {
        // ����: �����̽��ٸ� ������ ���� ������ ��������� ó��
        if (Input.GetKeyDown(KeyCode.Space))
        {
            isPulled = true;
        }
        if (Input.GetKeyUp(KeyCode.Space))
        {
            isPulled = false;
        }


        if (isPulled)
        {
            // ������ ������� �� (3���� ��: Ȱ ���κ�, ��, Ȱ �Ʒ��κ�)
            lineRenderer.positionCount = 3;
            lineRenderer.SetPosition(0, topPoint.localPosition);
            lineRenderer.SetPosition(1, handPoint.localPosition); // UseWorldSpace�� �����Ƿ� localPosition ���
            lineRenderer.SetPosition(2, bottomPoint.localPosition);
        }
        else
        {
            // ���� ���� (2���� ��: Ȱ ���κ�, Ȱ �Ʒ��κ�)
            lineRenderer.positionCount = 2;
            lineRenderer.SetPosition(0, topPoint.localPosition);
            lineRenderer.SetPosition(1, bottomPoint.localPosition);
        }
    }
}