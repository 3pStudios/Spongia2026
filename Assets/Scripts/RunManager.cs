using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class RunManager : MonoBehaviour
{
    public static RunManager Instance { get; private set; }

    [Header("Progress")]
    public int currentFloor = -1;
    public string mapSceneName = "Map";
    public string combatSceneName = "SampleScene";

    [Header("Deck")]
    public List<CardSO> masterDeck = new();
    public List<CardSO> starterDeck = new();
    public List<CardSO> humanRewardPool = new();
    private List<CardSO> currentRewardPool = new();

    [Header("Encounters")]
    public Encounter currentEncounter;
    public List<Encounter> earlyEncounters = new();
    public List<Encounter> lateEncounters = new();
    public List<Encounter> eliteEncounters = new();
    public Encounter bossEncounter;

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
        currentFloor = -1;
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

    public void StartCombatFromNode(string nodeType, int floor)
    {
        currentFloor = floor;

        if (nodeType == "Boss")
        {
            currentEncounter = bossEncounter;
        }
        else if (nodeType == "EliteFight")
        {
            currentEncounter = GetRandomEncounter(eliteEncounters);
        }
        else
        {
            currentEncounter = floor < 3 
                ? GetRandomEncounter(earlyEncounters) 
                : GetRandomEncounter(lateEncounters);
        }

        SceneManager.LoadScene(combatSceneName);
    }

    private Encounter GetRandomEncounter(List<Encounter> pool)
    {
        if (pool == null || pool.Count == 0) return currentEncounter;
        return pool[Random.Range(0, pool.Count)];
    }
}