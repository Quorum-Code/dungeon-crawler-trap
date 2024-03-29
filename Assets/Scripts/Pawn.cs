using UnityEngine;

public class Pawn
{
    public readonly Point point;

    public bool isPlayer { get; private set; } = false;

    public int maxHealth { get; protected set; } = 3;
    public int health { get; protected set; } = 3;

    protected int Str = 0;
    protected int Dex = 0;
    protected int Vit = 0;
    protected int Int = 0;

    protected GameMap gameMap;
    protected GameObject gameObject;
    protected Direction facing = Direction.North;

    public Pawn(int x, int z, GameObject gameObject, GameMap gameMap, bool isPlayer) 
    {
        point = new Point(x, z);
        this.gameObject = gameObject;
        this.gameMap = gameMap;
        this.isPlayer = isPlayer;
    }

    public virtual bool Move(int x, int z) 
    {
        return false;
    }

    public virtual void Damage(int d) 
    {
        // TODO: override with events for player/enemy pawns
        health -= d;
        Debug.Log("new health: " + health);
    }

    public virtual void AddXp(int xp) 
    {

    }

    protected enum Direction 
    {
        North,  // +z
        East,   // +x
        South,  // -z
        West,   // -x
    }
}
