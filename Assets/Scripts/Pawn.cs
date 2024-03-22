using UnityEngine;

public class Pawn
{
    int x, z;
    GameObject gameObject;

    public Pawn(int _x, int _z, GameObject _gameObject) 
    {
        x = _x; 
        z = _z; 
        gameObject = _gameObject;
    }
}
