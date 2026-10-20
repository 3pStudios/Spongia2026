using UnityEngine;
using System.Text;

public class MapGen : MonoBehaviour
{
public GameObject Tile;

    class Node
    {
        public bool inMap;
        public bool rightPath;
        public bool leftPath;
        public bool straightPath;
        public string type;

        public Node()
        {
            this.inMap = false;
            this.rightPath = false;
            this.leftPath = false;
            this.type = "None";
            this.straightPath = false;
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
            if (pos == 0 || map[pos-1, i].rightPath == true)
            {
                min = 1;
            }
            if (pos == collums - 1 || map[pos+1, i].leftPath == true)
            {
                max = 2;
            }

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
    }


    public void GenerateMap(int floors, int collums, int numOfPaths)
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

        LogMap(map, floors, collums);

    }

    public void TestGen()
    {
        GenerateMap(15, 7, 6);
    }
}
