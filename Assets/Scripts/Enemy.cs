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
        SetupTurn(0);
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
        GameManager.Instance.enemiesAlive.Remove(this);
        Destroy(gameObject);
        GameManager.Instance.CheckBattleOver();
    }

    public IEnumerator TickTurnRoutine(int turn)
    {
        if (currentAttack > 0)
        {
            yield return StartCoroutine(GameManager.Instance.EnemyAttackSequence(this, currentAttack));
        }

        int nextTurnIndex = (turn + 1) % enemyTurnsSO.enemyTurns.Length;
        SetupTurn(nextTurnIndex);
        yield return new WaitForSeconds(0.2f);
    }

    private void SetupTurn(int turnIndex)
    {
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
        healthText.text = $"{currentHealth}/{maxHealth}";
        healthBarFill.fillAmount = (float)currentHealth / maxHealth;
        attackIndicationText.text = currentAttack.ToString();
        shieldIndicationText.text = currentShield.ToString();
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
}