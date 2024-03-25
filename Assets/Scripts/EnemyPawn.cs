using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyPawn : Pawn
{
    public delegate void PawnMoved();
    PawnMoved pm;
    IEnumerator animate;

    public EnemyPawn(int x, int z, GameObject gameObject, GameMap gameMap) : base(x, z, gameObject, gameMap) 
    {
        EnemyController ec = gameObject.GetComponent<EnemyController>();
        if (ec != null ) 
        {
            ec.enemyPawn = this;
            pm = ec.PawnMoved;
        }

        health = 1;
    }

    public override bool Move(int x, int z) 
    {
        Point nextPoint = new Point(x, z);
        if (gameMap.canMoveTo(nextPoint)) 
        {
            gameMap.MovePawnTo(this, nextPoint);
            point.Set(nextPoint);

            pm();
            return true;
        }
        return false;
    }

    public void FinishMove() 
    {
        gameMap.FinishMove(point);
    }

    public void Death() 
    {
        gameMap.RemovePawnAtPoint(point);
    }
}
