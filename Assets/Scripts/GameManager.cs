using UnityEngine;
using System.Collections.Generic;
using System;
using UnityEngine.UI;
using UnityEditor.AdaptivePerformance.Editor;

public class GameManager : MonoBehaviour
{
    public List<CardSO> masterDeck = new();
    public List<CardSO> drawDeck = new();
    public List<CardSO> discardDeck = new();
    public List<CardSO> handDeck = new();
    public List<PlayedCardPlayer> playedDeckPlayer = new();
    public List<PlayedCardEnemy> playedDeckEnemy = new();


    public Player player;

    [SerializeField] private Canvas canvas;
    [SerializeField] private GameObject cardPlace;
    [SerializeField] private GameObject cardPrefab;
    public Vector2 originCardPosition = new(-520, 60);

    public static GameManager Instance { get; private set; }


    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            Instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        drawDeck = new List<CardSO>(masterDeck);
        ShuffleDraw();
        DrawCardsNewRound(5); // Draw 5 cards at the start of the game
    }

    // Update is called once per frame
    void Update()
    {
        
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
            DrawCard();
            var newCard = Instantiate(cardPrefab, cardPlace.transform);
            Card card = newCard.GetComponent<Card>();
            card.cardData = handDeck[i]; // Get the last drawn card
            newCard.GetComponent<Image>().sprite = card.cardData.cardImage;
        }
    }

    public void DrawCard()
    {
        if (drawDeck.Count == 0)
        {
            Debug.Log("Draw deck is empty, reshuffling discard deck into draw deck.");
            PutDiscardIntoDraw();
            ShuffleDraw();
        }
        if (drawDeck.Count > 0)
        {
            CardSO drawnCard = drawDeck[0];
            drawDeck.RemoveAt(0);
            handDeck.Add(drawnCard);
            Debug.Log($"Drew card: {drawnCard.cardName}");
        }
        else
        {
            Debug.Log("No cards left to draw.");
        }
    }
    public void PutDiscardIntoDraw()
    {
        drawDeck.AddRange(discardDeck);
        discardDeck.Clear();
        Debug.Log("Put discard deck into draw deck.");
    }
    public void ShuffleDraw()
    {
        drawDeck.Shuffle();
        Debug.Log("Shuffled draw deck.");
    }
    public void DiscardCard(CardSO card)
    {
        if (handDeck.Contains(card))
        {
            handDeck.Remove(card);
            discardDeck.Add(card);
            Debug.Log($"Discarded card: {card.cardName}");
        }
        else
        {
            Debug.LogWarning($"Attempted to discard a card not in hand: {card.cardName}");
        }
    }
    public void PlayCardPlayer(CardSO card)
    {
        
        playedDeckPlayer.Add(new PlayedCardPlayer { cardData = card });
        DiscardCard(card);
    }
    public void PlayCardEnemy(CardSO card, Enemy enemy)
    {
        playedDeckEnemy.Add(new PlayedCardEnemy { cardData = card, appliedEnemy = enemy });
        DiscardCard(card);
    }
    public void EndTurn()
    {
        foreach (var playedCard in playedDeckPlayer)
        {
            player.ApplyCard(playedCard.cardData);
        }
        foreach (var playedCard in playedDeckEnemy)
        {
            playedCard.appliedEnemy.ApplyCard(playedCard.cardData);
        }
        playedDeckPlayer.Clear();
        playedDeckEnemy.Clear();
        DrawCardsNewRound(5); // Draw 5 new cards at the start of the next turn
    }
}
[System.Serializable]
public class PlayedCardPlayer
{
    public CardSO cardData;
}
[System.Serializable]
public class PlayedCardEnemy
{
   public CardSO cardData;
    public Enemy appliedEnemy;
}
//Dakujem Unity Forums 
public static class IListExtensions
{
    /// Shuffles the element order of the specified list.
    public static void Shuffle<T>(this IList<T> ts)
    {
        var count = ts.Count;
        var last = count - 1;
        for (var i = 0; i < last; ++i)
        {
            var r = UnityEngine.Random.Range(i, count);
            var tmp = ts[i];
            ts[i] = ts[r];
            ts[r] = tmp;
        }
    }
}