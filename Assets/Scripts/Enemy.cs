using System.Collections;
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
    public int currentBleed = 0;
    public bool isFlying = false;
    public bool isStunned = false;

    private int nextBleedToInflict = 0;
    private int currentTurnIndex = 0;

    [Header("Turns Data")]
    public EnemyTurnsSO enemyTurnsSO;

    [Header("Current Turn UI")]
    [SerializeField] private TextMeshProUGUI healthText;
    [SerializeField] private Image healthBarFill;
    [SerializeField] private TextMeshProUGUI attackIndicationText;
    [SerializeField] private TextMeshProUGUI shieldIndicationText;
    [SerializeField] private TextMeshProUGUI bleedText;

    [Header("Next Turn Preview Indications")]
    [SerializeField] private Transform indicationsParent;
    [SerializeField] private GameObject attackIndicationPrefab;
    [SerializeField] private GameObject shieldIndicationPrefab;
    [SerializeField] private GameObject bleedIndicationPrefab;

    [Header("Attack FX")]
    [SerializeField] private GameObject arrowPrefab;
    [SerializeField] private Transform shootPoint;

    private void Start()
    {
        UpdateUI();
    }

    public void SetupEnemy()
    {
        currentTurnIndex = 0;
        StartCoroutine(SetupTurnRoutine(0));
    }

    public IEnumerator ExecuteTurnRoutine()
    {
        if (isStunned)
        {
            isStunned = false;
            yield return new WaitForSeconds(0.3f);
            yield break;
        }

        if (currentAttack > 0 || nextBleedToInflict > 0)
        {
            yield return StartCoroutine(AttackPlayerRoutine(GameManager.Instance.player, currentAttack, nextBleedToInflict));
        }
    }

    public IEnumerator SetupNextTurnRoutine(int turn)
    {
        currentTurnIndex = (currentTurnIndex + 1) % enemyTurnsSO.enemyTurns.Length;
        yield return StartCoroutine(SetupTurnRoutine(currentTurnIndex));
    }

    private IEnumerator SetupTurnRoutine(int turnIndex)
    {
        EnemyTurn turn = enemyTurnsSO.enemyTurns[turnIndex];

        currentAttack = turn.attack;
        nextBleedToInflict = turn.bleed;

        if (turn.shield > 0)
        {
            yield return StartCoroutine(ApplyShieldRoutine(turn.shield));
        }
        else
        {
            currentShield = 0;
        }

        UpdateNextTurnIndications(turnIndex);
        UpdateUI();
    }

    private void UpdateNextTurnIndications(int currentIdx)
    {
        if (indicationsParent == null || enemyTurnsSO == null || enemyTurnsSO.enemyTurns.Length == 0) return;

        foreach (Transform child in indicationsParent)
        {
            Destroy(child.gameObject);
        }

        int nextIdx = (currentIdx + 1) % enemyTurnsSO.enemyTurns.Length;
        EnemyTurn nextTurn = enemyTurnsSO.enemyTurns[nextIdx];

        if (nextTurn.attack > 0 && attackIndicationPrefab != null)
        {
            Instantiate(attackIndicationPrefab, indicationsParent);
        }

        if (nextTurn.shield > 0 && shieldIndicationPrefab != null)
        {
            Instantiate(shieldIndicationPrefab, indicationsParent);
        }

        if (nextTurn.bleed > 0 && bleedIndicationPrefab != null)
        {
            Instantiate(bleedIndicationPrefab, indicationsParent);
        }
    }

    public IEnumerator ApplyShieldRoutine(int amount)
    {
        yield return new WaitForSeconds(0.2f);
        Vector3 originalScale = transform.localScale;
        transform.localScale = originalScale * 1.15f;

        currentShield = amount;
        UpdateUI();

        yield return new WaitForSeconds(0.4f);
        transform.localScale = originalScale;
        yield return new WaitForSeconds(0.1f);
    }

    public void ApplyBleed(int amount)
    {
        currentBleed += amount;
        UpdateUI();
    }

    public void DoubleBleed()
    {
        currentBleed *= 2;
        UpdateUI();
    }

    public void Stun()
    {
        isStunned = true;
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
            Die();
        }
    }

    public IEnumerator AttackPlayerRoutine(Player target, int attackDamage, int bleedDamage = 0)
    {
        Vector3 originalPos = transform.position;
        Vector3 lungePos = originalPos - new Vector3(0.4f, 0, 0);

        transform.position = lungePos;
        yield return new WaitForSeconds(0.08f);

        GameObject arrowObj = Instantiate(arrowPrefab, shootPoint.position, Quaternion.identity);
        Projectile projectile = arrowObj.GetComponent<Projectile>();

        yield return StartCoroutine(projectile.FlyToTarget(target.transform.position, 30f));

        if (attackDamage > 0) target.TakeDamage(attackDamage);
        if (bleedDamage > 0) target.ApplyBleed(bleedDamage);

        if (target.hasThornyShield)
        {
            TakeDamage(2);
            ApplyBleed(2);
        }

        transform.position = originalPos;
    }

    public void TakeDamage(int damage)
    {
        int unblockedDamage = Mathf.Max(0, damage - currentShield);
        currentShield = Mathf.Max(0, currentShield - damage);
        currentHealth = Mathf.Clamp(currentHealth - unblockedDamage, 0, maxHealth);

        UpdateUI();
        CameraShake.Instance.Shake(0.1f, 0.08f);

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        GameManager.Instance.enemiesAlive.Remove(this);
        GameManager.Instance.CheckBattleOver();
        Destroy(gameObject);
    }

    private void UpdateUI()
    {
        if (healthText != null) healthText.text = $"{currentHealth}/{maxHealth}";
        if (healthBarFill != null) healthBarFill.fillAmount = (float)currentHealth / maxHealth;
        if (attackIndicationText != null) attackIndicationText.text = currentAttack.ToString();
        if (shieldIndicationText != null) shieldIndicationText.text = currentShield.ToString();

        if (bleedText != null)
        {
            bleedText.gameObject.SetActive(currentBleed > 0);
            bleedText.text = currentBleed.ToString();
        }
    }
}