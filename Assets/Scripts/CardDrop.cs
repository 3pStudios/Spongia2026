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
    private Vector3 dragOffset;
    private bool isDragging;

    private void Awake()
    {
        canvas = GetComponentInParent<Canvas>();
        rectTransform = GetComponent<RectTransform>();
        card = GetComponent<Card>();
        originalParent = transform.parent;
    }

    private void Update()
    {
        if (isDragging && !Input.GetMouseButton(0))
        {
            EndDragSequence();
        }
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (GameManager.Instance.isProcessingTurn) return;

        if (transform.parent != canvas.transform)
        {
            originalParent = transform.parent;
            originalSiblingIndex = transform.GetSiblingIndex();
        }

        isDragging = true;
        card.SetDragging(true);

        RectTransformUtility.ScreenPointToWorldPointInRectangle(
            canvas.transform as RectTransform,
            eventData.position,
            eventData.pressEventCamera,
            out Vector3 worldPoint);

        dragOffset = rectTransform.position - worldPoint;

        transform.SetParent(canvas.transform, true);
        transform.SetAsLastSibling();
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (!isDragging || GameManager.Instance.isProcessingTurn) return;

        if (RectTransformUtility.ScreenPointToWorldPointInRectangle(
            canvas.transform as RectTransform,
            eventData.position,
            eventData.pressEventCamera,
            out Vector3 worldPoint))
        {
            rectTransform.position = worldPoint + dragOffset;
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (isDragging)
        {
            EndDragSequence();
        }
    }

    private void EndDragSequence()
    {
        isDragging = false;
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
        transform.SetParent(originalParent, false);
        transform.SetSiblingIndex(originalSiblingIndex);
    }
}