using UnityEngine;
using UnityEngine.EventSystems;

public class MapNode : MonoBehaviour
{
    public int floor;
    public int column;
    public string nodeType = "BasicFight";

    public void Setup(int floor, int column, string nodeType)
    {
        this.floor = floor;
        this.column = column;
        this.nodeType = nodeType;
    }

    private void OnMouseDown()
    {
        if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject())
        {
            return;
        }

        if (RunManager.Instance == null) return;

        if (floor == RunManager.Instance.currentFloor + 1)
        {
            RunManager.Instance.SelectNode(nodeType, floor);
        }
    }
}