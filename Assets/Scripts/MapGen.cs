using UnityEngine;
using System.Text;
using System.Collections.Generic;
using Unity.VisualScripting;

public class MapGen : MonoBehaviour
{

    static Node[,] mapData;
    public static void ResetMapData()
    {
        mapData = null;
    }
    public GameObject Tile;
    public GameObject RightPath;
    public GameObject LeftPath;
    public GameObject StraightPath;

    public Vector3 bottomLeftCorner;
    public int floors;
    public int collums;
    public int horizontalDistance;
    public int verticalDistance;

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

    public Dictionary<string, int> nodeWeights;
    void Awake()
        {
            nodeWeights = new Dictionary<string, int> {
                {"BasicFight", chanceBasicFight}, 
                {"Shop", chanceShop}, 
                {"Event", chanceEvent}, 
                {"Campfire", chanceCampfire},
                {"EliteFight", chanceEliteFight},
                {"Chest", chanceChest}
            };

            TestGen();
        }

    public GameObject Start;
    public GameObject Boss;


    class Node
    {
        public bool inMap;
        public bool rightPath;
        public bool leftPath;
        public bool straightPath;
        public bool downRightPath;
        public bool downStraightPath;
        public bool downLeftPath;
        public string type;
        public int StartPath;
        public int BossPath;
        public List<string> cantBe;
        public List<string> childrenCantBe;

        public Node()
        {
            this.inMap = false;
            this.rightPath = false;
            this.straightPath = false;
            this.leftPath = false;
            this.downRightPath = false;
            this.downStraightPath = false;
            this.downLeftPath = false;
            this.type = "None";
            this.StartPath = -1;
            this.BossPath = -1;
            this.cantBe = new List<string>();
            this.childrenCantBe = new List<string>();
        }
    }

    void CreateNode(Node[,] map, int floor, int collum, string NodeType, Vector3 bottomLeftCorner,int horizontalDistance, int verticalDistance)
    {

        GameObject nodeObj = null;

        if (NodeType == "BasicFight")
            nodeObj = Instantiate(BasicFight, new Vector3(bottomLeftCorner.x + collum*horizontalDistance, bottomLeftCorner.y + floor*verticalDistance, bottomLeftCorner.z), Quaternion.identity);
        else if (NodeType == "Campfire")
            nodeObj = Instantiate(Campfire, new Vector3(bottomLeftCorner.x + collum*horizontalDistance, bottomLeftCorner.y + floor*verticalDistance, bottomLeftCorner.z), Quaternion.identity);
        else if (NodeType == "Chest")
            nodeObj = Instantiate(Chest, new Vector3(bottomLeftCorner.x + collum*horizontalDistance, bottomLeftCorner.y + floor*verticalDistance, bottomLeftCorner.z), Quaternion.identity);
        else if (NodeType == "EliteFight")
            nodeObj = Instantiate(EliteFight, new Vector3(bottomLeftCorner.x + collum*horizontalDistance, bottomLeftCorner.y + floor*verticalDistance, bottomLeftCorner.z), Quaternion.identity);
        else if (NodeType == "Event")
            nodeObj = Instantiate(Event, new Vector3(bottomLeftCorner.x + collum*horizontalDistance, bottomLeftCorner.y + floor*verticalDistance, bottomLeftCorner.z), Quaternion.identity);
        else if (NodeType == "Shop")
            nodeObj = Instantiate(Shop, new Vector3(bottomLeftCorner.x + collum*horizontalDistance, bottomLeftCorner.y + floor*verticalDistance, bottomLeftCorner.z), Quaternion.identity);

        if (nodeObj != null && nodeObj.TryGetComponent(out MapNode mapNode))
        {
            mapNode.Setup(floor, collum, NodeType);
        }


        if (map[collum, floor].leftPath)
        {
            if (NodeType == "EliteFight" || NodeType == "Shop" || NodeType == "Campfire")
                map[collum-1, floor+1].cantBe.Add(NodeType);
        }
        if (map[collum, floor].rightPath)
        {
            if (NodeType == "EliteFight" || NodeType == "Shop" || NodeType == "Campfire")
                map[collum+1, floor+1].cantBe.Add(NodeType);
        }
        if (map[collum, floor].straightPath)
        {
            if (NodeType == "EliteFight" || NodeType == "Shop" || NodeType == "Campfire")
                map[collum, floor+1].cantBe.Add(NodeType);
        }

        if (map[collum, floor].downStraightPath && map[collum, floor-1].rightPath)
            map[collum+1, floor].cantBe.Add(NodeType);
        if (map[collum, floor].downRightPath)
        {
            if (map[collum+1, floor-1].straightPath)
                map[collum+1, floor].cantBe.Add(NodeType);
            if (map[collum+1, floor-1].rightPath)
                map[collum+2, floor].cantBe.Add(NodeType);
        }
    }

    void InstantiatePathBehindIcons(GameObject path, Vector3 position)
    {
        GameObject pathObject = Instantiate(path, position, Quaternion.identity);
        foreach (SpriteRenderer spriteRenderer in pathObject.GetComponentsInChildren<SpriteRenderer>())
        {
            spriteRenderer.sortingOrder = -1;
        }
    }
    

    string ChooseNodeType(List<string> cantBe)
    {
        Dictionary<string, int> canBe = new Dictionary<string, int>{};

        foreach (KeyValuePair<string, int> pair in nodeWeights)
        {
            if (!cantBe.Contains(pair.Key))
            {
                canBe[pair.Key] = pair.Value;
            }
        }

        int total = 0;
        foreach (int w in canBe.Values)
        {
            total += w;
        }

        int roll = Random.Range(0, total);

        foreach (KeyValuePair<string, int> pair in canBe)
        {
            if (roll < pair.Value)
            {
                return pair.Key;
            }
            roll -= pair.Value;
        }

        return "BasicFight";

    }

    void DrawMap(Node[,] map, int floors, int collums, Vector3 bottomLeftCorner, int horizontalDistance, int verticalDistance)
    {
        float halfWidth = (collums-1)/2f;

        for (int floor = 0; floor < floors; floor++)
            for (int collum = 0; collum < collums; collum++)
            {
                if (map[collum, floor].inMap)
                {
                    if (map[collum, floor].leftPath)
                        InstantiatePathBehindIcons(LeftPath, new Vector3(bottomLeftCorner.x + collum*horizontalDistance, bottomLeftCorner.y + floor*verticalDistance, bottomLeftCorner.z));
                    if (map[collum, floor].rightPath)
                        InstantiatePathBehindIcons(RightPath, new Vector3(bottomLeftCorner.x + collum*horizontalDistance, bottomLeftCorner.y + floor*verticalDistance, bottomLeftCorner.z));
                    if (map[collum, floor].straightPath)
                        InstantiatePathBehindIcons(StraightPath, new Vector3(bottomLeftCorner.x + collum*horizontalDistance, bottomLeftCorner.y + floor*verticalDistance, bottomLeftCorner.z));
                    if (map[collum, floor].BossPath != -1)
                        InstantiatePathBehindIcons(BossPaths[collum], new Vector3(bottomLeftCorner.x + collum*horizontalDistance, bottomLeftCorner.y + floor*verticalDistance, bottomLeftCorner.z));
                    if (map[collum, floor].StartPath != -1)
                        InstantiatePathBehindIcons(StartPaths[collum], new Vector3(bottomLeftCorner.x + collum*horizontalDistance, bottomLeftCorner.y + floor*verticalDistance, bottomLeftCorner.z));
                }
            }

        Instantiate(Start, new Vector3(bottomLeftCorner.x + halfWidth*horizontalDistance, bottomLeftCorner.y + -2*verticalDistance, bottomLeftCorner.z), Quaternion.identity);
        GameObject bossObj = Instantiate(Boss, new Vector3(bottomLeftCorner.x + halfWidth * horizontalDistance, bottomLeftCorner.y + (floors + 1) * verticalDistance, bottomLeftCorner.z), Quaternion.identity);
        if (bossObj != null && bossObj.TryGetComponent(out MapNode bossNode))
        {
            bossNode.Setup(floors, 0, "Boss");
        }

        for (int floor = 0; floor < floors; floor++)
            for (int collum = 0; collum < collums; collum++)
            {
                if (map[collum, floor].inMap)
                {
                    if (map[collum, floor].type == "None")
                    {   
                        if (floor < 4)
                        {
                            map[collum, floor].cantBe.Add("EliteFight");
                            map[collum, floor].cantBe.Add("Campfire");
                        }
                        if (floor == floors-2)
                            map[collum, floor].cantBe.Add("Campfire");
                        if (floor == 0)                     
                            map[collum, floor].type = "BasicFight";
                        else if (floor == floors-1)
                            map[collum, floor].type = "Campfire";
                        else if (floor == floors/2)
                            map[collum, floor].type = "Chest";
                        else
                            map[collum, floor].type = ChooseNodeType(map[collum, floor].cantBe);
                    }

                    CreateNode(map, floor, collum, map[collum, floor].type, bottomLeftCorner, horizontalDistance, verticalDistance);
                }
            }
    }
    public static bool CanMove(int toCol, int toFloor)
    {
        if (RunManager.Instance == null) return true;

        int fromFloor = RunManager.Instance.currentFloor;
        int fromCol = RunManager.Instance.currentColumn;

        if (fromFloor == -1) return toFloor == 0;
        if (toFloor != fromFloor + 1) return false;
        if (mapData == null || fromCol < 0) return true;
        if (toFloor >= mapData.GetLength(1)) return true;

        Node from = mapData[fromCol, fromFloor];
        if (toCol == fromCol - 1) return from.leftPath;
        if (toCol == fromCol) return from.straightPath;
        if (toCol == fromCol + 1) return from.rightPath;

        return false;
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
                    map[pos, i+1].downRightPath = true;
                }
                else if (direction == 2)
                {
                    map[pos, i].rightPath = true;
                    pos += 1;
                    map[pos, i+1].downLeftPath = true;
                }
                else
                {
                    map[pos, i].straightPath = true;
                    map[pos, i+1].downStraightPath = true;
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


        DrawMap(map, floors, collums, bottomLeftCorner, horizontalDistance, verticalDistance);

        mapData = map;

    }

    public void TestGen()
    {
        if (mapData == null)
        GenerateMap(floors, collums, collums-1, bottomLeftCorner, horizontalDistance, verticalDistance);
        else
        DrawMap(mapData, floors, collums, bottomLeftCorner, horizontalDistance, verticalDistance);

    }
}
