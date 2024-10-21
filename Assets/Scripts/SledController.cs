using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class SledController : MonoBehaviour
{
    [Header("Sled Settings")]
    public float moveSpeed = 5f;
    public float extraSpeedMult = 1f;
    public float rotationSpeed = 100f;
    public float maxTurnAngle = 45f;

    [Header("Head Tracking")]
    public float posThreshold = 0.2f;
    public float rotThreshold = 15.0f;
    public Transform camCenter;

    private float sideLeanMagnitude;
    private float forwardLeanMagnitude;

    private bool leaningRight, leaningLeft, leaningForward, leaningBack;
    private Quaternion initialSledRot;
    private Rigidbody rb;

    private float curMoveSpeed;

    public AttachPlayerToSled attachToSled;
    
    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        initialSledRot = rb.rotation;

        // StartCoroutine(WaitAndRecenter());
    }

    void Update()
    {
        CaptureLeaning();
        curMoveSpeed = leaningForward ? moveSpeed * forwardLeanMagnitude * extraSpeedMult : moveSpeed;
    }

    void FixedUpdate()
    {
        // Move the Rigidbody forward
        rb.MovePosition(rb.position + transform.forward * curMoveSpeed * Time.fixedDeltaTime);
        HandleTurning();
    }

    
    private void CaptureLeaning()
    {
        // Get the current head position
        Vector3 currentHeadPosition = Camera.main.transform.localPosition;
        Vector3 currentSledPosition = camCenter.localPosition;

        // Calculate the head position compared to the sled position
        Vector3 headPosOffset = currentHeadPosition - currentSledPosition;

        // print(headPosOffset.x);

        // Leaning Conditions w/ head rotation
        leaningRight = headPosOffset.x > posThreshold;
        leaningLeft = headPosOffset.x < -posThreshold;
        leaningForward = headPosOffset.z > posThreshold;
        leaningBack = headPosOffset.z < -posThreshold;


        // Determine side lean magnitudes
        float maxLeanOffset = 0.3f;
        if (leaningRight) sideLeanMagnitude = Mathf.Clamp((headPosOffset.x - posThreshold) / maxLeanOffset, -1, 1);
        if (leaningLeft) sideLeanMagnitude = Mathf.Clamp((headPosOffset.x + posThreshold) / maxLeanOffset, -1, 1);

        if (leaningForward) forwardLeanMagnitude = Mathf.Clamp((headPosOffset.z - posThreshold) / maxLeanOffset + 1, 1, 2);
    }


    private void HandleTurning()
    {
        float yRotation = 0f;

        // Determine rotation based on leaning
        if (leaningRight || leaningLeft) yRotation = rotationSpeed * sideLeanMagnitude * Time.fixedDeltaTime;

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


    private IEnumerator WaitAndRecenter()
    {
        yield return new WaitForSeconds(1f);
        attachToSled.RecenterEasy();
    }
}
