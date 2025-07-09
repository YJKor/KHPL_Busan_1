using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StringPullDetector : MonoBehaviour
{
    public BowController bowController; // BowController�� �ν����Ϳ��� �Ҵ�

    private bool _isPulling = false;

    void OnTriggerEnter(Collider other)
    {
        // StringAttachPoint�� ����� ��
        if (other.CompareTag("StringAttachPoint") && !_isPulling)
        {
            _isPulling = true;
            bowController.StartPull(this.transform); // ������ Transform ����
        }
    }

    void OnTriggerExit(Collider other)
    {
        // StringAttachPoint���� ���� �������� ��
        if (other.CompareTag("StringAttachPoint") && _isPulling)
        {
            _isPulling = false;
            bowController.ReleasePull();
        }
    }
}
