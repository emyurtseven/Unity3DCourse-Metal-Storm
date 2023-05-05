using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VelocityReader : MonoBehaviour
{
    Vector3 prevPos;
    Vector3 newPos;

    Vector3 currentVelocity;
    Vector3 prevVelocity;
    Vector3 acceleration;
    
    Vector3 averageVelocity;
    Vector3 totalVelocity;

    [SerializeField] int samples = 10;
    int counter = 0;
    float elapsedTime = 0;

    public Vector3 AverageVelocity { get => averageVelocity; set => averageVelocity = value; }
    public Vector3 Acceleration { get => acceleration; set => acceleration = value; }

    // Use this for initialization
    void Start()
    {
        prevPos = transform.position;
        prevVelocity = Vector3.zero;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        newPos = transform.position;  // each frame track the new position
        currentVelocity = (newPos - prevPos) / Time.deltaTime;
        prevPos = newPos;  // update position for next frame calculation

        totalVelocity += currentVelocity;
        elapsedTime += Time.deltaTime;
        counter++;

        if (counter == samples)
        {
            averageVelocity = totalVelocity / samples;
            // Debug.Log("Average Velocity: " + averageVelocity);

            acceleration = (averageVelocity - prevVelocity) / elapsedTime;
            prevVelocity = averageVelocity;

            elapsedTime = 0;
            totalVelocity = Vector3.zero;
            counter = 0;

            // Debug.Log("Acceleration: " + acceleration);
        }
    }
}
