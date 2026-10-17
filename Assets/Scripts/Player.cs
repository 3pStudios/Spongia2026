using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Player : MonoBehaviour
{
    public int currentShield = 0;
    public int currentHealth = 20;
    public int maxHealth = 20;
    [SerializeField] private TextMeshProUGUI healthText;
    [SerializeField] private Image healthBarFill;
    void Start()
    {
        UpdateUI();
    }
    public void ApplyCard(CardSO cardData)
    {
        if (cardData.cardType == CardType.Shield)
        {
            Debug.Log($"Applying shield: {cardData.shieldValue}");
            currentShield += cardData.shieldValue;
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
            Debug.Log("Player died!");
        }
    }
    private void UpdateUI()
    {
        healthText.text = $"{currentHealth}/{maxHealth}";
        healthBarFill.fillAmount = (float)currentHealth / maxHealth;

    }

}
