using UnityEngine;

[CreateAssetMenu(fileName = "NewEnemyTurns", menuName = "Turns/EnemyTurns")]
public class EnemyTurnsSO : ScriptableObject
{
    public EnemyTurn[] enemyTurns;
}

[System.Serializable]
public class EnemyTurn
{
    public int value;
    public CardType turnType;
    public ApplyType applyType;
}