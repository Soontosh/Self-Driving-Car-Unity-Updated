using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class CheckpointManager
{
    //Run RaycastAll Again, if it hits a checkpoint, give it a layer of checkpointLayer variable.
    //Maybe lookpoints or turnboundaries for parameter fix later

    public static void CreateCheckpoints(Vector3[] lookPointsArray, GameObject carObject, LayerMask intersectionLayerMask) {
        List<Vector3> lookPoints = new List<Vector3>(lookPointsArray);
        //Next item in list
        Vector3 nextPoint;
        //Current Iteration
        int lookPointIndex = 0;
        //Add carobject position to start of list
        lookPoints.Insert(0, carObject.transform.position);
        Vector3 lookPoint;

        foreach(Vector3 lookPointItem in lookPoints) {
            lookPoint = lookPointItem;
            try {
                nextPoint = lookPoints[lookPointIndex + 1];
                Debug.Log("next point: " + nextPoint);
            } catch {
                Debug.Log("OH NAHHHH");
                break;
            }
            RaycastHit[] hits;
            lookPoint = new Vector3(lookPoint.x, 0f, lookPoint.z);
            nextPoint = new Vector3(nextPoint.x, 0f, nextPoint.z);
            hits = Physics.RaycastAll(lookPoint, nextPoint - lookPoint, Vector3.Distance(lookPoint, nextPoint), intersectionLayerMask);
            Debug.DrawLine(lookPoint, nextPoint, Color.red, 9999);
            Debug.Log("Look Point: " + lookPoint + " nextPoint: " + nextPoint);

            foreach(RaycastHit hit in hits) {
                GameObject hitObject = hit.collider.transform.parent.gameObject;
                Debug.Log("Hit! Home run!");
                Debug.Log("Hit! Child Count: " + hitObject.transform.childCount);
                foreach(Transform child in hitObject.transform) {
                    Debug.Log("children created");
                    Debug.Log("Trans Name: " + child.name);
                    if (child.gameObject.CompareTag("checkpoint")) {
                        child.gameObject.SetActive(true);
                        Debug.Log("MiniClip! Grandchild Count: " + child.childCount);
                        Debug.Log("MiniClip! Child Name: " + child.name);
                        foreach(Transform grandChild in child) {
                            grandChild.gameObject.tag = "enabled";
                            Debug.Log("MiniClip! GrandChild!");
                        }
                        Debug.Log("Checkpoint!");
                    }
                }
            }
        }
    }
}
