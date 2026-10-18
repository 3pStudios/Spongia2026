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




    [Header("Turns Data")]
    public EnemyTurnsSO enemyTurnsSO;
    private EnemyTurn currentTurn;



    [Header("UI Elements")]
    [SerializeField] private TextMeshProUGUI healthText;
    [SerializeField] private TextMeshProUGUI shieldIndicationText;
    [SerializeField] private TextMeshProUGUI attackIndicationText;
    [SerializeField] private Image healthBarFill;


    [Header("Attack FX")]
    [SerializeField] private GameObject arrowPrefab;
    [SerializeField] private Transform shootPoint;




    private void Start()
    {
        UpdateUI();
    }

    public void SetupEnemy()
    {
        StartCoroutine(SetupTurnRoutine(0));
    }

    public IEnumerator ExecuteTurnRoutine()
    {
        if (currentAttack > 0)
        {
            yield return StartCoroutine(GameManager.Instance.EnemyAttackSequence(this, currentAttack));
        }
    }

    public IEnumerator SetupNextTurnRoutine(int turn)
    {
        int nextTurnIndex = (turn + 1) % enemyTurnsSO.enemyTurns.Length;
        yield return StartCoroutine(SetupTurnRoutine(nextTurnIndex));
    }

    private IEnumerator SetupTurnRoutine(int turnIndex)
    {
        EnemyTurn turn = enemyTurnsSO.enemyTurns[turnIndex];

        if (turn.turnType == CardType.Attack)
        {
            currentAttack = turn.value;
            currentShield = 0;
            UpdateUI();
        }
        else if (turn.turnType == CardType.Shield)
        {
            currentAttack = 0;
            yield return StartCoroutine(ApplyShieldRoutine(turn.value));
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

    public IEnumerator AttackPlayerRoutine(Player target, int attackDamage)
    {
        Vector3 originalPos = transform.position;
        Vector3 lungePos = originalPos - new Vector3(0.4f, 0, 0);

        transform.position = lungePos;
        yield return new WaitForSeconds(0.08f);

        GameObject arrowObj = Instantiate(arrowPrefab, shootPoint.position, Quaternion.identity);
        Projectile projectile = arrowObj.GetComponent<Projectile>();

        yield return StartCoroutine(projectile.FlyToTarget(target.transform.position, 30f));

        target.TakeDamage(attackDamage);

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
        healthText.text = $"{currentHealth}/{maxHealth}";
        healthBarFill.fillAmount = (float)currentHealth / maxHealth;
        attackIndicationText.text = currentAttack.ToString();
        shieldIndicationText.text = currentShield.ToString();
    }
}