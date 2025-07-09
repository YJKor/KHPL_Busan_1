using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine;
using UnityEngine.UI;

public class WeaponUI : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private Button katanaButton;
    [SerializeField] private Button crossSpearButton;
    [SerializeField] private Button unequipButton;

    [Header("Weapon Controller")]
    [SerializeField] private WeaponController weaponController;

    private void Start()
    {
        // ��ư �̺�Ʈ ����
        if (katanaButton != null)
            katanaButton.onClick.AddListener(OnKatanaButtonClicked);

        if (crossSpearButton != null)
            crossSpearButton.onClick.AddListener(OnCrossSpearButtonClicked);

        if (unequipButton != null)
            unequipButton.onClick.AddListener(OnUnequipButtonClicked);
    }

    private void OnKatanaButtonClicked()
    {
        if (weaponController != null)
        {
            weaponController.EquipKatana();
            Debug.Log("īŸ�� ������");
        }
    }

    private void OnCrossSpearButtonClicked()
    {
        if (weaponController != null)
        {
            weaponController.EquipCrossSpear();
            Debug.Log("ũ�ν� ���Ǿ� ������");
        }
    }

    private void OnUnequipButtonClicked()
    {
        if (weaponController != null)
        {
            weaponController.UnequipWeapon();
            Debug.Log("���� ���ŵ�");
        }
    }

    private void OnDestroy()
    {
        // ��ư �̺�Ʈ ����
        if (katanaButton != null)
            katanaButton.onClick.RemoveListener(OnKatanaButtonClicked);

        if (crossSpearButton != null)
            crossSpearButton.onClick.RemoveListener(OnCrossSpearButtonClicked);

        if (unequipButton != null)
            unequipButton.onClick.RemoveListener(OnUnequipButtonClicked);
    }
}
