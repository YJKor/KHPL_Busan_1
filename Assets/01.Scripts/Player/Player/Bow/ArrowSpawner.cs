using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

public class ArrowSpawner : MonoBehaviour
{
    [SerializeField] private GameObject _arrowPrefab;
    [SerializeField] private GameObject _notchPoint;
    [SerializeField] private float _spawnDelay = 1;

    private XRGrabInteractable _bow;
    private XRPullInteractable _pullInteractable;
    private bool _arrowNotched = false;
    private GameObject _currentArrow = null;

    private void Start()
    {
        _bow = GetComponent<XRGrabInteractable>();
        _pullInteractable = GetComponentInChildren<XRPullInteractable>();

        if(_pullInteractable != null )
        {
            _pullInteractable.PullActionReleased += NotchEmpty;
        }
    }

    private void OnDestroy()
    {
        if(_pullInteractable != null)
        {
            _pullInteractable.PullActionReleased -= NotchEmpty;
        }
    }

    private void Update()
    {
        if(_bow.isSelected && !_arrowNotched)
        {
            _arrowNotched = true;
            StartCoroutine(DelayedSpawn());
        }

        if (_bow.isSelected && _currentArrow != null)
        {
            Destroy(_currentArrow);
            NotchEmpty(1f);
        }
    }

    private void NotchEmpty(float value)
    {
        _arrowNotched = false;
        _currentArrow = null;
    }

    private IEnumerator DelayedSpawn()
    {
        yield return new WaitForSeconds(_spawnDelay);

        _currentArrow = Instantiate(_arrowPrefab, _notchPoint.transform);

        ArrowLauncher launcher = _currentArrow.GetComponent<ArrowLauncher>();
        if(launcher != null && _pullInteractable != null)
        {
            launcher.Initialize(_pullInteractable);
        }
    }

}
