using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PotionController : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

public class Potion : Pawn
{
    public Potion(int x, int z, GameObject gameObject, GameMap gameMap) : base(x, z, gameObject, gameMap, PawnType.Potion) 
    {

    }
}
