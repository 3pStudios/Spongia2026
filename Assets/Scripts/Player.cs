using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class Player : MonoBehaviour
{
    [Header("Health")]
    public int maxHealth = 20;
    public int currentHealth = 20;

    [Header("Shield")]
    public int currentShield = 0;

    [Header("Bleed")]
    public int currentBleed = 0;

    [Header("Mana")]
    public int maxMana = 3;
    public int currentMana = 3;

    [Header("Passives")]
    public bool hasBloodletting = false;
    public bool hasThornyShield = false;

    [Header("UI Elements")]
    [SerializeField] private TextMeshProUGUI healthText;
    [SerializeField] private Image healthBarFill;
    [SerializeField] private TextMeshProUGUI manaText;
    [SerializeField] private TextMeshProUGUI bleedText;

    [Header("Attack FX")]
    [SerializeField] private GameObject arrowPrefab;
    [SerializeField] private Transform shootPoint;
    [Header("Medic / Regen")]
    public int currentMedic = 0;
    [SerializeField] private TextMeshProUGUI medicText;

    private void Start()
    {
        if (RunManager.Instance != null)
        {
            maxHealth = RunManager.Instance.playerMaxHealth;
            currentHealth = RunManager.Instance.playerCurrentHealth;
        }
        else
        {
            currentHealth = maxHealth;
        }

        currentMana = maxMana;
        UpdateUI();
    }
    public void ApplyMedic(int amount)
    {
        currentMedic += amount;
        UpdateUI();
    }

    public void Heal(int amount)
    {
        currentHealth = Mathf.Clamp(currentHealth + amount, 0, maxHealth);
        if (RunManager.Instance != null)
        {
            RunManager.Instance.playerCurrentHealth = currentHealth;
        }
        UpdateUI();
    }

    public void GainMana(int amount)
    {
        currentMana += amount;
        UpdateUI();
    }

    public IEnumerator TickMedicRoutine()
    {
        if (currentMedic <= 0) yield break;

        Heal(currentMedic);
        currentMedic--;
        UpdateUI();

        yield return new WaitForSeconds(0.25f);
    }
    public void ApplyBleed(int amount)
    {
        currentBleed += amount;
        UpdateUI();
    }

    public IEnumerator TickBleedRoutine()
    {
        if (currentBleed <= 0) yield break;

        currentHealth = Mathf.Clamp(currentHealth - currentBleed, 0, maxHealth);
        CameraShake.Instance.Shake(0.08f, 0.06f);

        currentBleed--;
        UpdateUI();

        yield return new WaitForSeconds(0.25f);

        if (currentHealth <= 0)
        {
            GameManager.Instance.PlayerDied();
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
        if (RunManager.Instance != null)
        {
            RunManager.Instance.playerCurrentHealth = currentHealth;
        }
        CameraShake.Instance.Shake(0.15f, 0.12f);

        if (currentHealth <= 0)
        {
            GameManager.Instance.PlayerDied();
        }
    }

    public void TakeDirectDamage(int damage)
    {
        currentHealth = Mathf.Clamp(currentHealth - damage, 0, maxHealth);
        UpdateUI();
        if (RunManager.Instance != null)
        {
            RunManager.Instance.playerCurrentHealth = currentHealth;
        }
        CameraShake.Instance.Shake(0.1f, 0.08f);

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

    public void ResetTurnPassives()
    {
        hasThornyShield = false;
    }

    public void UpdateUI()
    {
        healthText.text = $"{currentHealth}/{maxHealth}";
        healthBarFill.fillAmount = (float)currentHealth / maxHealth;
        manaText.text = $"{currentMana}/{maxMana}";

        if (bleedText != null)
        {
            bleedText.gameObject.SetActive(currentBleed > 0);
            bleedText.text = $"{currentBleed}";
        }
        if (medicText != null)
        {
            medicText.gameObject.SetActive(currentMedic > 0);
            medicText.text = $"+{currentMedic}";
        }
    }

    public IEnumerator ExecuteCardEffectsRoutine(CardSO card, Enemy singleTarget, List<Enemy> allEnemies)
    {
        if (card.selfDamage > 0)
        {
            TakeDirectDamage(card.selfDamage);
            if (currentHealth <= 0) yield break;
        }

        if (card.heal > 0) Heal(card.heal);
        if (card.medic > 0) ApplyMedic(card.medic);
        if (card.gainMana > 0) GainMana(card.gainMana);

        if (card.shield > 0)
        {
            yield return StartCoroutine(ApplyShieldRoutine(card.shield));
        }

        if (card.drawCards > 0)
        {
            yield return StartCoroutine(GameManager.Instance.DrawCardsRoutine(card.drawCards));
        }

        if (card.bloodletting) hasBloodletting = true;
        if (card.thornyShield) hasThornyShield = true;

        if (card.target == CardTarget.SingleEnemy && singleTarget != null)
        {
            if (card.doubleBleed) singleTarget.DoubleBleed();
            if (card.stun) singleTarget.Stun();
            if (card.damage > 0 || card.bleed > 0)
            {
                yield return StartCoroutine(AttackEnemyRoutine(singleTarget, card));
            }
        }
        else if (card.target == CardTarget.AllEnemies)
        {
            if (card.damage > 0 || card.bleed > 0)
            {
                yield return StartCoroutine(AttackAllEnemiesRoutine(allEnemies, card));
            }
        }
        else if (card.target == CardTarget.RandomEnemy)
        {
            if (card.damage > 0 || card.bleed > 0)
            {
                yield return StartCoroutine(AttackRandomEnemiesRoutine(card));
            }
        }
    }
    public IEnumerator AttackRandomEnemiesRoutine(CardSO card)
    {
        Vector3 originalPos = transform.position;
        Vector3 lungePos = originalPos + new Vector3(0.4f, 0, 0);

        transform.position = lungePos;
        yield return new WaitForSeconds(0.08f);

        for (int hit = 0; hit < card.hitCount; hit++)
        {
            List<Enemy> alive = GameManager.Instance.enemiesAlive.FindAll(e => e != null && e.currentHealth > 0);
            if (alive.Count == 0) break;

            Enemy randomTarget = alive[Random.Range(0, alive.Count)];

            int bleedToApply = (hit == 0) ? card.bleed : 0;
            yield return StartCoroutine(FireArrowRoutine(randomTarget, card.damage, bleedToApply));

            if (card.hitCount > 1)
            {
                yield return new WaitForSeconds(0.1f);
            }
        }

        transform.position = originalPos;
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

            int bleedToApply = (i == 0) ? card.bleed : 0;
            yield return StartCoroutine(FireArrowRoutine(target, card.damage, bleedToApply));

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

                int bleedToApply = (hit == 0) ? card.bleed : 0;
                lastArrowRoutine = StartCoroutine(FireArrowRoutine(target, card.damage, bleedToApply));

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

    private IEnumerator FireArrowRoutine(Enemy target, int damage, int bleed = 0)
    {
        GameObject arrowObj = Instantiate(arrowPrefab, shootPoint.position, Quaternion.identity);
        Projectile projectile = arrowObj.GetComponent<Projectile>();

        yield return StartCoroutine(projectile.FlyToTarget(target.transform.position, 30f));

        if (target != null && target.currentHealth > 0)
        {
            if (damage > 0) target.TakeDamage(damage);
            if (bleed > 0) target.ApplyBleed(bleed);
        }
    }
}