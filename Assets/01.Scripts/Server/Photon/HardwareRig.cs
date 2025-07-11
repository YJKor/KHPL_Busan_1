using UnityEngine;

// 이 스크립트는 씬에 있는 XR Interaction Setup에 붙입니다.
public class HardwareRig : MonoBehaviour
{
    public static HardwareRig Instance { get; private set; }

    public Transform head;
    public Transform leftHand;
    public Transform rightHand;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }
}