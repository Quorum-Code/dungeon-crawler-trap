using UnityEngine;

public enum PawnType 
{
    None,
    Player,
    Enemy,
    Potion,
    Interactable
}

public class Pawn
{
    public readonly Point point;

    public PawnType type;

    public int maxHealth { get; protected set; } = 3;
    public int health { get; protected set; } = 3;

    public int xp { get; protected set; } = 0;

    protected int Str = 0;
    protected int Dex = 0;
    protected int Vit = 0;
    protected int Int = 0;

    protected GameMap gameMap;
    protected GameObject gameObject;
    protected Direction facing = Direction.North;

    public Pawn(int x, int z, GameObject gameObject, GameMap gameMap, PawnType type) 
    {
        point = new Point(x, z);
        this.gameObject = gameObject;
        this.gameMap = gameMap;
        this.type = type;
    }

    public virtual bool Move(int x, int z) 
    {
        return false;
    }

    public virtual void Damage(int d) 
    {
        // TODO: override with events for player/enemy pawns
        health -= d;
    }

    public virtual void AddXp(int xp) 
    {
        this.xp += xp;
    }

    public virtual void Notify(PlayerPawn playerPawn) 
    {

    }

    public virtual void Interact(Pawn pawn) 
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
