using System.Collections;
using System.Collections.Generic;
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




    [Header("Attack FX")]
    [SerializeField] private GameObject arrowPrefab;
    [SerializeField] private Transform shootPoint;



    private void Start()
    {
        currentHealth = maxHealth;
        currentMana = maxMana;
        UpdateUI();
    }

    public IEnumerator ApplyCardRoutine(CardSO cardData)
    {
        switch (cardData.cardType)
        {
            case CardType.Shield:
                yield return StartCoroutine(ApplyShieldRoutine(cardData.shieldValue));
                break;
        }
    }
    public IEnumerator ApplyShieldRoutine(int amount)
    {
        yield return new WaitForSeconds(0.2f);
        Vector3 originalScale = transform.localScale;
        transform.localScale = originalScale * 1.15f;

        currentShield += amount;
        UpdateUI();

        yield return new WaitForSeconds(0.4f);
        transform.localScale = originalScale;
        yield return new WaitForSeconds(0.1f);
    }

    public void TakeDamage(int damage)
    {
        int unblockedDamage = Mathf.Max(0, damage - currentShield);
        currentShield = Mathf.Max(0, currentShield - damage);
        currentHealth = Mathf.Clamp(currentHealth - unblockedDamage, 0, maxHealth);

        UpdateUI();
        CameraShake.Instance.Shake(0.15f, 0.12f);

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

            yield return StartCoroutine(FireArrowRoutine(target, card.damage));

            transform.position = originalPos;
            if (card.hitCount > 1)
            {
                yield return new WaitForSeconds(0.12f);
            }
        }
        transform.position = originalPos;
    }
    public IEnumerator AttackAllEnemiesRoutine(List<Enemy> targets, CardSO card)
    {
        Vector3 originalPos = transform.position;
        Vector3 lungePos = originalPos + new Vector3(0.4f, 0, 0);

        transform.position = lungePos;
        yield return new WaitForSeconds(0.08f);
        for (int hit = 0; hit < card.hitCount; hit++)
        {
            Coroutine lastArrowRoutine = null;

            for (int i = 0; i < targets.Count; i++)
            {
                Enemy target = targets[i];
                if (target == null || target.currentHealth <= 0) continue;
                lastArrowRoutine = StartCoroutine(FireArrowRoutine(target, card.damage));
                if (i < targets.Count - 1)
                {
                    yield return new WaitForSeconds(0.1f);
                }
            }
            if (lastArrowRoutine != null)
            {
                yield return lastArrowRoutine;
            }

            if (card.hitCount > 1)
            {
                yield return new WaitForSeconds(0.12f);
            }
        }

        yield return new WaitForSeconds(0.1f);
        transform.position = originalPos;
    }


    private IEnumerator FireArrowRoutine(Enemy target, int damage)
    {
        GameObject arrowObj = Instantiate(arrowPrefab, shootPoint.position, Quaternion.identity);
        Projectile projectile = arrowObj.GetComponent<Projectile>();

        yield return StartCoroutine(projectile.FlyToTarget(target.transform.position, 30f));

        if (target != null && target.currentHealth > 0)
        {
            target.TakeDamage(damage);
        }
    }
}