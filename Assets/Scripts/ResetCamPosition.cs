using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;


// TODO: This needs testing. If this doesn't work we need some way to recenter the camera on start

public class ResetCamPosition : MonoBehaviour
{
    public Transform cameraRig; // Reference to the XR Rig (the parent of the camera)
    public Transform cameraTransform; // Reference to the XR Camera (the actual camera)
    public Vector3 initialPosition; // The starting position of the XR Rig
    public Quaternion initialRotation; // The starting rotation of the XR Rig
    private XRInputSubsystem xrInputSubsystem;

    void Start()
    {
        // Get the XRInputSubsystem to access XR tracking features
        var xrInputSubsystems = new List<XRInputSubsystem>();
        SubsystemManager.GetInstances(xrInputSubsystems);

        if (xrInputSubsystems.Count > 0)
        {
            xrInputSubsystem = xrInputSubsystems[0];
        }

        // Store the initial position and rotation of the camera rig
        initialPosition = cameraRig.position;
        initialRotation = cameraRig.rotation;

        // Recenter the XR Camera at the start
        ResetCameraPosition();
    }

    void ResetCameraPosition()
    {
        // Try to recenter the XR headset's position and orientation
        if (xrInputSubsystem != null)
        {
            xrInputSubsystem.TryRecenter();
        }

        // Reset the camera rig to the initial position and rotation
        cameraRig.position = initialPosition;
        cameraRig.rotation = initialRotation;
    }
}
