using UnityEngine;
using System.Text;
using System.Collections.Generic;
using UnityEditor;
using System.Data;
using UnityEditor.Experimental.GraphView;

public class MapGen : MonoBehaviour
{
public GameObject Tile;
public GameObject RightPath;
public GameObject LeftPath;
public GameObject StraightPath;

public GameObject[] StartPaths;
public GameObject[] BossPaths;

//Node types
public GameObject BasicFight;
public int chanceBasicFight;
public GameObject Shop;
public int chanceShop;
public GameObject Event;
public int chanceEvent;
public GameObject Campfire;
public int chanceCampfire;
public GameObject EliteFight;
public int chanceEliteFight;
public GameObject Chest;
public int chanceChest;

public GameObject Start;
public GameObject Boss;


    class Node
    {
        public bool inMap;
        public bool rightPath;
        public bool leftPath;
        public bool straightPath;
        public string type;
        public int StartPath;
        public int BossPath;

        public Node()
        {
            this.inMap = false;
            this.rightPath = false;
            this.leftPath = false;
            this.type = "None";
            this.straightPath = false;
            this.StartPath = -1;
            this.BossPath = -1;
        }
    }

    void DrawMap(Node[,] map, int floors, int collums, Vector3 bottomLeftCorner, int horizontalDistance, int verticalDistance)
    {
        float halfWidth = (collums-1)/2f;
        Debug.Log(collums);
        Debug.Log(halfWidth);

        Instantiate(Start, new Vector3(bottomLeftCorner.x + halfWidth*horizontalDistance, bottomLeftCorner.y + -2*verticalDistance, bottomLeftCorner.z), Quaternion.identity);
        Instantiate(Boss, new Vector3(bottomLeftCorner.x + halfWidth*horizontalDistance, bottomLeftCorner.y + (floors+1)*verticalDistance, bottomLeftCorner.z), Quaternion.identity);

        for (int floor = 0; floor < floors; floor++)
            for (int collum = 0; collum < collums; collum++)
            {
                if (map[collum, floor].inMap)
                {
                    if (map[collum, floor].leftPath)
                        Instantiate(LeftPath, new Vector3(bottomLeftCorner.x + collum*horizontalDistance, bottomLeftCorner.y + floor*verticalDistance, bottomLeftCorner.z), Quaternion.identity);
                    if (map[collum, floor].rightPath)
                        Instantiate(RightPath, new Vector3(bottomLeftCorner.x + collum*horizontalDistance, bottomLeftCorner.y + floor*verticalDistance, bottomLeftCorner.z), Quaternion.identity);
                    if (map[collum, floor].straightPath)
                        Instantiate(StraightPath, new Vector3(bottomLeftCorner.x + collum*horizontalDistance, bottomLeftCorner.y + floor*verticalDistance, bottomLeftCorner.z), Quaternion.identity);
                    if (map[collum, floor].BossPath != -1)
                        Instantiate(BossPaths[collum], new Vector3(bottomLeftCorner.x + collum*horizontalDistance, bottomLeftCorner.y + floor*verticalDistance, bottomLeftCorner.z), Quaternion.identity);
                    if (map[collum, floor].StartPath != -1)
                        Instantiate(StartPaths[collum], new Vector3(bottomLeftCorner.x + collum*horizontalDistance, bottomLeftCorner.y + floor*verticalDistance, bottomLeftCorner.z), Quaternion.identity);


                    int range;

                    if (floor < 4)
                        range = chanceBasicFight + chanceEvent + chanceShop;
                    else
                        range = chanceBasicFight + chanceCampfire + chanceEliteFight + chanceEvent + chanceShop + chanceChest;
                    int randomChance;

                    randomChance = Random.Range(1, range+1);

                    range = chanceBasicFight + chanceCampfire + chanceEliteFight + chanceEvent + chanceShop + chanceChest;
                    
                    if (floor == 0)
                        Instantiate(BasicFight, new Vector3(bottomLeftCorner.x + collum*horizontalDistance, bottomLeftCorner.y + floor*verticalDistance, bottomLeftCorner.z), Quaternion.identity);
                    else if (floor == floors-1)
                        Instantiate(Campfire, new Vector3(bottomLeftCorner.x + collum*horizontalDistance, bottomLeftCorner.y + floor*verticalDistance, bottomLeftCorner.z), Quaternion.identity);
                    else if (floor == floors/2)
                        Instantiate(Chest, new Vector3(bottomLeftCorner.x + collum*horizontalDistance, bottomLeftCorner.y + floor*verticalDistance, bottomLeftCorner.z), Quaternion.identity);
                    else if (randomChance > range - chanceEliteFight)
                        Instantiate(EliteFight, new Vector3(bottomLeftCorner.x + collum*horizontalDistance, bottomLeftCorner.y + floor*verticalDistance, bottomLeftCorner.z), Quaternion.identity);
                    else if (randomChance > range - chanceEliteFight - chanceCampfire)
                        Instantiate(Campfire, new Vector3(bottomLeftCorner.x + collum*horizontalDistance, bottomLeftCorner.y + floor*verticalDistance, bottomLeftCorner.z), Quaternion.identity);
                    else if (randomChance > range - chanceEliteFight - chanceCampfire - chanceChest)
                        Instantiate(Chest, new Vector3(bottomLeftCorner.x + collum*horizontalDistance, bottomLeftCorner.y + floor*verticalDistance, bottomLeftCorner.z), Quaternion.identity);
                    else if (randomChance > range - chanceBasicFight - chanceCampfire - chanceEliteFight - chanceChest)
                        Instantiate(BasicFight, new Vector3(bottomLeftCorner.x + collum*horizontalDistance, bottomLeftCorner.y + floor*verticalDistance, bottomLeftCorner.z), Quaternion.identity);
                    else if (randomChance > range - chanceBasicFight - chanceCampfire - chanceEliteFight - chanceEvent - chanceChest)
                        Instantiate(Event, new Vector3(bottomLeftCorner.x + collum*horizontalDistance, bottomLeftCorner.y + floor*verticalDistance, bottomLeftCorner.z), Quaternion.identity);
                    else if (randomChance > range - chanceBasicFight - chanceCampfire - chanceEliteFight - chanceEvent - chanceShop - chanceChest)
                        Instantiate(Shop, new Vector3(bottomLeftCorner.x + collum*horizontalDistance, bottomLeftCorner.y + floor*verticalDistance, bottomLeftCorner.z), Quaternion.identity);

                    //Instantiate(Tile, new Vector3(bottomLeftCorner.x + collum*horizontalDistance, bottomLeftCorner.y + floor*verticalDistance, bottomLeftCorner.z), Quaternion.identity);
                    

                }
            }
    }

    void LogMap(Node[,] map, int floors, int collums)
    {
        StringBuilder sb = new StringBuilder();

        // top floor first, so floor 0 is at the bottom
        for (int y = floors - 1; y >= 0; y--)
        {
            for (int x = 0; x < collums; x++)
            {
                Node n = map[x, y];

                if (!n.inMap)
                {
                    sb.Append(" . ");
                }
                else
                {
                    sb.Append(n.leftPath ? '\\' : ' ');
                    sb.Append(n.straightPath ? '|' : 'o');
                    sb.Append(n.rightPath ? '/' : ' ');
                }

                sb.Append(' ');
            }
            sb.AppendLine();
        }

        Debug.Log(sb.ToString());
    }

    void GeneratePath(Node[,] map, int pos, int floors, int collums)
    {
        int direction;

        for (int i = 0; i < floors; i++)
        {
            map[pos, i].inMap = true;
            int min = 0;
            int max = 3;
            if (i == 0)
                map[pos, i].StartPath = pos;
            if (pos == 0 || map[pos-1, i].rightPath == true)
            {
                min = 1;
            }
            if (pos == collums - 1 || map[pos+1, i].leftPath == true)
            {
                max = 2;
            }
            if (i < floors - 1)
            {
                direction = Random.Range(min, max);
                if (direction == 0)
                {
                    map[pos, i].leftPath = true;
                    pos -= 1;
                }
                else if (direction == 2)
                {
                    map[pos, i].rightPath = true;
                    pos += 1;
                }
                else
                {
                    map[pos, i].straightPath = true;
                }
            }
            else
                map[pos, i].BossPath = pos;

                
            }

    }


    public void GenerateMap(int floors, int collums, int numOfPaths, Vector3 bottomLeftCorner, int horizontalDistance, int verticalDistance)
    {
        Node[,] map = new Node[collums, floors];

        for (int x = 0; x < collums; x++)
            for (int y = 0; y < floors; y++)
                map[x, y] = new Node();
        
        int first = Random.Range(0, collums);

        GeneratePath(map, first, floors, collums);

        int start = Random.Range(0, collums);

        while (start == first)
        {
            start = Random.Range(0, collums);
        }
        
        GeneratePath(map, start, floors, collums);

        for (int i = 0; i < numOfPaths-2; i++)
        {
            start = Random.Range(0, collums);
            GeneratePath(map, start, floors, collums);
        }

        //LogMap(map, floors, collums);

        DrawMap(map, floors, collums, bottomLeftCorner, horizontalDistance, verticalDistance);

    }

    public void TestGen()
    {
        GenerateMap(12, 6, 5, new Vector3(0,0,0), 2, 2);
    }
}
