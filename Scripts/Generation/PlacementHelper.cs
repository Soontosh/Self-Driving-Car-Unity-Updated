using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Profiling;


public static class PlacementHelper
{
    public static List<Direction> FindNeighbor(Vector3Int position, ICollection<Vector3Int> collection) {
        List<Direction> neighborDirections = new List<Direction>();
        if (collection.Contains(position + Vector3Int.right)) {
            neighborDirections.Add(Direction.Right);
        }
        if (collection.Contains(position - Vector3Int.right)) {
            neighborDirections.Add(Direction.Left);
        }
        if (collection.Contains(position + new Vector3Int(0, 0, 1))) {
            neighborDirections.Add(Direction.Up);
        }
        if (collection.Contains(position - new Vector3Int(0, 0, 1))) {
            neighborDirections.Add(Direction.Down);
        }
        return neighborDirections;
        
    }

    internal static Vector3Int GetOffsetFromDirection(Direction direction) {
        switch (direction) {
            case (Direction.Up):
                return new Vector3Int(0, 0, 1);
            case (Direction.Down):
                return new Vector3Int(0, 0, -1);
            case (Direction.Right):
                return new Vector3Int(1, 0, 0);
            case (Direction.Left):
                return new Vector3Int(-1, 0, 0);
        }
        throw new System.Exception("No direction such as: " + direction);
    }

    public static Direction GetReverseDirection(Direction direction) {
        switch (direction) {
            case (Direction.Up):
                return Direction.Down;
            case (Direction.Down):
                return Direction.Up;
            case (Direction.Right):
                return Direction.Left;
            case (Direction.Left):
                return Direction.Right;
        }
        throw new System.Exception("No Such Direction As: " + direction);
    }
}

