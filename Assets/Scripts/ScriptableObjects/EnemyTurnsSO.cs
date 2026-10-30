using UnityEngine;

[CreateAssetMenu(fileName = "NewEnemyTurns", menuName = "Turns/EnemyTurns")]
public class EnemyTurnsSO : ScriptableObject
{
    public EnemyTurn[] enemyTurns;
}

[System.Serializable]
public class EnemyTurn
{
    public int attack;
    public int shield;
    public int bleed;
}