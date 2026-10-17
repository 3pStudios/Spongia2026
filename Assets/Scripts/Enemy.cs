using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Enemy : MonoBehaviour
{
    [Header("Stats")]
    public int maxHealth = 20;
    public int currentHealth = 20;
    public int currentShield = 0;
    public int currentAttack = 0;

    [Header("Turns Data")]
    public EnemyTurnsSO enemyTurnsSO;

    [Header("UI Elements")]
    [SerializeField] private TextMeshProUGUI healthText;
    [SerializeField] private TextMeshProUGUI shieldIndicationText;
    [SerializeField] private TextMeshProUGUI attackIndicationText;
    [SerializeField] private Image healthBarFill;

    private void Start()
    {
        UpdateUI();
    }

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
                if (currentHealth <= 0) break;
                TakeDamage(cardData.damage);
            }
        }
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
        if (GameManager.Instance != null)
        {
            GameManager.Instance.enemiesAlive.Remove(this);
        }
        Destroy(gameObject);
    }

    public void TickTurn(int turn)
    {
        if (currentAttack > 0)
        {
            Player player = FindFirstObjectByType<Player>();
            if (player != null)
            {
                player.TakeDamage(currentAttack);
            }
        }

        int nextTurnIndex = (turn + 1) % enemyTurnsSO.enemyTurns.Length;
        SetupTurn(nextTurnIndex);
    }

    private void SetupTurn(int turnIndex)
    {
        if (enemyTurnsSO == null || enemyTurnsSO.enemyTurns.Length == 0) return;

        EnemyTurn turn = enemyTurnsSO.enemyTurns[turnIndex];

        if (turn.turnType == CardType.Attack)
        {
            currentAttack = turn.value;
            currentShield = 0;
        }
        else if (turn.turnType == CardType.Shield)
        {
            currentShield = turn.value;
            currentAttack = 0;
        }

        UpdateUI();
    }

    private void UpdateUI()
    {
        if (healthText != null)
            healthText.text = $"{currentHealth}/{maxHealth}";

        if (healthBarFill != null && maxHealth > 0)
            healthBarFill.fillAmount = (float)currentHealth / maxHealth;

        if (attackIndicationText != null)
            attackIndicationText.text = currentAttack.ToString();

        if (shieldIndicationText != null)
            shieldIndicationText.text = currentShield.ToString();
    }
}