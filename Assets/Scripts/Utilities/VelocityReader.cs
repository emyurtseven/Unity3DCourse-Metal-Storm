using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VelocityReader : MonoBehaviour
{
    [SerializeField] bool debugMode = false;

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

    // int skipCheckCount = 3;
    // int framesElapsed = 0;

    List<string> speedValues = new List<string>();

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
        ReadSpeedFixedUpdate();
    }

    private void ReadSpeedFixedUpdate()
    {
        newPos = transform.position;  // each frame track the new position
        currentVelocity = (newPos - prevPos) / Time.fixedDeltaTime;
        prevPos = newPos;  // update position for next frame calculation

        totalVelocity += currentVelocity;
        elapsedTime += Time.fixedDeltaTime;
        counter++;

        if (counter == samples)
        {
            averageVelocity = totalVelocity / samples;
            // Debug.Log("Average Speed: " + averageVelocity.magnitude);

            if (debugMode)
            {
                string data = $"{transform.position.ToString()} - {averageVelocity.magnitude.ToString()}";
                speedValues.Add(data);
            }

            acceleration = (averageVelocity - prevVelocity) / elapsedTime;
            prevVelocity = averageVelocity;

            elapsedTime = 0;
            totalVelocity = Vector3.zero;
            counter = 0;

            // Debug.Log("Acceleration: " + acceleration);
        }
    }

    private void OnDisable() 
    {
        if (debugMode)
        {
            LogToFile(speedValues);
        }
    }

    void LogToFile(List<string> values)
    {
        // Write to disk
        string fileName = "speedValues.txt";
        
        StreamWriter writer = new System.IO.StreamWriter(@"D:\Downloads\" + fileName, true);

        foreach (string value in values)
        {
            writer.WriteLine(value);
        }

        writer.Close();
    }
}
