using System;
using TMPro;
using UnityEngine.UI;

[Serializable]
public class DuelistController
{
    public DeckController deckController;
    public HandController handController;
    public BattlefieldController battlefieldController;

    public void Initialize(CardController cardPrefab, DeckData deckData, Action<CardController> onCardReleased)
    {
        deckController.Initialize(cardPrefab, deckData, onCardReleased);
        handController.Initialize();
        battlefieldController.Initialize();
    }
}

[Serializable]
public class DuelistView
{
    public Image avatarImage;
    public TextMeshProUGUI vitalityText;
}