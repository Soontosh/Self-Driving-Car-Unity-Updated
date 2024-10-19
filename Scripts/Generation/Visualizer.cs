//When Placing Down Houses and Trees, Check if There is Any Road Nearby, then Check if There is a Connection. If there is a connection, then dont replace the road, otherwise replace is.
//Iterate through the dictionary using the current Vector3Int
//ACTUALLY RESET

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static SimpleVisualizer;
using Unity.AI.Navigation;

public class Visualizer : MonoBehaviour
{
    [SerializeField] private NavMeshSurface surface;
    public LSystemGenerator lsystem;
    public RoadHelper roadHelper = new RoadHelper();
    [SerializeField] private GameObject AStarParentObject;
    [SerializeField] private GameObject TargetObject;
    public StructureHelper structureHelper;
    [SerializeField]
    private int length = 10;
    [SerializeField]
    private int subtractingLength = 5;
    [Range(1, 90)]
    private float minAngle = 90f;
    private float maxAngle = 90f;
    private bool firstValue = true;
    private bool roadsFixed = false;
    //private ProfilerMarker myMarker = new ProfilerMarker("MyScript");
    public static Vector3 targetPosition;

    public int Length { 
        get { 
            if (length > 0) {
                return length;
            } else {
                firstValue = false;
                return Random.Range(6, 8);
            }
        }
        set => length = value;}
    
    private void Start() {
        Debug.Log("In Sample Gang");
        StartCoroutine(EnableAStar());
        CreateTown();
    }

    public void CreateTown() {
        Debug.Log("Hey poopoo");
        Debug.Log("First Structure Visualizer Loop");
        //length = 10;
        roadHelper.Reset();
        structureHelper.Reset();
        var sequence = lsystem.GenerateSentence();
        VisualizeSequence(sequence);
        roadsFixed = true;
        //surface.BuildNavMesh();
        //navLine.DrawPath();
    }

    IEnumerator EnableAStar() {
        yield return new WaitForSeconds(0.1f);
        if(roadsFixed) {
            AStarParentObject.SetActive(true);
            yield break;
        }
    }
    
    private void VisualizeSequence(string sequence) {
            Stack<AgentParameters> savePoints = new Stack<AgentParameters>();
            var currentPosition = Vector3.zero;

            Vector3 direction = Vector3.forward;
            Vector3 tempPosition = Vector3.zero;

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
                        Debug.Log("Slayer: " + roadHelper);
                        roadHelper.PlaceStreetPositions(tempPosition, Vector3Int.RoundToInt(direction), length);
                        if (firstValue) {
                            Length -= subtractingLength;
                        } else {
                            Length = 0;
                        }
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
            //Debug.Log("Count Thing: " + roadHelper.roadDictionary.Count);
            roadHelper.FixRoad();
            //bool checkRoad = roadHelper.CheckRoad();
            targetPosition = roadHelper.randomTarget().position;
            //structureHelper.PlaceStructuresAroundRoad(roadHelper.GetRoadPositions());
            //Get Rid of If Statement If Necessary
            /*
            if (checkRoad) {
                //CreateTown();
                //structureHelper.PlaceStructuresAroundRoad(roadHelper.GetRoadPositions());
            } else {
               // CreateTown();
            }
            */
    }
}
