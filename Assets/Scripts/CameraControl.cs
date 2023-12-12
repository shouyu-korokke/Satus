using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraControl : MonoBehaviour
{
    
    public float movementSpeed = 0.01f;
    public float rotationSpeed = 100f;

    void FixedUpdate()
    {
        // Horizontal movement
        float horizontalMovement = 0f;
        float verticalMovement = 0f;

        if (Input.GetKey(KeyCode.W))
            verticalMovement = 1f;
        if (Input.GetKey(KeyCode.S))
            verticalMovement = -1f;
        if (Input.GetKey(KeyCode.A))
            horizontalMovement = -1f;
        if (Input.GetKey(KeyCode.D))
            horizontalMovement = 1f;

        Vector3 movement = new Vector3(horizontalMovement, 0f, verticalMovement) * movementSpeed * Time.fixedDeltaTime;
        transform.Translate(movement, Space.Self);

        // Horizontal rotation
        float horizontalRotation = 0f;
        if (Input.GetKey(KeyCode.Q))
            horizontalRotation = -1f;
        if (Input.GetKey(KeyCode.E))
            horizontalRotation = 1f;

        transform.Rotate(Vector3.up, horizontalRotation * rotationSpeed * Time.fixedDeltaTime, Space.World);

        // Vertical rotation
        float verticalRotation = 0f;
        if (Input.GetKey(KeyCode.R))
            verticalRotation = 1f;
        if (Input.GetKey(KeyCode.F))
            verticalRotation = -1f;

        transform.Rotate(Vector3.right, verticalRotation * rotationSpeed * Time.fixedDeltaTime, Space.Self);
    }
}
