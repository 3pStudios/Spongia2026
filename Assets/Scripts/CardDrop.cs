using UnityEngine;
using UnityEngine.EventSystems;

public class CardDrop : MonoBehaviour, IPointerDownHandler, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    private Canvas canvas;
    private RectTransform rectTransform;
    private Vector2 originalAnchoredPosition;
    private Card card;
    private void Awake()
    {
        canvas = GetComponentInParent<Canvas>();
        rectTransform = GetComponent<RectTransform>();
        card = GetComponent<Card>();
    }
    public void OnPointerDown(PointerEventData eventData)
    {
    }
    public void OnBeginDrag(PointerEventData eventData)
    {
        SetOriginalPosition();
    }
    public void OnEndDrag(PointerEventData eventData)
    {
        Vector2 worldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        RaycastHit2D hit = Physics2D.Raycast(worldPos, Vector2.zero);

        if (hit.collider != null)
        {
            if(hit.collider.CompareTag("Player") && card.cardData.applyType == ApplyType.Player)
            {
                Debug.Log($"Dropped on {hit.collider.gameObject.name}");
                
                GameManager.Instance.PlayCardPlayer(card.cardData);

                Destroy(gameObject);

            }
            else if(hit.collider.CompareTag("Enemy") && card.cardData.applyType == ApplyType.Enemy)
            {
                Debug.Log($"Dropped on {hit.collider.gameObject.name}");
                GameManager.Instance.PlayCardEnemy(card.cardData, hit.collider.gameObject.GetComponent<Enemy>());
                Destroy(gameObject);


            }
            rectTransform.anchoredPosition = originalAnchoredPosition;

        }   
        else
        {
            rectTransform.anchoredPosition = originalAnchoredPosition;
        }
    }
    public void OnDrag(PointerEventData eventData)
    {
        rectTransform.anchoredPosition += eventData.delta / canvas.scaleFactor;
    }
    public void SetOriginalPosition()
    {
        originalAnchoredPosition = rectTransform.anchoredPosition;
    }
}
