using System.Collections;
using System.Collections.Generic;
using Unity.XR.CoreUtils;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.XR;

public class AttachPlayerToSled : MonoBehaviour
{
    public Transform head;
    public XROrigin xrOrigin;
    public Transform target; 
    public Transform posReference;

    public InputActionProperty recenterButton;

    void Start()
    {

    }

    void Update()
    {
        if (recenterButton.action.WasPressedThisFrame())
        {
            RecenterEasy();
        }
    }

    public void RecenterEasy()
    {
        print("doing things");
        xrOrigin.MoveCameraToWorldLocation(target.position);
        // xrOrigin.MatchOriginUpCameraForward(target.up, target.forward);
        posReference.localPosition = Camera.main.transform.localPosition;

        // xrOrigin.transform.position = target.position;
        // print(Camera.main.transform.localPosition - target.localPosition);

    }

    public void RecenterComplicated()
    {

        print("doing things");
        Vector3 offset = head.position - xrOrigin.transform.position;
        offset.y = 0;
        xrOrigin.transform.position = target.position - offset;

        Vector3 targetForward = target.forward;
        targetForward.y = 0;
        Vector3 cameraForward = head.forward;
        cameraForward.y = 0;

        float angle = Vector3.SignedAngle(cameraForward, targetForward, Vector3.up);

        xrOrigin.transform.RotateAround(head.position, Vector3.up, angle);
        posReference.localPosition = Camera.main.transform.localPosition;
    }
}
