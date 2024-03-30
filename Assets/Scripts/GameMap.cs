using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using static MapConfig;

public class GameMap
{
    public bool isLoading = true;
    MapConfig config;

    int width;      // x
    int length;     // z

    Tile[,] map;    // x,z

    public Point spawn;
    public Point end;
    public List<EnemyPawn> enemies;

    public delegate void MoveEvent();
    public MoveEvent endFound;

    private GameObject endObject;

    public GameMap(MapConfig mapConfig)
    {
        config = mapConfig;

        width = 32;
        length = 32;

        map = new Tile[width, length];
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < length; j++)
            {
                map[i, j] = new Tile(new Point(i, j), false);
            }
        }

        DungeonLayout dl = config.Dungeon0();
        InstanceDungeon(dl);
    }

    public IEnumerator LoadDungeon(DungeonLayout dl) 
    {
        isLoading = true;

        ClearDungeon();

        width = dl.GetWidth();
        length = dl.GetLength();

        PopulateMap();

        // Instance tiles
        GameObject g;
        for (int j = 0; j < dl.layout.Count; j++)
        {
            char[] s = dl.layout[j].ToCharArray();
            for (int i = 0; i < s.Length; i++)
            {
                // Open tile
                if (s[i] == ' ' || s[i] == 'S')
                {
                    g = GameObject.Instantiate(dl.tilePrefab);
                    map[i, j].SetPassable(true);
                }
                // Wall tile
                else if (s[i] == 'E')
                {
                    g = GameObject.Instantiate(dl.exitPrefab);
                    endObject = g;
                    map[i, j].SetPassable(true);
                    end = new Point(i, j);
                }
                else
                {
                    g = GameObject.Instantiate(dl.wallPrefab);
                    map[i, j].SetPassable(false);
                }
                g.transform.SetParent(config.mapParent.transform);
                g.transform.position = new Vector3(i, .5f, j);

                // Set spawn point
                if (s[i] == 'S')
                {
                    spawn = new Point(i, j);
                }
                yield return null;
            }
        }

        if (dl.trapNetwork != null)
        {
            List<Trap> traps = new List<Trap>();
            List<Trigger> triggers = new List<Trigger>();
            foreach (Point point in dl.trapNetwork.traps)
            {
                g = GameObject.Instantiate(dl.trapPrefab);
                g.transform.SetParent(config.mapParent.transform);
                g.transform.position = new Vector3(point.x, 0.5f, point.z);
                Trap t = g.GetComponent<Trap>();
                if (t != null)
                {
                    traps.Add(t);
                    t.gameMap = this;
                    t.point = point;
                }
                yield return null;
            }

            foreach (Point point in dl.trapNetwork.triggers)
            {
                g = GameObject.Instantiate(dl.triggerPrefab);
                g.transform.SetParent(config.mapParent.transform);
                g.transform.position = new Vector3(point.x, 0.5f, point.z);
                Trigger t = g.GetComponent<Trigger>();
                if (t != null)
                {
                    t.point = point;
                    triggers.Add(t);
                    t.gameMap = this;
                    t.AddTraps(traps);
                    Tile tile = GetTileAtPoint(point);
                    if (tile != null)
                    {
                        tile.trigger = t;
                    }
                }
                yield return null;
            }

            foreach (Trap t in traps)
            {
                t.AddTriggers(triggers);
            }
        }

        foreach ((Point, GameObject) e in dl.enemies)
        {
            Point point = e.Item1;

            if (!inBounds(point))
                continue;

            g = GameObject.Instantiate(e.Item2);
            g.transform.SetParent(config.mapParent.transform);
            g.transform.position = new Vector3(point.x, 0.5f, point.z);
            EnemyController ec = g.GetComponent<EnemyController>();
            if (ec != null)
            {
                Tile t = GetTileAtPoint(point);
                ec.Ready(point, this);
                t.pawn = ec.enemyPawn;
            }
            yield return null;
        }

        isLoading = false;
    }

    private void InstanceDungeon(DungeonLayout dl)
    {
        ClearDungeon();

        width = dl.GetWidth();
        length = dl.GetLength();

        PopulateMap();

        GameObject g;
        for (int j = 0; j < dl.layout.Count; j++)
        {
            char[] s = dl.layout[j].ToCharArray();
            for (int i = 0; i < s.Length; i++)
            {
                // Open tile
                if (s[i] == ' ' || s[i] == 'S')
                {
                    g = GameObject.Instantiate(dl.tilePrefab);
                    map[i, j].SetPassable(true);
                }
                // Wall tile
                else if (s[i] == 'E') 
                {
                    g = GameObject.Instantiate(dl.exitPrefab);
                    endObject = g;
                    map[i, j].SetPassable(true);
                    end = new Point(i, j);
                }
                else
                {
                    g = GameObject.Instantiate(dl.wallPrefab);
                    map[i, j].SetPassable(false);
                }
                g.transform.SetParent(config.mapParent.transform);
                g.transform.position = new Vector3(i, .5f, j);

                // Set spawn point
                if (s[i] == 'S')
                {
                    spawn = new Point(i, j);
                }
            }
        }

        if (dl.trapNetwork != null) 
        {
            List<Trap> traps = new List<Trap>();
            List<Trigger> triggers = new List<Trigger>();
            foreach (Point point in dl.trapNetwork.traps)
            {
                g = GameObject.Instantiate(dl.trapPrefab);
                g.transform.SetParent(config.mapParent.transform);
                g.transform.position = new Vector3(point.x, 0.5f, point.z);
                Trap t = g.GetComponent<Trap>();
                if (t != null)
                {
                    traps.Add(t);
                    t.gameMap = this;
                    t.point = point;
                }
            }

            foreach (Point point in dl.trapNetwork.triggers)
            {
                g = GameObject.Instantiate(dl.triggerPrefab);
                g.transform.SetParent(config.mapParent.transform);
                g.transform.position = new Vector3(point.x, 0.5f, point.z);
                Trigger t = g.GetComponent<Trigger>();
                if (t != null)
                {
                    t.point = point;
                    triggers.Add(t);
                    t.gameMap = this;
                    t.AddTraps(traps);
                    Tile tile = GetTileAtPoint(point);
                    if (tile != null)
                    {
                        tile.trigger = t;
                    }
                }
            }

            foreach (Trap t in traps)
            {
                t.AddTriggers(triggers);
            }
        }

        foreach ((Point, GameObject) e in dl.enemies) 
        {
            Point point = e.Item1;

            if (!inBounds(point))
                continue;

            g = GameObject.Instantiate(e.Item2);
            g.transform.SetParent(config.mapParent.transform);
            g.transform.position = new Vector3(point.x, 0.5f, point.z);
            EnemyController ec = g.GetComponent<EnemyController>();
            if (ec != null) 
            {
                Tile t = GetTileAtPoint(point);
                ec.Ready(point, this);
                t.pawn = ec.enemyPawn;
                Debug.Log("enemyController found");
            }
        }
    }

    public bool isPlayerAtPoint(Point point) 
    {
        Tile t = GetTileAtPoint(point);
        if (t != null && t.pawn != null && t.pawn.isPlayer)
        {
            return true;
        }
        return false;
    }

    public bool isPawnAtPoint(Point point) 
    {
        Tile t = GetTileAtPoint(point);
        if (t != null && t.pawn != null) 
        {
            return true;
        }
        return false;
    }

    private void PopulateMap()
    {
        map = new Tile[width, length];
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < length; j++)
            {
                map[i, j] = new Tile(new Point(i, j));
            }
        }
    }

    private void ClearDungeon()
    {
        foreach (Transform child in config.mapParent.transform) 
        {
            GameObject.Destroy(child.gameObject);
        }
    }

    public void EndMove() 
    {

    }

    public void CheckIsEnd(PlayerPawn playerPawn) 
    {
        if (playerPawn.point.isEqual(end)) 
        {
            Animator animator = endObject.GetComponent<Animator>();
            if (animator != null) 
            {
                animator.Play("OpenDoor");
            }
        }
    }

    public void NotifyEnemies(PlayerPawn playerPawn)
    {
        NotifyDirection(playerPawn, 0, 1);
        NotifyDirection(playerPawn, 0, -1);
        NotifyDirection(playerPawn, 1, 0);
        NotifyDirection(playerPawn, -1, 0);
    }

    private void NotifyDirection(PlayerPawn playerPawn, int x, int z) 
    {
        for (int i = 1; i <= 5; i++)
        {
            Point point = new Point(playerPawn.point.x + (i * x), playerPawn.point.z + (i * z));
            Tile t = GetTileAtPoint(point);
            if (!inBounds(point) || !t.isPassable)
                break;
            else if (t.pawn != null)
            {
                t.pawn.Notify(playerPawn);
            }
        }
    }

    public void AddEnemy(Point point, GameObject enemyPrefab) 
    {
        Tile t = GetTileAtPoint(point);
        if (t != null && t.pawn == null) 
        {
            GameObject g = GameObject.Instantiate(enemyPrefab);
            g.transform.position = new Vector3(point.x, 0.5f, point.z);
            g.transform.SetParent(config.mapParent.transform);

            EnemyPawn enemyPawn = new EnemyPawn(point.x, point.z, g, this);
            t.pawn = enemyPawn;
        }
    }

    public void FinishMove(Point point) 
    {
        Tile t = GetTileAtPoint(point);
        if (t != null && t.trigger) 
        {
            t.trigger.Activate();
        }

        if (t.pawn != null && t.pawn.health <= 0) 
        {
            t.pawn = null;
        }

        if (point.isEqual(end) && t != null && t.pawn != null && t.pawn.isPlayer) 
        {
            Debug.Log("endFound()");
            endFound();
        }
    }

    private Tile GetTileAtPoint(Point point) 
    {
        if (!inBounds(point))
        {
            return null;
        }
        else 
        {
            return map[point.x, point.z];
        }
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

        [Obsolete("Random dungeon generation no longer s")]
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

            float r = UnityEngine.Random.Range(0f, 1f);
            if (r < .1f) 
            {
                GameObject g = GameObject.Instantiate(map.config.triggerPrefabs[0]);
                g.transform.SetParent(map.config.mapParent.transform);
                g.transform.position = new Vector3(x, 0.5f, z);
                Trigger trig = g.GetComponent<Trigger>();
                map.map[nx, nz].trigger = trig;

                g = GameObject.Instantiate(map.config.trapPrefabs[0]);
                g.transform.SetParent(map.config.mapParent.transform);
                g.transform.position = new Vector3(x, 0.5f, z);
                Trap trap = g.GetComponent<Trap>();

                trap.point = new Point(x, z);
                trig.point = new Point(x, z);

                trap.gameMap = map;
                trig.gameMap = map;

                //trap.triggers.Add(trig);
                //trig.traps.Add(trap);
            }

            return true;
        }

        public void Turn() 
        {
            float r = UnityEngine.Random.Range(0f, 1f);

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
        map[width / 2, 0].SetPassable(true);
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
                r = UnityEngine.Random.Range(0f, 1f);
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
                g = GameObject.Instantiate(config.wallPrefabs[0]);
                g.transform.SetParent(config.mapParent.transform);
                g.transform.position = new Vector3(t.point.x, t.point.y + .5f, t.point.z);
            }
            else 
            {
                g = GameObject.Instantiate(config.tilePrefabs[0]);
                g.transform.SetParent(config.mapParent.transform);
                g.transform.position = new Vector3(t.point.x, t.point.y + .5f, t.point.z);
            }

            if (t.trigger != null) 
            {
                g = GameObject.Instantiate(config.triggerPrefabs[0]);
                g.transform.SetParent(config.mapParent.transform);
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
            g = GameObject.Instantiate(config.wallPrefabs[0]);
            g.transform.SetParent(config.mapParent.transform);
            g.transform.position = new Vector3(i, .5f, -1);

            g = GameObject.Instantiate(config.wallPrefabs[0]);
            g.transform.SetParent(config.mapParent.transform);
            g.transform.position = new Vector3(i, .5f, length);
        }

        for (int i = 0; i < length; i++)
        {
            g = GameObject.Instantiate(config.wallPrefabs[0]);
            g.transform.SetParent(config.mapParent.transform);
            g.transform.position = new Vector3(-1, .5f, i);

            g = GameObject.Instantiate(config.wallPrefabs[0]);
            g.transform.SetParent(config.mapParent.transform);
            g.transform.position = new Vector3(width, .5f, i);
        }
    }

    private class Tile
    {
        public Pawn pawn = null;
        public Trigger trigger;
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
        if (point == null)
            return false;
        return inBounds(point.x, point.y, point.z);
    }

    public bool canMoveTo(Pawn pawn, Point point) 
    {
        if (!inBounds(point) || !map[point.x, point.z].isPassable || map[point.x, point.z].pawn != null)
            return false;
        if (point == end && !pawn.isPlayer)
            return false;
        return true;
    }

    public void MovePawnTo(Pawn pawn, Point nextPoint)
    {
        if (inBounds(pawn.point)) 
        {
            if (map[pawn.point.x, pawn.point.z].trigger != null) 
            {
                map[pawn.point.x, pawn.point.z].trigger.Release();
            }
            map[pawn.point.x, pawn.point.z].pawn = null;
        }

        if (inBounds(nextPoint)) 
        {
            map[nextPoint.x, nextPoint.z].pawn = pawn;
        }
    }

    public void RemovePawnAtPoint(Point point) 
    {
        Tile t = GetTileAtPoint(point);
        if (t != null) 
        {
            t.pawn = null;
        }
    }

    public Pawn GetPawnAtPoint(Point point) 
    {
        if (!inBounds(point))
            return null;
        return map[point.x, point.z].pawn;
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

    public bool isEqual(Point point) 
    {
        if (x == point.x && y == point.y && z == point.z)
            return true;
        return false;
    }

    public static bool operator== (Point point1, Point point2) 
    {
        if (point1 is null && point2 is null)
            return true;

        if (point1 is null || point2 is null)
            return false;

        if (point1.x == point2.x && point1.y == point2.y && point1.z == point2.z)
            return true;
        return false;
    }

    public static bool operator !=(Point point1, Point point2)
    {
        return !(point1 == point2);
    }
}