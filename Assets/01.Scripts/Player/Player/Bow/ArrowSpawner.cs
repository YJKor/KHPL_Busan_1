using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class ArrowSpawner : MonoBehaviour
{
    public GameObject arrowPrefab;
    public Transform spawnPoint;
    private GameObject currentArrow;
    public AudioClip drawSound; // 활시위 당기는 소리
    public AudioClip fireSound; // 화살 발사 소리
    private AudioSource audioSource;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
            audioSource.spatialBlend = 1f; // VR용 3D 사운드
        }
        SpawnArrow();
    }

    void Update()
    {
       
    }

    
    public void SpawnArrow()
    {
        currentArrow = Instantiate(arrowPrefab, spawnPoint.position, spawnPoint.rotation);

        Arrow arrowScript = currentArrow.GetComponent<Arrow>();
        if (arrowScript != null)
        {
            arrowScript.spawner = this;
        }

        PlayDrawSound();
    }

    public void OnArrowShot()
    {
        SpawnArrow();
    }

    public void PlayDrawSound()
    {
        if (drawSound != null && audioSource != null)
        {
            audioSource.PlayOneShot(drawSound);
        }
    }

    public void PlayFireSound()
    {
        if (fireSound != null && audioSource != null)
        {
            audioSource.PlayOneShot(fireSound);
        }
    }
}

