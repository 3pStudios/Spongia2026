using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("Turn")]
    public int gameTurn = 0;

    [Header("Decks")]
    public List<CardSO> masterDeck = new();
    public List<CardSO> drawDeck = new();
    public List<CardSO> handDeck = new();
    public List<CardSO> discardDeck = new();

    [Header("Entities")]
    public Player player;
    public List<Enemy> enemiesAlive = new();

    [Header("UI & Spawning")]
    [SerializeField] private GameObject cardPlace;
    [SerializeField] private GameObject cardPrefab;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    private void Start()
    {
        gameTurn = 0;
        drawDeck = new List<CardSO>(masterDeck);
        drawDeck.Shuffle();

        if (player == null)
            player = FindFirstObjectByType<Player>();

        StartNewRound();

        foreach (var enemy in enemiesAlive)
        {
            enemy.SetupEnemy();
        }
    }

    public bool CanPlayCard(CardSO card)
    {
        return player != null && player.HasEnoughMana(card.cardCost);
    }

    public void PlayCardPlayer(CardSO card)
    {
        if (!CanPlayCard(card)) return;

        player.SpendMana(card.cardCost);
        player.ApplyCard(card);
        DiscardCard(card);
    }

    public void PlayCardEnemy(CardSO card, Enemy enemy)
    {
        if (!CanPlayCard(card)) return;

        player.SpendMana(card.cardCost);
        enemy.ApplyCard(card);
        DiscardCard(card);
    }

    public void EndTurn()
    {
        foreach (var enemy in new List<Enemy>(enemiesAlive))
        {
            if (enemy != null)
            {
                enemy.TickTurn(gameTurn);
            }
        }

        gameTurn++;
        StartNewRound();
    }

    private void StartNewRound()
    {
        player.ResetMana();
        player.ResetShield();
        DrawCardsNewRound(5);
    }

    public void DrawCardsNewRound(int numberOfCards)
    {
        discardDeck.AddRange(handDeck);
        handDeck.Clear();

        foreach (Transform child in cardPlace.transform)
        {
            Destroy(child.gameObject);
        }

        for (int i = 0; i < numberOfCards; i++)
        {
            CardSO drawnCard = DrawCard();
            if (drawnCard == null) continue;

            var newCard = Instantiate(cardPrefab, cardPlace.transform);
            Card cardComponent = newCard.GetComponent<Card>();
            cardComponent.cardData = drawnCard;

            Image cardImg = newCard.GetComponent<Image>();
            if (cardImg != null && drawnCard.cardImage != null)
            {
                cardImg.sprite = drawnCard.cardImage;
            }
        }
    }

    public CardSO DrawCard()
    {
        if (drawDeck.Count == 0)
        {
            if (discardDeck.Count == 0) return null;

            drawDeck.AddRange(discardDeck);
            discardDeck.Clear();
            drawDeck.Shuffle();
        }

        CardSO drawnCard = drawDeck[0];
        drawDeck.RemoveAt(0);
        handDeck.Add(drawnCard);
        return drawnCard;
    }

    public void DiscardCard(CardSO card)
    {
        if (handDeck.Contains(card))
        {
            handDeck.Remove(card);
            discardDeck.Add(card);
        }
    }
}

public static class IListExtensions
{
    public static void Shuffle<T>(this IList<T> list)
    {
        for (int i = 0; i < list.Count - 1; i++)
        {
            int r = Random.Range(i, list.Count);
            (list[i], list[r]) = (list[r], list[i]);
        }
    }
}