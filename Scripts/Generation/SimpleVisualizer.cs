using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Profiling;

public class SimpleVisualizer : MonoBehaviour
{
    public LSystemGenerator lsystem;
    List<Vector3> positions = new List<Vector3>();
    public GameObject prefab;
    [SerializeField]
    private GameObject SphereParent;
    [SerializeField]
    //public string sequence;
    private GameObject LineParent;
    public Material lineMaterial;

    [SerializeField]
    private int length = 10;
    [SerializeField]
    private int subtractingLength = 5;
    [Range(1, 90)]
    private int minAngle = 90;
    private float maxAngle = 90f;
    private bool firstValue = true;

    public int Length { 
        get { 
            if (length > 0) {
                return length;
            } else {
                firstValue = false;
                return Random.Range(3, 8);
            }
        }
        set => length = value;}
    
    private void Start() {
        var sequence = lsystem.GenerateSentence();
        VisualizeSequence(sequence);
    }
    
    private void VisualizeSequence(string sequence) {
        Stack<AgentParameters> savePoints = new Stack<AgentParameters>();
        var currentPosition = Vector3.zero;

        Vector3 direction = Vector3.forward;
        Vector3 tempPosition = Vector3.zero;

        positions.Add(currentPosition);

        foreach (var letter in sequence) {
            EncodingLetters encoding = (EncodingLetters)letter;
            switch (encoding) {
                case EncodingLetters.save:
                    savePoints.Push(new AgentParameters {
                        position = currentPosition,
                        direction = direction,
                        length = Length,
                    });
                    break;
                case EncodingLetters.load:
                if (savePoints.Count > 0) {
                        var agentParameter = savePoints.Pop();
                        currentPosition = agentParameter.position;
                        direction = agentParameter.direction;
                        Length = agentParameter.length;
                    } else {
                        throw new System.Exception("No Saved Point in Stack");
                    }
                    break;
                case EncodingLetters.draw:
                    tempPosition = currentPosition;
                    currentPosition += direction * length;
                    DrawLine(tempPosition, currentPosition, Color.red);

                    if (firstValue) {
                        Length -= subtractingLength;
                    } else {
                        Length = 0;
                    }

                    positions.Add(currentPosition);
                    break;
                case EncodingLetters.turnRight:
                    direction = Quaternion.AngleAxis(Random.Range(minAngle, maxAngle), Vector3.up) * direction;
                    break;
                case EncodingLetters.turnLeft:
                    direction = Quaternion.AngleAxis(-Random.Range(minAngle, maxAngle), Vector3.up) * direction;
                    break;
                default:
                    break;
            }
        }

        foreach (var position in positions) {
            GameObject NewSphere = Instantiate(prefab, position, Quaternion.identity, SphereParent.transform);
        }
    }

    private void DrawLine(Vector3 start, Vector3 end, Color color) {
        GameObject line = new GameObject("line");
        line.transform.parent = LineParent.gameObject.transform;
        line.transform.position = start;
        var lineRenderer = line.AddComponent<LineRenderer>();
        MeshCollider meshCollider = line.AddComponent<MeshCollider>();
        Mesh mesh = new Mesh();
        lineRenderer.BakeMesh(mesh);
        meshCollider.sharedMesh = mesh;
        //meshCollider.startWidth = 0.1f;
        //meshCollider.endWidth = 0.1f;
        //meshCollider.SetPosition(0, start);
        //meshCollider.SetPosition(1, end);

        lineRenderer.material = lineMaterial;
        lineRenderer.startColor = color;
        lineRenderer.endColor = color;
        lineRenderer.startWidth = 0.1f;
        lineRenderer.endWidth = 0.1f;
        lineRenderer.SetPosition(0, start);
        lineRenderer.SetPosition(1, end);
    }

    public enum EncodingLetters {
        unknown = '1',
        save = '[',
        load = ']',
        draw = 'F',
        turnRight = '+',
        turnLeft = '-'
    }
}
