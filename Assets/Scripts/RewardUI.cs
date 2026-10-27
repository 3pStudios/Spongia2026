using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class RewardUI : MonoBehaviour
{
    [SerializeField] private GameObject panel;
    [SerializeField] private Transform cardContainer;
    [SerializeField] private GameObject cardPrefab;
    [SerializeField] private Button skipButton;

    public string nextSceneName = "Start Menu";

    private void Awake()
    {
        if (panel != null)
        {
            panel.SetActive(false);
        }

        if (skipButton != null)
        {
            skipButton.onClick.AddListener(OnSkipClicked);
        }
    }

    public void ShowRewards()
    {
        if (panel != null)
        {
            panel.SetActive(true);
        }

        foreach (Transform child in cardContainer)
        {
            Destroy(child.gameObject);
        }

        List<CardSO> rewards = RunManager.Instance != null 
            ? RunManager.Instance.GetRandomRewards(3) 
            : new List<CardSO>();

        foreach (CardSO cardData in rewards)
        {
            GameObject cardObj = Instantiate(cardPrefab, cardContainer);

            if (cardObj.TryGetComponent(out CardDrop drop))
            {
                drop.enabled = false;
            }

            if (cardObj.TryGetComponent(out Card cardComponent))
            {
                cardComponent.SetupCard(cardData);
            }

            if (cardObj.TryGetComponent(out Image img))
            {
                img.sprite = cardData.cardImage;
            }

            Button btn = cardObj.GetComponent<Button>();
            if (btn == null)
            {
                btn = cardObj.AddComponent<Button>();
            }

            btn.onClick.AddListener(() => OnCardSelected(cardData));
        }
    }

    private void OnCardSelected(CardSO card)
    {
        if (RunManager.Instance != null)
        {
            RunManager.Instance.AddCard(card);
        }
        CloseAndProceed();
    }

    private void OnSkipClicked()
    {
        CloseAndProceed();
    }

    private void CloseAndProceed()
    {
        if (panel != null)
        {
            panel.SetActive(false);
        }
        SceneManager.LoadScene(nextSceneName);
    }
}