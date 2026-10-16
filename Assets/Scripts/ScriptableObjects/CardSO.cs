using UnityEngine;

[CreateAssetMenu(fileName = "Card", menuName = "Scriptable Objects/Card")]



public class CardSO : ScriptableObject
{
    
    public string cardName;
    public int cardCost;
    public CardType cardType;
    public ApplyType applyType;

    [Header("Attack Card Properties")]
    public int damage;
    public AttackType attackType;
    public int hitCount;

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
public enum ApplyType
{
    Player,
    Enemy,
}