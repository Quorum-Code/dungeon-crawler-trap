using UnityEngine;

public class Pawn
{
    public readonly Point point;

    protected float health;

    protected int Str = 0;
    protected int Dex = 0;
    protected int Vit = 0;
    protected int Int = 0;

    protected GameObject gameObject;
    protected Direction facing = Direction.North;

    public Pawn(int x, int z, GameObject _gameObject) 
    {
        point = new Point(x, z);
        gameObject = _gameObject;
    }

    protected enum Direction 
    {
        North,  // +z
        East,   // +x
        South,  // -z
        West,   // -x
    }
}
