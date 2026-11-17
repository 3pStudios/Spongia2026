using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MapGen : MonoBehaviour
{
    static Node[,] mapData;

    public static void ResetMapData()
    {
        mapData = null;
    }

    [Header("Scroll View References")]
    public ScrollRect mapScrollRect;
    public RectTransform contentRect;
    public RectTransform pathsContainer;
    public RectTransform nodesContainer;

    [Header("Layout Settings (UI Pixels)")]
    public int floors = 12;
    public int collums = 7;
    public float horizontalDistance = 150f;
    public float verticalDistance = 180f;
    public float bottomPadding = 160f;
    public float topPadding = 160f;

    [Header("Path Line Settings")]
    public Color pathColor = new Color(0.99f, 0.89f, 0.71f, 1f); // Cream
    public float pathThickness = 14f;
    [Tooltip("Optional: leave empty for a solid cream line, or assign a UI Image prefab.")]
    public GameObject pathLinePrefab;

    [Header("Special Node Prefabs")]
    public GameObject StartNode;
    public GameObject Boss;

    [Header("Node Types & Weights")]
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

    private class Node
    {
        public bool inMap;
        public bool rightPath;
        public bool leftPath;
        public bool straightPath;
        public bool downRightPath;
        public bool downStraightPath;
        public bool downLeftPath;
        public string type = "None";
        public int StartPath = -1;
        public int BossPath = -1;
        public List<string> cantBe = new List<string>();
    }

    private void Awake()
    {
        nodeWeights = new Dictionary<string, int>
        {
            { "BasicFight", chanceBasicFight },
            { "Shop", chanceShop },
            { "Event", chanceEvent },
            { "Campfire", chanceCampfire },
            { "EliteFight", chanceEliteFight },
            { "Chest", chanceChest }
        };

        TestGen();
    }

    private void Start()
    {
        ScrollToCurrentFloor();
    }

    private Vector2 GetAnchoredPos(float col, float floor)
    {
        float halfWidth = (collums - 1) / 2f;
        float x = (col - halfWidth) * horizontalDistance;
        float y = bottomPadding + (floor + 2f) * verticalDistance;
        return new Vector2(x, y);
    }

    private GameObject SpawnUIElement(GameObject prefab, RectTransform parent, Vector2 anchoredPos)
    {
        GameObject obj = Instantiate(prefab, parent);
        if (obj.TryGetComponent(out RectTransform rt))
        {
            rt.anchorMin = new Vector2(0.5f, 0f);
            rt.anchorMax = new Vector2(0.5f, 0f);
            rt.anchoredPosition3D = new Vector3(anchoredPos.x, anchoredPos.y, 0f);
        }
        return obj;
    }

    private void SpawnLine(Vector2 from, Vector2 to)
    {
        GameObject lineObj;
        if (pathLinePrefab != null)
        {
            lineObj = Instantiate(pathLinePrefab, pathsContainer);
        }
        else
        {
            lineObj = new GameObject("PathLine", typeof(RectTransform), typeof(Image));
            lineObj.transform.SetParent(pathsContainer, false);
        }

        Image img = lineObj.GetComponent<Image>();
        if (img != null)
        {
            img.color = pathColor;
            img.raycastTarget = false;
        }

        RectTransform rt = lineObj.GetComponent<RectTransform>();
        Vector2 dir = to - from;
        float length = dir.magnitude;
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;

        rt.anchorMin = new Vector2(0.5f, 0f);
        rt.anchorMax = new Vector2(0.5f, 0f);
        rt.pivot = new Vector2(0f, 0.5f);
        rt.anchoredPosition3D = new Vector3(from.x, from.y, 0f);
        rt.sizeDelta = new Vector2(length, pathThickness);
        rt.localRotation = Quaternion.Euler(0f, 0f, angle);
        rt.localScale = Vector3.one;
    }

    private void ResizeContent()
    {
        if (contentRect == null) return;

        float totalHeight = bottomPadding + (floors + 3) * verticalDistance + topPadding;
        contentRect.sizeDelta = new Vector2(contentRect.sizeDelta.x, totalHeight);
    }

    public void ScrollToCurrentFloor()
    {
        if (mapScrollRect == null || RunManager.Instance == null) return;

        Canvas.ForceUpdateCanvases();
        int floor = Mathf.Max(0, RunManager.Instance.currentFloor);
        float t = Mathf.Clamp01((float)floor / Mathf.Max(1, floors));
        mapScrollRect.verticalNormalizedPosition = t;
    }

    void CreateNode(Node[,] map, int floor, int collum, string NodeType)
    {
        GameObject prefab = null;
        if (NodeType == "BasicFight") prefab = BasicFight;
        else if (NodeType == "Campfire") prefab = Campfire;
        else if (NodeType == "Chest") prefab = Chest;
        else if (NodeType == "EliteFight") prefab = EliteFight;
        else if (NodeType == "Event") prefab = Event;
        else if (NodeType == "Shop") prefab = Shop;

        if (prefab != null)
        {
            GameObject nodeObj = SpawnUIElement(prefab, nodesContainer, GetAnchoredPos(collum, floor));
            if (nodeObj.TryGetComponent(out MapNode mapNode))
            {
                mapNode.Setup(floor, collum, NodeType);
            }
        }

        if (map[collum, floor].leftPath && (NodeType == "EliteFight" || NodeType == "Shop" || NodeType == "Campfire"))
            map[collum - 1, floor + 1].cantBe.Add(NodeType);

        if (map[collum, floor].rightPath && (NodeType == "EliteFight" || NodeType == "Shop" || NodeType == "Campfire"))
            map[collum + 1, floor + 1].cantBe.Add(NodeType);

        if (map[collum, floor].straightPath && (NodeType == "EliteFight" || NodeType == "Shop" || NodeType == "Campfire"))
            map[collum, floor + 1].cantBe.Add(NodeType);

        if (map[collum, floor].downStraightPath && map[collum, floor - 1].rightPath)
            map[collum + 1, floor].cantBe.Add(NodeType);

        if (map[collum, floor].downRightPath)
        {
            if (map[collum + 1, floor - 1].straightPath)
                map[collum + 1, floor].cantBe.Add(NodeType);
            if (map[collum + 1, floor - 1].rightPath)
                map[collum + 2, floor].cantBe.Add(NodeType);
        }
    }

    string ChooseNodeType(List<string> cantBe)
    {
        Dictionary<string, int> canBe = new Dictionary<string, int>();
        foreach (var pair in nodeWeights)
        {
            if (!cantBe.Contains(pair.Key))
                canBe[pair.Key] = pair.Value;
        }

        int total = 0;
        foreach (int w in canBe.Values) total += w;

        int roll = Random.Range(0, total);
        foreach (var pair in canBe)
        {
            if (roll < pair.Value) return pair.Key;
            roll -= pair.Value;
        }

        return "BasicFight";
    }

    void DrawMap(Node[,] map)
    {
        ResizeContent();

        float halfWidth = (collums - 1) / 2f;
        Vector2 startPos = GetAnchoredPos(halfWidth, -2f);
        Vector2 bossPos = GetAnchoredPos(halfWidth, floors + 1f);

        for (int floor = 0; floor < floors; floor++)
        {
            for (int collum = 0; collum < collums; collum++)
            {
                if (!map[collum, floor].inMap) continue;

                Vector2 pos = GetAnchoredPos(collum, floor);

                if (map[collum, floor].leftPath)
                    SpawnLine(pos, GetAnchoredPos(collum - 1, floor + 1));

                if (map[collum, floor].rightPath)
                    SpawnLine(pos, GetAnchoredPos(collum + 1, floor + 1));

                if (map[collum, floor].straightPath)
                    SpawnLine(pos, GetAnchoredPos(collum, floor + 1));

                if (map[collum, floor].StartPath != -1)
                    SpawnLine(startPos, pos);

                if (map[collum, floor].BossPath != -1)
                    SpawnLine(pos, bossPos);
            }
        }

        if (StartNode != null)
            SpawnUIElement(StartNode, nodesContainer, startPos);

        if (Boss != null)
        {
            GameObject bossObj = SpawnUIElement(Boss, nodesContainer, bossPos);
            if (bossObj.TryGetComponent(out MapNode bossNode))
            {
                bossNode.Setup(floors, 0, "Boss");
            }
        }

        for (int floor = 0; floor < floors; floor++)
        {
            for (int collum = 0; collum < collums; collum++)
            {
                if (!map[collum, floor].inMap) continue;

                if (map[collum, floor].type == "None")
                {
                    if (floor < 4)
                    {
                        map[collum, floor].cantBe.Add("EliteFight");
                        map[collum, floor].cantBe.Add("Campfire");
                    }
                    if (floor == floors - 2)
                        map[collum, floor].cantBe.Add("Campfire");

                    if (floor == 0)
                        map[collum, floor].type = "BasicFight";
                    else if (floor == floors - 1)
                        map[collum, floor].type = "Campfire";
                    else if (floor == floors / 2)
                        map[collum, floor].type = "Chest";
                    else
                        map[collum, floor].type = ChooseNodeType(map[collum, floor].cantBe);
                }

                CreateNode(map, floor, collum, map[collum, floor].type);
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

    void GeneratePath(Node[,] map, int pos)
    {
        for (int i = 0; i < floors; i++)
        {
            map[pos, i].inMap = true;
            int min = 0;
            int max = 3;

            if (i == 0) map[pos, i].StartPath = pos;
            if (pos == 0 || map[pos - 1, i].rightPath) min = 1;
            if (pos == collums - 1 || map[pos + 1, i].leftPath) max = 2;

            if (i < floors - 1)
            {
                int direction = Random.Range(min, max);
                if (direction == 0)
                {
                    map[pos, i].leftPath = true;
                    pos -= 1;
                    map[pos, i + 1].downRightPath = true;
                }
                else if (direction == 2)
                {
                    map[pos, i].rightPath = true;
                    pos += 1;
                    map[pos, i + 1].downLeftPath = true;
                }
                else
                {
                    map[pos, i].straightPath = true;
                    map[pos, i + 1].downStraightPath = true;
                }
            }
            else
            {
                map[pos, i].BossPath = pos;
            }
        }
    }

    public void GenerateMap(int numOfPaths)
    {
        Node[,] map = new Node[collums, floors];
        for (int x = 0; x < collums; x++)
            for (int y = 0; y < floors; y++)
                map[x, y] = new Node();

        int first = Random.Range(0, collums);
        GeneratePath(map, first);

        int start = Random.Range(0, collums);
        while (start == first) start = Random.Range(0, collums);
        GeneratePath(map, start);

        for (int i = 0; i < numOfPaths - 2; i++)
        {
            start = Random.Range(0, collums);
            GeneratePath(map, start);
        }

        DrawMap(map);
        mapData = map;
    }

    public void TestGen()
    {
        if (mapData == null)
            GenerateMap(collums - 1);
        else
            DrawMap(mapData);
    }
}