using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerPawn : Pawn
{
    public PlayerPawn(int x, int z, GameObject gameObject) : base(x, z, gameObject)
    {
        
    }

    public bool Move(int dx, int dz) 
    {
        if (facing == Direction.North)
        {
            point.Add(dx, dz);
        } 
        else if (facing == Direction.East) 
        {
            point.Add(dz, -dx);
        }
        else if (facing == Direction.South)
        {
            point.Add(-dx, -dz);
        }
        else if (facing == Direction.West)
        {
            point.Add(-dz, dx);
        }

        return true;
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
