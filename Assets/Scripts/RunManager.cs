using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public enum Race
{
    Halflings,
    Elves,
    Maiar
}
[System.Serializable]
public class RaceEncounters
{
    public List<Encounter> earlyEncounters = new();
    public List<Encounter> midEncounters = new();
    public List<Encounter> lateEncounters = new();
    public List<Encounter> eliteEncounters = new();
    public Encounter bossEncounter;
}
public class RunManager : MonoBehaviour
{
    public static RunManager Instance { get; private set; }

    [Header("Progress")]
    public Race currentRace = Race.Halflings;
    public int currentFloor = -1;
    public string mapSceneName = "Map";
    public string combatSceneName = "SampleScene";
    public bool isBossFight = false;
    public bool isChestRoom = false; 

    [Header("Persistent Stats")]
    public int playerMaxHealth = 20;
    public int playerCurrentHealth = 20;

    [Header("Current Deck State")]
    public List<CardSO> masterDeck = new();
    private List<CardSO> currentRewardPool = new();

    [Header("Halflings Decks")]
    public List<CardSO> halflingStarterDeck = new();
    public List<CardSO> halflingRewardPool = new();

    [Header("Elves Decks")]
    public List<CardSO> elfStarterDeck = new();
    public List<CardSO> elfRewardPool = new();

    [Header("Maiar Decks")]
    public List<CardSO> maiarStarterDeck = new();
    public List<CardSO> maiarRewardPool = new();

    [Header("Active Combat")]
    public Encounter currentEncounter;

    [Header("Encounters Per Race")]
    public RaceEncounters halflingEncounters;
    public RaceEncounters elfEncounters;
    public RaceEncounters maiarEncounters;

    [Header("Economy")]
    public int gold = 80;


    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        if (masterDeck.Count == 0)
        {
            StartNewRun(currentRace);
        }
    }

    public void StartNewRun(Race race)
    {
        currentRace = race;
        currentFloor = -1;
        isBossFight = false;
        gold = 80;
        playerMaxHealth = 20;
        playerCurrentHealth = playerMaxHealth;
        MapGen.ResetMapData();

        List<CardSO> selectedStarter = race switch
        {
            Race.Elves => elfStarterDeck,
            Race.Maiar => maiarStarterDeck,
            _ => halflingStarterDeck
        };

        List<CardSO> selectedPool = race switch
        {
            Race.Elves => elfRewardPool,
            Race.Maiar => maiarRewardPool,
            _ => halflingRewardPool
        };

        masterDeck = new List<CardSO>(selectedStarter);
        currentRewardPool = new List<CardSO>(selectedPool);
    }
    public void AddGold(int amount) => gold += amount;

    public bool SpendGold(int amount)
    {
        if (gold >= amount)
        {
            gold -= amount;
            return true;
        }
        return false;
    }

    public void RemoveCard(CardSO card)
    {
        masterDeck.Remove(card);
    }

    public void AddMaxHealth(int amount)
    {
        playerMaxHealth += amount;
        playerCurrentHealth += amount;
    }

    public void HealPlayer(int amount)
    {
        playerCurrentHealth = Mathf.Clamp(playerCurrentHealth + amount, 0, playerMaxHealth);
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

    public void SelectNode(string nodeType, int floor)
    {
        currentFloor = floor;
        isBossFight = false;
        isChestRoom = false;

        if (MapUIManager.Instance != null)
        {
            if (nodeType == "Campfire")
            {
                MapUIManager.Instance.OpenCampfire();
                return;
            }
            if (nodeType == "Shop")
            {
                MapUIManager.Instance.OpenShop();
                return;
            }
            if (nodeType == "Event")
            {
                MapUIManager.Instance.OpenEvent();
                return;
            }
        }

        if (nodeType == "Chest")
        {
            isChestRoom = true;
            currentEncounter = null;
            SceneManager.LoadScene(combatSceneName);
            return;
        }

        RaceEncounters activePool = currentRace switch
        {
            Race.Elves => elfEncounters,
            Race.Maiar => maiarEncounters,
            _ => halflingEncounters
        };

        if (nodeType == "Boss")
        {
            isBossFight = true;
            currentEncounter = activePool.bossEncounter;
            SceneManager.LoadScene(combatSceneName);
        }
        else if (nodeType == "EliteFight")
        {
            currentEncounter = GetRandomEncounter(activePool.eliteEncounters);
            SceneManager.LoadScene(combatSceneName);
        }
        else if (nodeType == "BasicFight")
        {
            if (floor < 4)
                currentEncounter = GetRandomEncounter(activePool.earlyEncounters);
            else if (floor < 8)
                currentEncounter = GetRandomEncounter(activePool.midEncounters);
            else
                currentEncounter = GetRandomEncounter(activePool.lateEncounters);

            SceneManager.LoadScene(combatSceneName);
        }
        else
        {
            Debug.Log("No combat.");
        }
    }

    private Encounter GetRandomEncounter(List<Encounter> pool)
    {
        if (pool == null || pool.Count == 0) return currentEncounter;
        return pool[Random.Range(0, pool.Count)];
    }
}