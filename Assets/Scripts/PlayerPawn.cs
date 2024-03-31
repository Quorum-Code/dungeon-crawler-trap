using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerPawn : Pawn
{
    public delegate void UpdateUI();

    public UpdateUI tookDamage;
    public UpdateUI updateHealth;
    public UpdateUI updateXp;
    public UpdateUI updateStamina;
    public UpdateUI shoveEvent;
    public UpdateUI bumpEvent;
    public UpdateUI healEvent;

    public int maxStamina { get; private set; } = 3;
    public int curStamina { get; private set; } = 2;
    public float toNextStamina { get; private set; } = 0f;
    public float regenStaminaTime { get; private set; } = 0.7f;

    public int skillPoints { get; private set; } = 1;

    public PlayerPawn(int x, int z, GameObject gameObject, GameMap gameMap) : base(x, z, gameObject, gameMap, PawnType.Player)
    {
        
    }

    public void SetMaxHealth(int maxHealth) 
    {
        this.maxHealth = maxHealth;
    }

    public void SetCurHealth(int curHealth) 
    {
        this.health = curHealth;
    }

    public void SetMaxStamina(int maxStamina) 
    {
        this.maxStamina = maxStamina;
    }

    public void SetXp(int xp) 
    {
        this.xp = xp;
    }

    public void Reset(GameMap gameMap) 
    {
        this.gameMap = gameMap;
        gameMap.MovePlayerToSpawn(this);
        facing = Direction.North;
    }

    public void SetGameMap(GameMap gameMap) 
    {
        this.gameMap = gameMap;
    }

    public override void AddXp(int xpToAdd)
    {
        base.AddXp(xpToAdd);

        if (xp > 5) 
        {
            skillPoints += xp / 6;
            xp %= 6;
        }

        updateXp();
    }

    public void AddVit() 
    {
        if (skillPoints > 0) 
        {
            skillPoints--;
            SetMaxHealth(maxHealth + 1);
            SetCurHealth(health + 1);
        }
    }

    public void AddDex() 
    {
        if (skillPoints > 0)
        {
            skillPoints--;
            SetMaxStamina(maxStamina + 1);
        }
    }

    public void IncStaminaTime(float time) 
    {
        toNextStamina += time;

        if (toNextStamina > regenStaminaTime) 
        {
            toNextStamina %= regenStaminaTime;
            if (curStamina + 1 <= maxStamina) 
            {
                curStamina++;
                updateStamina();
            }
        }
    }

    private bool hasStamina() 
    {
        if (curStamina > 0)
            return true;
        return false;
    }

    private void ConsumeStamina() 
    {
        if (curStamina == maxStamina)
        {
            curStamina--;
            toNextStamina = 0f;
        }
        else 
        {
            curStamina--;
        }
        updateStamina();
    }

    public override bool Move(int dx, int dz) 
    {
        Point nextPoint = MovePoint(dx, dz);

        if (!hasStamina()) 
        {
            return false;
        }
        else if (gameMap.canMoveTo(this, nextPoint))
        {
            ConsumeStamina();
            gameMap.MovePawnTo(this, nextPoint);
            point.Set(nextPoint);
            return true;
        }
        else if (gameMap.inBounds(nextPoint) && gameMap.GetPawnAtPoint(nextPoint) != null) 
        {
            ConsumeStamina();
            Pawn enemyPawn = gameMap.GetPawnAtPoint(nextPoint);
            Point pushPoint = MovePoint(dx * 2, dz * 2);
            enemyPawn.Move(pushPoint.x, pushPoint.z);
            // Debug.Log("Shoved enemy! to: " + pushPoint.x + " " + pushPoint.z);
            shoveEvent();
            return false;
        }
        else
        {
            bumpEvent();
            return false;
        }
    }

    public override void Damage(int d)
    {
        if (d > 0)
            tookDamage();
        else
            healEvent();

        base.Damage(d);
        updateHealth();
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

    public void FinishMove() 
    {
        gameMap.FinishMove(point);
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

    public void SetUpdateHealth(UpdateUI u) 
    {

    }
}
