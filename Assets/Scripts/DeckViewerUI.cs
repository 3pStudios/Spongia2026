using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DeckViewerUI : MonoBehaviour
{
    [SerializeField] private GameObject panel;
    [SerializeField] private Transform cardContainer;
    [SerializeField] private GameObject cardPrefab;
    [SerializeField] private TextMeshProUGUI titleText;
    [SerializeField] private Button closeButton;

    private Action<CardSO> onCardSelectedCallback;
    private bool isRemoveMode;

    private void Awake()
    {
        if (panel != null) panel.SetActive(false);
        if (closeButton != null) closeButton.onClick.AddListener(Close);
    }

    public void OpenViewOnly()
    {
        isRemoveMode = false;
        titleText.text = "Master Deck";
        closeButton.gameObject.SetActive(true);
        onCardSelectedCallback = null;
        PopulateDeck();
    }

    public void OpenForRemoval(Action<CardSO> onRemove)
    {
        isRemoveMode = true;
        titleText.text = "Select a Card to Remove";
        closeButton.gameObject.SetActive(false);
        onCardSelectedCallback = onRemove;
        PopulateDeck();
    }

    private void PopulateDeck()
    {
        panel.SetActive(true);

        foreach (Transform child in cardContainer)
        {
            Destroy(child.gameObject);
        }

        List<CardSO> deck = RunManager.Instance != null ? RunManager.Instance.masterDeck : new();

        foreach (CardSO cardData in deck)
        {
            GameObject cardObj = Instantiate(cardPrefab, cardContainer);

            if (cardObj.TryGetComponent(out CardDrop drop)) drop.enabled = false;
            if (cardObj.TryGetComponent(out Card cardComponent)) cardComponent.SetupCard(cardData);
            if (cardObj.TryGetComponent(out Image img)) img.sprite = cardData.cardImage;

            if (isRemoveMode)
            {
                Button btn = cardObj.GetComponent<Button>();
                if (btn == null) btn = cardObj.AddComponent<Button>();

                CardSO capturedCard = cardData;
                btn.onClick.AddListener(() =>
                {
                    RunManager.Instance.RemoveCard(capturedCard);
                    onCardSelectedCallback?.Invoke(capturedCard);
                    Close();
                });
            }
        }
    }

    public void Close()
    {
        panel.SetActive(false);
    }
}