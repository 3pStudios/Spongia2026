using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    public bool isProcessingTurn { get; private set; }

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
    [SerializeField] private RewardUI rewardUI;

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
        if (RunManager.Instance != null && RunManager.Instance.masterDeck.Count > 0)
        {
            masterDeck = new List<CardSO>(RunManager.Instance.masterDeck);
        }

        gameTurn = 0;
        drawDeck = new List<CardSO>(masterDeck);
        drawDeck.Shuffle();

        foreach (var enemy in enemiesAlive)
        {
            enemy.SetupEnemy();
        }

        StartCoroutine(StartGameRoutine());
    }

    private IEnumerator StartGameRoutine()
    {
        isProcessingTurn = true;
        player.ResetMana();
        player.ResetShield();
        yield return StartCoroutine(TurnStartDrawRoutine(5));
        isProcessingTurn = false;
    }

    public bool CanPlayCard(CardSO card)
    {
        return !isProcessingTurn && player.HasEnoughMana(card.cardCost);
    }

    public void PlayCard(CardSO card, Enemy target = null)
    {
        if (!CanPlayCard(card)) return;

        player.SpendMana(card.cardCost);
        DiscardCard(card);
        StartCoroutine(ExecuteCardSequence(card, target));
    }

    private IEnumerator ExecuteCardSequence(CardSO card, Enemy target)
    {
        isProcessingTurn = true;
        yield return StartCoroutine(player.ExecuteCardEffectsRoutine(card, target, new List<Enemy>(enemiesAlive)));
        isProcessingTurn = false;
    }

    public IEnumerator EnemyAttackSequence(Enemy enemy, int attackDamage)
    {
        yield return StartCoroutine(enemy.AttackPlayerRoutine(player, attackDamage));
    }

    public void EndTurn()
    {
        if (isProcessingTurn) return;
        StartCoroutine(EndTurnRoutine());
    }

    private IEnumerator EndTurnRoutine()
    {
        isProcessingTurn = true;

        foreach (var enemy in new List<Enemy>(enemiesAlive))
        {
            if (enemy.currentBleed > 0)
            {
                yield return StartCoroutine(enemy.TickBleedRoutine());
            }
        }

        if (enemiesAlive.Count == 0) yield break;

        foreach (var enemy in new List<Enemy>(enemiesAlive))
        {
            yield return StartCoroutine(enemy.ExecuteTurnRoutine());
        }

        foreach (var enemy in new List<Enemy>(enemiesAlive))
        {
            yield return StartCoroutine(enemy.SetupNextTurnRoutine(gameTurn));
        }

        if (player.currentBleed > 0)
        {
            yield return StartCoroutine(player.TickBleedRoutine());
        }

        gameTurn++;

        player.ResetMana();
        player.ResetShield();
        player.ResetTurnPassives();

        if (player.hasBloodletting)
        {
            foreach (var enemy in enemiesAlive)
            {
                enemy.ApplyBleed(2);
            }
        }

        yield return StartCoroutine(TurnStartDrawRoutine(5));

        isProcessingTurn = false;
    }

    public IEnumerator TurnStartDrawRoutine(int numberOfCards)
    {
        discardDeck.AddRange(handDeck);
        handDeck.Clear();

        foreach (Transform child in cardPlace.transform)
        {
            Destroy(child.gameObject);
        }

        yield return null;
        yield return StartCoroutine(DrawCardsRoutine(numberOfCards));
    }

    public IEnumerator DrawCardsRoutine(int numberOfCards)
    {
        for (int i = 0; i < numberOfCards; i++)
        {
            CardSO drawnCard = DrawCard();
            if (drawnCard == null) continue;

            var newCard = Instantiate(cardPrefab, cardPlace.transform);

            Card cardComponent = newCard.GetComponent<Card>();
            cardComponent.SetupCard(drawnCard);

            Image cardImg = newCard.GetComponent<Image>();
            cardImg.sprite = drawnCard.cardImage;

            yield return new WaitForSeconds(0.12f);
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

    public void CheckBattleOver()
    {
        if (enemiesAlive.Count == 0)
        {
            StartCoroutine(VictoryRoutine());
        }
    }

    public void PlayerDied()
    {
        StopAllCoroutines();
        StartCoroutine(GameOverRoutine());
    }

    private IEnumerator VictoryRoutine()
    {
        isProcessingTurn = true;
        yield return new WaitForSeconds(0.8f);

        if (rewardUI != null)
        {
            rewardUI.ShowRewards();
        }
        else
        {
            SceneManager.LoadScene("Start Menu");
        }
    }

    private IEnumerator GameOverRoutine()
    {
        isProcessingTurn = true;
        yield return new WaitForSeconds(1f);
        SceneManager.LoadScene("Start Menu");
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