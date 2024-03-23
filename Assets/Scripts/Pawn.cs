using UnityEngine;

public class Pawn
{
    public readonly Point point;

    protected float health;

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

    protected enum Direction 
    {
        North,  // +z
        East,   // +x
        South,  // -z
        West,   // -x
    }
}
