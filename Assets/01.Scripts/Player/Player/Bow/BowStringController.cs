using UnityEngine;

// 이 스크립트를 Line Renderer가 있는 게임 오브젝트에 추가합니다.
[RequireComponent(typeof(LineRenderer))]
public class BowstringController : MonoBehaviour
{
    private LineRenderer lineRenderer;

    // 활의 양 끝점과 손의 위치를 받아올 Transform
    public Transform topPoint;
    public Transform bottomPoint;
    public Transform handPoint; // 활시위를 당기는 손

    private bool isPulled = false; // 임시로 만든 '당겨짐' 상태 변수

    void Awake()
    {
        lineRenderer = GetComponent<LineRenderer>();
    }

    void Update()
    {
        // 예시: 스페이스바를 누르는 동안 시위가 당겨지도록 처리
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
            // 시위가 당겨졌을 때 (3개의 점: 활 윗부분, 손, 활 아랫부분)
            lineRenderer.positionCount = 3;
            lineRenderer.SetPosition(0, topPoint.localPosition);
            lineRenderer.SetPosition(1, handPoint.localPosition); // UseWorldSpace를 껐으므로 localPosition 사용
            lineRenderer.SetPosition(2, bottomPoint.localPosition);
        }
        else
        {
            // 평상시 상태 (2개의 점: 활 윗부분, 활 아랫부분)
            lineRenderer.positionCount = 2;
            lineRenderer.SetPosition(0, topPoint.localPosition);
            lineRenderer.SetPosition(1, bottomPoint.localPosition);
        }
    }
}