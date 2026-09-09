using UnityEngine;
using TMPro;
using System;
using UnityEngine.UI;

public class CardView : MonoBehaviour
{
    [SerializeField] private Image cardImage;
    [SerializeField] private TMP_Text cardNameText;
    [SerializeField] private TMP_Text descriptionText;
    [SerializeField] private TMP_Text manaCostText;
    [SerializeField] private Button cardButton;

    private CardData _cardData;
    private Action<CardData> _onSelected;

    public void Setup(CardData cardData, Action<CardData> onSelected)
    {
        _cardData = cardData;
        _onSelected = onSelected;

        if (_cardData == null)
        {
            Debug.LogError($"{name} recibió una carta vacía.");
            return;
        }

        cardNameText.text = _cardData.CardName;
        descriptionText.text = _cardData.Description;
        manaCostText.text = _cardData.ManaCost.ToString();

        if (_cardData.CardImage != null)
        {
            cardImage.sprite = _cardData.CardImage;
            cardImage.enabled = true;
        }
        else
        {
            cardImage.enabled = false;
        }

        cardButton.onClick.RemoveAllListeners();
        cardButton.onClick.AddListener(HandleClick);
    }

    private void HandleClick()
    {
        if (_cardData == null) return;

        _onSelected?.Invoke(_cardData);
    }
}
