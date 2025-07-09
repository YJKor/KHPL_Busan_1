using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine;

public class WeaponInput : MonoBehaviour
{
    [Header("Weapon Controller")]
    [SerializeField] private WeaponController weaponController;

    [Header("Input Keys")]
    [SerializeField] private KeyCode katanaKey = KeyCode.Alpha1;
    [SerializeField] private KeyCode crossSpearKey = KeyCode.Alpha2;
    [SerializeField] private KeyCode unequipKey = KeyCode.Alpha0;

    private void Update()
    {
        if (weaponController == null) return;

        // Ű���� �Է� ó��
        if (Input.GetKeyDown(katanaKey))
        {
            weaponController.EquipKatana();
            Debug.Log("Ű���� �Է�: īŸ�� ����");
        }

        if (Input.GetKeyDown(crossSpearKey))
        {
            weaponController.EquipCrossSpear();
            Debug.Log("Ű���� �Է�: ũ�ν� ���Ǿ� ����");
        }

        if (Input.GetKeyDown(unequipKey))
        {
            weaponController.UnequipWeapon();
            Debug.Log("Ű���� �Է�: ���� ����");
        }
    }
}
