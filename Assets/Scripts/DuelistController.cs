using System;
using TMPro;
using UnityEngine.UI;

[Serializable]
public class DuelistController
{
    public DeckController deckController;
    public HandController handController;
    public BattlefieldController battlefieldController;

    public DuelistView duelistView;
    
    private int _currentVitality;
    
    
    public int Vitality => _currentVitality;
    
    
    public void Initialize(CardController cardPrefab, DeckData deckData, Action<CardController> onCardReleased)
    {
        deckController.Initialize(cardPrefab, deckData, onCardReleased);
        handController.Initialize();
        battlefieldController.Initialize();

        const int DEFAULT_VITALITY = 4;
        SetVitality(DEFAULT_VITALITY);
    }

    public void Clear()
    {
        deckController.Clear();
        handController.Clear();
        battlefieldController.Clear();
    }

    public void SetVitality(int newVitality)
    {
        _currentVitality = newVitality;
        duelistView.UpdateVitality(_currentVitality);
    }

    public void TakeDamage(int damage)
    {
        SetVitality(_currentVitality - damage);
    }

    public bool IsDead()
    {
        return _currentVitality <= 0;
    }
}

[Serializable]
public class DuelistView
{
    public Image avatarImage;
    public TextMeshProUGUI vitalityText;
    
    public void UpdateVitality(int currentVitality)
    {
        vitalityText.text = currentVitality.ToString();
    }
}