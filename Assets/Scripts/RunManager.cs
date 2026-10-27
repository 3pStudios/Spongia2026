using System.Collections.Generic;
using UnityEngine;

public class RunManager : MonoBehaviour
{
    public static RunManager Instance { get; private set; }

    public List<CardSO> masterDeck = new();
    public List<CardSO> starterDeck = new();
    public List<CardSO> humanRewardPool = new();

    private List<CardSO> currentRewardPool = new();

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        ResetRun();
    }

    public void ResetRun()
    {
        masterDeck = new List<CardSO>(starterDeck);
        currentRewardPool = new List<CardSO>(humanRewardPool);
    }

    public void AddCard(CardSO card)
    {
        masterDeck.Add(card);
        if (currentRewardPool.Contains(card))
        {
            currentRewardPool.Remove(card);
        }
    }

    public List<CardSO> GetRandomRewards(int count = 3)
    {
        List<CardSO> pool = new List<CardSO>(currentRewardPool);
        List<CardSO> rewards = new();

        int pickCount = Mathf.Min(count, pool.Count);
        for (int i = 0; i < pickCount; i++)
        {
            int randomIndex = Random.Range(0, pool.Count);
            rewards.Add(pool[randomIndex]);
            pool.RemoveAt(randomIndex);
        }

        return rewards;
    }
}