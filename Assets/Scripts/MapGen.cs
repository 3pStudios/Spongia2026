using System.Collections.Generic;
using UnityEngine;

public class MapGen : MonoBehaviour
{
public GameObject Tile;

    class Node
    {
        public bool inMap;
        public bool rightPath;
        public bool leftPath;

        public Node()
        {
            this.inMap = false;
            this.rightPath = false;
            this.leftPath = false;
        }
    }

    void GeneratePath(Node[,] map, int start)
    {
        int floor = 0;
    }

    void CleanUpMap(Node[,] map)
    {
        
    }

    public void GenerateMap()
    {
        Node[,] map = new Node[15, 7];
        
        int first = Random.Range(0, 6);

        GeneratePath(map, first);

        int start = Random.Range(0, 6);

        while (start == first)
        {
            start = Random.Range(0, 6);
        }
        
        GeneratePath(map, start);

        for (int i = 0; i < 4; i++)
        {
            start = Random.Range(0, 6);
            GeneratePath(map, start);
        }

        CleanUpMap(map);

    }
}
