using UnityEngine;
using UnityEngine.EventSystems;

public class EnemyDrop : MonoBehaviour
{
    public void ApplyCard(CardSO cardData)
    {
        Debug.Log($"Applying card: {cardData.cardName}");
        if (cardData.cardType == CardType.Attack)
        {

            Debug.Log($"Applying attack: {cardData.damage}");
        }

    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
