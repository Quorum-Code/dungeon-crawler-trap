using System;
using System.Collections;
using System.Collections.Generic;
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

        // DungeonLayout dl = config.Dungeon0();
        // InstanceDungeon(dl);
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
                if (s[i] == 'T')
                {
                    g = GameObject.Instantiate(dl.trapPrefab);
                    g.transform.SetParent(config.mapParent.transform);
                    g.transform.position = new Vector3(i, .5f, j);
                    Trap trap = g.GetComponent<Trap>();
                    trap.gameMap = this;
                    trap.point = new Point(i, j);

                    g = GameObject.Instantiate(dl.triggerPrefab);
                    g.transform.SetParent(config.mapParent.transform);
                    g.transform.position = new Vector3(i, .5f, j);
                    Trigger trigger = g.GetComponent<Trigger>();

                    GetTileAtPoint(trap.point).trigger = trigger;

                    trap.AddTrigger(trigger);
                    trigger.AddTrap(trap);
                }

                // Open tile
                if (s[i] == ' ' || s[i] == 'S' || s[i] == 'T')
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

        // Spawn enemies
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

        // Spawn potions
        foreach ((Point, GameObject) p in dl.potions) 
        {
            Point point = p.Item1;

            if (!inBounds(point))
                continue;

            g = GameObject.Instantiate(p.Item2);
            g.transform.SetParent(config.mapParent.transform);
            g.transform.position = new Vector3(point.x, 0.5f, point.z);
            PotionController pc = g.GetComponent<PotionController>();
            if (pc != null) 
            {
                Tile t = GetTileAtPoint(point);
                pc.Ready(point, this);
                t.pawn = pc.potionPawn;
            }
            yield return null;
        }

        isLoading = false;
    }

    public void MovePlayerToSpawn(PlayerPawn playerPawn) 
    {
        MovePawnTo(playerPawn, spawn);
    }

    public bool isPlayerAtPoint(Point point) 
    {
        Tile t = GetTileAtPoint(point);
        if (t != null && t.pawn != null && t.pawn.type == PawnType.Player)
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

            AudioSource audioSource = endObject.GetComponent<AudioSource>();
            if (audioSource != null) 
            {
                audioSource.Play();
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

        if (point.isEqual(end) && t != null && t.pawn != null && t.pawn.type == PawnType.Player) 
        {
            //Debug.Log("endFound()");
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
        Tile t = GetTileAtPoint(point);
        if (pawn != null && pawn.type == PawnType.Player && t.pawn != null && t.pawn.type == PawnType.Potion)
        {
            t.pawn.Interact(pawn);
            return true;
        }

        if (!inBounds(point) || !map[point.x, point.z].isPassable || map[point.x, point.z].pawn != null)
            return false;
        if (point == end && pawn.type != PawnType.Player)
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
        pawn.point.Set(nextPoint);
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