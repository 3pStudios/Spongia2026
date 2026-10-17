using UnityEngine;
using UnityEngine.EventSystems;

[RequireComponent(typeof(Card), typeof(RectTransform))]
public class CardDrop : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
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

    public void OnBeginDrag(PointerEventData eventData)
    {
        originalAnchoredPosition = rectTransform.anchoredPosition;
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (canvas != null)
        {
            rectTransform.anchoredPosition += eventData.delta / canvas.scaleFactor;
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (!GameManager.Instance.CanPlayCard(card.cardData))
        {
            Debug.Log("Not enough Mana!");
            ResetPosition();
            return;
        }
        Vector2 worldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        RaycastHit2D hit = Physics2D.Raycast(worldPos, Vector2.zero);

        if (hit.collider != null)
        {
            if (hit.collider.CompareTag("Player") && card.cardData.applyType == ApplyType.Player)
            {
                GameManager.Instance.PlayCardPlayer(card.cardData);
                Destroy(gameObject);
                return;
            }
            if (hit.collider.CompareTag("Enemy") && card.cardData.applyType == ApplyType.Enemy)
            {
                Enemy enemy = hit.collider.GetComponent<Enemy>();
                if (enemy != null)
                {
                    GameManager.Instance.PlayCardEnemy(card.cardData, enemy);
                    Destroy(gameObject);
                    return;
                }
            }
        }
        ResetPosition();
    }

    private void ResetPosition()
    {
        rectTransform.anchoredPosition = originalAnchoredPosition;
    }
}