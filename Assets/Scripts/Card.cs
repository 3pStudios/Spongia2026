using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class Card : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [Header("Card Configuration")]
    public CardSO cardData;

    [Header("UI Text References")]
    [SerializeField] private TextMeshProUGUI costText;
    [SerializeField] private TextMeshProUGUI nameText;
    [SerializeField] private TextMeshProUGUI descriptionText;

    private Vector3 originalScale;
    private bool isDragging;

    private void Awake()
    {
        originalScale = transform.localScale;
    }

    public void SetupCard(CardSO data)
    {
        cardData = data;
        costText.text = data.cardCost.ToString();
        nameText.text = data.cardName;
        descriptionText.text = data.cardDescription;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (isDragging || GameManager.Instance.isProcessingTurn) return;
        transform.localScale = originalScale * 1.15f;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (isDragging) return;
        transform.localScale = originalScale;
    }

    public void SetDragging(bool dragging)
    {
        isDragging = dragging;
        if (!dragging) transform.localScale = originalScale;
    }
}