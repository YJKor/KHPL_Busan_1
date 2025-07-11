using UnityEngine.InputSystem;
using UnityEngine;

public class XRInputBridge : MonoBehaviour
{
    public static XRInputBridge Instance { get; private set; }

    public InputActionReference moveActionReference;

    public InputActionReference simulatorMoveActionReference;

    public Vector2 MoveInput { get; private set; }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Update()
    {
        Vector2 xrMoveInput = moveActionReference.action.ReadValue<Vector2>();

        if (xrMoveInput.sqrMagnitude > 0.01f)
        {
            MoveInput = xrMoveInput;
        }
        else
        {
            MoveInput = simulatorMoveActionReference.action.ReadValue<Vector2>();
        }
        Debug.Log($"Bridge Raw Input: {MoveInput}");
    }
}