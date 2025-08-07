using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class Arrow : MonoBehaviour
{
    public ArrowSpawner spawner; // ArrowSpawner ����
    private XRGrabInteractable grabInteractable; // VR ��ȣ�ۿ� ������Ʈ
    private Rigidbody rb; // ���� ������Ʈ
    private int damage = 10; // ȭ�� ������

    void Start()
    {
        // XRGrabInteractable ����
        grabInteractable = GetComponent<XRGrabInteractable>();
        if (grabInteractable != null)
        {
            grabInteractable.selectExited.AddListener(OnRelease); // ���� �� �̺�Ʈ ����
        }
        // Rigidbody�� kinematic���� ����
        rb = GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.isKinematic = true;
        }
    }

    void OnRelease(SelectExitEventArgs args)
    {
        // ȭ�� ���� �� �߻�
        Shoot();
    }

    void Shoot()
    {
        // ȭ�� �߻�: Rigidbody�� ���������� �����ϰ� �� �߰�
        if (rb != null)
        {
            rb.isKinematic = false;
            rb.AddForce(transform.forward * 10f, ForceMode.Impulse);
        }
        // �����ʿ� �߻� �˸�
        if (spawner != null)
        {
            spawner.OnArrowShot();
        }
    }


    void OnCollisionEnter(Collision collision)
    {
        // ���� �浹 �� ������ ����
        if (collision.gameObject.CompareTag("Enemy"))
        {
            Enemy enemy = collision.gameObject.GetComponent<Enemy>();
            if (enemy != null)
            {
                enemy.TakeDamage(damage);
            }
        }
    }
}
