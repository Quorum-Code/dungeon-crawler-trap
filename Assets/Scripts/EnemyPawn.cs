using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyPawn : Pawn
{
    EnemyController enemyController;
    public delegate void PawnMoved();
    PawnMoved pm;
    IEnumerator animate;

    public EnemyPawn(int x, int z, GameObject gameObject, GameMap gameMap) : base(x, z, gameObject, gameMap, false) 
    {
        enemyController = gameObject.GetComponent<EnemyController>();
        if (enemyController != null) 
        {
            enemyController.enemyPawn = this;
            pm = enemyController.PawnMoved;
        }

        health = 1;
    }

    public override bool Move(int x, int z) 
    {
        Point nextPoint = new Point(x, z);
        if (gameMap.canMoveTo(this, nextPoint)) 
        {
            gameMap.MovePawnTo(this, nextPoint);
            point.Set(nextPoint);

            pm();
            return true;
        }
        return false;
    }

    public bool MoveTowardsPlayer(PlayerPawn playerPawn) 
    {
        if (playerPawn == null)
            return false;

        // Get delta
        int dx = 0;
        int dz = 0;

        dx = Mathf.Clamp(playerPawn.point.x - point.x, -1, 1);
        dz = Mathf.Clamp(playerPawn.point.z - point.z, -1, 1);

        if (dx != 0 && dz != 0)
            dx = 0;

        return (Move(point.x + dx, point.z + dz));
    }

    public override void Notify(PlayerPawn playerPawn)
    {
        enemyController.StartChase(playerPawn);
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
