using UnityEngine;
using System.Collections.Generic;
using System;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public List<CardSO> masterDeck;
    public List<CardSO> drawDeck;
    public List<CardSO> discardDeck;
    public List<CardSO> handDeck;


    [SerializeField] private Canvas canvas;
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
        ReshuffleDiscardIntoDraw();
        for (int i = 0; i < 5; i++)
        {
            DrawCard();
            var newCard = Instantiate(cardPrefab, canvas.transform);
            newCard.GetComponent<RectTransform>().anchoredPosition = originCardPosition + new Vector2(i * 260, 0);
            Card card = newCard.GetComponent<Card>();
            card.cardData = handDeck[i];
            newCard.GetComponent<Image>().sprite = card.cardData.cardImage;
            card.GetComponent<CardDrop>().SetOriginalPosition();
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void DrawCard()
    {
        if (drawDeck.Count == 0)
        {
            Debug.Log("Draw deck is empty, reshuffling discard deck into draw deck.");
            ReshuffleDiscardIntoDraw();
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
    public void ReshuffleDiscardIntoDraw()
    {
        drawDeck.AddRange(discardDeck);
        discardDeck.Clear();
        drawDeck.Shuffle();
        Debug.Log("Reshuffled discard deck into draw deck.");
    }

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