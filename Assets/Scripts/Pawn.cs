using UnityEngine;

public class Pawn
{
    public readonly Point point;

    public int health { get; protected set; } = 5;

    protected int Str = 0;
    protected int Dex = 0;
    protected int Vit = 0;
    protected int Int = 0;

    protected GameMap gameMap;
    protected GameObject gameObject;
    protected Direction facing = Direction.North;

    public Pawn(int x, int z, GameObject gameObject, GameMap gameMap) 
    {
        point = new Point(x, z);
        this.gameObject = gameObject;
        this.gameMap = gameMap;
    }

    public virtual bool Move(int x, int z) 
    {
        return false;
    }

    public void Damage(int d) 
    {
        // TODO: override with events for player/enemy pawns
        health -= d;
        Debug.Log("new health: " + health);
    }

    protected enum Direction 
    {
        North,  // +z
        East,   // +x
        South,  // -z
        West,   // -x
    }
}
