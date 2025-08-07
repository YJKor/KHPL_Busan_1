using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class Arrow : MonoBehaviour
{
    public ArrowSpawner spawner; // ArrowSpawner 참조
    private XRGrabInteractable grabInteractable; // VR 상호작용 컴포넌트
    private Rigidbody rb; // 물리 컴포넌트
    private int damage = 10; // 화살 데미지

    void Start()
    {
        // XRGrabInteractable 설정
        grabInteractable = GetComponent<XRGrabInteractable>();
        if (grabInteractable != null)
        {
            grabInteractable.selectExited.AddListener(OnRelease); // 놓을 때 이벤트 연결
        }
        // Rigidbody를 kinematic으로 설정
        rb = GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.isKinematic = true;
        }
    }

    void OnRelease(SelectExitEventArgs args)
    {
        // 화살 놓을 때 발사
        Shoot();
    }

    void Shoot()
    {
        // 화살 발사: Rigidbody를 비운동학적으로 설정하고 힘 추가
        if (rb != null)
        {
            rb.isKinematic = false;
            rb.AddForce(transform.forward * 10f, ForceMode.Impulse);
        }
        // 스포너에 발사 알림
        if (spawner != null)
        {
            spawner.OnArrowShot();
        }
    }


    void OnCollisionEnter(Collision collision)
    {
        // 적과 충돌 시 데미지 전달
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
