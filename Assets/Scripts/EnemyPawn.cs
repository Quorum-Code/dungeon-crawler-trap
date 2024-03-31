using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class EnemyPawn : Pawn
{
    EnemyController enemyController;
    public delegate void PawnMoved();
    PawnMoved pm;
    IEnumerator animate;

    int damage = 1;

    public EnemyPawn(int x, int z, GameObject gameObject, GameMap gameMap) : base(x, z, gameObject, gameMap, PawnType.Enemy) 
    {
        enemyController = gameObject.GetComponent<EnemyController>();
        if (enemyController != null) 
        {
            enemyController.enemyPawn = this;
            pm = enemyController.PawnMoved;
        }

        health = 1;
    }

    public EnemyPawn(int x, int z, int damage, int health, GameObject gameObject, GameMap gameMap) : base(x, z, gameObject, gameMap, PawnType.Enemy) 
    {
        enemyController = gameObject.GetComponent<EnemyController>();
        if (enemyController != null)
        {
            enemyController.enemyPawn = this;
            pm = enemyController.PawnMoved;
        }

        this.health = health;
        this.damage = damage;
    }

    public override bool Move(int x, int z) 
    {
        if (health <= 0)
            return false;

        Point nextPoint = new Point(x, z);
        if (gameMap.canMoveTo(this, nextPoint))
        {
            gameMap.MovePawnTo(this, nextPoint);
            point.Set(nextPoint);

            pm();
            return true;
        }
        else if (gameMap.isPlayerAtPoint(nextPoint))
        {
            enemyController.playerPawn.Damage(damage);

            pm();
            return true;
        }
        else if (gameMap.isPawnAtPoint(nextPoint)) 
        {
            return true;
        }
        return false;
    }

    public bool MoveTowardsPlayer(PlayerPawn playerPawn) 
    {
        if (playerPawn == null)
            return false;

        (int, int) dir = DirTowardsPlayer(playerPawn);
        int dx = dir.Item1;
        int dz = dir.Item2;

        return (Move(point.x + dx, point.z + dz));
    }

    public (int, int) DirTowardsPlayer(PlayerPawn playerPawn) 
    {
        if (playerPawn == null)
            return (-1, -1);

        // Get delta
        int dx = 0;
        int dz = 0;

        dx = Mathf.Clamp(playerPawn.point.x - point.x, -1, 1);
        dz = Mathf.Clamp(playerPawn.point.z - point.z, -1, 1);

        if (dx != 0 && dz != 0)
            dx = 0;

        return (dx, dz);
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
