using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyPawn : Pawn
{
    public EnemyPawn(int x, int z, GameObject gameObject, GameMap gameMap) : base(x, z, gameObject, gameMap) 
    {

    }

    public override bool Move(int dx, int dz) 
    {
        Point nextPoint = new Point(point.x + dx, point.z + dz);
        if (gameMap.canMoveTo(nextPoint)) 
        {
            gameMap.MovePawnTo(this, nextPoint);
            point.Set(nextPoint);
            return true;
        }
        return false;
    }
}
