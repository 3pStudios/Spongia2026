using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;

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




    [Header("Attack FX")]
    [SerializeField] private GameObject arrowPrefab;
    [SerializeField] private Transform shootPoint;



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
            GameManager.Instance.PlayerDied();
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
        healthText.text = $"{currentHealth}/{maxHealth}";
        healthBarFill.fillAmount = (float)currentHealth / maxHealth;
        manaText.text = $"{currentMana}/{maxMana}";
    }
    public IEnumerator AttackEnemyRoutine(Enemy target, CardSO card)
    {
        Vector3 originalPos = transform.position;
        Vector3 lungePos = originalPos + new Vector3(0.4f, 0, 0);

        for (int i = 0; i < card.hitCount; i++)
        {
            if (target == null || target.currentHealth <= 0) break;
            transform.position = lungePos;
            yield return new WaitForSeconds(0.08f);

            GameObject arrowObj = Instantiate(arrowPrefab, shootPoint.position, Quaternion.identity);
            Projectile projectile = arrowObj.GetComponent<Projectile>();

            yield return StartCoroutine(projectile.FlyToTarget(target.transform.position, 30f));

            target.TakeDamage(card.damage);

            transform.position = originalPos;
            if (card.hitCount > 1)
            {
                yield return new WaitForSeconds(0.12f);
            }
        }

        transform.position = originalPos;
    }
}