using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grid : MonoBehaviour
{
    public bool displayGridGizmos;
    public static bool gridGenerated = false;
    public Transform player;
    public LayerMask unwalkableMask;
    public Vector2 gridWorldSize;
    public float nodeRadius;
    Node[,] grid;
    private float nodeDiameter;
    private int gridSizeX, gridSizeY;

    private void OnEnable() {
        nodeDiameter = nodeRadius * 2;
        //gridSizeX = Mathf.CeilToInt(gridWorldSize.x / nodeDiameter);
        //gridSizeY = Mathf.CeilToInt(gridWorldSize.y / nodeDiameter);
        //transform.position = Vector3.zero;
        //transform.localPosition = Vector3.zero;
        Debug.Log("Transform.Position before CreateGrid: " + transform.position);
        CreateGrid();
        Debug.Log("Transform.Position after CreateGrid: " + transform.position);
    }

    private void CreateGrid() {
        (Vector3 worldPos, float functionSizeX, float functionSizeY) = RoadHelper.SquareSize(nodeDiameter, transform.parent.transform.position.y);
        //transform.position = new Vector3(gameObject.transform.position.x, gameObject.transform.position.y, gameObject.transform.position.z + 1);
        //
        //objectTransform.position = worldPos;
        //gameObject.transform.position = worldPos;
        //functionSizeX += 3;
        Debug.Log("Hwey: " + RoadHelper.clearedRoads);
        gridSizeX = Mathf.CeilToInt(functionSizeX / nodeDiameter);
        gridSizeY = Mathf.CeilToInt(functionSizeY / nodeDiameter);
        gridSizeX += 3;
        gridSizeY += 3;
        gridWorldSize = new Vector2(functionSizeX, functionSizeY);
        grid = new Node[gridSizeX, gridSizeY];
        //transform.position = Vector3.zero;
        Debug.Log("Transform.Position before WorldBottomLeft: " + transform.position);
        Vector3 worldBottomLeft = transform.position - Vector3.right * gridWorldSize.x/2 - Vector3.forward * gridWorldSize.y/2;
        Debug.Log("Transform.Position after WorldBottomLeft: " + transform.position);

        for (int x = 0; x < gridSizeX; x++) {
            for (int y = 0; y < gridSizeY; y++) {
                Vector3 worldPoint = worldBottomLeft + Vector3.right * (x * nodeDiameter + nodeRadius) + Vector3.forward * (y * nodeDiameter + nodeRadius);
                bool walkable = !(Physics.CheckSphere(worldPoint, nodeRadius, unwalkableMask));
                grid[x, y] = new Node(walkable, worldPoint, x, y);
                //Debug.Log("World Point Check; X Number: " + x + " Y Number: " + y + "; WorldPosition: " + worldPoint);
                //Debug.Log("One Loop");
            }
        }
        gridGenerated = true;
        Debug.Log("Transform.Position after Grid Generated: " + transform.position);
    }

    public List<Node> GetNeighbors(Node node) {
        List<Node> neighbours = new List<Node>();
        
        for (int x = -1; x <= 1; x++) {
            for (int y = -1; y <= 1; y++) {
                if (x == 0 && y == 0) continue;

                int checkX = node.gridX + x;
                int checkY = node.gridY + y;

                if (checkX >= 0 && checkX < gridSizeX && checkY >= 0 && checkY < gridSizeY) {
                    neighbours.Add(grid[checkX, checkY]);
                }
            }
        }

        return neighbours;
    }

    public Node NodeFromWorldPoint(Vector3 worldPosition) {
        //Make world position relative to parent position
        Debug.Log("WorldPos: " + worldPosition);
        worldPosition -= transform.parent.transform.position;
        float percentX = Mathf.Clamp01((worldPosition.x + gridWorldSize.x/2) / gridWorldSize.x);
        float percentY = Mathf.Clamp01((worldPosition.z + gridWorldSize.y/2) / gridWorldSize.y);
        Debug.Log("WorldPos PercentX: " + percentX);
        Debug.Log("WorldPos PercentY: " + percentY);
        Debug.Log("Grid World Size: " + gridWorldSize);

        int x = Mathf.RoundToInt((gridSizeX-1) * percentX);
        int y = Mathf.RoundToInt((gridSizeY-1) * percentY);
        return grid[x,y];
    }

    void OnDrawGizmos() {
        
		Gizmos.DrawWireCube(transform.position,new Vector3(gridWorldSize.x,1,gridWorldSize.y));
        //Gizmos.DrawCube(Vector3.zero, Vector3.one * 20);
        
		if (grid != null && displayGridGizmos) {
			foreach (Node n in grid) {
                //Debug.Log(n.walkable);
				Gizmos.color = (n.walkable)?Color.white:Color.red;
				Gizmos.DrawCube(n.worldPosition, Vector3.one * (nodeDiameter-.01f));
                //Debug.Log("Gizmos Drawn");
			}
		} else {
            Debug.Log("Node Draw Fail");
        }
	}
    public int MaxSize {
		get {
			return gridSizeX * gridSizeY;
		}
	}

    public List<Node> GetNeighbours(Node node) {
		List<Node> neighbours = new List<Node>();

		for (int x = -1; x <= 1; x++) {
			for (int y = -1; y <= 1; y++) {
				if (x == 0 && y == 0)
					continue;

				int checkX = node.gridX + x;
				int checkY = node.gridY + y;

				if (checkX >= 0 && checkX < gridSizeX && checkY >= 0 && checkY < gridSizeY) {
					neighbours.Add(grid[checkX,checkY]);
				}
			}
		}

		return neighbours;
	}

    public void ResetEpisode() {
        
    }
}
