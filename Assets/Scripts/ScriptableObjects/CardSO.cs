using UnityEngine;

public enum CardTarget
{
    SingleEnemy,
    AllEnemies,
    Self
}

[CreateAssetMenu(fileName = "NewCard", menuName = "Cards/Card")]
public class CardSO : ScriptableObject
{
    [Header("General Info")]
    public string cardName;
    public int cardCost = 1;
    [TextArea(2, 4)] public string cardDescription;
    public CardTarget target = CardTarget.SingleEnemy;
    public Sprite cardImage;

    [Header("Direct Damage")]
    public int damage;
    public int hitCount = 1;

    [Header("Shield")]
    public int shield;

    [Header("Bleed")]
    public int bleed;
    public bool doubleBleed;

    [Header("Utility")]
    public int drawCards;
    public int selfDamage;

    [Header("Powers & Passives")]
    public bool bloodletting;
    public bool thornyShield;
}