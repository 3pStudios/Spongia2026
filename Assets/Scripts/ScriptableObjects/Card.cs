using UnityEngine;

[CreateAssetMenu(fileName = "Card", menuName = "Scriptable Objects/Card")]



public class Card : ScriptableObject
{
    
    public string cardName;
    public int cardCost;
    public CardType cardType;

    [Header("Attack Card Properties")]
    public int damage;
    public AttackType attackType;

    [Header("Shield Card Properties")]
    public int shieldValue;

}
[System.Serializable]
public enum CardType
{
    Attack,
    Shield,
}
public enum AttackType
{
    SingleEnemy,
    MultiEnemy,
}