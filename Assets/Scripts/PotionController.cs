using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PotionController : MonoBehaviour
{
    public Potion potionPawn { get; private set; }

    public void Ready(Point point, GameMap gameMap) 
    {
        potionPawn = new Potion(point.x, point.z, gameObject, gameMap);
    }
}

public class Potion : Pawn
{
    public Potion(int x, int z, GameObject gameObject, GameMap gameMap) : base(x, z, gameObject, gameMap, PawnType.Potion) 
    {

    }

    public override void Interact(Pawn pawn)
    {
        pawn.Damage(-1);
        gameMap.RemovePawnAtPoint(point);
        GameObject.Destroy(gameObject);
    }
}
