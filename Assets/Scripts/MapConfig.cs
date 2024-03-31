using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapConfig : MonoBehaviour
{
    public GameObject mapParent;

    public GameObject[] tilePrefabs;
    public GameObject[] wallPrefabs;
    public GameObject exitPrefab;

    public GameObject[] triggerPrefabs;
    public GameObject[] trapPrefabs;

    public GameObject[] enemyPrefabs;
    public GameObject[] potionPrefabs;

    private DungeonLayout[] dungeons;

    public DungeonLayout DungeonByLevel(int level) 
    {
        if (level == 0)
            return Dungeon6();
        if (level == 1)
            return Dungeon1();
        if (level == 2)
            return Dungeon2();
        if (level == 3)
            return Dungeon3();
        if (level == 4)
            return Dungeon4();
        if (level == 5)
            return Dungeon5();

        return null;
    }

    public DungeonLayout Dungeon0() 
    {
        DungeonLayout d = new DungeonLayout(tilePrefabs[0], wallPrefabs[0], exitPrefab);

        d.AddLine("XEX");
        d.AddLine("X X");
        d.AddLine("X X");
        d.AddLine("XSX");
        d.AddLine("XXX");

        return d;
    }

    public DungeonLayout Dungeon1() 
    {
        DungeonLayout d = new DungeonLayout(tilePrefabs[0], wallPrefabs[0], exitPrefab);

        d.AddLine("XXXEXXX");
        d.AddLine("X     X");
        d.AddLine("X     X");
        d.AddLine("X     X");
        d.AddLine("X     X");
        d.AddLine("X  S  X");
        d.AddLine("XXXXXXX");

        d.triggerPrefab = triggerPrefabs[0];
        d.trapPrefab = trapPrefabs[0];
        d.trapNetwork = new DungeonLayout.TrapNetwork();

        d.trapNetwork.traps.Add(new Point(1, 4));
        d.trapNetwork.triggers.Add(new Point(1, 4));
        d.trapNetwork.traps.Add(new Point(2, 4));
        d.trapNetwork.triggers.Add(new Point(2, 4));
        d.trapNetwork.traps.Add(new Point(3, 4));
        d.trapNetwork.triggers.Add(new Point(3, 4));
        d.trapNetwork.traps.Add(new Point(4, 4));
        d.trapNetwork.triggers.Add(new Point(4, 4));
        d.trapNetwork.traps.Add(new Point(5, 4));
        d.trapNetwork.triggers.Add(new Point(5, 4));

        d.AddEnemy(new Point(3, 5), enemyPrefabs[0]);

        d.AddPotion(new Point(1, 1), potionPrefabs[0]);

        return d;
    }

    public DungeonLayout Dungeon2()
    {
        DungeonLayout d = new DungeonLayout(tilePrefabs[0], wallPrefabs[0], exitPrefab);

        d.AddLine("XXXXXXX");
        d.AddLine("X XXXEX");
        d.AddLine("X     X");
        d.AddLine("X XXXXX");
        d.AddLine("X X");
        d.AddLine("XSX");
        d.AddLine("XXX");

        d.triggerPrefab = triggerPrefabs[0];
        d.trapPrefab = trapPrefabs[0];
        d.trapNetwork = new DungeonLayout.TrapNetwork();

        d.trapNetwork.traps.Add(new Point(1, 5));
        d.trapNetwork.triggers.Add(new Point(1, 5));

        d.AddEnemy(new Point(1, 4), enemyPrefabs[0]);

        return d;
    }

    public DungeonLayout Dungeon3()
    {
        DungeonLayout d = new DungeonLayout(tilePrefabs[0], wallPrefabs[0], exitPrefab);

        d.AddLine("XXXXXXXXXXXEXX"); // 2
        d.AddLine("X   T  X     X"); // 1
        d.AddLine("X      X XXXXX"); // 0
        d.AddLine("X  T   X     X"); // 9
        d.AddLine("X XXXXXXXX   X"); // 8
        d.AddLine("X            X"); // 7
        d.AddLine("X XXXXXX XXXXX"); // 6
        d.AddLine("X      X X   X"); // 5
        d.AddLine("X  XXX XXX X X"); // 4
        d.AddLine("X    X     X X"); // 3
        d.AddLine("XX X X XXXXX X"); // 2
        d.AddLine("X  X XSX     X"); // 1
        d.AddLine("XXXXXXXXXXXXXX"); // 0

        d.triggerPrefab = triggerPrefabs[0];
        d.trapPrefab = trapPrefabs[0];
        d.trapNetwork = new DungeonLayout.TrapNetwork();

        d.AddEnemy(new Point(5, 10), enemyPrefabs[0]);
        d.AddPotion(new Point(6, 11), potionPrefabs[0]);

        d.AddPotion(new Point(8, 1), potionPrefabs[0]);


        return d;
    }

    public DungeonLayout Dungeon4()
    {
        DungeonLayout d = new DungeonLayout(tilePrefabs[0], wallPrefabs[0], exitPrefab);

        d.AddLine("XXXXEXXXX");
        d.AddLine("X       X");
        d.AddLine("X T T T X");
        d.AddLine("X       X");
        d.AddLine("X T T T X");
        d.AddLine("X       X");
        d.AddLine("X T T T X");
        d.AddLine("X   S   X");
        d.AddLine("XXXXXXXXX");

        d.triggerPrefab = triggerPrefabs[0];
        d.trapPrefab = trapPrefabs[0];
        d.trapNetwork = new DungeonLayout.TrapNetwork();

        d.AddEnemy(new Point(1, 5), enemyPrefabs[0]);
        d.AddEnemy(new Point(2, 5), enemyPrefabs[0]);
        d.AddEnemy(new Point(3, 5), enemyPrefabs[0]);
        d.AddEnemy(new Point(4, 5), enemyPrefabs[0]);
        d.AddEnemy(new Point(5, 5), enemyPrefabs[0]);

        return d;
    }

    public DungeonLayout Dungeon5()
    {
        DungeonLayout d = new DungeonLayout(tilePrefabs[0], wallPrefabs[0], exitPrefab);

        d.AddLine("XXXXXXXXXXXXXEX"); // 2
        d.AddLine("X T           X"); // 1
        d.AddLine("X XTXTXTXTXTXXX"); // 0
        d.AddLine("X   T       X"); // 9
        d.AddLine("XTX X XTXTXTX"); // 8
        d.AddLine("X   T   T   X"); // 7
        d.AddLine("X XTXTX XTXTX"); // 6
        d.AddLine("X   T       X"); // 5
        d.AddLine("XTX XTXTX XTX"); // 4
        d.AddLine("X     T     X"); // 3
        d.AddLine("X XTX X XTXTX"); // 2
        d.AddLine("X     S     X"); // 1
        d.AddLine("XXXXXXXXXXXXX"); // 0

        d.triggerPrefab = triggerPrefabs[0];
        d.trapPrefab = trapPrefabs[0];
        d.trapNetwork = new DungeonLayout.TrapNetwork();

        d.AddPotion(new Point(1, 11), potionPrefabs[0]);

        return d;
    }

    public DungeonLayout Dungeon6()
    {
        DungeonLayout d = new DungeonLayout(tilePrefabs[0], wallPrefabs[0], exitPrefab);

        d.AddLine("XXXXXXXEXXXXXXXX"); // 6
        d.AddLine("XXXXXX   XXXXX X"); // 5
        d.AddLine("XXXXX     XXXX X"); // 4
        d.AddLine("X              X"); // 3
        d.AddLine("X X X     XXXX X"); // 2
        d.AddLine("X X XX T XXXXX X"); // 1
        d.AddLine("X X XXXXXXXXXX X"); // 0
        d.AddLine("XTX XXXXT XXXX X"); // 9
        d.AddLine("X X XX         X"); // 8
        d.AddLine("X   XX X TXXXXXX"); // 7
        d.AddLine("X X XX XXXXX"); // 6
        d.AddLine("X X XX     X"); // 5
        d.AddLine("X XTXX XXX X"); // 4
        d.AddLine("X    X XXX X"); // 3
        d.AddLine("X    X XXX X"); // 2
        d.AddLine("X    S     X"); // 1
        d.AddLine("XXXXXXXXXXXX"); // 0

        d.triggerPrefab = triggerPrefabs[0];
        d.trapPrefab = trapPrefabs[0];
        d.trapNetwork = new DungeonLayout.TrapNetwork();

        d.AddPotion(new Point(1, 11), potionPrefabs[0]);

        d.AddEnemy(new Point(7, 15), enemyPrefabs[1]);

        return d;
    }

    public class DungeonLayout 
    {
        public string title;

        public GameObject tilePrefab;
        public GameObject wallPrefab;
        public GameObject exitPrefab;
        public GameObject triggerPrefab;
        public GameObject trapPrefab;
        public TrapNetwork trapNetwork;

        public int width = -1;
        public int length = -1;

        public List<(Point, GameObject)> enemies = new List<(Point, GameObject)>();
        public List<(Point, GameObject)> potions = new List<(Point, GameObject)>();

        // Layout keys
        // ---------------
        // 'X' -> wall
        // ' ' -> open tile
        // 'S' -> spawn
        // 'E' -> exit
        public List<string> layout = new List<string>();

        public DungeonLayout(GameObject tilePrefab, GameObject wallPrefab, GameObject exitPrefab) 
        {
            this.tilePrefab = tilePrefab;
            this.wallPrefab = wallPrefab;
            this.exitPrefab = exitPrefab;
        }

        public void AddLine(string line) 
        {
            layout.Insert(0, line);
        }

        public int GetWidth() 
        {
            if (width == -1)
            {
                GetWidthAndLength();
            }
            return width;
        }

        public int GetLength() 
        {
            if (length == -1) 
            {
                GetWidthAndLength();
            }
            return length;
        }

        private void GetWidthAndLength() 
        {
            length = layout.Count;

            foreach (string s in layout) 
            {
                if (s.Length > width)
                    width = s.Length;
            }
        }

        public void AddEnemy(Point point, GameObject enemyPrefab) 
        {
            enemies.Add((point, enemyPrefab));
        }

        public void AddPotion(Point point, GameObject potionPrefab) 
        {
            potions.Add((point, potionPrefab));
        }

        public class TrapNetwork 
        {
            public List<Point> traps = new List<Point>();
            public List<Point> triggers = new List<Point>();

            public TrapNetwork() 
            {

            }
        }
    }
}
