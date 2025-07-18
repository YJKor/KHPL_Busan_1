using UnityEngine;

public class BowTest : MonoBehaviour
{
    [SerializeField] private LineRenderer _bowStringLine;
    [SerializeField] private GameObject _bowStringIndex1;
    [SerializeField] private GameObject _bowString;

    [SerializeField] private Transform _stringResetPoint;
    [SerializeField] private Transform _stringTopPoint;
    [SerializeField] private Transform _stringBottomPoint;

    private void Start()
    {
        _bowStringLine.positionCount = 3;
        _bowStringLine.SetPosition(1, _stringResetPoint.position);
    }

    private void Update()
    {
        _bowStringLine.SetPosition(0, _stringTopPoint.position);
        _bowStringLine.SetPosition(1, _bowStringIndex1.transform.position);
        _bowStringLine.SetPosition(2, _stringBottomPoint.position);

        _bowString.transform.position = _stringResetPoint.position;
    }
}

