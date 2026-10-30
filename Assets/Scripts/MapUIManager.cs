using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MapUIManager : MonoBehaviour
{
    public static MapUIManager Instance { get; private set; }

    [Header("Top Header")]
    [SerializeField] private TextMeshProUGUI hpText;
    [SerializeField] private TextMeshProUGUI goldText;
    [SerializeField] private Button deckButton;
    [SerializeField] private TextMeshProUGUI deckCountText;

    [Header("Deck Viewer")]
    [SerializeField] private DeckViewerUI deckViewer;

    [Header("Campfire Popup")]
    [SerializeField] private GameObject campfirePanel;
    [SerializeField] private Button campfireRestButton;

    [Header("Shop Popup")]
    [SerializeField] private GameObject shopPanel;
    [SerializeField] private Transform shopCardContainer;
    [SerializeField] private GameObject cardPrefab;
    [SerializeField] private Button shopRemoveCardButton;
    [SerializeField] private Button shopHealButton;
    [SerializeField] private Button shopLeaveButton;

    [Header("Event Popup")]
    [SerializeField] private GameObject eventPanel;
    [SerializeField] private TextMeshProUGUI eventTitleText;
    [SerializeField] private TextMeshProUGUI eventDescriptionText;
    [SerializeField] private Button eventChoice1Button;
    [SerializeField] private TextMeshProUGUI eventChoice1Text;
    [SerializeField] private Button eventChoice2Button;
    [SerializeField] private TextMeshProUGUI eventChoice2Text;

    private int activeEventIndex = 0;

    private void Awake()
    {
        Instance = this;

        if (campfirePanel) campfirePanel.SetActive(false);
        if (shopPanel) shopPanel.SetActive(false);
        if (eventPanel) eventPanel.SetActive(false);

        if (deckButton) deckButton.onClick.AddListener(() => deckViewer.OpenViewOnly());
        if (campfireRestButton) campfireRestButton.onClick.AddListener(OnCampfireRest);
        if (shopLeaveButton) shopLeaveButton.onClick.AddListener(() => { shopPanel.SetActive(false); UpdateHeaderUI(); });
        if (shopHealButton) shopHealButton.onClick.AddListener(OnShopHeal);
        if (shopRemoveCardButton) shopRemoveCardButton.onClick.AddListener(OnShopRemoveCard);
    }

    private void Start()
    {
        UpdateHeaderUI();
    }

    public void UpdateHeaderUI()
    {
        if (RunManager.Instance == null) return;
        if (hpText) hpText.text = $"{RunManager.Instance.playerCurrentHealth}/{RunManager.Instance.playerMaxHealth}";
        if (goldText) goldText.text = $"{RunManager.Instance.gold}";
        if (deckCountText) deckCountText.text = $"{RunManager.Instance.masterDeck.Count}";
    }

    public void OpenCampfire()
    {
        campfirePanel.SetActive(true);
    }

    private void OnCampfireRest()
    {
        RunManager.Instance.HealPlayer(8);
        campfirePanel.SetActive(false);
        UpdateHeaderUI();
    }
    public void OpenShop()
    {
        shopPanel.SetActive(true);
        shopRemoveCardButton.interactable = RunManager.Instance.gold >= 75;
        shopHealButton.interactable = RunManager.Instance.gold >= 40;

        foreach (Transform child in shopCardContainer) Destroy(child.gameObject);

        List<CardSO> shopCards = RunManager.Instance.GetRandomRewards(3);
        foreach (CardSO cardData in shopCards)
        {
            GameObject cardObj = Instantiate(cardPrefab, shopCardContainer);
            if (cardObj.TryGetComponent(out CardDrop drop)) drop.enabled = false;
            if (cardObj.TryGetComponent(out Card cardComponent)) cardComponent.SetupCard(cardData);
            if (cardObj.TryGetComponent(out Image img)) img.sprite = cardData.cardImage;

            Button btn = cardObj.GetComponent<Button>() ?? cardObj.AddComponent<Button>();
            int cardCost = 50;

            btn.onClick.AddListener(() =>
            {
                if (RunManager.Instance.SpendGold(cardCost))
                {
                    RunManager.Instance.AddCard(cardData);
                    Destroy(cardObj);
                    UpdateHeaderUI();
                }
            });
        }
    }

    private void OnShopHeal()
    {
        if (RunManager.Instance.SpendGold(40))
        {
            RunManager.Instance.HealPlayer(8);
            shopHealButton.interactable = false;
            UpdateHeaderUI();
        }
    }

    private void OnShopRemoveCard()
    {
        if (RunManager.Instance.gold >= 75)
        {
            deckViewer.OpenForRemoval(card =>
            {
                RunManager.Instance.SpendGold(75);
                shopRemoveCardButton.interactable = false;
                UpdateHeaderUI();
            });
        }
    }

    public void OpenEvent()
    {
        eventPanel.SetActive(true);
        activeEventIndex = Random.Range(0, 3);

        eventChoice1Button.onClick.RemoveAllListeners();
        eventChoice2Button.onClick.RemoveAllListeners();

        if (activeEventIndex == 0)
        {
            eventTitleText.text = "The Dark Shrine";
            eventDescriptionText.text = "You find an ancient altar dripping with fresh blood. A dark presence whispers: 'Give of your essence, and be relieved of your burdens.'";
            eventChoice1Text.text = "Spill Blood (-4 HP) -> Remove a Card";
            eventChoice2Text.text = "Walk Away";

            eventChoice1Button.onClick.AddListener(() =>
            {
                RunManager.Instance.HealPlayer(-4);
                eventPanel.SetActive(false);
                deckViewer.OpenForRemoval(card => UpdateHeaderUI());
            });
            eventChoice2Button.onClick.AddListener(() => { eventPanel.SetActive(false); UpdateHeaderUI(); });
        }
        else if (activeEventIndex == 1)
        {
            eventTitleText.text = "The Wandering Mercenary";
            eventDescriptionText.text = "A weary scout sits sharpening his daggers. 'Care to trade tales, stranger? Or buy a technique I picked up on the road?'";
            eventChoice1Text.text = "Buy Technique (-40 Gold) -> Gain Random Card";
            eventChoice2Text.text = "Share Rations -> Heal 6 HP";

            eventChoice1Button.interactable = RunManager.Instance.gold >= 40;
            eventChoice1Button.onClick.AddListener(() =>
            {
                if (RunManager.Instance.SpendGold(40))
                {
                    List<CardSO> rewards = RunManager.Instance.GetRandomRewards(1);
                    if (rewards.Count > 0) RunManager.Instance.AddCard(rewards[0]);
                    eventPanel.SetActive(false);
                    UpdateHeaderUI();
                }
            });
            eventChoice2Button.onClick.AddListener(() =>
            {
                RunManager.Instance.HealPlayer(6);
                eventPanel.SetActive(false);
                UpdateHeaderUI();
            });
        }
        else
        {
            eventTitleText.text = "The Forgotten Crypt";
            eventDescriptionText.text = "A grand sarcophagus rests in a ruined alcove. It glitters with ancient gold, but glyphs warn of a dormant ward.";
            eventChoice1Text.text = "Loot Sarcophagus (+60 Gold, Trap: -6 HP)";
            eventChoice2Text.text = "Pay Respects (+4 Max & Current HP)";

            eventChoice1Button.onClick.AddListener(() =>
            {
                RunManager.Instance.AddGold(60);
                RunManager.Instance.HealPlayer(-6);
                eventPanel.SetActive(false);
                UpdateHeaderUI();
            });
            eventChoice2Button.onClick.AddListener(() =>
            {
                RunManager.Instance.AddMaxHealth(4);
                eventPanel.SetActive(false);
                UpdateHeaderUI();
            });
        }
    }
}