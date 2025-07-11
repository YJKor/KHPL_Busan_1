using UnityEngine;

// �� ��ũ��Ʈ�� ���� �ִ� XR Interaction Setup�� ���Դϴ�.
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