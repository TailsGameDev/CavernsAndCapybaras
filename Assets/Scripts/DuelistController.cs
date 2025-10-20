using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[Serializable]
public class DuelistController
{
    public DeckController deckController;
    public HandController handController;
    public BattlefieldController battlefieldController;

    public DuelistView duelistView;

    public float tweenScale;
    public float tweenDuration;
    
    private int _currentVitality;
    private TransformScaleTween _vitalityTween;
    
    
    public int Vitality => _currentVitality;
    
    
    public void Initialize(CardController cardPrefab, DeckData deckData, Transform movingCardsParent, Action<CardController> onCardReleased)
    {
        deckController.Initialize(cardPrefab, deckData, movingCardsParent, onCardReleased);
        handController.Initialize();
        battlefieldController.Initialize();

        const int DEFAULT_VITALITY = 4;
        SetVitality(DEFAULT_VITALITY);
        
        _vitalityTween = new TransformScaleTween();
    }
    public void Clear()
    {
        deckController.Clear();
        handController.Clear();
        battlefieldController.Clear();
    }

    public void Update()
    {
        if (_vitalityTween.IsTweening)
        {
            _vitalityTween.Update();
        }
    }
    
    public void SetVitality(int newVitality)
    {
        _currentVitality = newVitality;
        duelistView.UpdateVitality(_currentVitality);
    }

    public void TakeDamage(int damage)
    {
        _currentVitality -= damage;

        // Tween to detach vitality loss
        _vitalityTween.StartTween(transform: duelistView.vitalityText.transform.parent,
            targetScale: Vector3.one * tweenScale,duration: tweenDuration,
            onComplete:() =>
            {
                duelistView.UpdateVitality(_currentVitality);
            });
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