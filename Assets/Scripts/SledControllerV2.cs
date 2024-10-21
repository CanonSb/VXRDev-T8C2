using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class SledControllerV2 : MonoBehaviour
{
    [Header("Sled Settings")]
    public float moveSpeed = 5f;
    public float extraSpeedMult = 1f;
    public float rotationSpeed = 100f;
    public float maxTurnAngle = 45f;

    [Header("Positional References")]
    public float distThreshold = 0.3f;
    public Transform[] frontRefs;
    public Transform[] leftRefs;
    public Transform[] rightRefs;

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

        StartCoroutine(WaitAndRecenter());
    }

    void Update()
    {
        CaptureLeaning();
        curMoveSpeed = leaningForward ? moveSpeed * (1 + forwardLeanMagnitude) * extraSpeedMult : moveSpeed;
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
        Vector3 currentHeadPosition = Camera.main.transform.position;

        // Get closest references and their distances
        float rightDist = GetMinDistance(currentHeadPosition, rightRefs);
        float leftDist = GetMinDistance(currentHeadPosition, leftRefs);
        float forwardDist = GetMinDistance(currentHeadPosition, frontRefs);

        // Leaning conditions with head rotation
        leaningRight = rightDist < distThreshold;
        leaningLeft = leftDist < distThreshold;
        leaningForward = forwardDist < distThreshold;

        // Calculate lean magnitudes
        if (leaningRight) sideLeanMagnitude = CalculateLeanMagnitude(rightDist);
        if (leaningLeft) sideLeanMagnitude = CalculateLeanMagnitude(leftDist);
        if (leaningForward) forwardLeanMagnitude = CalculateLeanMagnitude(forwardDist);
    }


    private void HandleTurning()
    {
        float yRotation = 0f;

        // Determine rotation based on leaning
        if (leaningRight) yRotation = rotationSpeed * sideLeanMagnitude * Time.fixedDeltaTime;
        if (leaningLeft) yRotation = -rotationSpeed * sideLeanMagnitude * Time.fixedDeltaTime;

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



    private float GetMinDistance(Vector3 headPosition, params Transform[] references)
    {
        float minDistance = float.MaxValue;
        foreach (Transform reference in references)
        {
            float distance = Vector3.Distance(headPosition, reference.position);
            if (distance < minDistance)
            {
                minDistance = distance;
            }
        }
        return minDistance; 
    }

    private float CalculateLeanMagnitude(float distance)
    {
        return Mathf.Clamp(1 - (distance / distThreshold), 0, 1);
    }
}
