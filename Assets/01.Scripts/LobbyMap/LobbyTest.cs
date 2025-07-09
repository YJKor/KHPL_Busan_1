using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LobbyTest : MonoBehaviour
{
    public float moveSpeed = 5f;
    public float jumpForce = 5f;
    public float mouseSensitivity = 3f;

    private Rigidbody rb;
    private bool isGrounded;
    private float yRotation;

    public LayerMask groundLayerMask;

    void Start()
    {
        rb = GetComponent<Rigidbody>();

        // ���콺 Ŀ�� ���
        Cursor.lockState = CursorLockMode.Locked;
    }

    void Update()
    {
        //  ���콺 �Է� �� ȸ�� ���� ����
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity;
        yRotation += mouseX;
        transform.rotation = Quaternion.Euler(0, yRotation, 0);

        // �̵� �Է� �� �ٶ󺸴� ���� ����
        float h = Input.GetAxisRaw("Horizontal"); // A/D
        float v = Input.GetAxisRaw("Vertical");   // W/S

        Vector3 move = (transform.right * h + transform.forward * v).normalized;
        Vector3 velocity = move * moveSpeed;
        velocity.y = rb.velocity.y; // ���� Y�ӵ� ����
        rb.velocity = velocity;

        //  ����
        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
            isGrounded = false;
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (((1 << collision.gameObject.layer) & groundLayerMask.value) != 0)
        {
            isGrounded = true;
        }
    }
}
