using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Can store prefabs for GameMap to instance
public class MapConfig 
{

}

public class GameMap
{
    int width;      // x
    int length;     // z

    Tile[,] map;    // x,z

    GameObject floorPrefab;
    GameObject wallPrefab;

    public GameMap(GameObject floorPrefab, GameObject wallPrefab) 
    {
        width = 10;
        length = 10;

        this.floorPrefab = floorPrefab;
        this.wallPrefab = wallPrefab;

        GameObject floor = GameObject.Instantiate(floorPrefab);
        floor.transform.position = new Vector3(width / 2, 0, length / 2);
        floor.transform.localScale = new Vector3(floor.transform.localScale.x * width + 1, floor.transform.localScale.y * 1f, floor.transform.localScale.x * length + 1);

        GameObject ceiling = GameObject.Instantiate(floorPrefab);
        ceiling.transform.position = new Vector3(width / 2, 1, length / 2);
        ceiling.transform.rotation = Quaternion.Euler(180, 0, 0);
        ceiling.transform.localScale = new Vector3(ceiling.transform.localScale.x * width + 1, ceiling.transform.localScale.y * 1f, ceiling.transform.localScale.x * length + 1);

        map = new Tile[width, length];
        for (int i = 0; i < width; i++) 
        {
            for (int j = 0; j < length; j++) 
            {
                map[i, j] = new Tile(new Point(i, j));
            }
        }

        map[0, 0].SetPassable(true);

        InstaceEdges();
    }

    private void InstanceMap() 
    {
        GameObject g;
        foreach (Tile t in map) 
        {
            if (!t.isPassable)
            {
                g = GameObject.Instantiate(wallPrefab);
                g.transform.position = new Vector3(t.point.x, t.point.y + .5f, t.point.z);
            }
        }
    }

    private void InstaceEdges() 
    {
        GameObject g;
        // Instance edges
        for (int i = 0; i < width; i++)
        {
            g = GameObject.Instantiate(wallPrefab);
            g.transform.position = new Vector3(i, .5f, -1);

            g = GameObject.Instantiate(wallPrefab);
            g.transform.position = new Vector3(i, .5f, length);
        }

        for (int i = 0; i < length; i++)
        {
            g = GameObject.Instantiate(wallPrefab);
            g.transform.position = new Vector3(-1, .5f, i);

            g = GameObject.Instantiate(wallPrefab);
            g.transform.position = new Vector3(width, .5f, i);
        }
    }

    private void RandomWalk() 
    {

    }

    private class Tile
    {
        public bool isPassable { get; private set; } = false;
        readonly public Point point;

        public Tile(Point point) 
        {
            this.point = point;
        }

        public Tile(Point point, bool isPassable) 
        {
            this.point = point;
            this.isPassable = isPassable;
        }

        public void SetPassable(bool isPassable) 
        {
            this.isPassable = isPassable;
        }
    }
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