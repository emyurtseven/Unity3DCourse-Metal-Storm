using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;


public class PlayerController : MonoBehaviour
{
    [Tooltip("Smaller number means snappier movement, larger means more 'slidy'")]
    [SerializeField] float smoothInputDelay = 0.15f;
    [SerializeField] float steerSpeedMultiplier = 1f;
    [SerializeField] float aircraftRollAngle = 20f;

    [SerializeField] float horizontalMovementPadding = 10f;
    [SerializeField] float verticalMovementPadding = 5f;

    float horizontalMovementRange;
    float verticalMovementRange;

    Vector2 inputVector;
    Vector2 smoothedInputVector;
    Vector2 smoothInputVelocity;  // REQUIRED for SmoothDamp function, unused otherwise

    void Start()
    {
        horizontalMovementRange = Mathf.Abs(ScreenUtils.ScreenRight - ScreenUtils.ScreenLeft) / 2;
        horizontalMovementRange -= horizontalMovementPadding;
        verticalMovementRange = Mathf.Abs(ScreenUtils.ScreenTop - ScreenUtils.ScreenBottom) / 2;
        verticalMovementRange -= verticalMovementPadding;
    }

    void Update()
    {
        SteerAircraft();
    }

    private void SteerAircraft()
    {
        SmoothInputVector();

        Vector3 movementVector = new Vector3(smoothedInputVector.x, smoothedInputVector.y, 0f);

        // Add movement vector to local position to translate vertically and horizontally
        transform.localPosition += movementVector;

        // Rotate aircraft on z axis to simulate rolling while steering horizontally
        // movementVector.x is multiplied by -1 to fix inverse rotation
        transform.localRotation = Quaternion.Euler(0, 0, aircraftRollAngle * -movementVector.x);

        float clampedXPos = Mathf.Clamp(transform.localPosition.x, -horizontalMovementRange, horizontalMovementRange);
        float clampedYPos = Mathf.Clamp(transform.localPosition.y, -verticalMovementRange, verticalMovementRange);

        transform.localPosition = new Vector3(clampedXPos, clampedYPos, transform.localPosition.z);
    }

    private void SmoothInputVector()
    {
        // Use SmoothDamp to smooth the input value from 0 to 1 or 0 to -1, instead of getting 
        // discrete integer values. smoothInputVelocity is a required but unused value, declared above.
        smoothedInputVector = Vector2.SmoothDamp(smoothedInputVector, inputVector,
                                                    ref smoothInputVelocity, smoothInputDelay);

        // Clamp the value to 0 if it's below a threshold
        if (Mathf.Abs(smoothedInputVector.magnitude) < 0.01f)
        {
            smoothedInputVector = Vector2.zero;
        }

        smoothedInputVector *= steerSpeedMultiplier;
    }

    void OnMove(InputValue value)
    {
        inputVector = value.Get<Vector2>();
    }

    
}
