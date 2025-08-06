using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

public class ArrowSpawner : MonoBehaviour
{
    public GameObject arrowPrefab; // ȭ�� ������
    public Transform spawnPoint; // ȭ�� ���� ��ġ (Ȱ ������)

    public void SpawnArrow()
    {
        // ȭ���� spawnPoint ��ġ�� ȸ���� �����ϰ�, spawnPoint�� �ڽ����� ����
        GameObject newArrow = Instantiate(arrowPrefab, spawnPoint.position, spawnPoint.rotation, spawnPoint);
        Arrow arrowScript = newArrow.GetComponent<Arrow>();
        if (arrowScript != null)
        {
            arrowScript.spawner = this; // Arrow ��ũ��Ʈ�� spawner ���� ����
        }
    }

    public void OnArrowShot()
    {
        // ȭ���� �߻�Ǹ� �� ȭ�� ����
        SpawnArrow();
    }

    void Start()
    {
        // ���� ���� �� ù ȭ�� ����
        SpawnArrow();
    }
}
