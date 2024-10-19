//Certain road prefab changes not working
//FIX
//
//
//OK SO YOU NEED TO FIX ADDING TO LIST AND DICTIONARY NOT WORKING RESULTING IN (0, 0, 0) NOT BEING FOUND!!
//

//Get row number, only allow one item from each row to connect to same row
using System;
using System.Collections;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using Unity.Profiling;
using UnityEngine.Profiling;

public class RoadHelper : MonoBehaviour
{
    
    public GameObject roadStraight, roadCorner, road3Way, road4Way, roadEnd;
    public static bool clearedRoads = false;
    //
    //Uncomment all instances if this results in error
    //


    //Keep Int, Harder to Compart Vector3 Floats
    static Dictionary<Vector3Int, GameObject> roadDictionary = new Dictionary<Vector3Int, GameObject>();
    //Dictionary containing Lists of neighbors and their directions.
    static Dictionary<Vector3Int, List<Direction>>  nearDictionary = new Dictionary<Vector3Int, List<Direction>>();
    static HashSet<Vector3Int> fixRoadCandidates = new HashSet<Vector3Int>();
    static List<Vector3Int> finalCandidateTest = new List<Vector3Int>();
    //check
    static Dictionary<int, List<Vector3Int>> checkDictionaryX = new Dictionary<int, List<Vector3Int>>();
    static Dictionary<int, List<Vector3Int>> checkDictionaryZ = new Dictionary<int, List<Vector3Int>>();
    static Visualizer visualizer = new Visualizer();
    //public MyContainer(string number, string title);
    //private ProfilerMarker myMarker = new ProfilerMarker("MyScript");
    public List<Vector3Int> GetRoadPositions() {
        return roadDictionary.Keys.ToList();
    }


    public void PlaceStreetPositions(Vector3 startPosition, Vector3Int direction, int length) {
        Debug.Log("Hi");
        var rotation = Quaternion.identity;
        //roadDictionary = new Dictionary<Vector3Int, GameObject>();
        if (direction.x == 0) {
            rotation = Quaternion.Euler(0, 90, 0);
        }

        for (int i = 0; i < length; i++) {
            Debug.Log("First Road Helper Loop");
            var position = Vector3Int.RoundToInt(startPosition + direction * i);
            Debug.Log("Fiirst Road Helper Loop");
            Debug.Log("Road Dictionary: " + roadDictionary);
            if (roadDictionary.ContainsKey(position)) {
                Debug.Log("Breh");
                finalCandidateTest.Add(position);
                continue;
            }
            Debug.Log("Breeddh");
            var road = Instantiate(roadStraight, position, rotation, transform);
            Debug.Log("Breeh");
            roadDictionary.Add(position, road);
            Debug.Log("Position 1: " + position);
            Debug.Log("Position Check: " + roadDictionary[position]);
            Debug.Log("Keys Count Yearr: " + roadDictionary.Count());
            Debug.Log("Breeeeh");
            if(i == 0 || i == length - 1) {
                //fixRoadCandidates.Add(position);
                Debug.Log("Final Candidate Log: " + finalCandidateTest);
                finalCandidateTest.Add(position);
                Debug.Log("Position 2: " + position);
            }
        }
        Debug.Log("Transform Child Count: " + transform.childCount);
        Debug.Log("Transform Child Count: " + transform.childCount);
    }

    public void FixRoad() {
        //Hi
        Debug.Log("Hee");
        //Profiler.BeginSample("Best Sample");
        foreach (var position in finalCandidateTest) {
            Debug.Log("Gang skrr skrr");
            Debug.Log("SlkrkrkrX: " + checkDictionaryX.Count());
            Debug.Log("SlkrkrkrZ: " + checkDictionaryZ.Count());
            List<Direction> neighbourDirections = PlacementHelper.FindNeighbor(position, roadDictionary.Keys);
            Debug.Log("Keys Count Year: " + roadDictionary.Count());
            //nearDictionary.Add(position, neighbourDirections);

            Quaternion rotation = Quaternion.identity;

            if (neighbourDirections.Count == 1) {
                Destroy(roadDictionary[position]);
                if (neighbourDirections.Contains(Direction.Down)) {
                    rotation = Quaternion.Euler(0, 90, 0);
                } else if (neighbourDirections.Contains(Direction.Left)) {
                    rotation = Quaternion.Euler(0, 180, 0);
                } else if (neighbourDirections.Contains(Direction.Up)) {
                    rotation = Quaternion.Euler(0, -90, 0);
                }
                roadDictionary[position] = Instantiate(roadEnd, position, rotation, transform);

            } else if (neighbourDirections.Count == 2) {
                Debug.Log("Double Location: " + position + ", List: " + neighbourDirections.Count);
                if (neighbourDirections.Contains(Direction.Up) && neighbourDirections.Contains(Direction.Down) || neighbourDirections.Contains(Direction.Right) && neighbourDirections.Contains(Direction.Left)) {
                    try {
                        checkDictionaryX.Add(key: position.x, value: new List<Vector3Int>());
                        checkDictionaryX[position.x].Add(position);
                        //Debug.Log("Position.x: " + position.x);
                    } 
                    catch (System.ArgumentException) {
                        checkDictionaryX[position.x].Add(position);
                    }

                    try {
                        checkDictionaryZ.Add(key: position.z, value: new List<Vector3Int>());
                        checkDictionaryZ[position.z].Add(position);
                    } 
                    catch (System.ArgumentException) {
                        checkDictionaryZ[position.z].Add(position);
                    }
                    Debug.Log("Yas QUuueueu");
                    continue;
                    
                }
                Destroy(roadDictionary[position]);

                if (neighbourDirections.Contains(Direction.Up) && neighbourDirections.Contains(Direction.Right)) {
                    rotation = Quaternion.Euler(0, 90, 0);
                } else if (neighbourDirections.Contains(Direction.Right) && neighbourDirections.Contains(Direction.Down)) {
                    rotation = Quaternion.Euler(0, 180, 0);
                } else if (neighbourDirections.Contains(Direction.Down) && neighbourDirections.Contains(Direction.Left)) {
                    rotation = Quaternion.Euler(0, -90, 0);
                }
                roadDictionary[position] = Instantiate(roadCorner, position, rotation, transform);
                
            } else if (neighbourDirections.Count == 3) {
                Destroy(roadDictionary[position]);
                Debug.Log("Triple Location: " + position + ", List: " + neighbourDirections.Count);

                if (neighbourDirections.Contains(Direction.Right) && neighbourDirections.Contains(Direction.Left) && neighbourDirections.Contains(Direction.Down)) {
                    rotation = Quaternion.Euler(0, 90, 0);
                } else if (neighbourDirections.Contains(Direction.Left) && neighbourDirections.Contains(Direction.Down) && neighbourDirections.Contains(Direction.Up)) {
                    rotation = Quaternion.Euler(0, 180, 0);
                } else if (neighbourDirections.Contains(Direction.Left) && neighbourDirections.Contains(Direction.Up) && neighbourDirections.Contains(Direction.Right)) {
                    rotation = Quaternion.Euler(0, -90, 0);
                }
                 roadDictionary[position] = Instantiate(road3Way, position, rotation, transform);
            } else {
                try {
                    Destroy(roadDictionary[position]);
                } catch (KeyNotFoundException) {
                    Debug.Log("True Position: " + position);
                    foreach (Vector3 item in roadDictionary.Keys) {
                        Debug.Log("Item 3: " + item);
                    }
                    //throw new KeyNotFoundException();
                }
                roadDictionary[position] = Instantiate(road4Way, position, rotation, transform);
                Debug.Log("4 Road Position: " + position + ", List: " + neighbourDirections.Count);
            }
            
            try {
                checkDictionaryX.Add(key: position.x, value: new List<Vector3Int>());
                checkDictionaryX[position.x].Add(position);
            } 
            catch (System.ArgumentException) {
                checkDictionaryX[position.x].Add(position);
            }

            try {
                checkDictionaryZ.Add(key: position.z, value: new List<Vector3Int>());
                checkDictionaryZ[position.z].Add(position);
            } 
            catch (System.ArgumentException) {
                checkDictionaryZ[position.z].Add(position);
            }
                finally {
                
            }
        }
        //Profiler.EndSample();
        clearedRoads = true;
    }
    public bool CheckRoad() {
        //Check if rows with a difference of 1 or -1 have high road counts, if so, add them to the list, then check if
        //check if they all have close but differing z values, if they have many, redo 
        //Just Add Z-Axis, then you can start adding houses
        List<int> rowsToCheckX = new List<int>();
        List<int> rowsToCheckZ = new List<int>();
        foreach (KeyValuePair<int, List<Vector3Int>> entry in checkDictionaryX) {
            if (checkDictionaryX.ContainsKey(entry.Key - 1) /*&& checkDictionaryX[entry.Key - 1].Count >= 2*/ || checkDictionaryX.ContainsKey(entry.Key + 1) /*&& checkDictionaryX[entry.Key + 1].Count >= 2*/)
                {
                    if (entry.Value.Count >= 1) {
                        rowsToCheckX.Add(entry.Key);
                        Debug.Log("Greater than one value X!");
                    }
                }
        }

       foreach (KeyValuePair<int, List<Vector3Int>> entry in checkDictionaryZ) {
            if (checkDictionaryZ.ContainsKey(entry.Key - 1) || checkDictionaryZ.ContainsKey(entry.Key + 1))
                {
                    if (entry.Value.Count >= 1) {
                        rowsToCheckZ.Add(entry.Key);
                        Debug.Log("Greater than one value Z!");
                    }
                }
        }
        
        foreach(int row in rowsToCheckX) {
            //Check if adjacent to row we have to check in anyway
            List<int> numberOfAdjacentsUp = new List<int>();
            List<int> numberOfAdjacentsDown = new List<int>();
            int currentUpAdjacent = 0;
            //List<int> numberOfAdjacentsRight;
            //List<int> numberOfAdjacentsLeft;
            int currentDownAdjacent = 0;
            bool finishedDown = false;
            
            
            foreach (Vector3Int entry in checkDictionaryX[row]) {
                var neighbourDirections = PlacementHelper.FindNeighbor(entry, roadDictionary.Keys);
                if (neighbourDirections.Contains(Direction.Up)) {
                    try {
                        numberOfAdjacentsUp[currentUpAdjacent] = numberOfAdjacentsUp[currentUpAdjacent] + 1;
                        Debug.Log("Upper Thing was good yuh. this is the streak: " + numberOfAdjacentsUp[currentUpAdjacent]);
                        //numberOfAdjacentsUp[currentUpAdjacent]++;
                    } catch (System.ArgumentOutOfRangeException) {
                        numberOfAdjacentsUp.Add(0);
                        currentUpAdjacent = 0;
                    }

                    
                    try {
                        if (numberOfAdjacentsUp[currentUpAdjacent] >= 3) {
                            //Reset
                            Debug.Log("More Than Two Boogers Up Sway Qween : " + entry);
                            return false;
                            Debug.Log("Current Up Adjacent NoError: " + currentUpAdjacent + " Current List Length: " + numberOfAdjacentsUp.Count);
                            
                        } else {
                            Debug.Log("Failed up attempt, value Shown: " + currentUpAdjacent);
                        }
                    } catch (System.ArgumentOutOfRangeException) {
                        Debug.Log("Current up Adjacent Error: " + currentDownAdjacent + " Current List Length: " + numberOfAdjacentsUp.Count +  "Current Stuff List Index: " + numberOfAdjacentsUp.Count);
                        //Debug.Log("Oh Yeah poop gang: " + Enumerable.Range(System.Convert.ToInt32(0, numberOfAdjacentsUp.Count()) - currentUpAdjacent + 1));
                        System.Convert.ToInt32(numberOfAdjacentsUp.Count());
                        foreach (int i in Enumerable.Range(0, System.Convert.ToInt32(Mathf.Abs(numberOfAdjacentsUp.Count() - currentUpAdjacent) + 1))) {
                            currentUpAdjacent++;
                            Debug.Log("More Than Two Boogers Fakse Fix: " + entry);
                            Debug.Log("Current Up Adjacent NoError: " + currentUpAdjacent + " Current List Length: " + numberOfAdjacentsUp.Count);
                        }
                    }
                    
                } else {
                    numberOfAdjacentsUp.Add(0);
                    currentUpAdjacent = 0;
                    Debug.Log("Broken Up Loop");
                } 
            }

            foreach (Vector3Int entry in checkDictionaryX[row]) {
                //Ok, fix the list issue like you did in the up part, and then add z, and then ou can FINALLY start working on the houses.
                var neighbourDirections = PlacementHelper.FindNeighbor(entry, roadDictionary.Keys);
                if (neighbourDirections.Contains(Direction.Down)) {
                    try {
                        numberOfAdjacentsDown[currentDownAdjacent] = numberOfAdjacentsDown[currentDownAdjacent]++;
                        Debug.Log("List Index: " + numberOfAdjacentsDown[currentDownAdjacent]);
                        //BRO MAKE CURRENT ELEMENT WORK
                        //Debug.Log("current Element: " + currentElement);
                        //numberOfAdjacentsUp[currentUpAdjacent]++;
                    } catch (System.ArgumentOutOfRangeException) {
                        numberOfAdjacentsDown.Add(0);
                        Debug.Log("Skipped Item");
                    }

                    
                    try {
                        if (numberOfAdjacentsDown[currentDownAdjacent] >= 3) {
                            //Reset
                            Debug.Log("More Than Two down Boogers Down: " + entry);
                            return false;
                            Debug.Log("Current Down Adjacent NoError: " + currentDownAdjacent + " Current List Length: " + numberOfAdjacentsDown.Count);
                            finishedDown = true;
                        } else {
                            Debug.Log("Failed  attempt, value Shown: " + currentDownAdjacent);
                        }
                    } catch (System.ArgumentOutOfRangeException) {
                        Debug.Log("Current Down Adjacent Error: " + currentDownAdjacent + " Current List Length: " + numberOfAdjacentsDown.Count +  "Current Stuff List Index: " + numberOfAdjacentsDown.Count);
                        //Debug.Log("Oh Yeah poop gang: " + Enumerable.Range(System.Convert.ToInt32(0, numberOfAdjacentsDown.Count()) - currentDownAdjacent + 1));
                        System.Convert.ToInt32(numberOfAdjacentsDown.Count());
                        foreach (int i in Enumerable.Range(0, System.Convert.ToInt32(Mathf.Abs(numberOfAdjacentsDown.Count() - currentDownAdjacent) + 1))) {
                            currentDownAdjacent++;
                            Debug.Log("Hey: " + entry);
                        }
                    }
                    
                } else {
                    numberOfAdjacentsDown.Add(0);
                    currentDownAdjacent = 0;
                    Debug.Log("Broken Down Loop");
                } 
            }
        
        }

        foreach(int row in rowsToCheckZ) {
            //Up = Right, Down = Left
            List<int> numberOfAdjacentsRight = new List<int>();
            List<int> numberOfAdjacentsLeft = new List<int>();
            int currentRightAdjacent = 0;
            int currentLeftAdjacent = 0;
            //List<int> numberOfAdjacentsRight;
            //List<int> numberOfAdjacentsLeft;
            
            bool finishedDown = false;
            
            
            foreach (Vector3Int entry in checkDictionaryZ[row]) {
                var neighbourDirections = PlacementHelper.FindNeighbor(entry, roadDictionary.Keys);
                if (neighbourDirections.Contains(Direction.Up)) {
                    try {
                        numberOfAdjacentsRight[currentRightAdjacent] = numberOfAdjacentsRight[currentRightAdjacent] + 1;
                        Debug.Log("Upper Thing was good. this is the streak: " + numberOfAdjacentsRight[currentRightAdjacent]);
                        //numberOfAdjacentsUp[currentUpAdjacent]++;
                    } catch (System.ArgumentOutOfRangeException) {
                        numberOfAdjacentsRight.Add(0);
                        currentRightAdjacent = 0;
                    }

                    
                    try {
                        if (numberOfAdjacentsRight[currentRightAdjacent] >= 3) {
                            //Reset
                            Debug.Log("More Than Two Boogers Right Sway Qween : " + entry);
                            return false;
                            Debug.Log("Current Up Adjacent NoError: " + currentRightAdjacent + " Current List Length: " + numberOfAdjacentsRight.Count);
                            
                        } else {
                            Debug.Log("Failed up attempt, value Shown: " + currentRightAdjacent);
                        }
                    } catch (System.ArgumentOutOfRangeException) {
                        Debug.Log("Current up Adjacent Error: " +  " Current List Length: " + numberOfAdjacentsRight.Count +  "Current Stuff List Index: " + numberOfAdjacentsRight.Count);
                        //Debug.Log("Oh Yeah poop gang: " + Enumerable.Range(System.Convert.ToInt32(0, numberOfAdjacentsRight.Count()) - currentRightAdjacent + 1));
                        System.Convert.ToInt32(numberOfAdjacentsRight.Count());
                        foreach (int i in Enumerable.Range(0, System.Convert.ToInt32(Mathf.Abs(numberOfAdjacentsRight.Count() - currentRightAdjacent) + 1))) {
                            currentRightAdjacent++;
                            Debug.Log("More Than Two Boogers Fakse Fix: " + entry);
                            Debug.Log("Current Up Adjacent NoError: " + currentRightAdjacent + " Current List Length: " + numberOfAdjacentsRight.Count);
                        }
                    }
                    
                } else {
                    numberOfAdjacentsRight.Add(0);
                    currentRightAdjacent = 0;
                    Debug.Log("Broken Up Loop");
                } 
            }

            
            foreach (Vector3Int entry in checkDictionaryZ[row]) {
                //Ok, fix the list issue like you did in the up part, and then add z, and then ou can FINALLY start working on the houses.
                var neighbourDirections = PlacementHelper.FindNeighbor(entry, roadDictionary.Keys);
                if (neighbourDirections.Contains(Direction.Down)) {
                    try {
                        numberOfAdjacentsLeft[currentLeftAdjacent] = numberOfAdjacentsLeft[currentLeftAdjacent]++;
                        Debug.Log("List Index: " + numberOfAdjacentsLeft[currentLeftAdjacent]);
                        //BRO MAKE CURRENT ELEMENT WORK
                        //Debug.Log("current Element: " + currentElement);
                        //numberOfAdjacentsRight[currentRightAdjacent]++;
                    } catch (System.ArgumentOutOfRangeException) {
                        numberOfAdjacentsLeft.Add(0);
                        Debug.Log("Skipped Item");
                    }

                    
                    try {
                        if (numberOfAdjacentsLeft[currentLeftAdjacent] >= 3) {
                            //Reset
                            Debug.Log("More Than Two down Boogers Left: " + entry);
                            return false;
                            Debug.Log("Current Down Adjacent NoError: " + currentLeftAdjacent + " Current List Length: " + numberOfAdjacentsLeft.Count);
                            finishedDown = true;
                        } else {
                            Debug.Log("Failed  attempt, value Shown: " + currentLeftAdjacent);
                        }
                    } catch (System.ArgumentOutOfRangeException) {
                        Debug.Log("Current Down Adjacent Error: " + currentLeftAdjacent + " Current List Length: " + numberOfAdjacentsLeft.Count +  "Current Stuff List Index: " + numberOfAdjacentsLeft.Count);
                        //Debug.Log("Oh Yeah poop gang: " + Enumerable.Range(System.Convert.ToInt32(0, numberOfAdjacentsLeft.Count()) - currentLeftAdjacent + 1));
                        System.Convert.ToInt32(numberOfAdjacentsLeft.Count());
                        foreach (int i in Enumerable.Range(0, System.Convert.ToInt32(Mathf.Abs(numberOfAdjacentsLeft.Count() - currentLeftAdjacent) + 1))) {
                            currentLeftAdjacent++;
                            Debug.Log("Hey: " + entry);
                        }
                    }
                    
                } else {
                    numberOfAdjacentsLeft.Add(0);
                    currentLeftAdjacent = 0;
                    Debug.Log("Broken Down Loop");
                } 
            }
        }
        return true;
    }

    public void Reset() {
        foreach (var item in roadDictionary.Values) {
            Destroy(item);
        }
        roadDictionary.Clear();
        //fixRoadCandidates = new HashSet<Vector3Int>();
        finalCandidateTest = new List<Vector3Int>();
        //checkDictionaryX = new Dictionary<int, List<Vector3Int>>();
        //checkDictionaryZ = new Dictionary<int, List<Vector3Int>>();
    }

    public Transform randomTarget() {
        System.Random random = new System.Random();
        int index = random.Next(roadDictionary.Count);
        return roadDictionary.ElementAt(index).Value.transform;
    }

    public static (Vector3, float, float) SquareSize(float gridSize, float squareY) {
        float maxX = float.MinValue;
        float maxZ = float.MinValue;
        float minX = float.MaxValue;
        float minZ = float.MaxValue;


        foreach (var road in roadDictionary.Values) {
            //Vector3 roadPosition = road.transform.TransformPoint(Vector3.zero);
            Vector3 roadPosition = road.transform.position;  
            maxX = Mathf.Max(maxX, roadPosition.x);
            maxZ = Mathf.Max(maxZ, roadPosition.z);
            minX = Mathf.Min(minX, roadPosition.x);
            minZ = Mathf.Min(minZ, roadPosition.z);
        }

        float sizeX = maxX - minX;
        float sizeY = maxZ - minZ;

        sizeX = Mathf.Ceil(sizeX / gridSize) * gridSize;
        sizeY = Mathf.Ceil(sizeY / gridSize) * gridSize;

        Vector3 position = new Vector3(minX + sizeX / 2f, squareY, minZ + sizeY / 2f);
        
        return (position, sizeX, sizeY);
    }

    /*
    public void restart() {
        //var sequence = lsystem.GenerateSentence();
        //VisualizeSequence(sequence);
    }
    */

    private IEnumerator LoggingEnumerator()
    {
        while (true)
        {
            Debug.Log("Near Dictionary Count: " + nearDictionary.Count());
            yield return new WaitForSeconds(0.5f);
        }
    }
    
}