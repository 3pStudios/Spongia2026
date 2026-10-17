using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;
using UnityEngine.UI;
using UnityEditor;
using System.Linq;
public class Enemy : MonoBehaviour
{
    public int maxHealth = 20;
    public int currentHealth = 20;
    public int currentShield = 0;

    public void ApplyCard(CardSO cardData)
    {
      
        if (cardData.cardType == CardType.Attack)
        {
            for (int i = 0; i < cardData.hitCount; i++)
            {
                Debug.Log($"Taking damage: {cardData.damage}");
                TakeDamage(cardData.damage);
                UpdateHealthUI();
            }
        }

    }

    public void ApplyShield(int shieldValue)
    {
        currentShield += shieldValue;
        Debug.Log($"Applying shield: {shieldValue}. Current shield: {currentShield}");
    }
    public void AttackPlayer()
    {
        
    }
    public EnemyTurnsSO enemyTurnsSO;
    [SerializeField] private TextMeshProUGUI healthText;
    [SerializeField] private Image healthBarFill;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        UpdateHealthUI();
    }

    // Update is called once per frame
    public void TakeDamage(int damage)
    {
        currentHealth = Mathf.Clamp(currentHealth -= damage, 0, maxHealth);
        UpdateHealthUI();
    }
    private void UpdateHealthUI()
    {
        healthText.text = $"{currentHealth}/{maxHealth}";
        healthBarFill.fillAmount = (float)currentHealth / maxHealth;
    }
    public void TickTurn(int turn)
    {
        int turnIndex = turn % enemyTurnsSO.enemyTurns.Count();
        PlayMove(turnIndex);
    }
    private void PlayMove(int move)
    {
        EnemyTurn currentTurn = enemyTurnsSO.enemyTurns[move];
        if(currentTurn.turnType == CardType.Attack)
        {
            Debug.Log($"Enemy attacks with {currentTurn.value} damage.");
            // Implement attack logic here
        }
        else if (currentTurn.turnType == CardType.Shield)
        {
            ApplyShield(currentTurn.value);
            Debug.Log($"Enemy applies {currentTurn.value} shield.");
        }
    }
}
