using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button), typeof(Image))]
public class MapNode : MonoBehaviour
{
    public int floor;
    public int column;
    public string nodeType = "BasicFight";

    private Vector3 baseScale;
    private Button button;
    private Image image;

    private void Awake()
    {
        baseScale = transform.localScale;
        button = GetComponent<Button>();
        image = GetComponent<Image>();

        button.onClick.AddListener(OnNodeClicked);
    }

    public void Setup(int floor, int column, string nodeType)
    {
        this.floor = floor;
        this.column = column;
        this.nodeType = nodeType;
    }

    private void Start()
    {
        UpdateVisual();
    }

    public void UpdateVisual()
    {
        if (RunManager.Instance == null || image == null) return;

        int curFloor = RunManager.Instance.currentFloor;
        int curCol = RunManager.Instance.currentColumn;

        if (floor == curFloor && column == curCol)
        {
            image.color = Color.yellow;
            transform.localScale = baseScale * 1.35f;
        }
        else if (floor <= curFloor)
        {
            image.color = new Color(0.35f, 0.35f, 0.35f, 0.5f);
            transform.localScale = baseScale;
        }
        else
        {
            image.color = Color.white;
            transform.localScale = baseScale;
        }
    }

    public static void RefreshAll()
    {
        MapNode[] allNodes = FindObjectsByType<MapNode>(FindObjectsSortMode.None);
        foreach (var node in allNodes)
        {
            node.UpdateVisual();
        }
    }

    private void OnNodeClicked()
    {
        if (RunManager.Instance == null) return;

        if (MapGen.CanMove(column, floor))
        {
            RunManager.Instance.currentColumn = column;
            RunManager.Instance.SelectNode(nodeType, floor);
            RefreshAll();
        }
    }
}