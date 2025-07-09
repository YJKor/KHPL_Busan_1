using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponController : MonoBehaviour
{
    [Header("Weapon Settings")]
    [SerializeField] private Transform weaponSocket; // ���⸦ ������ ��ġ
    [SerializeField] private GameObject currentWeapon; // ���� ������ ����

    [Header("Available Weapons")]
    [SerializeField] private GameObject katanaPrefab;
    [SerializeField] private GameObject crossSpearPrefab;

    private void Start()
    {
        // �⺻ ����� īŸ�� ����
        if (katanaPrefab != null)
        {
            EquipWeapon(katanaPrefab);
        }
    }

    /// <summary>
    /// ���⸦ �����մϴ�
    /// </summary>
    /// <param name="weaponPrefab">������ ���� ������</param>
    public void EquipWeapon(GameObject weaponPrefab)
    {
        // ���� ���� ����
        if (currentWeapon != null)
        {
            DestroyImmediate(currentWeapon);
        }

        // �� ���� ���� �� ����
        if (weaponPrefab != null && weaponSocket != null)
        {
            currentWeapon = Instantiate(weaponPrefab, weaponSocket);
            currentWeapon.transform.localPosition = Vector3.zero;
            currentWeapon.transform.localRotation = Quaternion.identity;
        }
    }

    /// <summary>
    /// īŸ�� ����
    /// </summary>
    public void EquipKatana()
    {
        EquipWeapon(katanaPrefab);
    }

    /// <summary>
    /// ũ�ν� ���Ǿ� ����
    /// </summary>
    public void EquipCrossSpear()
    {
        EquipWeapon(crossSpearPrefab);
    }

    /// <summary>
    /// ���� ����
    /// </summary>
    public void UnequipWeapon()
    {
        if (currentWeapon != null)
        {
            DestroyImmediate(currentWeapon);
            currentWeapon = null;
        }
    }

    /// <summary>
    /// ���� ������ ���� ��ȯ
    /// </summary>
    public GameObject GetCurrentWeapon()
    {
        return currentWeapon;
    }
}
