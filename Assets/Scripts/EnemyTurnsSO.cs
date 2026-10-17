using UnityEngine;

[CreateAssetMenu(fileName = "EnemyTurns", menuName = "Scriptable Objects/EnemyTurns")]
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