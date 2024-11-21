using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AvalancheMovement : MonoBehaviour
{
    // avalanche will move in the +z and -y direction
    // since slope is pointed down 30 degrees, the avalanche will move down the slope
    public float duration = 25f; // Duration of the avalanche in seconds
    public float speed = 10f; // Speed of the avalanche
    public float startDelay = 2f; // Delay before the avalanche starts

    public Transform sled; // Reference to the sled
    private Vector3 movementDirection;


    // Start is called before the first frame update
    void Start()
    {
        float angle = -45f * Mathf.Deg2Rad;
        movementDirection = new Vector3(0, -Mathf.Sin(angle), Mathf.Cos(angle)).normalized;
        //StartCoroutine(ReportDistance());
        StartCoroutine(MoveAvalanche());
    }

    // report distance of avalanche to sled to console every 0.1 seconds
    // IEnumerator ReportDistance()
    // {
    //     while (true)
    //     {
    //         Debug.Log(Vector3.Distance(transform.position, sled.position));
    //         yield return new WaitForSeconds(0.1f);
    //     }
    // }
    IEnumerator MoveAvalanche()
    {
        yield return new WaitForSeconds(startDelay);
        float startTime = Time.time;
        while (Time.time - startTime < duration)
        {
            transform.Translate(movementDirection * speed * Time.deltaTime);
            yield return null;
        }
        Destroy(gameObject);
    }
}
