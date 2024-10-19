using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;

public class SledSteering : MonoBehaviour
{
    public float posThreshold = 0.2f;
    public float rotThreshold = 15.0f;

    private Vector3 initialHeadPosition;
    private Quaternion initialHeadRotation;

    public Material onMat;
    public Material offMat;

    public Renderer forwardCube;
    public Renderer leftCube;
    public Renderer rightCube;
    public Renderer backwardCube;

    void Start()
    {
        // Assume the starting position is the reference for "centered"
        initialHeadPosition = Camera.main.transform.localPosition;
        initialHeadRotation = Camera.main.transform.localRotation;
    }

    void Update()
    {
        HandleSteering();
    }


    private void HandleSteering()
    {
        // Get the current head position (via the camera's local position)
        Vector3 currentHeadPosition = Camera.main.transform.localPosition;
        // Quaternion currentHeadRotation = Camera.main.transform.localRotation;

        Vector3 headPosOffset = currentHeadPosition - initialHeadPosition;

        // Leaning Conditions w/ head rotation
        bool leaningRight = headPosOffset.x > posThreshold;
        bool leaningLeft = headPosOffset.x < -posThreshold;
        bool leaningForward = headPosOffset.z > posThreshold;
        bool leaningBackward = headPosOffset.z < -posThreshold;

        // // Leaning Conditions w/ head rotation
        // // Calculate rotation differences
        // Vector3 headRotDiff = currentHeadRotation.eulerAngles - initialHeadRotation.eulerAngles;
        // headRotDiff = NormalizeAngles(headRotDiff);  // Normalize the angles for comparison

        // bool leaningRight = headPosOffset.x > posThreshold && headRotDiff.z < -rotThreshold;
        // bool leaningLeft = headPosOffset.x < -posThreshold && headRotDiff.z > rotThreshold;
        // bool leaningForward = headPosOffset.z > posThreshold && headRotDiff.x > rotThreshold;
        // bool leaningBackward = headPosOffset.z < -posThreshold && headRotDiff.x < -rotThreshold;

        // Change cube colors
        rightCube.material = leaningRight ? onMat : offMat;
        leftCube.material = leaningLeft ? onMat : offMat;
        forwardCube.material = leaningForward ? onMat : offMat;
        backwardCube.material = leaningBackward ? onMat : offMat;
    }


    // Normalize rotation angles to the range [-180, 180]
    Vector3 NormalizeAngles(Vector3 angles)
    {
        angles.x = (angles.x > 180) ? angles.x - 360 : angles.x;
        angles.y = (angles.y > 180) ? angles.y - 360 : angles.y;
        angles.z = (angles.z > 180) ? angles.z - 360 : angles.z;
        return angles;
    }

}
