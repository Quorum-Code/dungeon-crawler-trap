using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
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

    readonly public Point spawn;

    public GameMap(GameObject floorPrefab, GameObject wallPrefab) 
    {
        width = 32;
        length = 32;

        this.floorPrefab = floorPrefab;
        this.wallPrefab = wallPrefab;

        GameObject floor = GameObject.Instantiate(floorPrefab);
        floor.transform.position = new Vector3(width / 2, 0, length / 2);
        floor.transform.localScale = new Vector3(floor.transform.localScale.x * width + 1, floor.transform.localScale.y * 1f, floor.transform.localScale.x * length + 1);

        //GameObject ceiling = GameObject.Instantiate(floorPrefab);
        //ceiling.transform.position = new Vector3(width / 2, 1, length / 2);
        //ceiling.transform.rotation = Quaternion.Euler(180, 0, 0);
        //ceiling.transform.localScale = new Vector3(ceiling.transform.localScale.x * width + 1, ceiling.transform.localScale.y * 1f, ceiling.transform.localScale.x * length + 1);

        map = new Tile[width, length];
        for (int i = 0; i < width; i++) 
        {
            for (int j = 0; j < length; j++) 
            {
                map[i, j] = new Tile(new Point(i, j), false);
            }
        }

        spawn = new Point(width/2, 0);
        RandomWalk();
        InstanceMap();
        InstaceEdges();
    }

    private class Walker 
    {
        GameMap map;
        public int x { get; private set; }
        public int z { get; private set; }
        public int dx { get; private set; }
        public int dz { get; private set; }
        public int steps = 0;

        public Walker(int x, int z, int dx, int dz, GameMap map) 
        {
            this.x = x;
            this.z = z;
            this.dx = dx;
            this.dz = dz;
            this.map = map;
        }

        public Walker(Walker walker) 
        {
            this.x = walker.x;
            this.z = walker.z;
            this.dx = walker.dx;
            this.dz = walker.dz;
            this.map = walker.map;
        }

        public bool Walk() 
        {
            steps++;
            if (steps > 100)
                return false;

            int nx = x + dx;
            int nz = z + dz;

            // Stop walking if found another path
            if (!map.inBounds(new Point(nx, nz)) || map.map[nx, nz].isPassable) 
            {
                Turn();
                nx = x + dx;
                nz = z + dz;
                if (!map.inBounds(new Point(nx, nz)) || map.map[nx, nz].isPassable)
                    return false;
            }
            x = nx;
            z = nz;
            map.map[nx, nz].SetPassable(true);
            return true;
        }

        public void Turn() 
        {
            float r = Random.Range(0f, 1f);

            if (r < .5f)
            {
                if (dx == 0)
                {
                    dz = 0;
                    dx = 1;
                }
                else 
                {
                    dx = 0;
                    dz = 1;
                }
            }
            else 
            {
                if (dx == 0)
                {
                    dz = 0;
                    dx = -1;
                }
                else
                {
                    dx = 0;
                    dz = -1;
                }
            }
        }
    }

    private void RandomWalk() 
    {
        List<Walker> walkers = new List<Walker>();
        walkers.Add(new Walker(width / 2, 0, 0, 1, this));
        walkers.Add(new Walker(width / 2, 1, 1, 0, this));
        walkers.Add(new Walker(width / 2, 1, -1, 0, this));
        walkers.Add(new Walker(width / 2, 1, 0, 1, this));

        while (walkers.Count > 0) 
        {
            List<Walker> toRemove = new List<Walker>();
            List<Walker> toAdd = new List<Walker>();

            float r = 0f;
            foreach (Walker walker in walkers) 
            {
                r = Random.Range(0f, 1f);

                if (r < .15f)
                {
                    Walker w = new Walker(walker);
                    w.Turn();
                    toAdd.Add(w);
                }
                else if (r < .3f) 
                {
                    walker.Turn();
                }

                if (!walker.Walk())
                    toRemove.Add(walker);
            }

            foreach (Walker walker in toRemove) 
            {
                walkers.Remove(walker);
            }
            walkers.AddRange(toAdd);
        }
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

    public bool inBounds(int x, int y, int z) 
    {
        if (x < 0 || x >= width)
            return false;
        if (z < 0 || z >= length)
            return false;

        return true;
    }

    public bool inBounds(Point point) 
    {
        return inBounds(point.x, point.y, point.z);
    }

    public bool canMoveTo(Point point) 
    {
        if (!inBounds(point) || !map[point.x, point.z].isPassable)
            return false;
        return true;
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

    public void Set(Point point) 
    {
        x = point.x;
        y = point.y;
        z = point.z;
    }
}