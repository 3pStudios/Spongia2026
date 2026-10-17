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
    public int currentAttack = 0;
    public EnemyTurnsSO enemyTurnsSO;
    [SerializeField] private TextMeshProUGUI healthText;
    [SerializeField] private TextMeshProUGUI shieldInditionText;
    [SerializeField] private TextMeshProUGUI attackInditionText;



    [SerializeField] private Image healthBarFill;

    public void SetupEnemy()
    {
        SetupTurn(0);
    }
    public void ApplyCard(CardSO cardData)
    {
      
        if (cardData.cardType == CardType.Attack)
        {
            for (int i = 0; i < cardData.hitCount; i++)
            {
                Debug.Log($"Taking damage: {cardData.damage}");
                TakeDamage(cardData.damage);
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
    void Start()
    {
        UpdateUI();
    }

    public void TakeDamage(int damage)
    {
        int unblockedDamage = Mathf.Max(0, damage - currentShield);
        currentShield = Mathf.Max(0, currentShield - damage);
        currentHealth = Mathf.Clamp(currentHealth - unblockedDamage, 0, maxHealth);

        UpdateUI();
        if (currentHealth <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        Debug.Log($"{gameObject.name} died");
        if (GameManager.Instance != null)
        {
            GameManager.Instance.enemiesAlive.Remove(this);
        }
        Destroy(gameObject);
    }
    private void UpdateUI()
    {
        healthText.text = $"{currentHealth}/{maxHealth}";
        healthBarFill.fillAmount = (float)currentHealth / maxHealth;
        attackInditionText.text = currentAttack.ToString();
        shieldInditionText.text = currentShield.ToString();
    }
    public void TickTurn(int turn)
    {
        if (currentAttack != 0)
        {
            Debug.Log($"Attacking Player for {currentAttack}");
            Player player = FindFirstObjectByType<Player>();
            player.TakeDamage(currentAttack);
        }
        int turnIndex = turn % enemyTurnsSO.enemyTurns.Count();
        SetupTurn(turnIndex);
        
    }
    private void SetupTurn(int move)
    {
        EnemyTurn currentTurn = enemyTurnsSO.enemyTurns[move];
        Debug.Log("Current turn enemy is gonna play is " + move);
        if(currentTurn.turnType == CardType.Attack)
        {
            currentAttack = currentTurn.value;
            currentShield = 0;
        }
        else if (currentTurn.turnType == CardType.Shield)
        {
            currentShield = currentTurn.value;
            currentAttack = 0;
        }
        UpdateUI();

    }
}
