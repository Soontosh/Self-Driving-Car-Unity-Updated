using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;


public class AgentScript : Agent
{
    //0 = Straight, 1 = ThreeWay, 2 = FourWay, 3 = Curve, 4 = Dead End
    [SerializeField] public Transform StartingPosition;
    [SerializeField] private WheelCollider wheelCollider;
    [SerializeField] private Controller carController;
    [SerializeField] Unit unitScript;
    [SerializeField] private float rewardMultiplier = 0.05f;
    [SerializeField] private float maxDotReward = 10f;
    //[SerializeField] GameObject straightRoad, threeWayRoad, fourWayRoad, curvedRoad, deadEnd;
    [SerializeField] LayerMask unwalkableMask;
    
    Dictionary<GameObject, int> roadObservationDict;
    public static GameObject startingRoad;
    public static GameObject targetRoad;
    private Rigidbody rb;
    float dotProduct;
    private float totalDotReward;
    private float enterTime;

    private void Start() {
        rb = GetComponent<Rigidbody>();
        /*roadObservationDict = new Dictionary<GameObject, int>() {
            {straightRoad, 0},
            {threeWayRoad, 1},
            {fourWayRoad, 2},
            {curvedRoad, 3},
            {deadEnd, 4},
        };*/
    }

    
    

    private void Update() {
        if(StepCount >= 4900) {
            AddReward(-10);
        }
    }
    
    public override void OnEpisodeBegin() {
        //transform.position = StartingPosition.position;
        //transform.forward = StartingPosition.forward;
        GetComponent<Rigidbody>().isKinematic = true;
        GetComponent<Rigidbody>().isKinematic = false;
        //Debug.Log("Target Road Location: " + targetRoad.transform.position);
        //Debug.Log("Starting Road Location: " + startingRoad.transform.position);
        //NavLine.endPoint = targetRoad.transform.position;
        Debug.Log("atleast the episode began...");
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        Debug.Log("atleast the observations began...");
        float steeringAngle = wheelCollider.steerAngle;
        float normalizedSteeringAngle = steeringAngle / carController.maxSteeringAngle;
        Quaternion targetRotation;
        float filler;
        bool boolFiller;
        Vector3 targetPosition;
        (filler, filler, boolFiller, targetRotation, targetPosition) = unitScript.FollowPath();
        dotProduct = Quaternion.Dot(transform.rotation, targetRotation);
        int roadNumber = 0;

        RaycastHit hit;
        if (Physics.Raycast(gameObject.transform.position, Vector3.down, out hit, 2)) {
            GameObject groundObject = hit.collider.gameObject;
            roadNumber = roadObservationDict[groundObject];
        }

        //Normalized Steering Angle
        sensor.AddObservation(normalizedSteeringAngle);
        //X and Z Velocity
        sensor.AddObservation(rb.velocity.x);
        sensor.AddObservation(rb.velocity.z);
        //Speed
        sensor.AddObservation(rb.velocity.magnitude);
        //Angular Velocity
        sensor.AddObservation(rb.angularVelocity);
        //Dot Product Between Current and Next Checkpoint
        sensor.AddObservation(dotProduct);
        //Position of Current Target
        sensor.AddObservation(targetPosition);
        //Position of Car
        sensor.AddObservation(transform.position);
        //Road Under Car
        sensor.AddObservation(roadNumber);



        Debug.Log("atleast the observations began...");
        //Add acceleration as observation, next node location as an observation, the current forward, and the desired forward to reach the next node
    }

    public override void OnActionReceived(ActionBuffers actions) {
        Debug.Log("atleast the actions began...");
        float forwardNum = actions.ContinuousActions[1];
        float steerInput = actions.ContinuousActions[0];
        bool brakeBool = System.Convert.ToBoolean(actions.DiscreteActions[0]);
        float dotReward = 0;
        

        Debug.Log(actions.DiscreteActions[0]);
        carController.verticalInput = forwardNum;
        carController.horizontalInput = steerInput;
        carController.isBreaking = brakeBool;
        carController.AIUpdate();
        Debug.Log("Forward Num: " + forwardNum + "; Steer Input: " + steerInput + "; Brake Bool: " + brakeBool + ";");

        if(totalDotReward < maxDotReward) dotReward = Mathf.Clamp(1f - dotProduct, 0f, 1f) * rewardMultiplier;
        totalDotReward += dotReward;
        AddReward(dotReward);
        Debug.Log("Dot Reward: " + dotReward);
    }

    public override void Heuristic(in ActionBuffers actionsOut) {
        Debug.Log("atleast Heuristic Success");
        float angleDifference;
        float verticalInput;
        bool isBreaking;
        Quaternion filler;
        Vector3 filler2;
        (angleDifference, verticalInput, isBreaking, filler, filler2) = unitScript.FollowPath();
        float horizontalInput = Mathf.Clamp(angleDifference / carController.maxSteeringAngle, -1f, 1f);
        if(Mathf.Abs(horizontalInput) >= 0.2) {
            //verticalInput/=Mathf.Abs(horizontalInput/2f);
        }

        ActionSegment<float> continuousActions = actionsOut.ContinuousActions;
        ActionSegment<int> discreteActions = actionsOut.DiscreteActions;

        Debug.Log("First Vert Input: " + verticalInput);
        Debug.Log("First Horizontal Input: " + horizontalInput);
        continuousActions[0] = horizontalInput;
        continuousActions[1] = verticalInput;
        discreteActions[0] = System.Convert.ToInt32(isBreaking);
        Debug.Log("atleast Heuristic Success");
    }

    
    private void OnTriggerEnter(Collider other) {
        if(other.tag == "enabled") {
            //Checkpoint
            AddReward(1f);
            Destroy(other.gameObject);
        } else if(other.tag == "target") {
            enterTime = Time.time;
        }
    }

    private void OnTriggerStay(Collider other) {
        if(other.tag == "target") {
            float duration = Time.time - enterTime;
            AddReward(duration/10);
            if(duration >= 2.5f) {
                AddReward(10);
                //EndEpisode();
            }
        }
    }

    private void OnCollisionEnter(Collision other) {
        if(other.gameObject.layer == unwalkableMask) {
            AddReward(-10);
            //EndEpisode();
        }
    }
    
    
}
