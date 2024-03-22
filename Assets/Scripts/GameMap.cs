using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameMap
{
    int width;      // x
    int length;     // z

    Pawn[,] map;    // x,z
}

public class Point 
{
    public int x { get; private set; } = 0;
    public int y { get; private set; } = 0;
    public int z { get; private set; } = 0;

    public Point() 
    {
        x = 0; y = 0; z = 0;
    }

    public Point(int x, int z)
    {
        this.x = x;
        this.z = z;
    }

    public Point(int x, int y, int z)
    {
        this.x = x;
        this.y = y;
        this.z = z;
    }

    public void Add(int x, int z) 
    {
        this.x += x;
        this.z += z;
    }

    public void Add(int x, int y, int z)
    {
        this.x += x;
        this.y += y;
        this.z += z;
    }
}