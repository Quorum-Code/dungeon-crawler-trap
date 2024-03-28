using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapConfig : MonoBehaviour
{
    public GameObject mapParent;

    public GameObject[] tilePrefabs;
    public GameObject[] wallPrefabs;

    public GameObject[] triggerPrefabs;
    public GameObject[] trapPrefabs;

    public GameObject[] enemyPrefabs;

    private DungeonLayout[] dungeons;

    public DungeonLayout Dungeon0() 
    {
        DungeonLayout d = new DungeonLayout(tilePrefabs[0], wallPrefabs[0]);

        d.AddLine("XXXXX");
        d.AddLine("X   X");
        d.AddLine("X X X");
        d.AddLine("X   X");
        d.AddLine("X S X");
        d.AddLine("XXXXX");

        d.triggerPrefab = triggerPrefabs[0];
        d.trapPrefab = trapPrefabs[0];
        d.trapNetwork = new DungeonLayout.TrapNetwork();
        d.trapNetwork.traps.Add(new Point(1, 4));
        d.trapNetwork.triggers.Add(new Point(1, 4));

        d.AddEnemy(new Point(2, 4), enemyPrefabs[0]);

        return d;
    }

    public class DungeonLayout 
    {
        public string title;

        public GameObject tilePrefab;
        public GameObject wallPrefab;
        public GameObject triggerPrefab;
        public GameObject trapPrefab;
        public TrapNetwork trapNetwork;

        public int width = -1;
        public int length = -1;

        public List<(Point, GameObject)> enemies = new List<(Point, GameObject)>();

        // Layout keys
        // ---------------
        // 'X' -> wall
        // ' ' -> open tile
        // 'S' -> spawn
        public List<string> layout = new List<string>();

        public DungeonLayout(GameObject tilePrefab, GameObject wallPrefab) 
        {
            this.tilePrefab = tilePrefab;
            this.wallPrefab = wallPrefab;
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
