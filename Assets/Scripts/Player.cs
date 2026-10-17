using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Player : MonoBehaviour
{
    [Header("Health")]
    public int maxHealth = 20;
    public int currentHealth = 20;

    [Header("Shield")]
    public int currentShield = 0;

    [Header("Mana")]
    public int maxMana = 3;
    public int currentMana = 3;

    [Header("UI Elements")]
    [SerializeField] private TextMeshProUGUI healthText;
    [SerializeField] private Image healthBarFill;
    [SerializeField] private TextMeshProUGUI manaText;

    private void Start()
    {
        currentHealth = maxHealth;
        currentMana = maxMana;
        UpdateUI();
    }

    public void ApplyCard(CardSO cardData)
    {
        if (cardData.cardType == CardType.Shield)
        {
            ApplyShield(cardData.shieldValue);
        }
    }

    public void ApplyShield(int amount)
    {
        currentShield += amount;
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
            Debug.Log("Player died!");
        }
    }

    public bool HasEnoughMana(int amount) => currentMana >= amount;

    public void SpendMana(int amount)
    {
        currentMana = Mathf.Max(0, currentMana - amount);
        UpdateUI();
    }

    public void ResetMana()
    {
        currentMana = maxMana;
        UpdateUI();
    }

    public void ResetShield()
    {
        currentShield = 0;
        UpdateUI();
    }

    public void UpdateUI()
    {
        if (healthText != null)
            healthText.text = $"{currentHealth}/{maxHealth}";

        if (healthBarFill != null && maxHealth > 0)
            healthBarFill.fillAmount = (float)currentHealth / maxHealth;

        if (manaText != null)
            manaText.text = $"{currentMana}/{maxMana}";
    }
}