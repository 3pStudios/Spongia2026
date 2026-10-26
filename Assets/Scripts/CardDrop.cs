using UnityEngine;
using UnityEngine.EventSystems;

[RequireComponent(typeof(Card), typeof(RectTransform))]
public class CardDrop : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    private Canvas canvas;
    private RectTransform rectTransform;
    private Transform originalParent;
    private int originalSiblingIndex;
    private Card card;

    private void Awake()
    {
        canvas = GetComponentInParent<Canvas>();
        rectTransform = GetComponent<RectTransform>();
        card = GetComponent<Card>();
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (GameManager.Instance.isProcessingTurn) return;

        originalParent = transform.parent;
        originalSiblingIndex = transform.GetSiblingIndex();
        card.SetDragging(true);
        transform.SetParent(canvas.transform, true);
        transform.SetAsLastSibling();
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (GameManager.Instance.isProcessingTurn) return;

        if (canvas != null)
        {
            rectTransform.anchoredPosition += eventData.delta / canvas.scaleFactor;
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        card.SetDragging(false);
        if (!GameManager.Instance.CanPlayCard(card.cardData))
        {
            ResetPosition();
            return;
        }

        Vector2 worldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        RaycastHit2D hit = Physics2D.Raycast(worldPos, Vector2.zero);

        if (hit.collider != null)
        {
            if (card.cardData.target == CardTarget.Self && hit.collider.TryGetComponent(out Player _))
            {
                GameManager.Instance.PlayCard(card.cardData);
                Destroy(gameObject);
                return;
            }

            if (card.cardData.target != CardTarget.Self && hit.collider.TryGetComponent(out Enemy enemy))
            {
                GameManager.Instance.PlayCard(card.cardData, enemy);
                Destroy(gameObject);
                return;
            }
        }

        ResetPosition();
    }

    private void ResetPosition()
    {
        transform.SetParent(originalParent, true);
        transform.SetSiblingIndex(originalSiblingIndex);
    }
}