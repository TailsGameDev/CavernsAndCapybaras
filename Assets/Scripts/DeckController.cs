using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class DeckController
{
    private List<CardController> _cardsControllers;

    public void Initialize(CardController cardPrefab, DeckData deckData)
    {
        // Instantiate and initialize all cards
        _cardsControllers = new List<CardController>();
        for (int c = 0; c < deckData.cardIds.Length; c++)
        {
            CardController cardInstance = GameObject.Instantiate(cardPrefab);
            CardId cardId = deckData.cardIds[c];
            cardInstance.Initialize(cardId);
        }
    }
}
[Serializable]
public class DeckData
{
    public CardId[] cardIds;
}