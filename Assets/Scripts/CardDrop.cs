using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Card), typeof(RectTransform))]
public class CardDrop : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    private Canvas canvas;
    private RectTransform rectTransform;
    private Transform originalParent;
    private int originalSiblingIndex;
    private Card card;
    private Vector3 dragOffset;
    private Vector2 lastPointerPos;
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
        if (isDragging && (Pointer.current == null || !Pointer.current.press.isPressed))
        {
            Vector2 pos = Pointer.current != null ? Pointer.current.position.ReadValue() : lastPointerPos;
            EndDragSequence(pos);
        }
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (GameManager.Instance.isProcessingTurn) return;
        if (AudioManager.Instance != null) AudioManager.Instance.PlayCard();
        if (transform.parent != canvas.transform)
        {
            originalParent = transform.parent;
            originalSiblingIndex = transform.GetSiblingIndex();
        }

        isDragging = true;
        lastPointerPos = eventData.position;
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

        lastPointerPos = eventData.position;

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
            EndDragSequence(eventData.position);
        }
    }

    private void EndDragSequence(Vector2 screenPosition)
    {
        isDragging = false;
        card.SetDragging(false);

        if (!GameManager.Instance.CanPlayCard(card.cardData))
        {
            ResetPosition();
            return;
        }

        Vector2 worldPos = Camera.main.ScreenToWorldPoint(screenPosition);
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