using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;
using UnityEngine.UI;
using UnityEditor;
public class Enemy : MonoBehaviour
{
    public int maxHealth = 20;
    public int currentHealth = 20;
    public void ApplyCard(CardSO cardData)
    {
      
        if (cardData.cardType == CardType.Attack)
        { 
            Debug.Log($"Taking damage: {cardData.damage}");
            TakeDamage(cardData.damage);
            UpdateHealthUI();
        }

    }


    [SerializeField] private TextMeshProUGUI healthText;
    [SerializeField] private Image healthBarFill;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        UpdateHealthUI();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
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
}
