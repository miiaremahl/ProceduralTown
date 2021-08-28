using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

/*
 * Placement helper class
 * 
 * Miia Remahl 
 * mrema003@gold.ac.uk
 * Last edited: 28.02.2021
 * 
 * References:
 * 1. Sunny Valley Studio - Procedural town : https://www.youtube.com/watch?v=umedtEzrpvU&list=PLcRSafycjWFcbaI8Dzab9sTy5cAQzLHoy&index=1,
 * Used to make tha basis for placement helping, made some modifications.
 */

public static class PlacementLogic
{

    //Lists taken positions
    //ref:1 Sunny Valley Studio, took inspiration for the code
    public static List<Direction> FindTaken(Vector3 position, ICollection<Vector3> collection)
    {
        //create list for taken
        List<Direction> taken = new List<Direction>();

        //is right direction taken
        if (collection.Contains(position + Vector3.right))
        {
            taken.Add(Direction.Right);
        }

        //is left direction taken
        if (collection.Contains(position - Vector3.right))
        {
            taken.Add(Direction.Left);
        }

        //is up direction taken
        if (collection.Contains(position + new Vector3(0, 0, 1)))
        {
            taken.Add(Direction.Up);
        }

        //is down direction taken
        if (collection.Contains(position - new Vector3(0, 0, 1)))
        {
            taken.Add(Direction.Down);
        }
        return taken;
    }

    //returns the Reversed direction
    //ref:1
    internal static Direction GetReversedDirection(Direction direction)
    {
        switch (direction)
        {
            case Direction.Up:
                return Direction.Down;
            case Direction.Down:
                return Direction.Up;
            case Direction.Left:
                return Direction.Right;
            case Direction.Right:
                return Direction.Left;
            default:
                break;
        }
        return direction;
    }

    //gets the offset of given direction
    //ref:1 Sunny Valley Studio, took inspiration for the code
    internal static Vector3 GetOffsetFromDirection(Direction direction)
    {
        //return correct direction 
        switch (direction)
        {
            case Direction.Up:
                return new Vector3(0, 0, 1);
            case Direction.Down:
                return new Vector3(0, 0, -1);
            case Direction.Left:
                return Vector3.left;
            case Direction.Right:
                return Vector3.right;
            default:
                break;
        }

        //direction doesnt match item in enum
        throw new Exception();
    }

}