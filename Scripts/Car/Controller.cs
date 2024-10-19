using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Controller : MonoBehaviour
{
    [SerializeField] float maxSpeed;
    [SerializeField] float reverseSpeedMultiplier;
    [SerializeField] float carOffset;
    [SerializeField] bool isHuman;
    [SerializeField] Unit unitScript;
    public float horizontalInput;
    public float verticalInput;
    public float steerAngle;
    public bool isBreaking;

    public WheelCollider frontLeftWheelCollider;
    public WheelCollider frontRightWheelCollider;
    public WheelCollider rearLeftWheelCollider;
    public WheelCollider rearRightWheelCollider;
    public Transform frontLeftWheelTransform;
    public Transform frontRightWheelTransform;
    public Transform rearLeftWheelTransform;
    public Transform rearRightWheelTransform;
    private float angleDifference;
    public float maxSteeringAngle = 45f;
    public float minSteeringAngle = 5f;
    public float motorForce = 50f;
    public float brakeForce = 0f;
    private bool restarted = false;
    private float rotationThreshold = 1f;

    public void BeginPathfinding() {
        GetComponent<Rigidbody>().centerOfMass = new Vector3 (0, 0, 0);
    }


    public void AIUpdate()
    {
        GetInput();
        HandleMotor();
        HandleSteering();
        UpdateWheels();
    }

    private void GetInput()
    {
        if (isHuman) {
            
            horizontalInput = Input.GetAxis("Horizontal");
            verticalInput = Input.GetAxis("Vertical");
            isBreaking = Input.GetKey(KeyCode.Space);
        }
    }

    private void HandleSteering()
    {
        float forwardSpeed = GetComponent<Rigidbody>().velocity.magnitude;
        steerAngle = maxSteeringAngle * horizontalInput;
        steerAngle *= Mathf.Lerp(0.4f, 1f, forwardSpeed / 20f);
        if (horizontalInput == 0) steerAngle = 0;
        frontLeftWheelCollider.steerAngle = steerAngle;
        frontRightWheelCollider.steerAngle = steerAngle;
        //transform.Rotate(0,steerAngle,0);
        //Debug.Log(horizontalInput);
        
    }

    private void HandleMotor()
    {
        frontLeftWheelCollider.motorTorque = Mathf.Clamp(verticalInput * motorForce, (reverseSpeedMultiplier * -maxSpeed), maxSpeed);
        frontRightWheelCollider.motorTorque = Mathf.Clamp(verticalInput * motorForce, (reverseSpeedMultiplier * -maxSpeed), maxSpeed);

        brakeForce = isBreaking ? 500f : 0f;
        frontLeftWheelCollider.brakeTorque = brakeForce;
        frontRightWheelCollider.brakeTorque = brakeForce;
        rearLeftWheelCollider.brakeTorque = brakeForce;
        rearRightWheelCollider.brakeTorque = brakeForce;
    }

    private void UpdateWheels()
    {
        UpdateWheelPos(frontLeftWheelCollider, frontLeftWheelTransform);
        UpdateWheelPos(frontRightWheelCollider, frontRightWheelTransform);
        UpdateWheelPos(rearLeftWheelCollider, rearLeftWheelTransform);
        UpdateWheelPos(rearRightWheelCollider, rearRightWheelTransform);
    }

    private void UpdateWheelPos(WheelCollider wheelCollider, Transform trans)
    {
        Vector3 pos;
        Quaternion rot;
        wheelCollider.GetWorldPose(out pos, out rot);
        trans.rotation = rot;
        trans.position = pos;
    }

}