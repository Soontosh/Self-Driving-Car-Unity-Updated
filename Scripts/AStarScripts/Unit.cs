using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Unit : MonoBehaviour
{
    
    [SerializeField] Controller carController;
    [SerializeField] public LayerMask roadIntersectionLayerMask;
    [SerializeField] private GameObject carObject;
    const float minPathUpdateTime = 0.2f;
    private const float pathUpdateMoveThreshold = .5f;
    public Transform target;
    public float speed = 1f;
    public float turnSpeed = 3f;
    public float turnDst = 5f;
    public float stoppingDistance = 10f;
    Path path;
    public bool followingPath = false;
    int pathIndex = 0;
    //transform.LookAt(path.lookPoints[0]);
    float speedPercent = 1f;
    public Quaternion targetRotation;
    bool breaking;

    void OnEnable() {
        //transform.position = Vector3.zero;
        StartCoroutine("BeginUpdates");
    }

    public void OnPathFound(Vector3[] waypoints, bool pathSuccessful) {
        if (pathSuccessful) {
            path = new Path(waypoints, transform.position, turnDst, stoppingDistance);
            followingPath = true;
            CheckpointManager.CreateCheckpoints(path.lookPoints, carObject, roadIntersectionLayerMask);
            //carController.BeginPathfinding();
        }
    }

    public (float, float, bool, Quaternion, Vector3) FollowPath() {
       
        Vector2 pos2D = new Vector2(transform.position.x, transform.position.z);
        Debug.Log("Path Location: " + path.turnBoundaries[pathIndex]);
        if (path.turnBoundaries[pathIndex].HasCrossedLine(pos2D)) {
            if (pathIndex == path.finishLineIndex) {
                followingPath = false;
            } else {
                pathIndex++;
            }
        }

        if (followingPath) {
            if(pathIndex >= path.slowDownIndex && stoppingDistance > 0) {
                
                speedPercent = Mathf.Clamp01(path.turnBoundaries[path.finishLineIndex].DistanceFromPoint(pos2D) / stoppingDistance);
                if (speedPercent < 0.999f) breaking = true;
                if (speedPercent < 0.01) {
                    followingPath = false;
                }
            } else {
                breaking = false;
            }

            targetRotation = Quaternion.LookRotation(path.lookPoints[pathIndex] - transform.position);
            //transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, Time.deltaTime * turnSpeed);
            //transform.Translate(Vector3.forward * Time.deltaTime * speed * speedPercent, Space.Self);
            float angleDifference = Mathf.DeltaAngle(transform.eulerAngles.y, targetRotation.eulerAngles.y);
            //Convert the transform angle to quaternion?
            return (angleDifference, speedPercent, breaking, targetRotation, path.lookPoints[pathIndex]);
        }
        return (0f, 0f, false, Quaternion.identity, Vector3.zero);
    }

    IEnumerator UpdatePath() {
        if(Time.timeSinceLevelLoad < .3f) {
            yield return new WaitForSeconds(.3f - Time.timeSinceLevelLoad + .1f);
        }

        PathRequestManager.RequestPath(new PathRequest(transform.position, target.position, OnPathFound));
        float sqrMoveThreshold = pathUpdateMoveThreshold * pathUpdateMoveThreshold;
        Vector3 targetPosOld = target.position;

        while (true) {
            yield return new WaitForSeconds(minPathUpdateTime);
            if((target.position - targetPosOld).sqrMagnitude > sqrMoveThreshold) {
                PathRequestManager.RequestPath(new PathRequest(transform.position, target.position, OnPathFound));
                targetPosOld = target.position;
            }
        }
    }

    IEnumerator BeginUpdates() {
        yield return new WaitForSeconds(0.1f);

        if (Grid.gridGenerated == true) {
            StartCoroutine("UpdatePath");
            yield break;
        }
        
    }

    private void OnDrawGizmos() {
        if (path != null) {
            path.DrawWithGizmos();
        }
    }
}
