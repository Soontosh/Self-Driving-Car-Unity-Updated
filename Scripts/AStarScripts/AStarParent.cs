using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;

public class AStarParent : MonoBehaviour
{
    [SerializeField] GameObject prefab, seekerPrefab, targetPrefab, selfPrefab;
    //[SerializeField] GameObject playerCapsule;
    [SerializeField] float nodeRadius;
    [SerializeField] private float stepInterval = 0.05f;
    [SerializeField] LayerMask roadLayerMask;
    //[SerializeField] Transform startingPosition;
    GameObject AStarObject;
    //private Academy academy = Academy.Instance;
    //[SerializeField] LayerMask unwalkableMask;

    private void OnEnable() {
        (Vector3 worldPos, float functionSizeX, float functionSizeY) = RoadHelper.SquareSize(0.1f, transform.position.y);
        //worldPos = Vector3.zero;
        transform.position = worldPos;
        Debug.Log("World pos: " + worldPos);
        
        AStarObject = Instantiate(prefab, worldPos, Quaternion.identity);
        AStarObject.transform.SetParent(transform);
        Grid AStarGridScript = AStarObject.GetComponent<Grid>();
        AStarGridScript.player = seekerPrefab.transform;
        AStarGridScript.nodeRadius = nodeRadius;
        AStarObject.SetActive(true);
        

        GameObject targetObject = Instantiate(targetPrefab, Visualizer.targetPosition, Quaternion.identity);
        targetObject.transform.SetParent(transform);
        targetObject.SetActive(true);

        GameObject seekerObject = Instantiate(seekerPrefab, new Vector3(0, 0, 0.5f), Quaternion.identity);
        seekerObject.transform.SetParent(transform);
        //seekerObject.GetComponent<AgentScript>().StartingPosition = startingPosition;
        seekerObject.GetComponent<Unit>().target = targetObject.transform;
        seekerObject.GetComponent<Unit>().roadIntersectionLayerMask = roadLayerMask;
        seekerObject.SetActive(true);
    }

    /*public void ResetEpisode() {
        //CallLast
        
        GameObject newMe = Instantiate(selfPrefab, Vector3.zero, Quaternion.identity);
        AStarParent aStarParent = newMe.GetComponent<AStarParent>();
        Destroy(AStarObject);
        //aStarParent.prefab = prefab;
        //aStarParent.seekerPrefab = seekerPrefab;
        //aStarParent.targetPrefab = seekerPrefab;
        //aStarParent.selfPrefab = selfPrefab;
        //aStarParent.nodeRadius = nodeRadius;
        //aStarParent.stepInterval = stepInterval;
        //aStarParent.roadLayerMask = roadLayerMask;
        //aStarParent.startingPosition = startingPosition;
        Destroy(this);
        
    }*/
}
