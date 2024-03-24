using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerPawn : Pawn
{
    public PlayerPawn(int x, int z, GameObject gameObject, GameMap gameMap) : base(x, z, gameObject, gameMap)
    {
        
    }

    public bool Move(int dx, int dz) 
    {
        Point nextPoint = MovePoint(dx, dz);

        if (gameMap.canMoveTo(nextPoint))
        {
            point.Set(nextPoint);
            return true;
        }
        else 
        {
            return false;
        }
    }

    private Point MovePoint(int dx, int dz) 
    {
        if (facing == Direction.North)
        {
            return new Point(point.x + dx, point.z + dz);
        }
        else if (facing == Direction.East)
        {
            return new Point(point.x + dz, point.z - dx);
        }
        else if (facing == Direction.South)
        {
            return new Point(point.x - dx, point.z - dz);
        }
        else
        {
            return new Point(point.x - dz, point.z + dx);
        }
    }

    public Point InputToPoint(int dx, int dz) 
    {
        if (facing == Direction.North)
        {
            return new Point(dx, dz);
        }
        else if (facing == Direction.East)
        {
            return new Point(dz, -dx);
        }
        else if (facing == Direction.South)
        {
            return new Point(-dx, -dz);
        }
        else
        {
            return new Point(-dz, dx);
        }
    }

    public void TurnLeft()
    {
        switch(facing)
        {
            case Direction.North:
                facing = Direction.West;
                break;
            case Direction.East:
                facing = Direction.North;
                break;
            case Direction.South:
                facing = Direction.East;
                break;
            case Direction.West:
                facing = Direction.South;
                break;
        }
    }

    public void TurnRight() 
    {
        switch (facing)
        {
            case Direction.North:
                facing = Direction.East;
                break;
            case Direction.East:
                facing = Direction.South;
                break;
            case Direction.South:
                facing = Direction.West;
                break;
            case Direction.West:
                facing = Direction.North;
                break;
        }
    }
}