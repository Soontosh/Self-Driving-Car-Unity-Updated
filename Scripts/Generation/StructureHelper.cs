//Add Script to Town Hall and Hospital, Delete all Structures Colliding With Them
using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Profiling;

public class StructureHelper : MonoBehaviour
{
    public GameObject prefab;
    public Dictionary<Vector3Int, GameObject> structuresDictionary = new Dictionary<Vector3Int, GameObject>();
    public BuildingType[] buildingTypes;
    public GameObject[] naturePrefabs;
    public bool randomNaturePlacement = false;
    [Range(0, 1f)]
    public float randomNaturePlacementThreshold = 0.3f;
    //List that will be mixed up later!
    private List<BuildingType> buildingTypesMixed = new List<BuildingType>();

    public void PlaceStructuresAroundRoad(List<Vector3Int> roadPositions) {
        Dictionary<Vector3Int, Direction> freeEstateSpots = FindFreeSpacesAroundRoad(roadPositions);
        List<Vector3Int> BlockedPositions = new List<Vector3Int>();
        List<Vector3Int> BlockedPositionsNew = new List<Vector3Int>();
        List<BuildingType> rejectedItems = new List<BuildingType>();
        int currentIteration = 0;
        foreach (var freeSpot in freeEstateSpots) {
            if (BlockedPositions.Contains(freeSpot.Key)) {
                Debug.Log("Death has been blocked");
                continue;
            }
            for (int i = 0; i < buildingTypes.Length; i++) {
                Debug.Log("First Structure Helper Loop");
                if (buildingTypes[i].quantity == -1) {
                    buildingTypesMixed.Add(buildingTypes[i]);
                    break;
                }

                if (buildingTypes[i].IsBuildingAvailable()) {
                    if(buildingTypes[i].sizeRequired > 1) {
                        var halfSize = Mathf.FloorToInt(buildingTypes[i].sizeRequired / 2f);
                        List<Vector3Int> tempPositionsBlocked = new List<Vector3Int>();
                        if (VerifyIfBuildingFits(halfSize, freeEstateSpots, freeSpot, BlockedPositions, ref tempPositionsBlocked)) {
                            BlockedPositions.AddRange(tempPositionsBlocked);
                            Debug.Log("Temp Positions Blocked Length: " + tempPositionsBlocked.Count);
                            buildingTypes[i].GetPrefab();
                            buildingTypesMixed.Add(buildingTypes[i]);
                            //Check Pos of Last Object, if It is Just One in Any Direction, Move Opposite Direction
                            foreach (int checkI in Enumerable.Range(0, tempPositionsBlocked.Count + 1)) {
                                //buildingTypesMixed.Add(buildingTypes[i]);
                            }
                            
                        }
                    } else {
                        buildingTypes[i].GetPrefab();
                        buildingTypesMixed.Add(buildingTypes[i]);
                    }
                    break;
                }
            }
        }
        //Mix up list here
        buildingTypesMixed = ShuffleList(buildingTypesMixed);
        foreach (var freeSpot in freeEstateSpots) {
            //So Make Seperate LIst, add all long buildings into there, then check that list only if not current one is long
            Debug.Log("COopsu Poopsu Curent Iteration: " + currentIteration);
            Debug.Log("COopsu Poopsu Curent Size Main List: " + (buildingTypesMixed.Count - 1));
            Debug.Log("COopsu Poopsu Curent Size Side List: " + (freeEstateSpots.Count - 1));
            try {
            if (BlockedPositionsNew.Contains(freeSpot.Key)) {
                currentIteration++;
                Debug.Log("Nah Bruh!!");
                continue;
            } else if (!(buildingTypesMixed[currentIteration].sizeRequired > 1)) {
                currentIteration++;
                continue;
            }
            } catch (ArgumentOutOfRangeException) {
                Debug.Log("Oopsu Poopsu Error Fixed!");
                continue;
            }

            var rotation = Quaternion.identity;
            
            
            switch (freeSpot.Value) {
                case (Direction.Up):
                    rotation = Quaternion.Euler(0, 90, 0);
                    break;
                case (Direction.Down):
                    rotation = Quaternion.Euler(0, -90, 0);
                    break;
                case (Direction.Right):
                    rotation = Quaternion.Euler(0, 180, 0);
                    break;
            }

            //Add structures dictionary implementation if useful in future, convert buildingTyoesMixed into Dictionary<BuildingType, List<Vector3Int>>
            //Bruh Spawn Based On Actual Position XD
            try {
                if(buildingTypesMixed[currentIteration].sizeRequired > 1) {
                    var halfSize = Mathf.FloorToInt(buildingTypesMixed[currentIteration].sizeRequired / 2f);
                    List<Vector3Int> tempPositionsBlocked = new List<Vector3Int>();
                    if (VerifyIfBuildingFits(halfSize, freeEstateSpots, freeSpot, BlockedPositionsNew, ref tempPositionsBlocked)) {
                        BlockedPositionsNew.AddRange(tempPositionsBlocked);
                        var building = SpawnPrefab(buildingTypesMixed[currentIteration].GetPrefab(), freeSpot.Key, rotation);
                        structuresDictionary.Add(freeSpot.Key, building);
                        currentIteration++;
                        continue;
                        //Debug.Log("Temp Positions Blocked Length: " + tempPositionsBlocked.Count);
                    }
                }
            } catch (ArgumentOutOfRangeException) {
                Debug.Log("Poop Gane: Current Iteration Fail: " + currentIteration);
                Debug.Log("Poop Gane: Ze Size Ze Failiure: " + buildingTypesMixed.Count);
                currentIteration++;
                continue;
                //throw new ArgumentOutOfRangeException();
            }

            //nvm from here just get a random value from dictionary
            //int currentIndex = UnityEngine.Random.Range(0, buildingTypes.Length);
            /*
            
            for (int i = buildingTypesMixed.Count - 1; i >= 0; i--) {
                Debug.Log("Da Count: " + buildingTypesMixed.Count);
                Debug.Log("Da I: " + i);
                SpawnPrefab(buildingTypesMixed[0].GetPrefab(), freeSpot.Key, rotation);
                buildingTypesMixed.RemoveAt(i);
            }
            */
        }

        currentIteration = 0;
        foreach (var freeSpot in freeEstateSpots) {
            //So Make Seperate LIst, add all long buildings into there, then check that list only if not current one is long
            Debug.Log("Oopsu Poopsu Curent Iteration: " + currentIteration);
            Debug.Log("Oopsu Poopsu Curent Size Main List: " + (buildingTypesMixed.Count - 1));
            Debug.Log("Oopsu Poopsu Curent Size Side List: " + (freeEstateSpots.Count - 1));
            try {
                if (BlockedPositionsNew.Contains(freeSpot.Key)) {
                    currentIteration++;
                    Debug.Log("Nah Bruh!!");
                    continue;
                } else if (!(buildingTypesMixed[currentIteration].sizeRequired == 1)) {
                    currentIteration++;
                    continue;
                }
            } catch (ArgumentOutOfRangeException) {
                Debug.Log("Oopsu Poopsu Error Fixed!");
                continue;
            }

            var rotation = Quaternion.identity;
            
            
            switch (freeSpot.Value) {
                case (Direction.Up):
                    rotation = Quaternion.Euler(0, 90, 0);
                    break;
                case (Direction.Down):
                    rotation = Quaternion.Euler(0, -90, 0);
                    break;
                case (Direction.Right):
                    rotation = Quaternion.Euler(0, 180, 0);
                    break;
            }

            //Add structures dictionary implementation if useful in future, convert buildingTyoesMixed into Dictionary<BuildingType, List<Vector3Int>>
            //Bruh Spawn Based On Actual Position XD
            if (buildingTypesMixed[currentIteration].quantity == -1) {
                var random = UnityEngine.Random.value;
                if (random < randomNaturePlacementThreshold) {
                    var tree = SpawnPrefab(naturePrefabs[UnityEngine.Random.Range(0, naturePrefabs.Length)], freeSpot.Key, rotation);
                    try {
                        structuresDictionary.Add(freeSpot.Key, tree);
                    } catch (ArgumentException) {}
                    continue;
                }
            }
            var building = SpawnPrefab(buildingTypesMixed[currentIteration].GetPrefab(), freeSpot.Key, rotation);
            try {
                structuresDictionary.Add(freeSpot.Key, building);
            } catch (ArgumentException) {}
            BlockedPositionsNew.Add(freeSpot.Key);
            currentIteration++;
            

            

            //nvm from here just get a random value from dictionary
            //int currentIndex = UnityEngine.Random.Range(0, buildingTypes.Length);
            /*
            
            for (int i = buildingTypesMixed.Count - 1; i >= 0; i--) {
                Debug.Log("Da Count: " + buildingTypesMixed.Count);
                Debug.Log("Da I: " + i);
                SpawnPrefab(buildingTypesMixed[0].GetPrefab(), freeSpot.Key, rotation);
                buildingTypesMixed.RemoveAt(i);
            }
            */
        }
    }

    public Dictionary<Vector3Int, Direction> FindFreeSpacesAroundRoad(List<Vector3Int> roadPositions) {
        Dictionary<Vector3Int, Direction> freeSpaces = new Dictionary<Vector3Int, Direction>();
        foreach (var position in roadPositions) {

            var neighborDirections = PlacementHelper.FindNeighbor(position, roadPositions);
            foreach (Direction direction in Enum.GetValues(typeof(Direction))) {

                if (neighborDirections.Contains(direction) == false) {
                    var newPosition = position + PlacementHelper.GetOffsetFromDirection(direction);
                    if (freeSpaces.ContainsKey(newPosition)) continue;
                    freeSpaces.Add(newPosition, PlacementHelper.GetReverseDirection(direction));
                }
            }
        }
        return freeSpaces;
    }

    private GameObject SpawnPrefab(GameObject prefab, Vector3Int position, Quaternion rotation) {
        var newStructure = Instantiate(prefab, position, rotation, transform);
        return newStructure;
    }

    public List<BuildingType> ShuffleList(List<BuildingType> listToShuffle) {
        for (int i = listToShuffle.Count - 1; i > 0; i--) {
            Debug.Log("Second Structure Helper Loop");
            var _rand = new System.Random();
            var k = _rand.Next(i + 1);
            var value = listToShuffle[k];
            listToShuffle[k] = listToShuffle[i];
            listToShuffle[i] = value;
        }
        return listToShuffle;
    }

    private bool VerifyIfBuildingFits(int halfSize, Dictionary<Vector3Int, Direction> freeEstateSpots, KeyValuePair<Vector3Int, Direction> freeSpot, List<Vector3Int> blockedPositions, ref List<Vector3Int> tempPositionsBlocked) {
        Vector3Int direction = Vector3Int.zero;
        if (freeSpot.Value == Direction.Down || freeSpot.Value == Direction.Up) {
            direction = Vector3Int.right;
        } else {
            direction = new Vector3Int(0, 0, 1);
        }

        for (int i = 1; i <= halfSize; i++) {
            Debug.Log("Third Structure Helper Loop");
            var pos1 = freeSpot.Key + direction * i;
            var pos2 = freeSpot.Key - direction * i;
            if (!freeEstateSpots.ContainsKey(pos1) || !freeEstateSpots.ContainsKey(pos2) || blockedPositions.Contains(pos1) || blockedPositions.Contains(pos2)) return false;
            tempPositionsBlocked.Add(pos1);
            tempPositionsBlocked.Add(pos2);
        }
        return true;
    }

    public void Reset() {
        foreach(var item in structuresDictionary.Values) {
            Destroy(item);
        }
        structuresDictionary.Clear();
        foreach (var buildingType in buildingTypes) {
            buildingType.Reset();
        }
        foreach (var buildingType in buildingTypesMixed) {
            buildingType.Reset();
        }
        
    }
    
}
