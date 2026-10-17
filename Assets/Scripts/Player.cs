using UnityEngine;
using UnityEngine.EventSystems;

public class Player : MonoBehaviour
{
    public int currentShield = 0;
    public void ApplyCard(CardSO cardData)
    {
        if (cardData.cardType == CardType.Shield)
        {
            Debug.Log($"Applying shield: {cardData.shieldValue}");
            currentShield += cardData.shieldValue;
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
