using UnityEngine;

public enum CardType
{
    Attack,
    Shield
}

public enum AttackType
{
    SingleEnemy,
    MultiEnemy
}

public enum ApplyType
{
    Player,
    Enemy
}

[CreateAssetMenu(fileName = "NewCard", menuName = "Cards/Card")]
public class CardSO : ScriptableObject
{
    [Header("General Info")]
    public string cardName;
    public int cardCost = 1;
    public CardType cardType;
    public ApplyType applyType;
    public Sprite cardImage;

    [Header("Attack Properties")]
    public int damage;
    public AttackType attackType;
    public int hitCount = 1;

    [Header("Shield Properties")]
    public int shieldValue;
}