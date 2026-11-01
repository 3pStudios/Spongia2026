using UnityEngine;
using UnityEngine.EventSystems;

public class MapNode : MonoBehaviour
{
    public int floor;
    public int column;
    public string nodeType = "BasicFight";

    private Vector3 baseScale;

    private void Awake()
    {
        baseScale = transform.localScale;
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
        if (RunManager.Instance == null) return;
        if (!TryGetComponent(out SpriteRenderer sr)) return;

        int curFloor = RunManager.Instance.currentFloor;
        int curCol = RunManager.Instance.currentColumn;

        if (floor == curFloor && column == curCol)
        {
            sr.color = Color.yellow;
            transform.localScale = baseScale * 1.35f;
        }
        else if (floor <= curFloor)
        {
            sr.color = new Color(0.35f, 0.35f, 0.35f, 0.5f);
            transform.localScale = baseScale;
        }
        else
        {
            sr.color = Color.white;
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

    private void OnMouseDown()
    {
        if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject()) return;
        if (RunManager.Instance == null) return;

        if (MapGen.CanMove(column, floor))
        {
            RunManager.Instance.currentColumn = column;
            RunManager.Instance.SelectNode(nodeType, floor);
            RefreshAll();
        }
    }
}