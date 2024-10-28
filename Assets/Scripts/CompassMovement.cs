using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CompassMovement : MonoBehaviour
{
    public GameObject sled;
    public GameObject compass;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        // Move compass pointer rotation according to sled rotation
        compass.transform.localRotation = Quaternion.Euler(0, 0, sled.transform.eulerAngles.y);
    }
}
