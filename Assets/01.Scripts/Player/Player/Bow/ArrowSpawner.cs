using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

public class ArrowSpawner : MonoBehaviour
{
    public GameObject arrowPrefab; // 화살 프리팹
    public Transform spawnPoint; // 화살 생성 위치 (활 손잡이)

    public void SpawnArrow()
    {
        // 화살을 spawnPoint 위치와 회전에 생성하고, spawnPoint의 자식으로 설정
        GameObject newArrow = Instantiate(arrowPrefab, spawnPoint.position, spawnPoint.rotation, spawnPoint);
        Arrow arrowScript = newArrow.GetComponent<Arrow>();
        if (arrowScript != null)
        {
            arrowScript.spawner = this; // Arrow 스크립트에 spawner 참조 설정
        }
    }

    public void OnArrowShot()
    {
        // 화살이 발사되면 새 화살 생성
        SpawnArrow();
    }

    void Start()
    {
        // 게임 시작 시 첫 화살 생성
        SpawnArrow();
    }
}
