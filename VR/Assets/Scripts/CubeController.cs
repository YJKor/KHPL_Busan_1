using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CubeController : MonoBehaviour
{
    [Range(1f, 100f)]
    public float speed = 20.0f;
    void Update()
    {
        transform.position += Time.deltaTime * -transform.forward * speed;
    }
}
