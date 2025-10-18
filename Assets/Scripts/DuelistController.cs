using System;
using UnityEngine;
using UnityEngine.Serialization;

[Serializable]
public class DuelistController
{
    public DeckController deckController;
    [FormerlySerializedAs("hand")] public HandController handController;

    public void Initialize(CardController cardPrefab, DeckData deckData)
    {
        deckController.Initialize(cardPrefab, deckData);
    }
}