using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SledController : MonoBehaviour
{
    [Header("Sled Settings")]
    public float moveSpeed = 5f;
    public float rotationSpeed = 100f;
    public float maxTurnAngle = 45f;
    public Transform sledCenter;

    [Header("Head Tracking")]
    public float posThreshold = 0.2f;
    public float rotThreshold = 15.0f;


    private bool leaningRight, leaningLeft, leaningForward, leaningBack;
    private Quaternion initialSledRot;
    private Rigidbody rb;
    private float curMoveSpeed;
    
    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        initialSledRot = rb.rotation;
    }

    void Update()
    {
        CaptureLeaning();
        curMoveSpeed = leaningForward ? moveSpeed * 1.5f : moveSpeed;
    }

    void FixedUpdate()
    {
        // Move the Rigidbody forward
        rb.MovePosition(rb.position + transform.forward * curMoveSpeed * Time.fixedDeltaTime);
        HandleTurning();
    }

    
    private void CaptureLeaning()
    {
        // Get the current head position (via the camera's local position)
        Vector3 currentHeadPosition = Camera.main.transform.localPosition;
        Vector3 currentSledPosition = sledCenter.localPosition;

        // Calculate the head position compared to the sled position
        Vector3 headPosOffset = currentHeadPosition - currentSledPosition;

        // Leaning Conditions w/ head rotation
        leaningRight = headPosOffset.x > posThreshold;
        leaningLeft = headPosOffset.x < -posThreshold;
        leaningForward = headPosOffset.z > posThreshold;
        leaningBack = headPosOffset.z < -posThreshold;
    }

    
    // TODO: Lerp the speed change and rotations based on the magnitude of the leaning
    //       (Bigger leans = faster turning & faster speed)

    private void HandleTurning()
    {
        float yRotation = 0f;

        // Determine rotation based on leaning
        if (leaningRight) yRotation = rotationSpeed * Time.fixedDeltaTime;
        if (leaningLeft) yRotation = -rotationSpeed * Time.fixedDeltaTime;

        // Calculate the new desired rotation
        Quaternion deltaRotation = Quaternion.Euler(0, yRotation, 0);
        Quaternion newRotation = rb.rotation * deltaRotation;

        // Convert newRotation to Euler angles
        Vector3 newRotationEuler = newRotation.eulerAngles;

        // Normalize the Y angle to be between -180 and 180
        if (newRotationEuler.y > 180) newRotationEuler.y -= 360;

        // Get the initial Y angle from initialSledRot and normalize it
        float initialY = initialSledRot.eulerAngles.y;
        if (initialY > 180) initialY -= 360;

        // Clamp the Y rotation
        newRotationEuler.y = Mathf.Clamp(newRotationEuler.y, initialY - maxTurnAngle, initialY + maxTurnAngle);

        // Apply the clamped rotation
        rb.MoveRotation(Quaternion.Euler(newRotationEuler));
    }
}
