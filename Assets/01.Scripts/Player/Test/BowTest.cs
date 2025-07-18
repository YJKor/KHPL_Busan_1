using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BowTest : MonoBehaviour
{
    [SerializeField] private LineRenderer _bowString;
    [SerializeField] private GameObject _bowStringIndex1;

    [SerializeField] private GameObject _arrowPrefab;
     private Transform _arrowSpawnPoint;
    
    [SerializeField] private Transform _stringResetPoint;
    [SerializeField] private Transform _stringTopPoint;
    [SerializeField] private Transform _stringBottomPoint;



    private void Start()
    {
        _bowString.positionCount = 3;
        _bowString.SetPosition(1, _stringResetPoint.localPosition);
        _arrowSpawnPoint.position = _bowString.GetPosition(1);
        
    }

    private void Update()
    {
        _bowStringIndex1.transform.position = _stringResetPoint.position;
        _bowString.SetPosition(0, _stringTopPoint.transform.localPosition);
        _bowString.SetPosition(1, _bowStringIndex1.transform.position);
        _bowString.SetPosition(2, _stringBottomPoint.transform.localPosition);
        _arrowPrefab.transform.position = _bowString.GetPosition(1);

    }



}
