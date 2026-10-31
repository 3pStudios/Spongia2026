using UnityEngine;

public enum CardTarget
{
    SingleEnemy,
    AllEnemies,
    RandomEnemy,
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

    [Header("Healing & Regen")]
    public int heal;
    public int medic;

    [Header("Bleed")]
    public int bleed;
    public bool doubleBleed;

    [Header("Utility")]
    public int drawCards;
    public int gainMana;
    public int selfDamage;

    [Header("Powers & Passives")]
    public bool bloodletting;
    public bool thornyShield;
    public bool stun;
}