using System;
using System.Collections.Generic;
using UnityEngine;
using Random = System.Random;

[Serializable]
public class DeckController
{
    public RectTransform root;

    private List<CardController> _cardsControllers;

    public void Initialize(CardController cardPrefab, DeckData deckData, Action<CardController> onCardReleased)
    {
        // Instantiate and initialize all cards. Insert cards randomly so the deck gets shuffled
        _cardsControllers = new List<CardController>();
        for (int c = 0; c < deckData.cardIds.Length; c++)
        {
            CardController cardInstance = GameObject.Instantiate(cardPrefab,
                root.transform.position, Quaternion.identity, parent: root);
            CardId cardId = deckData.cardIds[c];
            cardInstance.Initialize(cardId, onCardReleased);
            _cardsControllers.Insert(index: UnityEngine.Random.Range(0, _cardsControllers.Count), cardInstance);
        }
    }

    public CardController DrawCard()
    {
        CardController card = _cardsControllers[0];
        _cardsControllers.RemoveAt(0);
        return card;
    }
}
[Serializable]
public class DeckData
{
    public CardId[] cardIds;
}